namespace KiriEdit
{
    partial class NewProjectForm
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
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                if (_library != null)
                {
                    _library.Dispose();
                    _library = null;
                }
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
            this.projNameLabel = new System.Windows.Forms.Label();
            this.projNameTextBox = new System.Windows.Forms.TextBox();
            this.projPathLabel = new System.Windows.Forms.Label();
            this.projPathTextBox = new System.Windows.Forms.TextBox();
            this.projPathButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.fontGroupBox = new System.Windows.Forms.GroupBox();
            this.infoLinkLabel = new System.Windows.Forms.LinkLabel();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.typefaceLabel = new System.Windows.Forms.Label();
            this.copyrightTextBox = new System.Windows.Forms.TextBox();
            this.typefaceComboBox = new System.Windows.Forms.ComboBox();
            this.fontPathButton = new System.Windows.Forms.Button();
            this.fontPathLabel = new System.Windows.Forms.Label();
            this.fontPathTextBox = new System.Windows.Forms.TextBox();
            this.fontGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // projNameLabel
            // 
            this.projNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.projNameLabel.AutoSize = true;
            this.projNameLabel.Location = new System.Drawing.Point(12, 138);
            this.projNameLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.projNameLabel.Name = "projNameLabel";
            this.projNameLabel.Size = new System.Drawing.Size(38, 13);
            this.projNameLabel.TabIndex = 0;
            this.projNameLabel.Text = "Name:";
            // 
            // projNameTextBox
            // 
            this.projNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projNameTextBox.Location = new System.Drawing.Point(92, 138);
            this.projNameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.projNameTextBox.Name = "projNameTextBox";
            this.projNameTextBox.Size = new System.Drawing.Size(126, 20);
            this.projNameTextBox.TabIndex = 1;
            this.projNameTextBox.TextChanged += new System.EventHandler(this.projNameTextBox_TextChanged);
            // 
            // projPathLabel
            // 
            this.projPathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.projPathLabel.AutoSize = true;
            this.projPathLabel.Location = new System.Drawing.Point(12, 162);
            this.projPathLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.projPathLabel.Name = "projPathLabel";
            this.projPathLabel.Size = new System.Drawing.Size(51, 13);
            this.projPathLabel.TabIndex = 2;
            this.projPathLabel.Text = "Location:";
            // 
            // projPathTextBox
            // 
            this.projPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projPathTextBox.Location = new System.Drawing.Point(92, 162);
            this.projPathTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.projPathTextBox.Name = "projPathTextBox";
            this.projPathTextBox.Size = new System.Drawing.Size(126, 20);
            this.projPathTextBox.TabIndex = 3;
            this.projPathTextBox.TextChanged += new System.EventHandler(this.projPathTextBox_TextChanged);
            // 
            // projPathButton
            // 
            this.projPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.projPathButton.Location = new System.Drawing.Point(228, 157);
            this.projPathButton.Margin = new System.Windows.Forms.Padding(1);
            this.projPathButton.Name = "projPathButton";
            this.projPathButton.Size = new System.Drawing.Size(64, 24);
            this.projPathButton.TabIndex = 6;
            this.projPathButton.Text = "Browse ...";
            this.projPathButton.UseVisualStyleBackColor = true;
            this.projPathButton.Click += new System.EventHandler(this.projPathButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(185, 195);
            this.okButton.Margin = new System.Windows.Forms.Padding(1);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(64, 24);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(256, 195);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(1);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(64, 24);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // fontGroupBox
            // 
            this.fontGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontGroupBox.Controls.Add(this.infoLinkLabel);
            this.fontGroupBox.Controls.Add(this.copyrightLabel);
            this.fontGroupBox.Controls.Add(this.typefaceLabel);
            this.fontGroupBox.Controls.Add(this.copyrightTextBox);
            this.fontGroupBox.Controls.Add(this.typefaceComboBox);
            this.fontGroupBox.Controls.Add(this.fontPathButton);
            this.fontGroupBox.Controls.Add(this.fontPathLabel);
            this.fontGroupBox.Controls.Add(this.fontPathTextBox);
            this.fontGroupBox.Location = new System.Drawing.Point(7, 5);
            this.fontGroupBox.Margin = new System.Windows.Forms.Padding(1);
            this.fontGroupBox.Name = "fontGroupBox";
            this.fontGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.fontGroupBox.Size = new System.Drawing.Size(314, 123);
            this.fontGroupBox.TabIndex = 9;
            this.fontGroupBox.TabStop = false;
            this.fontGroupBox.Text = "Font";
            // 
            // infoLinkLabel
            // 
            this.infoLinkLabel.AutoSize = true;
            this.infoLinkLabel.Enabled = false;
            this.infoLinkLabel.Location = new System.Drawing.Point(85, 98);
            this.infoLinkLabel.Name = "infoLinkLabel";
            this.infoLinkLabel.Size = new System.Drawing.Size(116, 13);
            this.infoLinkLabel.TabIndex = 13;
            this.infoLinkLabel.TabStop = true;
            this.infoLinkLabel.Text = "See license information";
            this.infoLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.infoLinkLabel_LinkClicked);
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Location = new System.Drawing.Point(5, 70);
            this.copyrightLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(54, 13);
            this.copyrightLabel.TabIndex = 12;
            this.copyrightLabel.Text = "Copyright:";
            // 
            // typefaceLabel
            // 
            this.typefaceLabel.AutoSize = true;
            this.typefaceLabel.Location = new System.Drawing.Point(5, 46);
            this.typefaceLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.typefaceLabel.Name = "typefaceLabel";
            this.typefaceLabel.Size = new System.Drawing.Size(55, 13);
            this.typefaceLabel.TabIndex = 11;
            this.typefaceLabel.Text = "Typeface:";
            // 
            // copyrightTextBox
            // 
            this.copyrightTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.copyrightTextBox.Location = new System.Drawing.Point(85, 70);
            this.copyrightTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.copyrightTextBox.Name = "copyrightTextBox";
            this.copyrightTextBox.ReadOnly = true;
            this.copyrightTextBox.Size = new System.Drawing.Size(126, 20);
            this.copyrightTextBox.TabIndex = 10;
            // 
            // typefaceComboBox
            // 
            this.typefaceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.typefaceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typefaceComboBox.FormattingEnabled = true;
            this.typefaceComboBox.Location = new System.Drawing.Point(85, 46);
            this.typefaceComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.typefaceComboBox.Name = "typefaceComboBox";
            this.typefaceComboBox.Size = new System.Drawing.Size(126, 21);
            this.typefaceComboBox.TabIndex = 9;
            this.typefaceComboBox.SelectedIndexChanged += new System.EventHandler(this.FaceIndexComboBox_SelectedIndexChanged);
            // 
            // fontPathButton
            // 
            this.fontPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fontPathButton.Location = new System.Drawing.Point(222, 18);
            this.fontPathButton.Margin = new System.Windows.Forms.Padding(1);
            this.fontPathButton.Name = "fontPathButton";
            this.fontPathButton.Size = new System.Drawing.Size(64, 24);
            this.fontPathButton.TabIndex = 8;
            this.fontPathButton.Text = "Browse ...";
            this.fontPathButton.UseVisualStyleBackColor = true;
            this.fontPathButton.Click += new System.EventHandler(this.fontPathButton_Click);
            // 
            // fontPathLabel
            // 
            this.fontPathLabel.AutoSize = true;
            this.fontPathLabel.Location = new System.Drawing.Point(5, 21);
            this.fontPathLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.fontPathLabel.Name = "fontPathLabel";
            this.fontPathLabel.Size = new System.Drawing.Size(51, 13);
            this.fontPathLabel.TabIndex = 6;
            this.fontPathLabel.Text = "Location:";
            // 
            // fontPathTextBox
            // 
            this.fontPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontPathTextBox.Location = new System.Drawing.Point(85, 21);
            this.fontPathTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.fontPathTextBox.Name = "fontPathTextBox";
            this.fontPathTextBox.ReadOnly = true;
            this.fontPathTextBox.Size = new System.Drawing.Size(126, 20);
            this.fontPathTextBox.TabIndex = 4;
            this.fontPathTextBox.TextChanged += new System.EventHandler(this.fontPathTextBox_TextChanged);
            // 
            // NewProjectForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(326, 235);
            this.Controls.Add(this.fontGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.projPathButton);
            this.Controls.Add(this.projPathTextBox);
            this.Controls.Add(this.projPathLabel);
            this.Controls.Add(this.projNameTextBox);
            this.Controls.Add(this.projNameLabel);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(235, 274);
            this.Name = "NewProjectForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Project";
            this.fontGroupBox.ResumeLayout(false);
            this.fontGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label projNameLabel;
        private System.Windows.Forms.TextBox projNameTextBox;
        private System.Windows.Forms.Label projPathLabel;
        private System.Windows.Forms.TextBox projPathTextBox;
        private System.Windows.Forms.Button projPathButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox fontGroupBox;
        private System.Windows.Forms.Label fontPathLabel;
        private System.Windows.Forms.TextBox fontPathTextBox;
        private System.Windows.Forms.Button fontPathButton;
        private System.Windows.Forms.ComboBox typefaceComboBox;
        private System.Windows.Forms.TextBox copyrightTextBox;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.Label typefaceLabel;
        private System.Windows.Forms.LinkLabel infoLinkLabel;
    }
}