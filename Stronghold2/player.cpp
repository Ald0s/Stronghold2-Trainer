#include "stdafx.h"
#include "player.h"

CPlayer::CPlayer() {

}

CPlayer::~CPlayer() {

}

void CPlayer::TargetPlayer(intptr_t staticAddress) {
	this->addr_static = staticAddress;
	this->Update();
}

PlayerUpdate_t* CPlayer::GetPointerToStatus() {
	this->UpdateStatus();
	return &status;
}

void CPlayer::Update() {
	unsigned int dyn = mem.ReadUInt(addr_static);
	this->addr_dyn = dyn;

	this->UpdateMyself();
}

void CPlayer::UpdateMyself() {
	this->iPlayerNum = mem.ReadInt(addr_dyn + 0x8);
}

int CPlayer::GetResourceAmount(int offset) {
	this->Update();

	int value = mem.ReadInt((addr_dyn + 0xF5C) + offset);
	return value;
}

float CPlayer::GetResourceAmountF(int offset) {
	this->Update();

	float value = mem.ReadFloat((addr_dyn + 0xF5C) + offset);
	return value;
}

void CPlayer::SetResourceAmount(int offset, int value) {
	this->Update();

	mem.WriteInt((addr_dyn + 0xF5C) + offset, value);
}

void CPlayer::SetResourceAmount(int offset, float value) {
	this->Update();

	mem.WriteFloat((addr_dyn + 0xF5C) + offset, value);
}

int CPlayer::GetPlayerNum() {
	this->Update();

	return this->iPlayerNum;
}

void CPlayer::UpdateStatus() {
	this->Update();

	PlayerUpdate_t ply;
	ply.iPlayerNum = iPlayerNum;

	ply.iWood = GetResourceAmount(Resources::Wood);
	ply.iStone = GetResourceAmount(Resources::Stone);
	ply.iIron = GetResourceAmount(Resources::Iron);
	ply.iWheat = GetResourceAmount(Resources::Wheat);
	ply.iGrain = GetResourceAmount(Resources::Grain);
	ply.iHops = GetResourceAmount(Resources::Hops);
	ply.iBeer = GetResourceAmount(Resources::Beer);
	ply.iGrapes = GetResourceAmount(Resources::Grapes);
	ply.iOil = GetResourceAmount(Resources::Oil);
	ply.iCandles = GetResourceAmount(Resources::Candles);
	ply.iWool = GetResourceAmount(Resources::Wool);
	ply.iLinen = GetResourceAmount(Resources::Linen);

	ply.iApples = GetResourceAmount(Resources::Apples);
	ply.iBread = GetResourceAmount(Resources::Bread);
	ply.iCheese = GetResourceAmount(Resources::Cheese);
	ply.iMeat = GetResourceAmount(Resources::Meat);

	ply.fGold = GetResourceAmountF(Resources::Gold);
	ply.iHonour = GetResourceAmount(Resources::Honour);

	ply.iBow = GetResourceAmount(Resources::Bow);
	ply.iCrossbow = GetResourceAmount(Resources::Crossbow);
	ply.iSword = GetResourceAmount(Resources::Sword);
	ply.iMace = GetResourceAmount(Resources::Mace);
	ply.iPike = GetResourceAmount(Resources::Pike);
	ply.iSpear = GetResourceAmount(Resources::Spear);
	ply.iChestplate = GetResourceAmount(Resources::Chestplate);
	ply.iLeather = GetResourceAmount(Resources::Leather);

	this->status = ply;
}

bool CPlayer::IsMacroEnabled(int macro) {
	switch (macro) {
	case Macros::Starve:
		return should_starve;

	case Macros::NoWeapons:
		return should_lose_weapons;

	case Macros::NoResources:
		return should_lose_res;

	case Macros::InfiniteResources:
		return should_infinite_res;

	case Macros::Invincible:
		return should_inv;

	default:
		return false;
	}
}

void CPlayer::SetMacroEnabled(int macro, bool enabled) {
	switch (macro) {
	case Macros::Starve:
		should_starve = enabled;
		break;

	case Macros::NoWeapons:
		should_lose_weapons = enabled;
		break;

	case Macros::NoResources:
		should_lose_res = enabled;
		break;

	case Macros::InfiniteResources:
		should_infinite_res = enabled;
		break;

	case Macros::Invincible:
		should_inv = enabled;
		break;
	}
}

float CPlayer::EntityDamaged(void* entAddress, float flDamage) {
	if (should_inv) {
		return 0;
	}
	return flDamage;
}

// For updating things like macros.
void CPlayer::UpdateState() {
	this->Update();

	if (should_starve) {
		SetResourceAmount(Resources::Apples, 0);
		SetResourceAmount(Resources::Bread, 0);
		SetResourceAmount(Resources::Cheese, 0);
		SetResourceAmount(Resources::Meat, 0);
	}

	if (should_lose_weapons) {
		SetResourceAmount(Resources::Bow, 0);
		SetResourceAmount(Resources::Crossbow, 0);
		SetResourceAmount(Resources::Sword, 0);
		SetResourceAmount(Resources::Mace, 0);
		SetResourceAmount(Resources::Pike, 0);
		SetResourceAmount(Resources::Spear, 0);
		SetResourceAmount(Resources::Chestplate, 0);
		SetResourceAmount(Resources::Leather, 0);
	}

	if (should_lose_res) {
		SetResourceAmount(Resources::Wood, 0);
		SetResourceAmount(Resources::Stone, 0);
		SetResourceAmount(Resources::Iron, 0);
		SetResourceAmount(Resources::Wheat, 0);
		SetResourceAmount(Resources::Grain, 0);
		SetResourceAmount(Resources::Hops, 0);
		SetResourceAmount(Resources::Beer, 0);
		SetResourceAmount(Resources::Grapes, 0);
		SetResourceAmount(Resources::Oil, 0);
		SetResourceAmount(Resources::Candles, 0);
		SetResourceAmount(Resources::Wool, 0);
		SetResourceAmount(Resources::Linen, 0);
	}

	if (should_infinite_res) {
		SetResourceAmount(Resources::Honour, 100000);
		SetResourceAmount(Resources::Gold, (float)100000);

		SetResourceAmount(Resources::Apples, 8000);
		SetResourceAmount(Resources::Bread, 8000);
		SetResourceAmount(Resources::Cheese, 8000);
		SetResourceAmount(Resources::Meat, 8000);

		SetResourceAmount(Resources::Bow, 8000);
		SetResourceAmount(Resources::Crossbow, 8000);
		SetResourceAmount(Resources::Sword, 8000);
		SetResourceAmount(Resources::Mace, 8000);
		SetResourceAmount(Resources::Pike, 8000);
		SetResourceAmount(Resources::Spear, 8000);
		SetResourceAmount(Resources::Chestplate, 8000);
		SetResourceAmount(Resources::Leather, 8000);

		SetResourceAmount(Resources::Wood, 8000);
		SetResourceAmount(Resources::Stone, 8000);
		SetResourceAmount(Resources::Iron, 8000);
		SetResourceAmount(Resources::Wheat, 8000);
		SetResourceAmount(Resources::Grain, 8000);
		SetResourceAmount(Resources::Hops, 8000);
		SetResourceAmount(Resources::Beer, 8000);
		SetResourceAmount(Resources::Grapes, 8000);
		SetResourceAmount(Resources::Oil, 8000);
		SetResourceAmount(Resources::Candles, 8000);
		SetResourceAmount(Resources::Wool, 8000);
		SetResourceAmount(Resources::Linen, 8000);
	}
}