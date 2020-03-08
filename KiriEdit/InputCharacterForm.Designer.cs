namespace KiriEdit
{
    partial class InputCharacterForm
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
            this.charLabel = new System.Windows.Forms.Label();
            this.charTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.codePointTextBox = new System.Windows.Forms.TextBox();
            this.codePointLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // charLabel
            // 
            this.charLabel.AutoSize = true;
            this.charLabel.Location = new System.Drawing.Point(19, 21);
            this.charLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.charLabel.Name = "charLabel";
            this.charLabel.Size = new System.Drawing.Size(56, 13);
            this.charLabel.TabIndex = 0;
            this.charLabel.Text = "Character:";
            // 
            // charTextBox
            // 
            this.charTextBox.Location = new System.Drawing.Point(103, 21);
            this.charTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.charTextBox.Name = "charTextBox";
            this.charTextBox.Size = new System.Drawing.Size(68, 20);
            this.charTextBox.TabIndex = 1;
            this.charTextBox.TextChanged += new System.EventHandler(this.charTextBox_TextChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.okButton.Location = new System.Drawing.Point(62, 86);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(65, 24);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(131, 86);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(65, 24);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // codePointTextBox
            // 
            this.codePointTextBox.Location = new System.Drawing.Point(103, 46);
            this.codePointTextBox.Name = "codePointTextBox";
            this.codePointTextBox.Size = new System.Drawing.Size(68, 20);
            this.codePointTextBox.TabIndex = 3;
            this.codePointTextBox.TextChanged += new System.EventHandler(this.codepointTextBox_TextChanged);
            // 
            // codePointLabel
            // 
            this.codePointLabel.AutoSize = true;
            this.codePointLabel.Location = new System.Drawing.Point(19, 46);
            this.codePointLabel.Name = "codePointLabel";
            this.codePointLabel.Size = new System.Drawing.Size(62, 13);
            this.codePointLabel.TabIndex = 2;
            this.codePointLabel.Text = "Code Point:";
            // 
            // InputCharacterForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(203, 118);
            this.Controls.Add(this.codePointLabel);
            this.Controls.Add(this.codePointTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.charTextBox);
            this.Controls.Add(this.charLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputCharacterForm";
            this.Text = "Input Character";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label charLabel;
        private System.Windows.Forms.TextBox charTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox codePointTextBox;
        private System.Windows.Forms.Label codePointLabel;
    }
}