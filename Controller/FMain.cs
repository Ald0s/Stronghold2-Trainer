/*
== Stronghold 2 Trainer ==
A trainer for the old RTS game, Stronghold 2; originally released in 2005.

By Alden Viljoen
https://github.com/ald0s
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using IPPC;

namespace Controller {
    public partial class FMain : Form {
        private bool bShouldRun = false;

        private CControl control;
        private CIPPC ippc;

        private Process target = null;
        private IntPtr ptrBaseAddress = IntPtr.Zero;
        private bool bInjected = false;

        private PlayerUpdate_t targettedUser;

        private System.Timers.Timer timer;

        public FMain() {
            InitializeComponent();
        }

        private void FMain_Load(object sender, EventArgs e) {
            // Do init stuff here.
            ippc = new CIPPC();
            control = new CControl(ippc);
            control.UpdateFound += Control_UpdateFound;

            timer = new System.Timers.Timer();
            timer.Interval = 5000;
            timer.AutoReset = true;
            timer.Elapsed += delegate {
                btnUpdate_Click(null, null);
            };
            //timer.Start();

            lsPlayers.MouseClick += LsPlayers_MouseClick;

            this.FormClosing += FMain_FormClosing;
            this.btnInject.Click += BtnInject_Click;

            Thread thrLogic = new Thread(() => StartLogic());
            thrLogic.Start();
        }

        private void LsPlayers_MouseClick(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left && lsPlayers.GetItemAt(e.X, e.Y) != null) {
                TargetUser((PlayerUpdate_t)lsPlayers.SelectedItems[0].Tag);
            }
        }

        private void BtnInject_Click(object sender, EventArgs e) {
            if(bInjected) {
                // Free.
                FreeLibrary();
            } else {
                // Inject.
                InjectStronghold2();
            }
        }

        private void FMain_FormClosing(object sender, FormClosingEventArgs e) {
            bShouldRun = false;
        }

        private delegate void Control_UpdateFound_Delegate(CPlayerUpdate update);
        private void Control_UpdateFound(CPlayerUpdate update) {
            if (this.InvokeRequired) {
                Control_UpdateFound_Delegate call = new Control_UpdateFound_Delegate(Control_UpdateFound);
                this.Invoke(call, new object[] { update });
            } else {
                lsPlayers.Items.Clear();

                bool added = false;
                for (int i = 0; i < update.GetPlayerList().Length; i++) {
                    PlayerUpdate_t ply = (PlayerUpdate_t)update.GetPlayerList()[i];

                    ListViewItem it = new ListViewItem();
                    it.Tag = ply;
                    it.Text = ply.iPlayerNum.ToString();
                    it.SubItems.Add(ply.fGold.ToString());
                    it.SubItems.Add(update.GetTotalResources(ply.iPlayerNum).ToString());
                    it.SubItems.Add(update.GetTotalFood(ply.iPlayerNum).ToString());
                    it.SubItems.Add(update.GetTotalWeapons(ply.iPlayerNum).ToString());

                    if (ply.iPlayerNum == update.iLocalPlayerNum) {
                        if (added)
                            continue;

                        added = true;
                        it.Text = it.Text + " (You)";
                    }

                    lsPlayers.Items.Add(it);
                }
            }
        }

        private void TargetUser(PlayerUpdate_t tag) {
            checkFamine.CheckedChanged -= CheckFamine_CheckedChanged;
            checkStripWeapons.CheckedChanged -= CheckStripWeapons_CheckedChanged;
            checkRemoveResource.CheckedChanged -= CheckRemoveResource_CheckedChanged;
            checkInfinite.CheckedChanged -= CheckInfinite_CheckedChanged;
            checkInvincible.CheckedChanged -= CheckInvincible_CheckedChanged;

            targettedUser = tag;

            lblTargetted.Text = "Targetting User #" + tag.iPlayerNum;
            groupMacros.Enabled = true;

            CControl.GetMacroState_t state = control.GetMacroState(target.Handle, ptrBaseAddress, tag.iPlayerNum);
            checkFamine.Checked = state.bStarve == 1;
            checkStripWeapons.Checked = state.bNoWeapons == 1;
            checkRemoveResource.Checked = state.bNoResources == 1;
            checkInfinite.Checked = state.bInfiniteResources == 1;
            checkInvincible.Checked = state.bInvincibleTroops == 1;

            checkFamine.CheckedChanged += CheckFamine_CheckedChanged;
            checkStripWeapons.CheckedChanged += CheckStripWeapons_CheckedChanged;
            checkRemoveResource.CheckedChanged += CheckRemoveResource_CheckedChanged;
            checkInfinite.CheckedChanged += CheckInfinite_CheckedChanged;
            checkInvincible.CheckedChanged += CheckInvincible_CheckedChanged;
        }

        private void CheckFamine_CheckedChanged(object sender, EventArgs e) {
            if (targettedUser.iPlayerNum == 0)
                return;

            control.SetMacroEnabled(target.Handle, 
                ptrBaseAddress, 
                targettedUser.iPlayerNum, 
                0, 
                checkFamine.Checked);
        }

        private void CheckStripWeapons_CheckedChanged(object sender, EventArgs e) {
            if (targettedUser.iPlayerNum == 0)
                return;

            control.SetMacroEnabled(target.Handle,
                ptrBaseAddress,
                targettedUser.iPlayerNum,
                1,
                checkStripWeapons.Checked);
        }

        private void CheckRemoveResource_CheckedChanged(object sender, EventArgs e) {
            if (targettedUser.iPlayerNum == 0)
                return;

            control.SetMacroEnabled(target.Handle,
                ptrBaseAddress,
                targettedUser.iPlayerNum,
                2,
                checkRemoveResource.Checked);
        }

        private void CheckInfinite_CheckedChanged(object sender, EventArgs e) {
            if (targettedUser.iPlayerNum == 0)
                return;

            control.SetMacroEnabled(target.Handle,
                ptrBaseAddress,
                targettedUser.iPlayerNum,
                3,
                checkInfinite.Checked);
        }

        private void CheckInvincible_CheckedChanged(object sender, EventArgs e) {
            if (targettedUser.iPlayerNum == 0)
                return;

            control.SetMacroEnabled(target.Handle,
                ptrBaseAddress,
                targettedUser.iPlayerNum,
                4,
                checkInvincible.Checked);
        }

        private void InjectStronghold2() {
            if(!File.Exists("Stronghold2.dll")) {
                MessageBox.Show("Please make sure Stronghold2.dll is in this directory.");
                return;
            }

            IntPtr ret = IntPtr.Zero;
            if((ret = ippc.LoadLibrary(target.Handle, Path.GetFullPath("Stronghold2.dll"))) == IntPtr.Zero) {
                MessageBox.Show("Failed to inject stronghold2!");
                return;
            }

            ptrBaseAddress = new IntPtr(ippc.GetThreadReturnValue(ret));
        }

        private void FreeLibrary() {
            ProcessModule pm = control.GetModule(target, "Stronghold2.dll");
            ippc.FreeLibrary(target.Handle, ptrBaseAddress);
        }

        private void StartLogic() {
            bShouldRun = true;

            while(bShouldRun) {
                // Check if SH2 is running.
                Process p = control.GetProcess("Stronghold2.exe");
                StrongholdFound(p != null, p);

                // Check if we're injected.
                if(target != null) {
                    CheckInjection();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e) {
            try {
                if (bInjected) {
                    control.GetUpdate(target.Handle, ptrBaseAddress);
                }
            } catch (NullReferenceException) {
                Process p = control.GetProcess("Stronghold2.exe");
                target = p;
                btnUpdate_Click(sender, e);
            } catch (InvalidOperationException) {
                return;
            }
        }

        private void CheckInjection() {
            if(control.IsModuleLoaded(target, "Stronghold2.dll")) {
                // Module is injected.
                bInjected = true;
                UpdateInjection(true);

                if(ptrBaseAddress == IntPtr.Zero) {
                    ProcessModule pm = control.GetModule(target, "Stronghold2.dll");
                    this.ptrBaseAddress = pm.BaseAddress;
                }
            } else {
                // Module isn't injected.
                bInjected = false;
                UpdateInjection(false);
            }
        }

        private void StrongholdFound(bool found, Process p) {
            if (found) {
                target = p;

                // Fire UI unlocking events.
                UpdateCanInject(true);
            } else {
                target = null;

                // Fire UI locking events.
                UpdateCanInject(false);
            }
        }

        private delegate void UpdateCanInject_Delegate(bool btnEnabled);
        private void UpdateCanInject(bool enabled) {
            try {
                if (this.InvokeRequired) {
                    UpdateCanInject_Delegate inj = new UpdateCanInject_Delegate(UpdateCanInject);
                    this.Invoke(inj, new object[] { enabled });
                } else {
                    btnInject.Enabled = enabled;
                    lblFoundProcess.Text = (enabled) ? "Stronghold found! (" + target.Id + ")" : "Please launch Stronghold 2";
                }
            } catch (ObjectDisposedException) {
                return;
            }
        }

        private delegate void UpdateInjection_Delegate(bool injected);
        private void UpdateInjection(bool injected) {
            try { 
                if (this.InvokeRequired) {
                    UpdateInjection_Delegate inj = new UpdateInjection_Delegate(UpdateInjection);
                    this.Invoke(inj, new object[] { injected });
                } else {
                    lblInjectionStatus.Text = (injected) ? "Injected" : "Not Injected";
                    lblInjectionStatus.ForeColor = (injected) ? Color.Green : Color.Red;
                    btnInject.Text = (injected) ? "Unload" : "Inject";
                }
            } catch (ObjectDisposedException) {
                return;
            }
        }

        private void btnUpdate_Click_1(object sender, EventArgs e) {
            btnUpdate_Click(null, null);
        }
    }
}
