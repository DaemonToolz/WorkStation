

#include <ostream>
#include <iostream>

#include "FileAnalyzer.h"

using namespace std;
void main()
{
	
	cout << "Initializing Analyzer" << endl;
	Workstation::Analytic::FileAnalyzer* analyzer = new Workstation::Analytic::FileAnalyzer("a.txt", "b.txt", "C:\\Users\\macie\\Source\\Repos\\WorkStation\\WorkstationClient\\Debug\\");

	cout << "Done" << endl;

	cout << "Discovering files" << endl;
	analyzer->AnalyseFile();

	cout << "Discovery finished" << endl;

	auto& resultList = analyzer->getResultListCopy();

	auto firstItem = resultList.pop_front();

	cout << "There are " << abs(firstItem->AdditionalLines) << " new / missing lines" << endl;

	for (auto iter = resultList.begin(); iter != resultList.end(); ++iter) {
		cout << (iter->changeType) << endl;
		cout << iter->StartingLine << " - " << iter->EndLine << endl;
		cout << iter->OriginalLineContent << endl;
		cout << iter->ModifiedContent << std::endl;
		
		if (!iter->ChangeSet.empty())
			for (auto subiter = iter->ChangeSet.begin(); subiter != iter->ChangeSet.end(); ++subiter)
				cout << "----+ " << (*subiter) << endl;

		std::cout << std::endl;
	}

	getchar();
}
