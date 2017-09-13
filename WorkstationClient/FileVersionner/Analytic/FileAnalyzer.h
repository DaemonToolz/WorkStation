#pragma once

#include <algorithm>
#include <iterator>
#include <string>
#include <fstream>
#include "FileCompareResultList.h"
#include <iostream>
#include <chrono>
#include <ctime>

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
				originalFilename = original;
				newFilename = fnew;

				originalFilepath = filepath;
				tempFilepath = filepath + "/temp/";
				backupPath = filepath + "/backup/";

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

			void OpenFile();
			void CloseFile();

		private:
			FileCompareResultList* comparator;
			std::ifstream original, modified, temp;
			std::string originalFilename, newFilename, originalFilepath, tempFilepath, backupPath;

			void Merge();
			void Overwrite();
			void SelectChanges(int*);


		protected:

		};
	}
}