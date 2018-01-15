#include "memory.h"

namespace AldosV {
	CMemory::CMemory() {

	}

	CMemory::~CMemory() {

	}

	float CMemory::ReadFloat(intptr_t address) {
		float result = 0;
		SIZE_T num_read;

		ReadProcessMemory(GetCurrentProcess(),
			(void*)address,
			&result,
			sizeof(float),
			&num_read);

		return result;
	}

	int CMemory::ReadInt(intptr_t address) {
		int result = 0;
		SIZE_T num_read;

		ReadProcessMemory(GetCurrentProcess(),
			(void*)address,
			&result,
			sizeof(int),
			&num_read);

		return result;
	}

	unsigned int CMemory::ReadUInt(intptr_t address) {
		unsigned int result = 0;
		SIZE_T num_read;

		ReadProcessMemory(GetCurrentProcess(),
			(void*)address,
			&result,
			sizeof(unsigned int),
			&num_read);

		return result;
	}

	void CMemory::WriteInt(intptr_t address, int value) {
		SIZE_T written = 0;

		WriteProcessMemory(GetCurrentProcess(),
			(void*)address,
			&value,
			sizeof(int),
			&written);
	}

	void CMemory::WriteFloat(intptr_t address, float value) {
		SIZE_T written = 0;

		WriteProcessMemory(GetCurrentProcess(),
			(void*)address,
			&value,
			sizeof(float),
			&written);
	}

	bool CMemory::ConfirmSignature(char* pcCurrentByte, const char* pszSignature, const char* pszMask, int iCount) {
		int iOffset = 0;

		for (int i = 0; i < iCount; i++, iOffset++) {
			char* check = pcCurrentByte + iOffset;
			if (pszMask[i] == 'x') {
				continue;
			}

			if (*check != pszSignature[i]) {
				return false;
			}
		}
		return true;
	}

	void* CMemory::SignatureScan(HMODULE hModule, const char* pszSignature, const char* pszMask, intptr_t start) {
		if (pszSignature == NULL || pszMask == NULL)
			return NULL;

		MODULEINFO info;
		if (GetModuleInformation(GetCurrentProcess(), hModule, &info, sizeof(MODULEINFO))) {
			DWORD dwAmountToScan = info.SizeOfImage;
			DWORD dwBase = (DWORD)info.lpBaseOfDll;

			int size = strlen(pszMask);

			intptr_t s = (start == 0) ? dwBase : start + size;

			char* pcCurrentByte = (char*)s;
			for (int i = s; i < dwAmountToScan; i++, pcCurrentByte++) {
				if (*pcCurrentByte == pszSignature[0] && ConfirmSignature(pcCurrentByte, pszSignature, pszMask, size)) {
					return (void*)(pcCurrentByte + size);
				}
			}
		}
		return NULL;
	}
}