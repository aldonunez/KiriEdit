﻿namespace KiriEdit
{
    partial class WindowsForm
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
            this.windowsListBox = new System.Windows.Forms.ListBox();
            this.activateButton = new System.Windows.Forms.Button();
            this.closeWindowButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // windowsListBox
            // 
            this.windowsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowsListBox.Location = new System.Drawing.Point(0, 0);
            this.windowsListBox.Margin = new System.Windows.Forms.Padding(0);
            this.windowsListBox.Name = "windowsListBox";
            this.windowsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.windowsListBox.Size = new System.Drawing.Size(200, 116);
            this.windowsListBox.TabIndex = 0;
            this.windowsListBox.SelectedIndexChanged += new System.EventHandler(this.windowsListBox_SelectedIndexChanged);
            // 
            // activateButton
            // 
            this.activateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.activateButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.activateButton.Enabled = false;
            this.activateButton.Location = new System.Drawing.Point(217, 12);
            this.activateButton.Name = "activateButton";
            this.activateButton.Size = new System.Drawing.Size(111, 23);
            this.activateButton.TabIndex = 1;
            this.activateButton.Text = "Activate";
            this.activateButton.UseVisualStyleBackColor = true;
            this.activateButton.Click += new System.EventHandler(this.activateButton_Click);
            // 
            // closeWindowButton
            // 
            this.closeWindowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeWindowButton.Enabled = false;
            this.closeWindowButton.Location = new System.Drawing.Point(217, 41);
            this.closeWindowButton.Name = "closeWindowButton";
            this.closeWindowButton.Size = new System.Drawing.Size(111, 23);
            this.closeWindowButton.TabIndex = 2;
            this.closeWindowButton.Text = "Close Windows";
            this.closeWindowButton.UseVisualStyleBackColor = true;
            this.closeWindowButton.Click += new System.EventHandler(this.closeWindowButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(217, 141);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(111, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.windowsListBox, 0, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(11, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(200, 152);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // WindowsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(340, 176);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.closeWindowButton);
            this.Controls.Add(this.activateButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WindowsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Windows";
            this.Load += new System.EventHandler(this.WindowsForm_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox windowsListBox;
        private System.Windows.Forms.Button activateButton;
        private System.Windows.Forms.Button closeWindowButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}