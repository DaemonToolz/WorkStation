#include "Analytic/FileAnalyzer.h"
#include <chrono>
#include <Windows.h>


void Workstation::Analytic::FileAnalyzer::CreateShadowCopy(const char* filepath){
	// TODO Copy the original file
}

void Workstation::Analytic::FileAnalyzer::AnalyseFile() {
	// TODO Analyze both files
}

void Workstation::Analytic::FileAnalyzer::Decide(FileAction action) {
	// TODO Chose between Merge / Keep ...
}

void Workstation::Analytic::FileAnalyzer::CreateBackup(int fileno){
	//std::ofstream backup = std::ofstream(backupPath + originalFilename + std::to_string(fileno));
	std::string from = originalFilepath + originalFilename, to = backupPath + originalFilename + std::to_string(fileno);
	CopyFile(from.c_str(), to.c_str(), FALSE);
}

void Workstation::Analytic::FileAnalyzer::OpenFile() {
	this->original = std::ifstream(this->originalFilepath + this->originalFilename, std::ios::in);
	this->modified = std::ifstream(this->tempFilepath + this->newFilename, std::ios::in);
	const time_t timet = std::chrono::system_clock::to_time_t(std::chrono::system_clock::now());
	this->temp = std::ifstream(std::ctime(&timet) + (this->tempFilepath + this->newFilename), std::ios::in);
}

void Workstation::Analytic::FileAnalyzer::CloseFile(){
	original.close();
	modified.close();
	temp.close();
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
