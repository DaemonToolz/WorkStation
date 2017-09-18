#define _CRT_SECURE_NO_WARNINGS

// Temporary solution
#include "Analytic/FileAnalyzer.h"

#include <chrono>
#include <Windows.h>



void Workstation::Analytic::FileAnalyzer::CreateShadowCopy(const char* filepath){
	//temp = std::ifstream(filepath);
}

void Workstation::Analytic::FileAnalyzer::AnalyseFile() {
	comparator->clear();
	original->discoverFile();
	modified->discoverFile();
	original->compareTo(*modified, comparator);
	//modified->compareTo(*original, comparator);

}

// TODO COPY TO C#

void Workstation::Analytic::FileAnalyzer::CreateBackup(int fileno){
	//std::ofstream backup = std::ofstream(backupPath + originalFilename + std::to_string(fileno));
	std::string from = 
		original->getAbsolutePath(), 
			to = originalFilepath + "temp\\" + original->getFilename() + "_" + std::to_string(fileno);
	
	try {
		CreateDirectory((LPCTSTR)(originalFilepath + "temp\\").c_str(), NULL);
	} catch(std::exception e){
	}

	if(GetLastError() != ERROR_ALREADY_EXISTS)
		throw std::exception("Could not create the directory");
	CopyFile(from.c_str(), to.c_str(), FALSE);
}

void Workstation::Analytic::FileAnalyzer::OpenFile() {
	
	this->original->openFile();
	this->modified->openFile();
	const time_t timet = std::chrono::system_clock::to_time_t(std::chrono::system_clock::now());
	this->temp = new FileItem(originalFilepath + "/temp/", std::ctime(&timet) + original->getFilename(), std::ios::binary);
	
}

void Workstation::Analytic::FileAnalyzer::CloseFile() const{
	original->closeFile();
	modified->closeFile();
	temp->closeFile();
}
