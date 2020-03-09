namespace KiriEdit
{
    partial class CharacterGrid
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
                if (_renderArgs != null)
                {
                    _renderArgs.Dispose();
                    _renderArgs = null;
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
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(289, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(26, 237);
            this.vScrollBar.TabIndex = 0;
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
            // 
            // CharacterGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vScrollBar);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CharacterGrid";
            this.Size = new System.Drawing.Size(315, 237);
            this.Load += new System.EventHandler(this.CharacterGrid_Load);
            this.GotFocus += new System.EventHandler(this.CharacterGrid_GotFocus);
            this.LostFocus += new System.EventHandler(this.CharacterGrid_LostFocus);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CharacterGrid_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CharacterGrid_MouseMove);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.CharacterGrid_MouseWheel);
            this.Resize += new System.EventHandler(this.CharacterGrid_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vScrollBar;
    }
}
