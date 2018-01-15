/*
== Stronghold 2 Trainer ==
A trainer for the old RTS game, Stronghold 2; originally released in 2005.

By Alden Viljoen
https://github.com/ald0s

memory.h
Just some macros for reading/writing memory.
Makes stuff easier as you go along.
*/

#pragma once
#include <Windows.h>
#include <Psapi.h>
#include <iostream>

namespace AldosV {
	class CMemory
	{
	public:
		CMemory();
		~CMemory();

		void WriteInt(intptr_t address, int value);
		void WriteFloat(intptr_t address, float value);

		float ReadFloat(intptr_t address);
		int ReadInt(intptr_t address);
		unsigned int ReadUInt(intptr_t address);

		void* SignatureScan(HMODULE hModule, const char* pszSignature, const char* pszMask, intptr_t start = 0);

	private:
		bool ConfirmSignature(char* pcCurrentByte, const char* pszSignature, const char* pszMask, int iCount);
	};
}