#include "jmp.h"

namespace AldosV {
	CJmpHook::CJmpHook() {

	}

	CJmpHook::~CJmpHook() {

	}

	void CJmpHook::Hook(void* target_memory, void* callback) {
		original_target_memory = target_memory;

		SYSTEM_INFO system_info;
		GetSystemInfo(&system_info);
		DWORD dwPageSize = system_info.dwPageSize;
		inter = (char*)VirtualAlloc(NULL, dwPageSize, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);

		payload[0] = 0xE9;
		payload[5] = 0xC3;

		intptr_t distance = (intptr_t)inter - (intptr_t)target_memory - 5;
		memcpy(&payload[1], &distance, 4);

		// Save the beginning of the target function, and write the jmp to intermediary.
		DWORD old;
		VirtualProtect(target_memory, 6, PAGE_EXECUTE_READWRITE, &old);
		memcpy(original, target_memory, 6);
		memcpy(target_memory, payload, 6);
		VirtualProtect(target_memory, 6, old, 0);

		// Now we can populate our intermediary.
		// The first 5 bytes must send control to the module.
		distance = (intptr_t)callback - (intptr_t)inter - 5;
		memcpy(&payload[1], &distance, 4);

		// Directly after, process the first 6 bytes of the old function.
		memcpy(&payload[6], original, 6);

		// Finally, send control back to the target memory.
		distance = (intptr_t)target_memory - (intptr_t)inter - 11;
		memcpy(&payload[13], &distance, 4);
		payload[12] = 0xE9;
		payload[17] = 0xC3;

		memcpy(inter, payload, 18);

		// Set original function to the location of the 6th indicy. This is where the original function starts.
		// Control will flow through these 6 bytes and hit the jump hook back to the original function, after 6 bytes.
		original_func = (void*)&inter[6]; 

		VirtualProtect(inter,
			18,
			PAGE_EXECUTE_READ,
			&old);
	}

	void* CJmpHook::GetOriginal() {
		return original_func;
	}

	void CJmpHook::Remove() {
		DWORD old;
		VirtualProtect(original_target_memory, 6, PAGE_EXECUTE_READWRITE, &old);
		memcpy(original_target_memory, original, 6);
		VirtualProtect(original_target_memory, 6, old, 0);

		VirtualFree(inter, 18, MEM_RELEASE);
	}
}