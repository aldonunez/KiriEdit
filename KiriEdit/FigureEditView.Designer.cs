namespace KiriEdit
{
    partial class FigureEditView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.piecesLabel = new System.Windows.Forms.Label();
            this.masterPictureBox = new System.Windows.Forms.PictureBox();
            this.masterLabel = new System.Windows.Forms.Label();
            this.piecesListView = new System.Windows.Forms.ListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addPieceButton = new System.Windows.Forms.ToolStripButton();
            this.deletePieceButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.masterPictureBox)).BeginInit();
            this.toolStrip1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.piecesLabel);
            this.splitContainer1.Panel1.Controls.Add(this.masterPictureBox);
            this.splitContainer1.Panel1.Controls.Add(this.masterLabel);
            this.splitContainer1.Panel1.Controls.Add(this.piecesListView);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(391, 250);
            this.splitContainer1.SplitterDistance = 130;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // piecesLabel
            // 
            this.piecesLabel.AutoSize = true;
            this.piecesLabel.Location = new System.Drawing.Point(3, 119);
            this.piecesLabel.Name = "piecesLabel";
            this.piecesLabel.Size = new System.Drawing.Size(42, 13);
            this.piecesLabel.TabIndex = 4;
            this.piecesLabel.Text = "Pieces:";
            this.piecesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // masterPictureBox
            // 
            this.masterPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.masterPictureBox.BackColor = System.Drawing.Color.White;
            this.masterPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.masterPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.masterPictureBox.Location = new System.Drawing.Point(0, 41);
            this.masterPictureBox.Name = "masterPictureBox";
            this.masterPictureBox.Size = new System.Drawing.Size(130, 75);
            this.masterPictureBox.TabIndex = 3;
            this.masterPictureBox.TabStop = false;
            // 
            // masterLabel
            // 
            this.masterLabel.AutoSize = true;
            this.masterLabel.Location = new System.Drawing.Point(3, 25);
            this.masterLabel.Name = "masterLabel";
            this.masterLabel.Size = new System.Drawing.Size(42, 13);
            this.masterLabel.TabIndex = 2;
            this.masterLabel.Text = "Master:";
            this.masterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // piecesListView
            // 
            this.piecesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.piecesListView.HideSelection = false;
            this.piecesListView.Location = new System.Drawing.Point(0, 135);
            this.piecesListView.Name = "piecesListView";
            this.piecesListView.Size = new System.Drawing.Size(130, 112);
            this.piecesListView.TabIndex = 1;
            this.piecesListView.UseCompatibleStateImageBehavior = false;
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
            this.addPieceButton.Text = "toolStripButton1";
            // 
            // deletePieceButton
            // 
            this.deletePieceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deletePieceButton.Image = global::KiriEdit.Properties.Resources.Trash_16x;
            this.deletePieceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deletePieceButton.Name = "deletePieceButton";
            this.deletePieceButton.Size = new System.Drawing.Size(23, 22);
            this.deletePieceButton.Text = "toolStripButton2";
            // 
            // FigureEditView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FigureEditView";
            this.Size = new System.Drawing.Size(391, 250);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
    }
}
