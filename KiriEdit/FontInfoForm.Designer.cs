namespace KiriEdit
{
    partial class FontInfoForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.licenseTextBox = new System.Windows.Forms.TextBox();
            this.copyrightTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.typefaceTextBox = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.licenseUrlRichTextBox = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.versionTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 137);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 9, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "License:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 105);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 9, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "License URL:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 9, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Copyright:";
            // 
            // licenseTextBox
            // 
            this.licenseTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.licenseTextBox.Location = new System.Drawing.Point(119, 134);
            this.licenseTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.licenseTextBox.Multiline = true;
            this.licenseTextBox.Name = "licenseTextBox";
            this.licenseTextBox.ReadOnly = true;
            this.licenseTextBox.Size = new System.Drawing.Size(271, 153);
            this.licenseTextBox.TabIndex = 10;
            // 
            // copyrightTextBox
            // 
            this.copyrightTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.copyrightTextBox.Location = new System.Drawing.Point(119, 70);
            this.copyrightTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.copyrightTextBox.Name = "copyrightTextBox";
            this.copyrightTextBox.ReadOnly = true;
            this.copyrightTextBox.Size = new System.Drawing.Size(271, 20);
            this.copyrightTextBox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 9, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Typeface:";
            // 
            // typefaceTextBox
            // 
            this.typefaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.typefaceTextBox.Location = new System.Drawing.Point(119, 6);
            this.typefaceTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.typefaceTextBox.Name = "typefaceTextBox";
            this.typefaceTextBox.ReadOnly = true;
            this.typefaceTextBox.Size = new System.Drawing.Size(271, 20);
            this.typefaceTextBox.TabIndex = 2;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(315, 300);
            this.closeButton.Margin = new System.Windows.Forms.Padding(6);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // licenseUrlRichTextBox
            // 
            this.licenseUrlRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.licenseUrlRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.licenseUrlRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.licenseUrlRichTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.licenseUrlRichTextBox.Multiline = false;
            this.licenseUrlRichTextBox.Name = "licenseUrlRichTextBox";
            this.licenseUrlRichTextBox.ReadOnly = true;
            this.licenseUrlRichTextBox.Size = new System.Drawing.Size(269, 18);
            this.licenseUrlRichTextBox.TabIndex = 0;
            this.licenseUrlRichTextBox.Text = "";
            this.licenseUrlRichTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.licenseUrlRichTextBox_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.licenseUrlRichTextBox);
            this.panel1.Location = new System.Drawing.Point(119, 102);
            this.panel1.Margin = new System.Windows.Forms.Padding(6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(271, 20);
            this.panel1.TabIndex = 8;
            // 
            // versionTextBox
            // 
            this.versionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.versionTextBox.Location = new System.Drawing.Point(119, 38);
            this.versionTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.ReadOnly = true;
            this.versionTextBox.Size = new System.Drawing.Size(271, 20);
            this.versionTextBox.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 41);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 9, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Version:";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.typefaceTextBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.versionTextBox, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.closeButton, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.panel1, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.copyrightTextBox, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.licenseTextBox, 1, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(396, 329);
            this.tableLayoutPanel.TabIndex = 11;
            // 
            // FontInfoForm
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(408, 341);
            this.Controls.Add(this.tableLayoutPanel);
            this.MinimizeBox = false;
            this.Name = "FontInfoForm";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Font Information";
            this.Load += new System.EventHandler(this.FontInfoForm_Load);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox licenseTextBox;
        private System.Windows.Forms.TextBox copyrightTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox typefaceTextBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.RichTextBox licenseUrlRichTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox versionTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}