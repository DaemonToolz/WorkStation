#define _CRT_SECURE_NO_WARNINGS

// Temporary solution
#include "Analytic/FileAnalyzer.h"

#include <chrono>
#include <Windows.h>



void Workstation::Analytic::FileAnalyzer::CreateShadowCopy(const char* filepath){
	//temp = std::ifstream(filepath);
}

void Workstation::Analytic::FileAnalyzer::AnalyseFile() {
	int maxLines = 0;

	std::cout << "Opening original and modified files..." << std::endl;
	original->discoverFile();
	modified->discoverFile();
	std::cout << "Done" << std::endl;
	std::cout << "Comparing sequences" << std::endl;
	original->compareTo(*modified, comparator);

	std::cout << "Done" << std::endl;
}

void Workstation::Analytic::FileAnalyzer::Decide(FileAction action) {
	// TODO Chose between Merge / Keep ...
}

void Workstation::Analytic::FileAnalyzer::CreateBackup(int fileno){
	//std::ofstream backup = std::ofstream(backupPath + originalFilename + std::to_string(fileno));
	std::string from = 
		original->getAbsolutePath(), 
			to = originalFilepath + "/temp/" + original->getFilename() + std::to_string(fileno);
	
	CopyFile(from.c_str(), to.c_str(), FALSE);
}

void Workstation::Analytic::FileAnalyzer::OpenFile() {
	
	this->original->openFile();
	this->modified->openFile();
	const time_t timet = std::chrono::system_clock::to_time_t(std::chrono::system_clock::now());
	this->temp = new FileItem(originalFilepath + "/temp/", std::ctime(&timet) + original->getFilename(), std::ios::binary);
	
}

void Workstation::Analytic::FileAnalyzer::CloseFile(){
	original->closeFile();
	modified->closeFile();
	temp->closeFile();
}

void Workstation::Analytic::FileAnalyzer::Merge()
{
}

void Workstation::Analytic::FileAnalyzer::Overwrite()
{
}

void Workstation::Analytic::FileAnalyzer::SelectChanges(int*)
{
}
