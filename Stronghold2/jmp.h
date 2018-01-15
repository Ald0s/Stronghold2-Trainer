/*
== Stronghold 2 Trainer ==
A trainer for the old RTS game, Stronghold 2; originally released in 2005.

By Alden Viljoen
https://github.com/ald0s

jmp.h
A modified version of my earlier JMP hook class.
This class utilises intermediary code to faciliate control flow to the hook function and optionally the original function.

Intermediary memory breakdown:
Length: 18 bytes.

[
	E9 00 00 00 00 C3		; Jump to module code and return
	00 00 00 00 00 00		; 6 bytes of the original function.
	E9 00 00 00 00 C3		; Jump back to the original function code, after the initial jump hook.
]
*/

#pragma once
#include <Windows.h>
#include <iostream>

namespace AldosV {
	class CJmpHook {
	public:
		CJmpHook();
		~CJmpHook();

		void Hook(void* target_memory, void* callback);
		void* GetOriginal();
		void Remove();

	private:
		char payload[18] = { 0 };
		char original[6] = { 0 };

		char* inter = 0;
		void* original_target_memory = 0;
		void* original_func = 0;
	};
}