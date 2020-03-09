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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharMapView));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.charListBox = new System.Windows.Forms.ListBox();
            this.sortComboBox = new System.Windows.Forms.ComboBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addListCharButton = new System.Windows.Forms.ToolStripButton();
            this.deleteListCharButton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fontNameLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.findCharButton = new System.Windows.Forms.Button();
            this.charGrid = new KiriEdit.CharacterGrid();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
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
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.splitContainer1.Panel2MinSize = 300;
            this.splitContainer1.Size = new System.Drawing.Size(427, 265);
            this.splitContainer1.SplitterDistance = 121;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // charListBox
            // 
            this.charListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charListBox.FormattingEnabled = true;
            this.charListBox.Location = new System.Drawing.Point(0, 52);
            this.charListBox.Margin = new System.Windows.Forms.Padding(2);
            this.charListBox.Name = "charListBox";
            this.charListBox.Size = new System.Drawing.Size(121, 213);
            this.charListBox.TabIndex = 2;
            this.charListBox.SelectedIndexChanged += new System.EventHandler(this.charListBox_SelectedIndexChanged);
            // 
            // sortComboBox
            // 
            this.sortComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.sortComboBox.Location = new System.Drawing.Point(0, 31);
            this.sortComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.sortComboBox.Name = "sortComboBox";
            this.sortComboBox.Size = new System.Drawing.Size(121, 21);
            this.sortComboBox.TabIndex = 1;
            this.sortComboBox.SelectedIndexChanged += new System.EventHandler(this.SortComboBox_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addListCharButton,
            this.deleteListCharButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(121, 31);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addListCharButton
            // 
            this.addListCharButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addListCharButton.Image = ((System.Drawing.Image)(resources.GetObject("addListCharButton.Image")));
            this.addListCharButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addListCharButton.Name = "addListCharButton";
            this.addListCharButton.Size = new System.Drawing.Size(28, 28);
            this.addListCharButton.Text = "Add character";
            this.addListCharButton.Click += new System.EventHandler(this.addListCharButton_Click);
            // 
            // deleteListCharButton
            // 
            this.deleteListCharButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteListCharButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteListCharButton.Image")));
            this.deleteListCharButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteListCharButton.Name = "deleteListCharButton";
            this.deleteListCharButton.Size = new System.Drawing.Size(28, 28);
            this.deleteListCharButton.Text = "Delete character";
            this.deleteListCharButton.Click += new System.EventHandler(this.deleteListCharButton_Click);
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
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.findCharButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(8, 227);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(285, 30);
            this.panel2.TabIndex = 2;
            // 
            // findCharButton
            // 
            this.findCharButton.Location = new System.Drawing.Point(4, 4);
            this.findCharButton.Name = "findCharButton";
            this.findCharButton.Size = new System.Drawing.Size(75, 23);
            this.findCharButton.TabIndex = 0;
            this.findCharButton.Text = "Find";
            this.findCharButton.UseVisualStyleBackColor = true;
            this.findCharButton.Click += new System.EventHandler(this.findCharButton_Click);
            // 
            // charGrid
            // 
            this.charGrid.Columns = 20;
            this.charGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charGrid.Location = new System.Drawing.Point(8, 32);
            this.charGrid.Margin = new System.Windows.Forms.Padding(2);
            this.charGrid.Name = "charGrid";
            this.charGrid.OnCharacterColor = System.Drawing.Color.Red;
            this.charGrid.Size = new System.Drawing.Size(285, 195);
            this.charGrid.TabIndex = 0;
            // 
            // CharMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CharMapView";
            this.Size = new System.Drawing.Size(427, 265);
            this.Load += new System.EventHandler(this.CharMapView_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox charListBox;
        private System.Windows.Forms.ComboBox sortComboBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addListCharButton;
        private System.Windows.Forms.ToolStripButton deleteListCharButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label fontNameLabel;
        private CharacterGrid charGrid;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button findCharButton;
    }
}
