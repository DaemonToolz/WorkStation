#pragma once
#include <list>

namespace Workstation {

	enum ChangeType
	{
		LineChange = 0,
		NewContent,
		RemovedContent
	};

	struct FileCompareResult final {
		int StartingLine, EndLine, AdditionalLines;
		ChangeType changeType;
		
		// Single line change
		char *OriginalLineContent, *ModifiedContent; 

		std::list<std::string> ChangeSet;

	};

}
