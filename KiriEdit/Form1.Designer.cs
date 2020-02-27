namespace KiriEdit
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(800, 33);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectMenuItem,
            this.saveProjectMenuItem,
            this.saveProjectAsMenuItem,
            this.openProjectMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileMenuItem.Text = "&File";
            // 
            // newProjectMenuItem
            // 
            this.newProjectMenuItem.Name = "newProjectMenuItem";
            this.newProjectMenuItem.Size = new System.Drawing.Size(270, 34);
            this.newProjectMenuItem.Text = "New &Project";
            this.newProjectMenuItem.Click += new System.EventHandler(this.newProjectMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(270, 34);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // saveProjectMenuItem
            // 
            this.saveProjectMenuItem.Name = "saveProjectMenuItem";
            this.saveProjectMenuItem.Size = new System.Drawing.Size(270, 34);
            this.saveProjectMenuItem.Text = "Save Project";
            // 
            // saveProjectAsMenuItem
            // 
            this.saveProjectAsMenuItem.Name = "saveProjectAsMenuItem";
            this.saveProjectAsMenuItem.Size = new System.Drawing.Size(270, 34);
            this.saveProjectAsMenuItem.Text = "Save Project As";
            // 
            // openProjectMenuItem
            // 
            this.openProjectMenuItem.Name = "openProjectMenuItem";
            this.openProjectMenuItem.Size = new System.Drawing.Size(270, 34);
            this.openProjectMenuItem.Text = "Open Project";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(267, 6);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "Form1";
            this.Text = "Form1";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

