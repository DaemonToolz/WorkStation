// This is the main DLL file.

#include "stdafx.h"

#include "WorkstationClrVersionner.h"

WorkstationClrVersionner::AnalyzerWrapper::AnalyzerWrapper(String^ file, String^ newfile, String^ path)
{
	analyzer = new FileAnalyzer(
		marshal_as< std::string >(file), 
		marshal_as< std::string >(newfile), 
		marshal_as< std::string >(path));
	

}

void WorkstationClrVersionner::AnalyzerWrapper::OpenFileUnsafe()
{
	analyzer->OpenFile();
}

FileCompareResultList WorkstationClrVersionner::AnalyzerWrapper::AnalyzeFileUnsafe(){
	analyzer->CreateBackup(0);
	analyzer->AnalyseFile();
	return analyzer->getResultListCopy();
}


void WorkstationClrVersionner::AnalyzerWrapper::CloseFileUnsafe()
{
	analyzer->CloseFile();
}

WorkstationClrVersionner::AnalyzeResultList^ WorkstationClrVersionner::AnalyzerWrapper::AnalyzeFile(){
	return gcnew AnalyzeResultList(AnalyzeFileUnsafe());
}

WorkstationClrVersionner::AnalyzerWrapper::~AnalyzerWrapper(){
	this->!AnalyzerWrapper();
}

WorkstationClrVersionner::AnalyzerWrapper::!AnalyzerWrapper()
{
	analyzer->CloseFile();
	delete analyzer;
}
