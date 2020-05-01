namespace KiriEdit
{
    partial class FigureEditor
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
            this.editorToolStrip = new System.Windows.Forms.ToolStrip();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.lineButton = new System.Windows.Forms.ToolStripButton();
            this.pointButton = new System.Windows.Forms.ToolStripButton();
            this.editorToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // editorToolStrip
            // 
            this.editorToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineButton,
            this.pointButton});
            this.editorToolStrip.Location = new System.Drawing.Point(0, 0);
            this.editorToolStrip.Name = "editorToolStrip";
            this.editorToolStrip.Size = new System.Drawing.Size(286, 25);
            this.editorToolStrip.TabIndex = 0;
            this.editorToolStrip.Text = "toolStrip1";
            // 
            // canvas
            // 
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(0, 25);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(286, 230);
            this.canvas.TabIndex = 1;
            this.canvas.TabStop = false;
            this.canvas.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseClick);
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseDown);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseMove);
            // 
            // lineButton
            // 
            this.lineButton.Checked = true;
            this.lineButton.CheckOnClick = true;
            this.lineButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lineButton.Image = global::KiriEdit.Properties.Resources.DashedLine;
            this.lineButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lineButton.Name = "lineButton";
            this.lineButton.Size = new System.Drawing.Size(23, 22);
            this.lineButton.Text = "Cut";
            this.lineButton.CheckedChanged += new System.EventHandler(this.lineButton_CheckedChanged);
            // 
            // pointButton
            // 
            this.pointButton.CheckOnClick = true;
            this.pointButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pointButton.Image = global::KiriEdit.Properties.Resources.Point;
            this.pointButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pointButton.Name = "pointButton";
            this.pointButton.Size = new System.Drawing.Size(23, 22);
            this.pointButton.Text = "Point";
            this.pointButton.CheckedChanged += new System.EventHandler(this.pointButton_CheckedChanged);
            // 
            // FigureEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.editorToolStrip);
            this.Name = "FigureEditor";
            this.Size = new System.Drawing.Size(286, 255);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FigureEditor_KeyUp);
            this.VisibleChanged += new System.EventHandler(this.FigureEditor_VisibleChanged);
            this.Resize += new System.EventHandler(this.FigureEditor_Resize);
            this.editorToolStrip.ResumeLayout(false);
            this.editorToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip editorToolStrip;
        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.ToolStripButton lineButton;
        private System.Windows.Forms.ToolStripButton pointButton;
    }
}
