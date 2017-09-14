#pragma once

#include <algorithm>
#include <iterator>
#include <string>
#include <fstream>
#include "FileCompareResultList.h"
#include <iostream>
#include <chrono>
#include <ctime>
#include "FileItem.h"
namespace Workstation {
	namespace Analytic {
		enum FileAction{
			KeepOriginal = 0,
			Overwrite,
			Merge,
			SelectChanges
		};

		class FileAnalyzer {

		public:
			FileAnalyzer(std::string original, std::string fnew, std::string filepath){
				originalFilepath = filepath;
				this->original = new FileItem(filepath, original, std::ios::binary);
				this->modified = new FileItem(filepath, fnew, std::ios::binary);
				comparator = new FileCompareResultList();
			}

			~FileAnalyzer()
			{
				CloseFile();
				comparator->~FileCompareResultList();
			}
			

			void CreateShadowCopy(const char* filepath);
			void AnalyseFile();
			void Decide(FileAction);
			void CreateBackup(int);

			const FileCompareResultList& getResultList()const{
				return *comparator;
			}

			FileCompareResultList getResultListCopy()const {
				return *comparator;
			}

			void OpenFile();
			void CloseFile();

		private:
			FileCompareResultList* comparator;
			FileItem* original, *modified, *temp;
			std::string originalFilepath;
			//std::ifstream original, modified, temp;
			//std::string originalFilename, newFilename, originalFilepath, tempFilepath, backupPath;

			void Merge();
			void Overwrite();
			void SelectChanges(int*);


		protected:

		};
	}
}