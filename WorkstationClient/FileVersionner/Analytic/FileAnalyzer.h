#pragma once

#ifdef WS_VERSIONER_API  
#define WS_VERSIONER_API __declspec(dllexport)   
#else  
#define WS_VERSIONER_API __declspec(dllimport)   
#endif  

#include <algorithm>
#include <iterator>
#include "FileCompareResultList.h"
#include <chrono>
#include <ctime>
#include "FileItem.h"
namespace Workstation {
	namespace Analytic {
		WS_VERSIONER_API enum FileAction{
			KeepOriginal = 0,
			Overwrite,
			Merge,
			SelectChanges
		};

		WS_VERSIONER_API class FileAnalyzer {

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
				delete comparator;
				delete original, temp, modified;	
			}
			

			void CreateShadowCopy(const char* filepath);
			void AnalyseFile();
			void CreateBackup(int);

			const FileCompareResultList& getResultList()const{
				return *comparator;
			}

			FileCompareResultList getResultListCopy()const {
				return *comparator;
			}

			void OpenFile();
			void CloseFile() const;

		private:
			FileCompareResultList* comparator;
			FileItem* original, *modified, *temp;
			std::string originalFilepath;
			//std::ifstream original, modified, temp;
			//std::string originalFilename, newFilename, originalFilepath, tempFilepath, backupPath;

			

		protected:

		};
	}
}