namespace Controller {
    partial class FMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lblInjectionStatus = new System.Windows.Forms.Label();
            this.btnInject = new System.Windows.Forms.Button();
            this.lblFoundProcess = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lsPlayers = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupMacros = new System.Windows.Forms.GroupBox();
            this.checkInvincible = new System.Windows.Forms.CheckBox();
            this.checkInfinite = new System.Windows.Forms.CheckBox();
            this.checkStripWeapons = new System.Windows.Forms.CheckBox();
            this.checkRemoveResource = new System.Windows.Forms.CheckBox();
            this.checkFamine = new System.Windows.Forms.CheckBox();
            this.lblTargetted = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupMacros.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInjectionStatus
            // 
            this.lblInjectionStatus.AutoSize = true;
            this.lblInjectionStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblInjectionStatus.ForeColor = System.Drawing.Color.Red;
            this.lblInjectionStatus.Location = new System.Drawing.Point(145, 43);
            this.lblInjectionStatus.Name = "lblInjectionStatus";
            this.lblInjectionStatus.Size = new System.Drawing.Size(102, 21);
            this.lblInjectionStatus.TabIndex = 0;
            this.lblInjectionStatus.Text = "Not Injected";
            // 
            // btnInject
            // 
            this.btnInject.Enabled = false;
            this.btnInject.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.btnInject.Location = new System.Drawing.Point(12, 38);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(127, 33);
            this.btnInject.TabIndex = 1;
            this.btnInject.Text = "Inject";
            this.btnInject.UseVisualStyleBackColor = true;
            // 
            // lblFoundProcess
            // 
            this.lblFoundProcess.AutoSize = true;
            this.lblFoundProcess.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblFoundProcess.ForeColor = System.Drawing.Color.Black;
            this.lblFoundProcess.Location = new System.Drawing.Point(12, 9);
            this.lblFoundProcess.Name = "lblFoundProcess";
            this.lblFoundProcess.Size = new System.Drawing.Size(207, 21);
            this.lblFoundProcess.TabIndex = 2;
            this.lblFoundProcess.Text = "Please launch Stronghold 2";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 88);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(483, 256);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lsPlayers);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(475, 228);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "All Players";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lsPlayers
            // 
            this.lsPlayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader2});
            this.lsPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsPlayers.GridLines = true;
            this.lsPlayers.Location = new System.Drawing.Point(3, 3);
            this.lsPlayers.Name = "lsPlayers";
            this.lsPlayers.Size = new System.Drawing.Size(469, 222);
            this.lsPlayers.TabIndex = 5;
            this.lsPlayers.UseCompatibleStateImageBehavior = false;
            this.lsPlayers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Player";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Gold";
            this.columnHeader5.Width = 80;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Resources";
            this.columnHeader6.Width = 80;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Food";
            this.columnHeader7.Width = 80;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupMacros);
            this.tabPage2.Controls.Add(this.lblTargetted);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(475, 228);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Targetted User";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupMacros
            // 
            this.groupMacros.Controls.Add(this.checkInvincible);
            this.groupMacros.Controls.Add(this.checkInfinite);
            this.groupMacros.Controls.Add(this.checkStripWeapons);
            this.groupMacros.Controls.Add(this.checkRemoveResource);
            this.groupMacros.Controls.Add(this.checkFamine);
            this.groupMacros.Enabled = false;
            this.groupMacros.Location = new System.Drawing.Point(10, 38);
            this.groupMacros.Name = "groupMacros";
            this.groupMacros.Size = new System.Drawing.Size(200, 149);
            this.groupMacros.TabIndex = 10;
            this.groupMacros.TabStop = false;
            this.groupMacros.Text = "Macros";
            // 
            // checkInvincible
            // 
            this.checkInvincible.AutoSize = true;
            this.checkInvincible.Location = new System.Drawing.Point(6, 124);
            this.checkInvincible.Name = "checkInvincible";
            this.checkInvincible.Size = new System.Drawing.Size(118, 19);
            this.checkInvincible.TabIndex = 11;
            this.checkInvincible.Text = "Invincible Players";
            this.checkInvincible.UseVisualStyleBackColor = true;
            // 
            // checkInfinite
            // 
            this.checkInfinite.AutoSize = true;
            this.checkInfinite.Location = new System.Drawing.Point(6, 99);
            this.checkInfinite.Name = "checkInfinite";
            this.checkInfinite.Size = new System.Drawing.Size(120, 19);
            this.checkInfinite.TabIndex = 10;
            this.checkInfinite.Text = "Infinite Resources";
            this.checkInfinite.UseVisualStyleBackColor = true;
            // 
            // checkStripWeapons
            // 
            this.checkStripWeapons.AutoSize = true;
            this.checkStripWeapons.Location = new System.Drawing.Point(6, 50);
            this.checkStripWeapons.Name = "checkStripWeapons";
            this.checkStripWeapons.Size = new System.Drawing.Size(134, 19);
            this.checkStripWeapons.TabIndex = 8;
            this.checkStripWeapons.Text = "Confiscate Weapons";
            this.checkStripWeapons.UseVisualStyleBackColor = true;
            // 
            // checkRemoveResource
            // 
            this.checkRemoveResource.AutoSize = true;
            this.checkRemoveResource.Location = new System.Drawing.Point(6, 75);
            this.checkRemoveResource.Name = "checkRemoveResource";
            this.checkRemoveResource.Size = new System.Drawing.Size(116, 19);
            this.checkRemoveResource.TabIndex = 9;
            this.checkRemoveResource.Text = "Delete Resources";
            this.checkRemoveResource.UseVisualStyleBackColor = true;
            // 
            // checkFamine
            // 
            this.checkFamine.AutoSize = true;
            this.checkFamine.Location = new System.Drawing.Point(6, 25);
            this.checkFamine.Name = "checkFamine";
            this.checkFamine.Size = new System.Drawing.Size(94, 19);
            this.checkFamine.TabIndex = 7;
            this.checkFamine.Text = "Starve Player";
            this.checkFamine.UseVisualStyleBackColor = true;
            // 
            // lblTargetted
            // 
            this.lblTargetted.AutoSize = true;
            this.lblTargetted.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblTargetted.ForeColor = System.Drawing.Color.Black;
            this.lblTargetted.Location = new System.Drawing.Point(6, 3);
            this.lblTargetted.Name = "lblTargetted";
            this.lblTargetted.Size = new System.Drawing.Size(104, 21);
            this.lblTargetted.TabIndex = 6;
            this.lblTargetted.Text = "Select a User";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.Location = new System.Drawing.Point(368, 4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(127, 33);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click_1);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Weapons";
            this.columnHeader2.Width = 80;
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 354);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblFoundProcess);
            this.Controls.Add(this.btnInject);
            this.Controls.Add(this.lblInjectionStatus);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FMain";
            this.Text = "Stronghold 2";
            this.Load += new System.EventHandler(this.FMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupMacros.ResumeLayout(false);
            this.groupMacros.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInjectionStatus;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.Label lblFoundProcess;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView lsPlayers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblTargetted;
        private System.Windows.Forms.CheckBox checkRemoveResource;
        private System.Windows.Forms.CheckBox checkStripWeapons;
        private System.Windows.Forms.CheckBox checkFamine;
        private System.Windows.Forms.GroupBox groupMacros;
        private System.Windows.Forms.CheckBox checkInfinite;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.CheckBox checkInvincible;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}

