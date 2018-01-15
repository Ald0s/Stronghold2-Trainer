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

using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using IPPC;

namespace Controller {
    public class CControl {
        public const int PLAYER_NUM = 15;

        public delegate void UpdateFound_Delegate(CPlayerUpdate update);
        public event UpdateFound_Delegate UpdateFound;

        public struct Update_t {
            public int iLocalPlayerNum;

            public IntPtr ptrLocalPlayer;
            public IntPtr ptrSlots;
        }

        public struct SetMacro_t {
            public int iPlayerNum;

            public int iMacro;
            public bool bEnabled;
        };

        public struct GetMacroState_t {
            public int iPlayerNum;

            public int bStarve;
            public int bNoWeapons;
            public int bNoResources;
            public int bInfiniteResources;
            public int bInvincibleTroops;
        };

        private CIPPC ippc;

        public CControl(CIPPC cIPPC) {
            this.ippc = cIPPC;
        }

        // Retrieves a current state of all player slots.
        public void GetUpdate(IntPtr ptrHandle, IntPtr ptrBaseAddress) {
            // Create an Update_t in foreign process.
            Update_t update = new Update_t();
            IntPtr ptrUpdate = ippc.WriteStruct(ptrHandle, update);

            // Pass pointer to Update_t to GetUpdate C++.
            IntPtr ptrGetUpdate = ippc.GetRemoteProcAddress(ptrHandle, ptrBaseAddress, "GetUpdate");
            if(ptrGetUpdate == IntPtr.Zero) {
                Console.WriteLine("Failed to get update. GetUpdate exported function not found!");
                return;
            }

            IntPtr ptrThread = ippc.Run(ptrHandle, ptrGetUpdate, ptrUpdate);
            uint uiResult = ippc.GetThreadReturnValue(ptrThread);

            // Read Update_t from foreign process.
            update = (Update_t)ippc.ReadStruct(ptrHandle, ptrUpdate, update);

            PlayerUpdate_t ply = new PlayerUpdate_t();
            object local_struct = ippc.ReadStruct(ptrHandle, update.ptrLocalPlayer, ply);

            uint[] uiPlayers = ippc.ReadUIntArray(ptrHandle, update.ptrSlots, PLAYER_NUM);
            PlayerUpdate_t[] players = new PlayerUpdate_t[PLAYER_NUM];
            for(int i = 0; i < PLAYER_NUM; i++) {
                players[i] = (PlayerUpdate_t)ippc.ReadStruct(ptrHandle, new IntPtr(uiPlayers[i]), ply);
            }

            CPlayerUpdate result = new CPlayerUpdate(update.iLocalPlayerNum, players, (PlayerUpdate_t)local_struct);
            UpdateFound(result);

            // Process & Destory memory.
            ippc.FreeMemory(ptrHandle, update.ptrSlots);
            ippc.FreeMemory(ptrHandle, ptrUpdate);
        }

        // Gets the set state of all macros, so we can update the UI.
        public GetMacroState_t GetMacroState(IntPtr ptrHandle, IntPtr ptrBaseAddress, int player) {
            GetMacroState_t macro = new GetMacroState_t();
            macro.iPlayerNum = player;

            IntPtr ptrMacro = ippc.WriteStruct(ptrHandle, macro);
            IntPtr ptrGetMacroState = ippc.GetRemoteProcAddress(ptrHandle, ptrBaseAddress, "GetMacroState");
            if(ptrGetMacroState == IntPtr.Zero) {
                Console.WriteLine("Failed to get macro state for #" + player + ". GetMacroState exported function not found!");
                return macro;
            }

            IntPtr ptrThread = ippc.Run(ptrHandle, ptrGetMacroState, ptrMacro);
            ippc.GetThreadReturnValue(ptrThread);

            macro = (GetMacroState_t)ippc.ReadStruct(ptrHandle, ptrMacro, macro);
            ippc.FreeMemory(ptrHandle, ptrMacro);

            return macro;
        }

        // Requests a change of active state for a specified macro.
        public void SetMacroEnabled(IntPtr ptrHandle, IntPtr ptrBaseAddress, int player, int macro, bool enabled) {
            SetMacro_t set = new SetMacro_t();
            set.iPlayerNum = player;
            set.iMacro = macro;
            set.bEnabled = enabled;

            IntPtr ptrMacro = ippc.WriteStruct(ptrHandle, set);
            IntPtr ptrSetMacro = ippc.GetRemoteProcAddress(ptrHandle, ptrBaseAddress, "SetMacro");
            if (ptrSetMacro == IntPtr.Zero) {
                Console.WriteLine("Failed to set macro for #" + player + ". SetMacro exported function not found!");
                return;
            }

            IntPtr ptrThread = ippc.Run(ptrHandle, ptrSetMacro, ptrMacro);
            ippc.GetThreadReturnValue(ptrThread);

            ippc.FreeMemory(ptrHandle, ptrMacro);
        }

        public bool IsModuleLoaded(Process target, string sModuleName) {
            ProcessModule pm = GetModule(target, sModuleName);
            return pm != null;
        }

        public ProcessModule GetModule(Process target, string sModuleName) {
            ProcessModuleCollection pmc = target.Modules;

            foreach (ProcessModule pm in pmc) {
                if (Path.GetFileName(pm.FileName) == sModuleName)
                    return pm;
            }
            return null;
        }

        public Process GetProcess(IntPtr hHandle) {
            Process[] p = Process.GetProcesses();
            for (int i = 0; i < p.Length; i++) {
                if (p[i].Handle == hHandle) {
                    return p[i];
                }
            }
            return null;
        }

        public Process GetProcess(string sName) {
            Process[] p = Process.GetProcesses();
            for (int i = 0; i < p.Length; i++) {
                try {
                    if (Path.GetFileName(p[i].MainModule.FileName) == sName) {
                        return p[i];
                    }
                } catch (InvalidOperationException) {
                    return null;
                } catch (Win32Exception) {
                    continue;
                }
            }
            return null;
        }
    }

    // All the information we need pertaining to an instance of player.
    public struct PlayerUpdate_t {
        public int iPlayerNum;

        // Basic resources (stockpile.)
        public int iWood;
        public int iStone;
        public int iIron;
        public int iWheat;
        public int iGrain;
        public int iHops;
        public int iBeer;
        public int iGrapes;
        public int iOil;
        public int iCandles;
        public int iWool;
        public int iLinen;

        // Granary.
        public int iApples;
        public int iBread;
        public int iCheese;
        public int iMeat;

        // Special resources.
        public float fGold;
        public int iHonour;

        // Armory.
        public int iBow;
        public int iCrossbow;
        public int iSword;
        public int iMace;
        public int iPike;
        public int iSpear;
        public int iChestplate;
        public int iLeather;

        // Needed: Lord's Kitchen.. ?
    };
}
