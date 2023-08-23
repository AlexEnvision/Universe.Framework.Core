namespace Universe.Windows.Forms.Controls.UI
{
    partial class BrowserControl
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbSelectRootDirectory = new System.Windows.Forms.Button();
            this.tbStartFolder = new System.Windows.Forms.TextBox();
            this.scHeaderBoby = new System.Windows.Forms.SplitContainer();
            this.scBrowser = new System.Windows.Forms.SplitContainer();
            this.tvFolderBrowser = new Universe.Windows.Forms.Controls.UI.TreeFolderBrowserControl();
            this.lvFileBrowser = new Universe.Windows.Forms.Controls.UI.ListViewFileBrowserControl();
            this.ctxListItemsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scHeaderBoby)).BeginInit();
            this.scHeaderBoby.Panel1.SuspendLayout();
            this.scHeaderBoby.Panel2.SuspendLayout();
            this.scHeaderBoby.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scBrowser)).BeginInit();
            this.scBrowser.Panel1.SuspendLayout();
            this.scBrowser.Panel2.SuspendLayout();
            this.scBrowser.SuspendLayout();
            this.ctxListItemsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.Controls.Add(this.tbSelectRootDirectory);
            this.groupBox3.Controls.Add(this.tbStartFolder);
            this.groupBox3.Location = new System.Drawing.Point(12, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(450, 65);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            // 
            // tbSelectRootDirectory
            // 
            this.tbSelectRootDirectory.Location = new System.Drawing.Point(403, 19);
            this.tbSelectRootDirectory.Name = "tbSelectRootDirectory";
            this.tbSelectRootDirectory.Size = new System.Drawing.Size(37, 27);
            this.tbSelectRootDirectory.TabIndex = 5;
            this.tbSelectRootDirectory.Text = "...";
            this.tbSelectRootDirectory.UseVisualStyleBackColor = true;
            this.tbSelectRootDirectory.Click += new System.EventHandler(this.tbSelectRootDirectory_Click);
            // 
            // tbStartFolder
            // 
            this.tbStartFolder.Location = new System.Drawing.Point(6, 19);
            this.tbStartFolder.Multiline = true;
            this.tbStartFolder.Name = "tbStartFolder";
            this.tbStartFolder.Size = new System.Drawing.Size(391, 27);
            this.tbStartFolder.TabIndex = 4;
            // 
            // scHeaderBoby
            // 
            this.scHeaderBoby.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scHeaderBoby.Location = new System.Drawing.Point(0, 0);
            this.scHeaderBoby.Name = "scHeaderBoby";
            this.scHeaderBoby.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scHeaderBoby.Panel1
            // 
            this.scHeaderBoby.Panel1.Controls.Add(this.groupBox3);
            // 
            // scHeaderBoby.Panel2
            // 
            this.scHeaderBoby.Panel2.Controls.Add(this.scBrowser);
            this.scHeaderBoby.Size = new System.Drawing.Size(475, 414);
            this.scHeaderBoby.SplitterDistance = 80;
            this.scHeaderBoby.TabIndex = 4;
            // 
            // scBrowser
            // 
            this.scBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scBrowser.Location = new System.Drawing.Point(0, 0);
            this.scBrowser.Name = "scBrowser";
            // 
            // scBrowser.Panel1
            // 
            this.scBrowser.Panel1.Controls.Add(this.tvFolderBrowser);
            // 
            // scBrowser.Panel2
            // 
            this.scBrowser.Panel2.Controls.Add(this.lvFileBrowser);
            this.scBrowser.Size = new System.Drawing.Size(475, 330);
            this.scBrowser.SplitterDistance = 224;
            this.scBrowser.TabIndex = 0;
            // 
            // tvFolderBrowser
            // 
            this.tvFolderBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolderBrowser.Location = new System.Drawing.Point(0, 0);
            this.tvFolderBrowser.Name = "tvFolderBrowser";
            this.tvFolderBrowser.Size = new System.Drawing.Size(224, 330);
            this.tvFolderBrowser.TabIndex = 0;
            // 
            // lvFileBrowser
            // 
            this.lvFileBrowser.AllowDrop = true;
            this.lvFileBrowser.ContextMenuStrip = this.ctxListItemsMenu;
            this.lvFileBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFileBrowser.Location = new System.Drawing.Point(0, 0);
            this.lvFileBrowser.Name = "lvFileBrowser";
            this.lvFileBrowser.Size = new System.Drawing.Size(247, 330);
            this.lvFileBrowser.TabIndex = 0;
            // 
            // ctxListItemsMenu
            // 
            this.ctxListItemsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.moveToolStripMenuItem,
            this.removeItem,
            this.toolStripSeparator1,
            this.pasteToolStripMenuItem});
            this.ctxListItemsMenu.Name = "ctxItemMenu";
            this.ctxListItemsMenu.Size = new System.Drawing.Size(147, 98);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.copyToolStripMenuItem.Text = "Копировать";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.moveToolStripMenuItem.Text = "Переместить";
            this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveToolStripMenuItem_Click);
            // 
            // removeItem
            // 
            this.removeItem.Name = "removeItem";
            this.removeItem.Size = new System.Drawing.Size(146, 22);
            this.removeItem.Text = "Удалить";
            this.removeItem.Click += new System.EventHandler(this.removeItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.pasteToolStripMenuItem.Text = "Вставить";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // BrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scHeaderBoby);
            this.Name = "BrowserControl";
            this.Size = new System.Drawing.Size(475, 414);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.scHeaderBoby.Panel1.ResumeLayout(false);
            this.scHeaderBoby.Panel1.PerformLayout();
            this.scHeaderBoby.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scHeaderBoby)).EndInit();
            this.scHeaderBoby.ResumeLayout(false);
            this.scBrowser.Panel1.ResumeLayout(false);
            this.scBrowser.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scBrowser)).EndInit();
            this.scBrowser.ResumeLayout(false);
            this.ctxListItemsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button tbSelectRootDirectory;
        private System.Windows.Forms.TextBox tbStartFolder;
        private System.Windows.Forms.SplitContainer scHeaderBoby;
        private System.Windows.Forms.SplitContainer scBrowser;
        private TreeFolderBrowserControl tvFolderBrowser;
        private ListViewFileBrowserControl lvFileBrowser;
        private System.Windows.Forms.ContextMenuStrip ctxListItemsMenu;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeItem;
    }
}
