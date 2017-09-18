#pragma once
#include <fstream>
#include <string>
#include <filesystem>
#include <windows.h>

//#include <boost\interprocess\file_mapping.hpp>
//#include <boost\interprocess\mapped_region.hpp>

namespace Workstation{
	namespace Analytic{
		WS_VERSIONER_API class FileItem{
		public:
			const std::ifstream& getFile() const{ return file; }
		
			const int& getTotalLines() const{ return totalLines; }

			FileItem(std::string path, std::string filename, int flag){
				this->flag = flag;
				totalLines = 0;
				file = std::ifstream(absolute = (this->path = path) + (this->filename = filename), flag);
				
				try {
					CreateDirectory((LPCTSTR)path.c_str(), NULL);
				}
				catch (std::exception e) {
				}


				//if (!(CreateDirectory((LPCTSTR)path.c_str(), NULL) && !ERROR_ALREADY_EXISTS == GetLastError()))
					//throw std::exception("Could not create the directory because " + GetLastError());
			}

			~FileItem(){
				if(&file != nullptr)
					if (file.is_open())
						closeFile();
			}

			bool isOpen()
			{
				return file.is_open();
			}

			void discoverFile(){
				totalLines = 0;
				std::string nutshell;
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


			void compareTo(FileItem& other, FileCompareResultList* output){
				
				file.clear();   //  sets a new value for the error control state
				file.seekg(0, std::ios::beg);

				other.file.clear();
				other.file.seekg(0, std::ios::beg);

				const std::streamsize size = 4096;
				char originalContent[size], newContent[size], newContentCopy[size], originalContentCopy[size];

				int currentLine = 0,
					lineVariation = 0,
					changes = 0,
					variation = 0, movingOffset = 0,
					currentOffset = 0, originalOffset = 0;

				bool atleastOneChange = false;
				bool fileLimitReached = false;
				bool foundInCopy = false;

				// Scan the files
				// TODO Optimize
				while (!file.eof()) {
					originalOffset = file.tellg();
					variation = 0;
					if(other.file.eof()){
						auto fcr = FileCompareResult();

						fcr.StartingLine = other.getTotalLines() + 1;
						
						fcr.ChangeSet = std::list<std::string>();
						fcr.changeType = ChangeType::RemovedContent;
						fcr.ModifiedContent = fcr.OriginalLineContent = "";

						while (!file.eof()) {
							variation++;

							file.getline(originalContentCopy, size);
							fcr.ChangeSet.push_back(originalContentCopy);
						}

						fcr.EndLine = currentLine + variation;

						output->push_back(fcr);
						variation = 0;
						break;
					}

					file.getline(originalContent, size);
					other.file.getline(newContent, size);
					atleastOneChange = foundInCopy = fileLimitReached = false;

					currentLine++;
					
					changes = strcmp(originalContent, newContent);
					movingOffset = other.file.tellg();

					if ((atleastOneChange = (changes != 0))) currentOffset = other.file.tellg();
					
					while (atleastOneChange && !other.file.eof() && (fileLimitReached = (variation + currentLine < other.getTotalLines()))) {
						other.file.getline(newContentCopy, size);
						variation++;

						if ((changes = strcmp(newContentCopy, originalContent)) != 0) {
							movingOffset = other.file.tellg();
							continue;
						}

						foundInCopy = true;

						int thisOffset = movingOffset >= 0 ? movingOffset : other.file.tellg();

						auto fcr = FileCompareResult();

						fcr.StartingLine = currentLine;
						fcr.EndLine = currentLine + variation;
						fcr.ChangeSet = std::list<std::string>();
						fcr.changeType = ChangeType::NewContent;
						fcr.ModifiedContent = fcr.OriginalLineContent = "";

						other.file.clear();
						other.file.seekg(currentOffset, std::ios::beg);

						// Reading from the beginning
						while(!other.file.eof() && other.file.tellg().seekpos() != thisOffset){
							other.file.getline(newContentCopy, size);
							fcr.ChangeSet.push_back(newContentCopy);
						}

						other.file.getline(newContentCopy, size);
						currentOffset = thisOffset;
						output->push_back(fcr);
						atleastOneChange = false;
						break;
					}

					if (atleastOneChange && !foundInCopy) {
						
						other.file.clear();
						other.file.seekg(currentOffset, std::ios::beg);
						currentOffset = movingOffset = file.tellg();
						
						variation = 0;
					}

					while (atleastOneChange && !foundInCopy && !file.eof() && (fileLimitReached = (variation + currentLine < getTotalLines()))) {
						file.getline(originalContentCopy, size);
						variation++;

						if ((changes = strcmp(newContent, originalContentCopy)) != 0) {
							movingOffset = file.tellg();
							continue;
						}

						foundInCopy = true;

						int thisOffset = movingOffset >= 0 ? movingOffset : file.tellg();

						auto fcr = FileCompareResult();

						fcr.StartingLine = currentLine;
						fcr.EndLine = currentLine + variation;
						fcr.ChangeSet = std::list<std::string>();
						fcr.changeType = ChangeType::RemovedContent;
						fcr.ModifiedContent = fcr.OriginalLineContent = "";

						file.clear();
						file.seekg(originalOffset, std::ios::beg);

						// Reading from the beginning
						while (!file.eof() && file.tellg().seekpos() != thisOffset) {
							file.getline(originalContentCopy, size);
							fcr.ChangeSet.push_back(originalContentCopy);
						}

						file.getline(originalContentCopy, size);
						currentOffset = thisOffset;
						output->push_back(fcr);
						atleastOneChange = false;
						break;
					}

					if (atleastOneChange) {
						
						file.clear();
						file.seekg(currentOffset, std::ios::beg);
						
						atleastOneChange = false;
						
						auto fcr = FileCompareResult();

						fcr.StartingLine = currentLine;
						fcr.EndLine = currentLine;
						fcr.changeType = ChangeType::LineChange;
						fcr.ChangeSet = std::list<std::string>();
						fcr.ChangeSet.push_back(originalContent);
						fcr.ModifiedContent = fcr.OriginalLineContent = "";
						output->push_back(fcr);
					}

				}

				if(file.eof() && !other.file.eof()){
					auto fcr = FileCompareResult();

					fcr.StartingLine = getTotalLines() + 1;
					
					fcr.ChangeSet = std::list<std::string>();
					fcr.changeType = ChangeType::NewContent;
					fcr.ModifiedContent = fcr.OriginalLineContent = "";

					variation = 0;
					while (!other.file.eof()) {
						variation++;
						other.file.getline(newContentCopy, size);
						fcr.ChangeSet.push_back(newContentCopy);
					}
					fcr.EndLine = getTotalLines() + variation;
					output->push_back(fcr);
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
