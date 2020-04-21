namespace KiriEdit
{
    partial class CharMapView
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

            if (disposing)
            {
                if (_fontCollection != null)
                {
                    _fontCollection.Dispose();
                    _fontCollection = null;
                }

                if (_project != null)
                {
                    _project.CharacterItemModified -= project_CharacterItemModified;
                    _project = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharMapView));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.charListBox = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainImageList = new System.Windows.Forms.ImageList(this.components);
            this.sortComboBox = new System.Windows.Forms.ComboBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addListCharButton = new System.Windows.Forms.ToolStripButton();
            this.deleteListCharButton = new System.Windows.Forms.ToolStripButton();
            this.checkCompleteButton = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.charDescriptionLabel = new System.Windows.Forms.Label();
            this.findCharButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fontNameLabel = new System.Windows.Forms.Label();
            this.characterContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addCharacterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCharacterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCharacterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.charGrid = new KiriEdit.CharacterGrid();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.characterContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.charListBox);
            this.splitContainer1.Panel1.Controls.Add(this.sortComboBox);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.charGrid);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.splitContainer1.Panel2MinSize = 300;
            this.splitContainer1.Size = new System.Drawing.Size(431, 265);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // charListBox
            // 
            this.charListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.charListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.charListBox.FullRowSelect = true;
            this.charListBox.GridLines = true;
            this.charListBox.HideSelection = false;
            this.charListBox.Location = new System.Drawing.Point(0, 46);
            this.charListBox.MultiSelect = false;
            this.charListBox.Name = "charListBox";
            this.charListBox.Size = new System.Drawing.Size(125, 219);
            this.charListBox.SmallImageList = this.mainImageList;
            this.charListBox.TabIndex = 2;
            this.charListBox.UseCompatibleStateImageBehavior = false;
            this.charListBox.View = System.Windows.Forms.View.Details;
            this.charListBox.SelectedIndexChanged += new System.EventHandler(this.charListBox_SelectedIndexChanged);
            this.charListBox.DoubleClick += new System.EventHandler(this.CharListBox_DoubleClick);
            this.charListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CharListBox_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 27;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Code point";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Character";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Pieces";
            // 
            // mainImageList
            // 
            this.mainImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mainImageList.ImageStream")));
            this.mainImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mainImageList.Images.SetKeyName(0, "tick.png");
            this.mainImageList.Images.SetKeyName(1, "pencil.png");
            this.mainImageList.Images.SetKeyName(2, "tick-white.png");
            // 
            // sortComboBox
            // 
            this.sortComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.sortComboBox.Location = new System.Drawing.Point(0, 25);
            this.sortComboBox.Name = "sortComboBox";
            this.sortComboBox.Size = new System.Drawing.Size(125, 21);
            this.sortComboBox.TabIndex = 1;
            this.sortComboBox.SelectedIndexChanged += new System.EventHandler(this.SortComboBox_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addListCharButton,
            this.deleteListCharButton,
            this.checkCompleteButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(125, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addListCharButton
            // 
            this.addListCharButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addListCharButton.Image = global::KiriEdit.Properties.Resources.Add;
            this.addListCharButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addListCharButton.Name = "addListCharButton";
            this.addListCharButton.Size = new System.Drawing.Size(23, 22);
            this.addListCharButton.Text = "Add character";
            this.addListCharButton.Click += new System.EventHandler(this.addListCharButton_Click);
            // 
            // deleteListCharButton
            // 
            this.deleteListCharButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteListCharButton.Image = global::KiriEdit.Properties.Resources.Delete;
            this.deleteListCharButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteListCharButton.Name = "deleteListCharButton";
            this.deleteListCharButton.Size = new System.Drawing.Size(23, 22);
            this.deleteListCharButton.Text = "Delete character";
            this.deleteListCharButton.Click += new System.EventHandler(this.deleteListCharButton_Click);
            // 
            // checkCompleteButton
            // 
            this.checkCompleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.checkCompleteButton.Image = global::KiriEdit.Properties.Resources.CheckState;
            this.checkCompleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.checkCompleteButton.Name = "checkCompleteButton";
            this.checkCompleteButton.Size = new System.Drawing.Size(23, 22);
            this.checkCompleteButton.Text = "Check complete";
            this.checkCompleteButton.Click += new System.EventHandler(this.checkCompleteButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.charDescriptionLabel);
            this.panel2.Controls.Add(this.findCharButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(8, 202);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel2.Size = new System.Drawing.Size(285, 63);
            this.panel2.TabIndex = 2;
            // 
            // charDescriptionLabel
            // 
            this.charDescriptionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.charDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.charDescriptionLabel.Location = new System.Drawing.Point(0, 37);
            this.charDescriptionLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.charDescriptionLabel.Name = "charDescriptionLabel";
            this.charDescriptionLabel.Size = new System.Drawing.Size(285, 23);
            this.charDescriptionLabel.TabIndex = 1;
            this.charDescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // findCharButton
            // 
            this.findCharButton.Location = new System.Drawing.Point(3, 6);
            this.findCharButton.Name = "findCharButton";
            this.findCharButton.Size = new System.Drawing.Size(75, 23);
            this.findCharButton.TabIndex = 0;
            this.findCharButton.Text = "Find";
            this.findCharButton.UseVisualStyleBackColor = true;
            this.findCharButton.Click += new System.EventHandler(this.findCharButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fontNameLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(8, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(285, 32);
            this.panel1.TabIndex = 1;
            // 
            // fontNameLabel
            // 
            this.fontNameLabel.AutoSize = true;
            this.fontNameLabel.Location = new System.Drawing.Point(3, 10);
            this.fontNameLabel.Name = "fontNameLabel";
            this.fontNameLabel.Size = new System.Drawing.Size(35, 13);
            this.fontNameLabel.TabIndex = 0;
            this.fontNameLabel.Text = "label1";
            // 
            // characterContextMenu
            // 
            this.characterContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCharacterMenuItem,
            this.deleteCharacterMenuItem,
            this.editCharacterMenuItem});
            this.characterContextMenu.Name = "characterContextMenu";
            this.characterContextMenu.Size = new System.Drawing.Size(172, 70);
            // 
            // addCharacterMenuItem
            // 
            this.addCharacterMenuItem.Name = "addCharacterMenuItem";
            this.addCharacterMenuItem.Size = new System.Drawing.Size(171, 22);
            this.addCharacterMenuItem.Text = "Add character";
            this.addCharacterMenuItem.Click += new System.EventHandler(this.addCharacterMenuItem_Click);
            // 
            // deleteCharacterMenuItem
            // 
            this.deleteCharacterMenuItem.Name = "deleteCharacterMenuItem";
            this.deleteCharacterMenuItem.Size = new System.Drawing.Size(171, 22);
            this.deleteCharacterMenuItem.Text = "Delete character ...";
            this.deleteCharacterMenuItem.Click += new System.EventHandler(this.deleteCharacterMenuItem_Click);
            // 
            // editCharacterMenuItem
            // 
            this.editCharacterMenuItem.Name = "editCharacterMenuItem";
            this.editCharacterMenuItem.Size = new System.Drawing.Size(171, 22);
            this.editCharacterMenuItem.Text = "Edit character";
            this.editCharacterMenuItem.Click += new System.EventHandler(this.editCharacterMenuItem_Click);
            // 
            // charGrid
            // 
            this.charGrid.Columns = 20;
            this.charGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charGrid.Location = new System.Drawing.Point(8, 32);
            this.charGrid.Margin = new System.Windows.Forms.Padding(2);
            this.charGrid.Name = "charGrid";
            this.charGrid.OnCharacterColor = System.Drawing.Color.Red;
            this.charGrid.Size = new System.Drawing.Size(285, 170);
            this.charGrid.TabIndex = 0;
            this.charGrid.SelectedIndexChanged += new System.EventHandler(this.charGrid_SelectedIndexChanged);
            this.charGrid.DoubleClick += new System.EventHandler(this.CharGrid_DoubleClick);
            this.charGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CharGrid_MouseUp);
            // 
            // CharMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 265);
            this.Controls.Add(this.splitContainer1);
            this.Name = "CharMapView";
            this.Text = "Character Map";
            this.Load += new System.EventHandler(this.CharMapView_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.characterContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView charListBox;
        private System.Windows.Forms.ComboBox sortComboBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addListCharButton;
        private System.Windows.Forms.ToolStripButton deleteListCharButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label fontNameLabel;
        private CharacterGrid charGrid;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button findCharButton;
        private System.Windows.Forms.Label charDescriptionLabel;
        private System.Windows.Forms.ContextMenuStrip characterContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addCharacterMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteCharacterMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCharacterMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripButton checkCompleteButton;
        private System.Windows.Forms.ImageList mainImageList;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}
