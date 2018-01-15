/*
== Stronghold 2 Trainer ==
A trainer for the old RTS game, Stronghold 2; originally released in 2005.

By Alden Viljoen
https://github.com/ald0s

player.h
Handles a player.
This class is responsible for actualising macros requested by the client,
and keeping the player's ingame info updated with the client.
*/

#pragma once
#include <iostream>

#include "memory.h"

struct PlayerUpdate_t {
	int iPlayerNum;

	int iWood;
	int iStone;
	int iIron;
	int iWheat;
	int iGrain;
	int iHops;
	int iBeer;
	int iGrapes;
	int iOil;
	int iCandles;
	int iWool;
	int iLinen;

	int iApples;
	int iBread;
	int iCheese;
	int iMeat;

	float fGold;
	int iHonour;
	
	int iBow;
	int iCrossbow;
	int iSword;
	int iMace;
	int iPike;
	int iSpear;
	int iChestplate;
	int iLeather;
};

namespace Resources {
	enum Basic_e {
		Wood	= 0x4,
		Stone	= 0x8,
		Iron	= 0xC,
		Wheat	= 0x10,
		Grain	= 0x14,
		Hops	= 0x18,
		Beer	= 0x1C,
		Grapes	= 0x20,
		Oil		= 0x24,
		Candles	= 0x28,
		Wool	= 0x2C,
		Linen	= 0x30
	};

	enum Granary_e {
		Apples	= 0x58,
		Bread	= 0x5C,
		Cheese	= 0x60,
		Meat	= 0x64
	};

	enum Special_e {
		Gold	= 0xB4,
		Honour	= -0xF40
	};

	enum Weapons_e {
		Bow			= 0x78,
		Crossbow	= 0x7C,
		Sword		= 0x80,
		Mace		= 0x84,
		Pike		= 0x88,
		Spear		= 0x8C,
		Chestplate	= 0x90,
		Leather		= 0x94
	};
}

namespace Macros {
	enum Macros_e {
		Starve = 0,
		NoWeapons = 1,
		NoResources = 2,
		InfiniteResources = 3,
		Invincible = 4
	};
}

class CPlayer
{
public:
	CPlayer();
	~CPlayer();

	bool IsMacroEnabled(int macro);
	void SetMacroEnabled(int macro, bool enabled);

	int GetPlayerNum();
	void UpdateStatus();
	PlayerUpdate_t* GetPointerToStatus();

	void TargetPlayer(intptr_t staticAddress);

	int GetResourceAmount(int offset);
	float GetResourceAmountF(int offset);

	void SetResourceAmount(int offset, int value);
	void SetResourceAmount(int offset, float value);

	float EntityDamaged(void* entAddress, float flDamage);

	void UpdateState();

private:
	int iPlayerNum;

	intptr_t addr_static;
	intptr_t addr_dyn;

	PlayerUpdate_t status;
	AldosV::CMemory mem;

	bool should_starve;
	bool should_lose_weapons;
	bool should_lose_res;
	bool should_infinite_res;
	bool should_inv;

	void Update();
	void UpdateMyself();
};

