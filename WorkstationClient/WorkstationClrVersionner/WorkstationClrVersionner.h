// WorkstationClrVersionner.h

#pragma once
#include "FileAnalyzer.h"

#include "FileCompareResultList.h"
#include <string>
#include <list>
#include <msclr/marshal_cppstd.h>

using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace Workstation;
using namespace Workstation::Analytic;
using namespace msclr::interop;

namespace WorkstationClrVersionner {

	public ref class AnalyzeResult {
	protected:
		int _BeginLine, _Endline;
		List<String^>^ _Results;
		int _ChangeType;

	public:
		AnalyzeResult(FileCompareResult& originalResult) {
			_Results = gcnew List<String^>();
			_BeginLine = originalResult.StartingLine;
			_Endline = originalResult.EndLine;
			_ChangeType = originalResult.changeType;

			for each(auto& input in originalResult.ChangeSet)
				_Results->Add(marshal_as<String^>(input.c_str()));

		}

		property List<String^>^ Results {
			List<String^>^ get() { return _Results; }
		}

		property int ChangeType {
			int get() { return _ChangeType; }
		}

		property int BeginLine {
			int get() { return _BeginLine; }
		}
		property int EndLine {
			int get() { return _Endline; }
		}
	};


	public ref class AnalyzeResultList{
	private:
		List<AnalyzeResult^>^ _Results;
	
	public:
		AnalyzeResultList(FileCompareResultList originalList){
			_Results = gcnew List<AnalyzeResult^>();

			for each(auto element in originalList){
				_Results->Add(gcnew AnalyzeResult(element));
			}
		}

		property List<AnalyzeResult^>^ Results {
			List<AnalyzeResult^>^ get() { return _Results; }
		}
	};

	public ref class AnalyzerWrapper{
	private:
		FileAnalyzer* analyzer;

		void OpenFileUnsafe();
		FileCompareResultList AnalyzeFileUnsafe();
		void CloseFileUnsafe();
		bool IsOpenUnsafe();

	public:
		AnalyzerWrapper(String^ oldfile, String^ newfile, String^ path);

		void OpenFile() { OpenFileUnsafe(); }
		void CloseFile() { CloseFileUnsafe(); }
		bool IsOpen() { return IsOpenUnsafe(); }

		AnalyzeResultList^ AnalyzeFile();

		~AnalyzerWrapper();
		!AnalyzerWrapper();

	};
}
