namespace KiriEdit
{
    partial class CharEditView
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
                if (_characterItem != null)
                {
                    _characterItem.FigureItemModified -= CharacterItem_FigureItemModified;
                    _characterItem.Deleted -= CharacterItem_Deleted;
                    _characterItem = null;
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressPictureBox = new System.Windows.Forms.PictureBox();
            this.masterLabel = new System.Windows.Forms.Label();
            this.masterPictureBox = new System.Windows.Forms.PictureBox();
            this.piecesLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addPieceButton = new System.Windows.Forms.ToolStripButton();
            this.deletePieceButton = new System.Windows.Forms.ToolStripButton();
            this.piecesListView = new System.Windows.Forms.ListView();
            this.piecesImageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterPictureBox)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.piecesListView);
            this.splitContainer1.Size = new System.Drawing.Size(391, 250);
            this.splitContainer1.SplitterDistance = 130;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.progressPictureBox);
            this.panel1.Controls.Add(this.masterLabel);
            this.panel1.Controls.Add(this.masterPictureBox);
            this.panel1.Controls.Add(this.piecesLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(130, 225);
            this.panel1.TabIndex = 0;
            // 
            // progressPictureBox
            // 
            this.progressPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressPictureBox.BackColor = System.Drawing.Color.White;
            this.progressPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.progressPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.progressPictureBox.Location = new System.Drawing.Point(3, 126);
            this.progressPictureBox.Name = "progressPictureBox";
            this.progressPictureBox.Size = new System.Drawing.Size(124, 85);
            this.progressPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.progressPictureBox.TabIndex = 5;
            this.progressPictureBox.TabStop = false;
            // 
            // masterLabel
            // 
            this.masterLabel.AutoSize = true;
            this.masterLabel.Location = new System.Drawing.Point(3, 3);
            this.masterLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.masterLabel.Name = "masterLabel";
            this.masterLabel.Size = new System.Drawing.Size(42, 13);
            this.masterLabel.TabIndex = 2;
            this.masterLabel.Text = "Master:";
            this.masterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // masterPictureBox
            // 
            this.masterPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.masterPictureBox.BackColor = System.Drawing.Color.White;
            this.masterPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.masterPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.masterPictureBox.Location = new System.Drawing.Point(3, 19);
            this.masterPictureBox.Name = "masterPictureBox";
            this.masterPictureBox.Size = new System.Drawing.Size(124, 85);
            this.masterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.masterPictureBox.TabIndex = 3;
            this.masterPictureBox.TabStop = false;
            // 
            // piecesLabel
            // 
            this.piecesLabel.AutoSize = true;
            this.piecesLabel.Location = new System.Drawing.Point(3, 110);
            this.piecesLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.piecesLabel.Name = "piecesLabel";
            this.piecesLabel.Size = new System.Drawing.Size(51, 13);
            this.piecesLabel.TabIndex = 4;
            this.piecesLabel.Text = "Progress:";
            this.piecesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPieceButton,
            this.deletePieceButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(130, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addPieceButton
            // 
            this.addPieceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addPieceButton.Image = global::KiriEdit.Properties.Resources.Add_16x;
            this.addPieceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addPieceButton.Name = "addPieceButton";
            this.addPieceButton.Size = new System.Drawing.Size(23, 22);
            this.addPieceButton.Text = "Add piece";
            this.addPieceButton.Click += new System.EventHandler(this.addPieceButton_Click);
            // 
            // deletePieceButton
            // 
            this.deletePieceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deletePieceButton.Enabled = false;
            this.deletePieceButton.Image = global::KiriEdit.Properties.Resources.Trash_16x;
            this.deletePieceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deletePieceButton.Name = "deletePieceButton";
            this.deletePieceButton.Size = new System.Drawing.Size(23, 22);
            this.deletePieceButton.Text = "Delete piece";
            this.deletePieceButton.Click += new System.EventHandler(this.deletePieceButton_Click);
            // 
            // piecesListView
            // 
            this.piecesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.piecesListView.HideSelection = false;
            this.piecesListView.LargeImageList = this.piecesImageList;
            this.piecesListView.Location = new System.Drawing.Point(0, 0);
            this.piecesListView.MultiSelect = false;
            this.piecesListView.Name = "piecesListView";
            this.piecesListView.Size = new System.Drawing.Size(256, 250);
            this.piecesListView.TabIndex = 1;
            this.piecesListView.UseCompatibleStateImageBehavior = false;
            this.piecesListView.ItemActivate += new System.EventHandler(this.piecesListView_ItemActivate);
            this.piecesListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PiecesListView_KeyUp);
            this.piecesListView.SelectedIndexChanged += new System.EventHandler(this.PiecesListView_SelectedIndexChanged);
            // 
            // piecesImageList
            // 
            this.piecesImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.piecesImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.piecesImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // CharEditView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 250);
            this.Controls.Add(this.splitContainer1);
            this.Name = "CharEditView";
            this.Load += new System.EventHandler(this.FigureEditView_Load);
            this.Shown += new System.EventHandler(this.CharEditView_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterPictureBox)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addPieceButton;
        private System.Windows.Forms.ToolStripButton deletePieceButton;
        private System.Windows.Forms.ListView piecesListView;
        private System.Windows.Forms.PictureBox masterPictureBox;
        private System.Windows.Forms.Label masterLabel;
        private System.Windows.Forms.Label piecesLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ImageList piecesImageList;
        private System.Windows.Forms.PictureBox progressPictureBox;
    }
}
