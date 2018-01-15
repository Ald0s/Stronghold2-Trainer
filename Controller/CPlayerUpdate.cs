/*
== Stronghold 2 Trainer ==
A trainer for the old RTS game, Stronghold 2; originally released in 2005.

By Alden Viljoen
https://github.com/ald0s
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller {
    public class CPlayerUpdate {
        public int iLocalPlayerNum;

        public PlayerUpdate_t[] players;
        public PlayerUpdate_t local;

        public CPlayerUpdate(int _num, PlayerUpdate_t[] _plys, PlayerUpdate_t _local) {
            iLocalPlayerNum = _num;

            this.players = _plys;
            this.local = _local;
        }

        public PlayerUpdate_t[] GetPlayerList() {
            return this.players;
        }

        public int GetTotalResources(int iPlayerID) {
            PlayerUpdate_t ply;
            if (!GetPlayer(iPlayerID, out ply))
                return -1;

            return ply.iWood + ply.iStone + ply.iIron + ply.iWheat + ply.iGrain + ply.iHops + ply.iBeer + ply.iGrapes + ply.iOil + ply.iCandles + ply.iWool + ply.iLinen;
        }

        public int GetTotalFood(int iPlayerID) {
            PlayerUpdate_t ply;
            if (!GetPlayer(iPlayerID, out ply))
                return -1;

            return ply.iApples + ply.iBread + ply.iCheese + ply.iMeat;
        }

        public int GetTotalWeapons(int iPlayerID) {
            PlayerUpdate_t ply;
            if (!GetPlayer(iPlayerID, out ply))
                return -1;

            return ply.iBow + ply.iCrossbow + ply.iSword + ply.iMace + ply.iPike + ply.iSpear + ply.iChestplate + ply.iLeather;
        }

        private bool GetPlayer(int iPlayerID, out PlayerUpdate_t ply) {
            for(int i = 0; i < players.Length; i++) {
                if (players[i].iPlayerNum == iPlayerID) {
                    ply = players[i];
                    return true;
                }
            }

            ply = new PlayerUpdate_t();
            return false;
        }
    }
}
