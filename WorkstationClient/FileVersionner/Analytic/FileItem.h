#pragma once
#include <fstream>
#include <string>
#include <cstddef>
#include <string>
#include <filesystem>
#include <windows.h>

//#include <boost\interprocess\file_mapping.hpp>
//#include <boost\interprocess\mapped_region.hpp>

namespace Workstation{
	namespace Analytic{
		class FileItem{
		public:
			const std::ifstream& getFile() const{ return file; }
		
			const int& getTotalLines() const{ return totalLines; }

			FileItem(std::string path, std::string filename, int flag){
				this->flag = flag;
				totalLines = 0;
				file = std::ifstream(absolute = (this->path = path) + (this->filename = filename), flag);
				
				if (!(CreateDirectory((LPCTSTR)path.c_str(), NULL) || ERROR_ALREADY_EXISTS == GetLastError()))
					throw std::exception("Could not create the directory");
			}

			~FileItem(){
				if(&file != nullptr)
					if (file.is_open())
						closeFile();
			}

			void discoverFile(){
				totalLines = 0;
				std::string nutshell;

				std::cout << "Opening file " << absolute << std::endl;

				while (!file.eof()) {
					getline(file, nutshell);
					totalLines++;
					

				}
				file.clear();   //  sets a new value for the error control state
				file.seekg(0, std::ios::beg);
			}

			void openFile() {
				closeFile();
				file.open(absolute);
			}

			void openFile(int flag) {
				closeFile();
				file.open(absolute,flag);
			}

			void openFile(std::string newFilename,int flag) {
				closeFile();
				filename = newFilename;
				file.open(filename, flag);
			}


			void closeFile()
			{
				if(file.is_open())
					file.close();
			}

			std::string getAbsolutePath()const{
				return absolute;
			}

			std::string getFilename()const{
				return filename;
			}


			void compareTo(FileItem& other, FileCompareResultList* output)
			{
				int maxLines = 0;
				
				if (totalLines != other.getTotalLines()) {
					FileCompareResult firstChanges = FileCompareResult();

					firstChanges.AdditionalLines = totalLines - other.getTotalLines();
					
					output->insert(output->begin(),firstChanges);

					if (totalLines > other.getTotalLines())  maxLines = other.getTotalLines();
					else maxLines = totalLines;
					
				} else  maxLines = totalLines;
		
				file.clear();   //  sets a new value for the error control state
				file.seekg(0, std::ios::beg);

				other.file.clear();
				other.file.seekg(0, std::ios::beg);

				const std::streamsize size = 4096;
				char originalContent[size], newContent[size], newContentCopy[size];

				int currentLine = 0,
					lineVariation = 0,
					changes = 0,
					variation = 0, movingOffset = -1,
					currentOffset = 0;

				std::cout << "Starting sequence" << std::endl << std::endl;

				bool atleastOneChange = false;
				bool fileLimitReached = false;
				// First Scan
				while (!file.eof()) {
					file.getline(originalContent, size);
					other.file.getline(newContent, size);
					atleastOneChange = fileLimitReached = false;
					currentLine++;
					variation = 0;
					changes = strcmp(originalContent, newContent);
					movingOffset = -1;

					if ((atleastOneChange = (changes != 0))) {
						currentOffset = other.file.tellg();

						std::cout << "DIFFERENCE DETECTED " << std::endl;
						std::cout << "ORIGINAL " << originalContent << std::endl;
						std::cout << "MODIFIED " << newContent << std::endl;
						std::cout << "OFFSET" << currentOffset << " / " << file.tellg() << std::endl;
						std::cout << "----------------------" << std::endl;
					}
				
					while (atleastOneChange && !other.file.eof() && (fileLimitReached = (variation + currentLine < other.getTotalLines()))) {
						other.file.getline(newContentCopy, size);
						variation++;

						std::cout << "---------------------- " << std::endl;
						std::cout << "ORIGINAL " << originalContent << std::endl;
						std::cout << "MODIFIED " << newContentCopy << std::endl;
						std::cout << "ROUND " << variation << std::endl;
						std::cout << "OFFSET " << other.file.tellg() << " / " << file.tellg() << std::endl;
						std::cout << "EOF " << !fileLimitReached << std::endl;
						std::cout << "SAVED " << currentOffset << std::endl;
						std::cout << "----------------------" << std::endl;

						
						if ((changes = strcmp(newContentCopy, originalContent)) != 0) {
							movingOffset = other.file.tellg();
							continue;
						}

						std::cout << std::endl;

						int thisOffset = movingOffset >= 0 ? movingOffset : other.file.tellg();

						auto fcr = FileCompareResult();

						fcr.StartingLine = currentLine;
						fcr.EndLine = variation;
						fcr.ChangeSet = std::list<std::string>();
						fcr.changeType = ChangeType::NewContent;
						fcr.ModifiedContent = fcr.OriginalLineContent = "";

						other.file.clear();
						other.file.seekg(currentOffset, std::ios::beg);

						// Reading from the beginning
						while(!other.file.eof() && other.file.tellg().seekpos() != thisOffset){
							other.file.getline(newContentCopy, size);
							std::cout << newContentCopy << std::endl;
							fcr.ChangeSet.push_back(newContentCopy);
						}

						other.file.getline(newContentCopy, size);
						currentOffset = thisOffset;
						output->push_back(fcr);
						atleastOneChange = false;
						break;
					}

					
					if (atleastOneChange) {
						std::cout << "Reset position" << std::endl;
						other.file.clear();
						other.file.seekg(currentOffset, std::ios::beg);
					
						auto fcr = FileCompareResult();

						fcr.StartingLine = currentLine;
						fcr.EndLine = variation;
						fcr.ChangeSet = std::list<std::string>();
						fcr.changeType = ChangeType::LineChange;

						fcr.OriginalLineContent = originalContent;
						fcr.ModifiedContent = newContent;
					}
				}
				file.clear();   //  sets a new value for the error control state
				file.seekg(0, std::ios::beg);

				other.file.clear();   
				other.file.seekg(0, std::ios::beg);
	
			}
		private:
			
			std::string path, filename, absolute;
			int totalLines, flag;
		

		protected:
			std::ifstream file;
		};
	}
}
