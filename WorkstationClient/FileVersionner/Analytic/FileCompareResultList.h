#pragma once

#include "FileCompareResult.h"
#include <vector>
#include <type_traits>
#include <stdexcept>
namespace Workstation {
	 
	class FileCompareResultList  final : public std::vector<FileCompareResult> {
	public:
		FileCompareResultList():vector<FileCompareResult>(){

		}

		~FileCompareResultList()
		{
			vector::~vector();
		}
		
		
		const FileCompareResult* pop_front() {
			if (!empty()) {
				FileCompareResult frontObject = *(begin());
				erase(begin());
				return &frontObject;
			}
			return nullptr;
		}

	private:


	protected:
	};

}