#pragma once

namespace Workstation {
	struct FileCompareResult final {
		int StartingLine, EndLine;

		char *OriginalLineContent, *ModifiedContent; 
	};

}