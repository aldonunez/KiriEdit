namespace KiriEdit
{
    partial class ShellForm
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
            this.openProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveItemMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.characterMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllDocumentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.window0MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.window1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.window2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.window3MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutAppMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.fontInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.viewMenuItem,
            this.windowMenuItem,
            this.helpMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(533, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectMenuItem,
            this.openProjectMenuItem,
            this.closeProjectMenuItem,
            this.toolStripSeparator2,
            this.saveItemMenuItem,
            this.saveAllMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "&File";
            // 
            // newProjectMenuItem
            // 
            this.newProjectMenuItem.Name = "newProjectMenuItem";
            this.newProjectMenuItem.Size = new System.Drawing.Size(198, 22);
            this.newProjectMenuItem.Text = "&New Project ...";
            this.newProjectMenuItem.Click += new System.EventHandler(this.newProjectMenuItem_Click);
            // 
            // openProjectMenuItem
            // 
            this.openProjectMenuItem.Name = "openProjectMenuItem";
            this.openProjectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProjectMenuItem.Size = new System.Drawing.Size(198, 22);
            this.openProjectMenuItem.Text = "&Open Project ...";
            this.openProjectMenuItem.Click += new System.EventHandler(this.openProjectMenuItem_Click);
            // 
            // closeProjectMenuItem
            // 
            this.closeProjectMenuItem.Name = "closeProjectMenuItem";
            this.closeProjectMenuItem.Size = new System.Drawing.Size(198, 22);
            this.closeProjectMenuItem.Text = "&Close Project";
            this.closeProjectMenuItem.Click += new System.EventHandler(this.closeProjectMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
            // 
            // saveItemMenuItem
            // 
            this.saveItemMenuItem.Name = "saveItemMenuItem";
            this.saveItemMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveItemMenuItem.Size = new System.Drawing.Size(198, 22);
            this.saveItemMenuItem.Text = "&Save Item";
            this.saveItemMenuItem.Click += new System.EventHandler(this.saveItemMenuItem_Click);
            // 
            // saveAllMenuItem
            // 
            this.saveAllMenuItem.Name = "saveAllMenuItem";
            this.saveAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAllMenuItem.Size = new System.Drawing.Size(198, 22);
            this.saveAllMenuItem.Text = "Save A&ll";
            this.saveAllMenuItem.Click += new System.EventHandler(this.saveAllMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(198, 22);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // editMenuItem
            // 
            this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.redoMenuItem});
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editMenuItem.Text = "&Edit";
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.Enabled = false;
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoMenuItem.Size = new System.Drawing.Size(144, 22);
            this.undoMenuItem.Text = "&Undo";
            this.undoMenuItem.Click += new System.EventHandler(this.undoMenuItem_Click);
            // 
            // redoMenuItem
            // 
            this.redoMenuItem.Enabled = false;
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoMenuItem.Size = new System.Drawing.Size(144, 22);
            this.redoMenuItem.Text = "&Redo";
            this.redoMenuItem.Click += new System.EventHandler(this.redoMenuItem_Click);
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.characterMapMenuItem,
            this.toolStripSeparator4,
            this.fontInfoMenuItem});
            this.viewMenuItem.Name = "viewMenuItem";
            this.viewMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewMenuItem.Text = "&View";
            // 
            // characterMapMenuItem
            // 
            this.characterMapMenuItem.Name = "characterMapMenuItem";
            this.characterMapMenuItem.Size = new System.Drawing.Size(180, 22);
            this.characterMapMenuItem.Text = "Character &Map";
            this.characterMapMenuItem.Click += new System.EventHandler(this.characterMapMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeAllDocumentsMenuItem,
            this.toolStripSeparator3,
            this.window0MenuItem,
            this.window1MenuItem,
            this.window2MenuItem,
            this.window3MenuItem,
            this.windowsMenuItem});
            this.windowMenuItem.Name = "windowMenuItem";
            this.windowMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowMenuItem.Text = "&Window";
            this.windowMenuItem.DropDownOpening += new System.EventHandler(this.WindowMenuItem_DropDownOpening);
            // 
            // closeAllDocumentsMenuItem
            // 
            this.closeAllDocumentsMenuItem.Name = "closeAllDocumentsMenuItem";
            this.closeAllDocumentsMenuItem.Size = new System.Drawing.Size(184, 22);
            this.closeAllDocumentsMenuItem.Text = "Close All Documents";
            this.closeAllDocumentsMenuItem.Click += new System.EventHandler(this.closeAllDocumentsMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(181, 6);
            // 
            // window0MenuItem
            // 
            this.window0MenuItem.Name = "window0MenuItem";
            this.window0MenuItem.Size = new System.Drawing.Size(184, 22);
            this.window0MenuItem.Text = "Window 0";
            this.window0MenuItem.Visible = false;
            this.window0MenuItem.Click += new System.EventHandler(this.windowXMenuItem_Click);
            // 
            // window1MenuItem
            // 
            this.window1MenuItem.Name = "window1MenuItem";
            this.window1MenuItem.Size = new System.Drawing.Size(184, 22);
            this.window1MenuItem.Text = "Window 1";
            this.window1MenuItem.Visible = false;
            this.window1MenuItem.Click += new System.EventHandler(this.windowXMenuItem_Click);
            // 
            // window2MenuItem
            // 
            this.window2MenuItem.Name = "window2MenuItem";
            this.window2MenuItem.Size = new System.Drawing.Size(184, 22);
            this.window2MenuItem.Text = "Window 2";
            this.window2MenuItem.Visible = false;
            this.window2MenuItem.Click += new System.EventHandler(this.windowXMenuItem_Click);
            // 
            // window3MenuItem
            // 
            this.window3MenuItem.Name = "window3MenuItem";
            this.window3MenuItem.Size = new System.Drawing.Size(184, 22);
            this.window3MenuItem.Text = "Window 3";
            this.window3MenuItem.Visible = false;
            this.window3MenuItem.Click += new System.EventHandler(this.windowXMenuItem_Click);
            // 
            // windowsMenuItem
            // 
            this.windowsMenuItem.Name = "windowsMenuItem";
            this.windowsMenuItem.Size = new System.Drawing.Size(184, 22);
            this.windowsMenuItem.Text = "Windows ...";
            this.windowsMenuItem.Click += new System.EventHandler(this.windowsMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutAppMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpMenuItem.Text = "&Help";
            // 
            // aboutAppMenuItem
            // 
            this.aboutAppMenuItem.Name = "aboutAppMenuItem";
            this.aboutAppMenuItem.Size = new System.Drawing.Size(147, 22);
            this.aboutAppMenuItem.Text = "&About KiriEdit";
            this.aboutAppMenuItem.Click += new System.EventHandler(this.aboutAppMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 270);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(533, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(177, 6);
            // 
            // fontInfoMenuItem
            // 
            this.fontInfoMenuItem.Enabled = false;
            this.fontInfoMenuItem.Name = "fontInfoMenuItem";
            this.fontInfoMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fontInfoMenuItem.Text = "&Font Information ...";
            this.fontInfoMenuItem.Click += new System.EventHandler(this.fontInfoMenuItem_Click);
            // 
            // ShellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 292);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainMenuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mainMenuStrip;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "ShellForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShellForm_FormClosing);
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
        private System.Windows.Forms.ToolStripMenuItem saveItemMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeProjectMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem characterMapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllDocumentsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem window0MenuItem;
        private System.Windows.Forms.ToolStripMenuItem window1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem window2MenuItem;
        private System.Windows.Forms.ToolStripMenuItem window3MenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutAppMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem fontInfoMenuItem;
    }
}

