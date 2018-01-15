#include <Windows.h>
#include <iostream>
#include <string>
#include <sstream>

#include "jmp.h"
#include "player.h"

#define PLAYER_NUM 15

struct Update_t {
	int localply_id;

	uintptr_t localply;
	uintptr_t slotsptr;
};

struct ClientSet_t {
	int ply_id;
	int offset;

	int value;
	float fvalue;
};

struct SetMacro_t {
	int player;

	int macro;
	bool enabled;
};

struct GetMacroState_t {
	int player;

	int famine;
	int noweapons;
	int noresources;
	int infinite;
	int invincible;
};

void StartModule(HINSTANCE hinstDLL);
DWORD WINAPI StateThread();
void TargetAllPlayers(intptr_t lst_start);
void EndModule();

extern "C" __declspec(dllexport) bool GetUpdate(LPVOID lpParam);
extern "C" __declspec(dllexport) bool RequestSet(LPVOID lpParam);

extern "C" __declspec(dllexport) bool GetMacroState(LPVOID lpParam);
extern "C" __declspec(dllexport) bool SetMacro(LPVOID lpParam);

typedef int(__fastcall* hEntityDamage)(void* ecx, void* edx, float flt, int it);
int __fastcall hook_EntityDamage(void* ecx, void* edx, float flt, int it);

bool shouldrun = true;

AldosV::CMemory mem;
AldosV::CJmpHook hkEntDamage;
CPlayer local;
CPlayer slots[PLAYER_NUM];

DWORD WINAPI InitialiseStateThread();
void CheckForCompatibleVersion(HMODULE base);

const char* pszSupportedVersion = "";

BOOL WINAPI DllMain(
	_In_ HINSTANCE hinstDLL,
	_In_ DWORD     fdwReason,
	_In_ LPVOID    lpvReserved
) {
	switch (fdwReason) {
	case DLL_PROCESS_ATTACH:
		StartModule(hinstDLL);
		break;

	case DLL_PROCESS_DETACH:
		EndModule();
		break;
	}

	return TRUE;
}
void StartModule(HINSTANCE hinstDLL) {
	AllocConsole();
	freopen("CONOUT$", "w", stdout);

	printf("Attempting to get handle to Stronghold2.exe ...\n");
	HMODULE game = GetModuleHandleA("Stronghold2.exe");

	if (!game) {
		printf("Failed to get handle to main module (returned null)\n");
		return;
	}

	printf("Stronghold 2 Trainer - github.com/ald0s\nChecking for compatability...\n\n");
	CheckForCompatibleVersion(game);

	uintptr_t localplayer = (uintptr_t)game + 0x6E7C60;
	local.TargetPlayer(localplayer);
	TargetAllPlayers(localplayer - 0x84);

	hEntityDamage dmg_func = (hEntityDamage)((uintptr_t)game + (0x385380));
	hkEntDamage.Hook((void*)dmg_func, (void*)&hook_EntityDamage);

	CreateThread(0, 0, (LPTHREAD_START_ROUTINE)StateThread, 0, 0, 0);
}

void EndModule() {
	printf("Closing module ...\n");
	FreeConsole();
	shouldrun = false;
}

// The function that applies damage to the GlorwWorm::Entity class.
int __fastcall hook_EntityDamage(void* ecx, void* edx, float flt, int it) {
	hEntityDamage damage_ent = (hEntityDamage)hkEntDamage.GetOriginal();
	float damage = flt;

	int player_num = mem.ReadInt((uintptr_t)ecx + 0x58);
	for (int i = 0; i < PLAYER_NUM; i++) {
		if (slots[i].GetPlayerNum() == player_num) {
			damage = slots[i].EntityDamaged(ecx, damage);
			break;
		}
	}

	return damage_ent(ecx, edx, damage, it);
}

void TargetAllPlayers(intptr_t lst_start) {
	for (int i = 0; i < PLAYER_NUM; i++) {
		intptr_t ptr = lst_start + (i * 4);
		slots[i].TargetPlayer(ptr);
	}
}

bool GetUpdate(LPVOID lpParam) {
	if (!lpParam)
		lpParam = new Update_t();

	Update_t* update = (Update_t*)lpParam;
	update->localply_id = local.GetPlayerNum();
	update->localply = (uintptr_t)local.GetPointerToStatus();

	uintptr_t* players = new uintptr_t[PLAYER_NUM];
	for (int i = 0; i < PLAYER_NUM; i++) {
		players[i] = (uintptr_t)slots[i].GetPointerToStatus();
	}
	update->slotsptr = (uintptr_t)players;

	return true;
}

CPlayer* GetPlayer(int id) {
	for (int i = 0; i < PLAYER_NUM; i++) {
		if (slots[i].GetPlayerNum() == id)
			return &slots[i];
	}
	return NULL;
}

bool RequestSet(LPVOID lpParam) {
	if (!lpParam)
		return false;

	ClientSet_t* order = (ClientSet_t*)lpParam;
	// Process order.
}

bool SetMacro(LPVOID lpParam) {
	if (!lpParam)
		return false;

	SetMacro_t* macro = (SetMacro_t*)lpParam;
	CPlayer* ply = GetPlayer(macro->player);
	
	if (!ply)
		return false; 

	ply->SetMacroEnabled(macro->macro, macro->enabled);
	return true;
}

bool GetMacroState(LPVOID lpParam) {
	if (!lpParam)
		return false;

	GetMacroState_t* macro = (GetMacroState_t*)lpParam;
	CPlayer* ply = GetPlayer(macro->player);

	if (!ply)
		return false;

	macro->famine = (int)ply->IsMacroEnabled(Macros::Starve);
	macro->noweapons = (int)ply->IsMacroEnabled(Macros::NoWeapons);
	macro->noresources = (int)ply->IsMacroEnabled(Macros::NoResources);
	macro->infinite = (int)ply->IsMacroEnabled(Macros::InfiniteResources);
	macro->invincible = (int)ply->IsMacroEnabled(Macros::Invincible);

	return true;
}

void CheckForCompatibleVersion(HMODULE base) {
	
}

DWORD WINAPI StateThread() {
	shouldrun = true;
	while (shouldrun) {
		for (int i = 0; i < PLAYER_NUM; i++) {
			slots[i].UpdateState();
		}
	}

	return 0;
}