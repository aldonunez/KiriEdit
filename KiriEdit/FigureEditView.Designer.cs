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
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressPictureBox = new System.Windows.Forms.PictureBox();
            this.masterLabel = new System.Windows.Forms.Label();
            this.masterPictureBox = new System.Windows.Forms.PictureBox();
            this.progressLabel = new System.Windows.Forms.Label();
            this.figureEditor = new KiriEdit.FigureEditor();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterPictureBox)).BeginInit();
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
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.figureEditor);
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
            this.panel1.Controls.Add(this.progressLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(130, 250);
            this.panel1.TabIndex = 0;
            // 
            // progressPictureBox
            // 
            this.progressPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressPictureBox.BackColor = System.Drawing.Color.White;
            this.progressPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.progressPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.progressPictureBox.Location = new System.Drawing.Point(3, 116);
            this.progressPictureBox.Name = "progressPictureBox";
            this.progressPictureBox.Size = new System.Drawing.Size(124, 75);
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
            this.masterPictureBox.Size = new System.Drawing.Size(124, 75);
            this.masterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.masterPictureBox.TabIndex = 3;
            this.masterPictureBox.TabStop = false;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(3, 100);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(51, 13);
            this.progressLabel.TabIndex = 4;
            this.progressLabel.Text = "Progress:";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // figureEditor
            // 
            this.figureEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.figureEditor.Document = null;
            this.figureEditor.Location = new System.Drawing.Point(0, 0);
            this.figureEditor.Name = "figureEditor";
            this.figureEditor.Size = new System.Drawing.Size(256, 250);
            this.figureEditor.TabIndex = 0;
            this.figureEditor.Modified += new System.EventHandler(this.FigureEditor_Modified);
            // 
            // FigureEditView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 250);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FigureEditView";
            this.Load += new System.EventHandler(this.FigureEditView_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FigureEditView_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox masterPictureBox;
        private System.Windows.Forms.Label masterLabel;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox progressPictureBox;
        private FigureEditor figureEditor;
    }
}
