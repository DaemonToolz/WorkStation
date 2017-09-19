using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WorkstationUtilities.Analytic {
    public class FileItem : IDisposable {
        public string Path { get; private set;}
        public string Filename { get; private set; }

        public string Absolute => Path + Filename;
        protected FileStream File { get; set; }

        public int TotalLine { get; private set; }
        public FileMode FileMode { get; private set; }
        public FileAccess FileAccess { get; private set; }

        public FileItem(string Path, string Filename, FileMode Mode = FileMode.Open, FileAccess Access = FileAccess.Read)
        {
            this.Path = Path;
            this.Filename = Filename;
            FileMode = Mode;
            FileAccess = Access;
            File = new FileStream(Absolute, FileMode, FileAccess);
            Directory.CreateDirectory(Path);
        }

        public void OpenFile()
        {
            File?.Close();
            File = new FileStream(Absolute, FileMode, FileAccess);
        }

        public void DiscoverFile()
        {
            TotalLine = 0;
            using (StreamReader r = new StreamReader(File)){
                while (r.ReadLine() != null) { TotalLine++; }    
            }   
        }


        public void CompareTo(ref FileItem Other, ref List<FileComparisonItem> Output)
        {
            File.Dispose();
            Other.File.Dispose();

            OpenFile();
            Other.OpenFile();

            File.Seek(0, SeekOrigin.Begin);
            Other.File.Seek(0, SeekOrigin.Begin);
            
            //const int StreamSize = 4096;

            long CurrentLine = 0, CurrentOffset = 0;

            bool FileLimitReached = false, FoundInCopy = false;

            using (StreamReader FileParser = new StreamReader(File)){
                using (StreamReader OtherFileParser = new StreamReader(Other.File)) {
                    long Variation = 0;
                    while (!FileParser.EndOfStream) {
                        var OriginalOffset = File.Position;
                        Variation = 0;

                        // Case 1 : No more lines in the other file
                        string OriginalContentCopy;
                        if (OtherFileParser.EndOfStream){
                            var Fcr = new FileComparisonItem()
                            {
                                StartingLine = Other.TotalLine + 1,
                                ChangeSet = new List<String>(),
                                changeType = ChangeType.RemovedContent
                            };

                            while (!FileParser.EndOfStream){
                                Variation++;
                                OriginalContentCopy = FileParser.ReadLine();
                                Fcr.ChangeSet.Add(OriginalContentCopy);
                            }

                            Fcr.EndLine = (int)(CurrentLine + Variation);
                            Output.Add(Fcr);

                            Variation = 0;
                            break;
                        }

                        var OriginalContent = FileParser.ReadLine();
                        var NewContent = OtherFileParser.ReadLine();

                        var AtLeastOneChange = !OriginalContent.Equals(NewContent);
                        CurrentLine++;

                        var MovingOffset = Other.File.Position;


                        // Case 2 : At least one string changed
                        if (AtLeastOneChange) CurrentOffset = Other.File.Position;

                        while (AtLeastOneChange && !OtherFileParser.EndOfStream){
                            var NewContentCopy = OtherFileParser.ReadLine();
                            Variation++;

                            if (!NewContentCopy.Equals(OriginalContent)) {
                                MovingOffset = Other.File.Position;
                                continue;
                            }

                            FoundInCopy = true;
                            long ThisOffset = (MovingOffset >= 0 ? MovingOffset : Other.File.Position);

                            var Fcr = new FileComparisonItem() {
                                StartingLine = (int)CurrentLine,
                                EndLine = (int) (CurrentLine + Variation),
                                ChangeSet = new List<String>(),
                                changeType = ChangeType.NewContent
                            };

                            Other.File.Seek(CurrentOffset, SeekOrigin.Begin);

                            while (!OtherFileParser.EndOfStream && Other.File.Position != ThisOffset)
                                Fcr.ChangeSet.Add(OtherFileParser.ReadLine());
                            NewContentCopy = OtherFileParser.ReadLine();

                            Output.Add(Fcr);

                            CurrentOffset = ThisOffset;
                            AtLeastOneChange = false;
                            break;
                        }


                        if (AtLeastOneChange && !FoundInCopy){
                            Other.File.Seek(CurrentOffset, SeekOrigin.Begin);
                            CurrentOffset = MovingOffset = File.Position;
                            Variation = 0;
                        }

                        // Case 3 : Cannot find changes in the other file, check the original doc
                        while (AtLeastOneChange && !FoundInCopy && !FileParser.EndOfStream) {
                            OriginalContentCopy = FileParser.ReadLine();
                            Variation++;
                            if (!NewContent.Equals(OriginalContentCopy))
                            {
                                MovingOffset = File.Position;
                                continue;
                            }

                            long ThisOffset = (MovingOffset >= 0 ? MovingOffset : File.Position);

                            var Fcr = new FileComparisonItem()
                            {
                                StartingLine = (int) CurrentLine,
                                EndLine = (int) (CurrentLine + Variation),
                                ChangeSet = new List<String>(),
                                changeType = ChangeType.RemovedContent
                            };

                            File.Seek(OriginalOffset, SeekOrigin.Begin);

                            while (!FileParser.EndOfStream && File.Position != ThisOffset)
                                Fcr.ChangeSet.Add(FileParser.ReadLine());
                            OriginalContentCopy = FileParser.ReadLine();
                            CurrentOffset = ThisOffset;
                            Output.Add(Fcr);
                            AtLeastOneChange = false;
                            break;
                        }

                        if (AtLeastOneChange){
                            File.Seek(CurrentOffset, SeekOrigin.Begin);
                            AtLeastOneChange = false;

                            Output.Add(new FileComparisonItem(){
                                StartingLine = (int)CurrentLine,
                                EndLine = (int)(CurrentLine),
                                ChangeSet = new List<String>(){ OriginalContent },
                                changeType = ChangeType.LineChange
                            });
                        }
                    }

                    // Final case: new Lines in the modified file
                    if (FileParser.EndOfStream && !OtherFileParser.EndOfStream){
                        var Fcr = new FileComparisonItem(){
                            StartingLine = TotalLine + 1,
                            ChangeSet = new List<String>(),
                            changeType = ChangeType.NewContent
                        };

                        Variation = 0;
                        while (!OtherFileParser.EndOfStream)
                        {
                            Variation++;
                            Fcr.ChangeSet.Add(OtherFileParser.ReadLine());
                        }
                        Fcr.EndLine = (int)(TotalLine + Variation);
                        Output.Add(Fcr);
                    }

                    File.Seek(0, SeekOrigin.Begin);
                    Other.File.Seek(0, SeekOrigin.Begin);
                }

            }
        }

        public void Dispose(){
            File?.Dispose();
        }


        public void CompareTo_2(ref FileItem Other, ref List<FileComparisonItem> Output)
        {
            string[] OriginalScript = System.IO.File.ReadAllLines(Absolute),
                ModifiedScript = System.IO.File.ReadAllLines(Other.Absolute);

            //const int StreamSize = 4096;

            long CurrentLine = 0,
                CurrentOffset = 0,
                OriginalReadingBufferIndex = 0,
                ModifiedReadingBufferIndex = 0;

            bool FoundInCopy = false;

            Func<long, String[], bool> EndOfStream = 
                (long Buffer, String[] Script) =>  Buffer == Script.LongLength;
          

            long Variation = 0;
            while (!EndOfStream(OriginalReadingBufferIndex, OriginalScript)){
                var OriginalOffset = OriginalReadingBufferIndex;
                Variation = 0;
                
                // Case 1 : No more lines in the other file
                string OriginalContentCopy;

                if (EndOfStream(ModifiedReadingBufferIndex, ModifiedScript)){
                    var Fcr = new FileComparisonItem() {
                        StartingLine = Other.TotalLine + 1,
                        ChangeSet = new List<String>(),
                        changeType = ChangeType.RemovedContent
                    };

                    while (!EndOfStream(OriginalReadingBufferIndex, OriginalScript)){
                        Variation++;
                        OriginalContentCopy = OriginalScript[OriginalReadingBufferIndex];
                        OriginalReadingBufferIndex++;
                        Fcr.ChangeSet.Add(OriginalContentCopy);
                    }

                    Fcr.EndLine = (int)(CurrentLine + Variation);
                    Output.Add(Fcr);

                    Variation = 0;
                    break;
                }

                var OriginalContent = OriginalScript[OriginalReadingBufferIndex++];
                var NewContent = ModifiedScript[ModifiedReadingBufferIndex++];

                var AtLeastOneChange = !OriginalContent.Equals(NewContent);
                CurrentLine++;

                var MovingOffset = ModifiedReadingBufferIndex;


                // Case 2 : At least one string changed
                if (AtLeastOneChange) CurrentOffset = ModifiedReadingBufferIndex;

                while (AtLeastOneChange && !EndOfStream(ModifiedReadingBufferIndex, ModifiedScript)){
                    var NewContentCopy = ModifiedScript[ModifiedReadingBufferIndex++];
                    Variation++;

                    if (!NewContentCopy.Equals(OriginalContent)){
                        MovingOffset = ModifiedReadingBufferIndex;
                        continue;
                    }

                    FoundInCopy = true;
                    long ThisOffset = (MovingOffset >= 0 ? MovingOffset : ModifiedReadingBufferIndex);

                    var Fcr = new FileComparisonItem() {
                        StartingLine = (int)CurrentLine,
                        EndLine = (int)(CurrentLine + Variation),
                        ChangeSet = new List<String>(),
                        changeType = ChangeType.NewContent
                    };

                    ModifiedReadingBufferIndex = CurrentOffset;

                    while (!EndOfStream(ModifiedReadingBufferIndex, ModifiedScript) && ModifiedReadingBufferIndex != ThisOffset)
                        Fcr.ChangeSet.Add(ModifiedScript[ModifiedReadingBufferIndex++]);
                    NewContentCopy = ModifiedScript[ModifiedReadingBufferIndex++];

                    Output.Add(Fcr);

                    CurrentOffset = ThisOffset;
                    AtLeastOneChange = false;
                    break;
                }

                if (AtLeastOneChange && !FoundInCopy){
                    ModifiedReadingBufferIndex = CurrentOffset;
                    CurrentOffset = MovingOffset = OriginalReadingBufferIndex;
                    Variation = 0;
                }

                // Case 3 : Cannot find changes in the other file, check the original doc
                while (AtLeastOneChange && !FoundInCopy && !EndOfStream(OriginalReadingBufferIndex, OriginalScript)){
                    OriginalContentCopy = OriginalScript[OriginalReadingBufferIndex++];
                    Variation++;
                    if (!NewContent.Equals(OriginalContentCopy)){
                        MovingOffset = OriginalReadingBufferIndex;
                        continue;
                    }

                    long ThisOffset = (MovingOffset >= 0 ? MovingOffset : OriginalReadingBufferIndex);

                    var Fcr = new FileComparisonItem(){
                        StartingLine = (int)CurrentLine,
                        EndLine = (int)(CurrentLine + Variation),
                        ChangeSet = new List<String>(),
                        changeType = ChangeType.RemovedContent
                    };

                    OriginalReadingBufferIndex = OriginalOffset;

                    while (!EndOfStream(OriginalReadingBufferIndex, OriginalScript) && OriginalReadingBufferIndex  != ThisOffset)
                        Fcr.ChangeSet.Add(OriginalScript[OriginalReadingBufferIndex++]);
                    OriginalContentCopy = OriginalScript[OriginalReadingBufferIndex++];
                    CurrentOffset = ThisOffset;
                    Output.Add(Fcr);
                    AtLeastOneChange = false;
                    break;
                }

                if (AtLeastOneChange){
                    OriginalReadingBufferIndex = (CurrentOffset);
                    AtLeastOneChange = false;

                    Output.Add(new FileComparisonItem(){
                        StartingLine = (int)CurrentLine,
                        EndLine = (int)(CurrentLine),
                        ChangeSet = new List<String>() { NewContent },
                        changeType = ChangeType.LineChange
                    });
                }
            }

            // Final case: new Lines in the modified file
            if (EndOfStream(OriginalReadingBufferIndex, OriginalScript) && !EndOfStream(ModifiedReadingBufferIndex, ModifiedScript)){
                var Fcr = new FileComparisonItem(){
                    StartingLine = TotalLine + 1,
                    ChangeSet = new List<String>(),
                    changeType = ChangeType.NewContent
                };

                Variation = 0;
                while (!EndOfStream(ModifiedReadingBufferIndex, ModifiedScript)){
                    Variation++;
                    Fcr.ChangeSet.Add(ModifiedScript[ModifiedReadingBufferIndex++]);
                }

                Fcr.EndLine = (int)(TotalLine + Variation);
                Output.Add(Fcr);
            }

        }

    }
}
