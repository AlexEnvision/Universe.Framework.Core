using Universe.Windows.Forms.Controls.UI;

namespace Universe.Framework.FilesExplorer.Tests
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pSettings = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.btConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pWorkSpace = new System.Windows.Forms.Panel();
            this.mainPanel = new System.Windows.Forms.SplitContainer();
            this.browserControlSource = new Universe.Windows.Forms.Controls.UI.BrowserControl();
            this.browserControlDest = new Universe.Windows.Forms.Controls.UI.BrowserControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.cbAllowRunAsSystemUser = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.pSettings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pWorkSpace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPanel)).BeginInit();
            this.mainPanel.Panel1.SuspendLayout();
            this.mainPanel.Panel2.SuspendLayout();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1169, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(91, 20);
            this.toolStripMenuItem1.Text = "Приложение";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.exitToolStripMenuItem.Text = "Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // pSettings
            // 
            this.pSettings.Controls.Add(this.groupBox2);
            this.pSettings.Controls.Add(this.groupBox1);
            this.pSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pSettings.Location = new System.Drawing.Point(0, 0);
            this.pSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pSettings.Name = "pSettings";
            this.pSettings.Size = new System.Drawing.Size(1169, 279);
            this.pSettings.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbLog);
            this.groupBox2.Location = new System.Drawing.Point(586, 3);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(573, 267);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Лог";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(7, 25);
            this.tbLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(558, 234);
            this.tbLog.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAllowRunAsSystemUser);
            this.groupBox1.Controls.Add(this.tbPassword);
            this.groupBox1.Controls.Add(this.tbLogin);
            this.groupBox1.Controls.Add(this.btConnect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(565, 133);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Учётные данные";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(68, 57);
            this.tbPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(249, 23);
            this.tbPassword.TabIndex = 4;
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(68, 27);
            this.tbLogin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(249, 23);
            this.tbLogin.TabIndex = 3;
            // 
            // btConnect
            // 
            this.btConnect.Location = new System.Drawing.Point(324, 25);
            this.btConnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(220, 24);
            this.btConnect.TabIndex = 2;
            this.btConnect.Text = "Подключиться";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Visible = false;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Пароль:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Логин:";
            // 
            // pWorkSpace
            // 
            this.pWorkSpace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pWorkSpace.Controls.Add(this.mainPanel);
            this.pWorkSpace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pWorkSpace.Location = new System.Drawing.Point(0, 0);
            this.pWorkSpace.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pWorkSpace.Name = "pWorkSpace";
            this.pWorkSpace.Size = new System.Drawing.Size(1169, 553);
            this.pWorkSpace.TabIndex = 3;
            // 
            // mainPanel
            // 
            this.mainPanel.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainPanel.Name = "mainPanel";
            // 
            // mainPanel.Panel1
            // 
            this.mainPanel.Panel1.Controls.Add(this.browserControlSource);
            // 
            // mainPanel.Panel2
            // 
            this.mainPanel.Panel2.Controls.Add(this.browserControlDest);
            this.mainPanel.Size = new System.Drawing.Size(1167, 551);
            this.mainPanel.SplitterDistance = 586;
            this.mainPanel.SplitterWidth = 5;
            this.mainPanel.TabIndex = 4;
            // 
            // browserControlSource
            // 
            this.browserControlSource.AllowDrop = true;
            this.browserControlSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserControlSource.Location = new System.Drawing.Point(0, 0);
            this.browserControlSource.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.browserControlSource.Name = "browserControlSource";
            this.browserControlSource.RootFolder = null;
            this.browserControlSource.Size = new System.Drawing.Size(586, 551);
            this.browserControlSource.TabIndex = 1;
            // 
            // browserControlDest
            // 
            this.browserControlDest.AllowDrop = true;
            this.browserControlDest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserControlDest.Location = new System.Drawing.Point(0, 0);
            this.browserControlDest.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.browserControlDest.Name = "browserControlDest";
            this.browserControlDest.RootFolder = null;
            this.browserControlDest.Size = new System.Drawing.Size(576, 551);
            this.browserControlDest.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 861);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1169, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 24);
            this.scMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.scMain.Name = "scMain";
            this.scMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.pSettings);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.pWorkSpace);
            this.scMain.Size = new System.Drawing.Size(1169, 837);
            this.scMain.SplitterDistance = 279;
            this.scMain.SplitterWidth = 5;
            this.scMain.TabIndex = 5;
            // 
            // cbAllowRunAsSystemUser
            // 
            this.cbAllowRunAsSystemUser.AutoSize = true;
            this.cbAllowRunAsSystemUser.Location = new System.Drawing.Point(12, 94);
            this.cbAllowRunAsSystemUser.Name = "cbAllowRunAsSystemUser";
            this.cbAllowRunAsSystemUser.Size = new System.Drawing.Size(234, 19);
            this.cbAllowRunAsSystemUser.TabIndex = 5;
            this.cbAllowRunAsSystemUser.Text = "Вход под пользователем приложения";
            this.cbAllowRunAsSystemUser.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 883);
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pSettings.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pWorkSpace.ResumeLayout(false);
            this.mainPanel.Panel1.ResumeLayout(false);
            this.mainPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainPanel)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel pSettings;
        private System.Windows.Forms.Panel pWorkSpace;
        private System.Windows.Forms.SplitContainer mainPanel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbLog;
        private BrowserControl browserControlDest;
        private BrowserControl browserControlSource;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.CheckBox cbAllowRunAsSystemUser;
    }
}

