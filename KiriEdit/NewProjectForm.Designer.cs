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
            this.fontPathButton = new System.Windows.Forms.Button();
            this.fontPathLabel = new System.Windows.Forms.Label();
            this.fontNameLabel = new System.Windows.Forms.Label();
            this.fontPathTextBox = new System.Windows.Forms.TextBox();
            this.fontNameTextBox = new System.Windows.Forms.TextBox();
            this.fontGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // projNameLabel
            // 
            this.projNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.projNameLabel.AutoSize = true;
            this.projNameLabel.Location = new System.Drawing.Point(31, 306);
            this.projNameLabel.Name = "projNameLabel";
            this.projNameLabel.Size = new System.Drawing.Size(98, 32);
            this.projNameLabel.TabIndex = 0;
            this.projNameLabel.Text = "Name:";
            // 
            // projNameTextBox
            // 
            this.projNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projNameTextBox.Location = new System.Drawing.Point(244, 306);
            this.projNameTextBox.Margin = new System.Windows.Forms.Padding(10);
            this.projNameTextBox.Name = "projNameTextBox";
            this.projNameTextBox.Size = new System.Drawing.Size(275, 38);
            this.projNameTextBox.TabIndex = 1;
            this.projNameTextBox.TextChanged += new System.EventHandler(this.projNameTextBox_TextChanged);
            // 
            // projPathLabel
            // 
            this.projPathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.projPathLabel.AutoSize = true;
            this.projPathLabel.Location = new System.Drawing.Point(31, 364);
            this.projPathLabel.Name = "projPathLabel";
            this.projPathLabel.Size = new System.Drawing.Size(132, 32);
            this.projPathLabel.TabIndex = 2;
            this.projPathLabel.Text = "Location:";
            // 
            // projPathTextBox
            // 
            this.projPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projPathTextBox.Location = new System.Drawing.Point(244, 364);
            this.projPathTextBox.Margin = new System.Windows.Forms.Padding(10);
            this.projPathTextBox.Name = "projPathTextBox";
            this.projPathTextBox.Size = new System.Drawing.Size(275, 38);
            this.projPathTextBox.TabIndex = 3;
            this.projPathTextBox.TextChanged += new System.EventHandler(this.projPathTextBox_TextChanged);
            // 
            // projPathButton
            // 
            this.projPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.projPathButton.Location = new System.Drawing.Point(541, 353);
            this.projPathButton.Name = "projPathButton";
            this.projPathButton.Size = new System.Drawing.Size(172, 58);
            this.projPathButton.TabIndex = 6;
            this.projPathButton.Text = "Browse ...";
            this.projPathButton.UseVisualStyleBackColor = true;
            this.projPathButton.Click += new System.EventHandler(this.projPathButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(426, 444);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(172, 58);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(616, 444);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(172, 58);
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
            this.fontGroupBox.Controls.Add(this.fontPathButton);
            this.fontGroupBox.Controls.Add(this.fontPathLabel);
            this.fontGroupBox.Controls.Add(this.fontNameLabel);
            this.fontGroupBox.Controls.Add(this.fontPathTextBox);
            this.fontGroupBox.Controls.Add(this.fontNameTextBox);
            this.fontGroupBox.Location = new System.Drawing.Point(18, 12);
            this.fontGroupBox.Name = "fontGroupBox";
            this.fontGroupBox.Padding = new System.Windows.Forms.Padding(10);
            this.fontGroupBox.Size = new System.Drawing.Size(770, 272);
            this.fontGroupBox.TabIndex = 9;
            this.fontGroupBox.TabStop = false;
            this.fontGroupBox.Text = "Font";
            // 
            // fontPathButton
            // 
            this.fontPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fontPathButton.Location = new System.Drawing.Point(519, 98);
            this.fontPathButton.Name = "fontPathButton";
            this.fontPathButton.Size = new System.Drawing.Size(172, 58);
            this.fontPathButton.TabIndex = 8;
            this.fontPathButton.Text = "Browse ...";
            this.fontPathButton.UseVisualStyleBackColor = true;
            this.fontPathButton.Click += new System.EventHandler(this.fontPathButton_Click);
            // 
            // fontPathLabel
            // 
            this.fontPathLabel.AutoSize = true;
            this.fontPathLabel.Location = new System.Drawing.Point(13, 109);
            this.fontPathLabel.Name = "fontPathLabel";
            this.fontPathLabel.Size = new System.Drawing.Size(132, 32);
            this.fontPathLabel.TabIndex = 6;
            this.fontPathLabel.Text = "Location:";
            // 
            // fontNameLabel
            // 
            this.fontNameLabel.AutoSize = true;
            this.fontNameLabel.Location = new System.Drawing.Point(13, 51);
            this.fontNameLabel.Name = "fontNameLabel";
            this.fontNameLabel.Size = new System.Drawing.Size(98, 32);
            this.fontNameLabel.TabIndex = 5;
            this.fontNameLabel.Text = "Name:";
            // 
            // fontPathTextBox
            // 
            this.fontPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontPathTextBox.Location = new System.Drawing.Point(230, 109);
            this.fontPathTextBox.Margin = new System.Windows.Forms.Padding(10);
            this.fontPathTextBox.Name = "fontPathTextBox";
            this.fontPathTextBox.Size = new System.Drawing.Size(261, 38);
            this.fontPathTextBox.TabIndex = 4;
            this.fontPathTextBox.TextChanged += new System.EventHandler(this.fontPathTextBox_TextChanged);
            // 
            // fontNameTextBox
            // 
            this.fontNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontNameTextBox.Location = new System.Drawing.Point(230, 51);
            this.fontNameTextBox.Margin = new System.Windows.Forms.Padding(10);
            this.fontNameTextBox.Name = "fontNameTextBox";
            this.fontNameTextBox.ReadOnly = true;
            this.fontNameTextBox.Size = new System.Drawing.Size(261, 38);
            this.fontNameTextBox.TabIndex = 3;
            // 
            // NewProjectForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(800, 519);
            this.Controls.Add(this.fontGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.projPathButton);
            this.Controls.Add(this.projPathTextBox);
            this.Controls.Add(this.projPathLabel);
            this.Controls.Add(this.projNameTextBox);
            this.Controls.Add(this.projNameLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 600);
            this.Name = "NewProjectForm";
            this.ShowInTaskbar = false;
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
        private System.Windows.Forms.Label fontNameLabel;
        private System.Windows.Forms.TextBox fontPathTextBox;
        private System.Windows.Forms.TextBox fontNameTextBox;
        private System.Windows.Forms.Button fontPathButton;
    }
}