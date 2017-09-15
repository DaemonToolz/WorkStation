#pragma once
#include <list>

namespace Workstation {

	WS_VERSIONER_API enum ChangeType
	{
		LineChange = 0,
		NewContent,
		RemovedContent
	};

	WS_VERSIONER_API struct FileCompareResult final {
		int StartingLine, EndLine, AdditionalLines;
		ChangeType changeType;
		
		// Single line change
		std::string OriginalLineContent, ModifiedContent; 

		std::list<std::string> ChangeSet;

	};

}
