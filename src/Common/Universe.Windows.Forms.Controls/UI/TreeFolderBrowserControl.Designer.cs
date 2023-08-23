namespace Universe.Windows.Forms.Controls.UI
{
    partial class TreeFolderBrowserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeFolderBrowserControl));
            this.tvMain = new System.Windows.Forms.TreeView();
            this.FileFolder = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // tvMain
            // 
            this.tvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvMain.ImageIndex = 0;
            this.tvMain.ImageList = this.FileFolder;
            this.tvMain.Location = new System.Drawing.Point(0, 0);
            this.tvMain.Name = "tvMain";
            this.tvMain.SelectedImageIndex = 0;
            this.tvMain.Size = new System.Drawing.Size(215, 275);
            this.tvMain.TabIndex = 0;
            // 
            // FileFolder
            // 
            this.FileFolder.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("FileFolder.ImageStream")));
            this.FileFolder.TransparentColor = System.Drawing.Color.Transparent;
            this.FileFolder.Images.SetKeyName(0, "");
            this.FileFolder.Images.SetKeyName(1, "");
            // 
            // TreeFolderBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvMain);
            this.Name = "TreeFolderBrowserControl";
            this.Size = new System.Drawing.Size(215, 275);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvMain;
        private System.Windows.Forms.ImageList FileFolder;
    }
}
