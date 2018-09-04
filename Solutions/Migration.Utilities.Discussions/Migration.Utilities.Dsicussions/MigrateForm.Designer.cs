namespace SP_Discussion_Migrator
{
    partial class MigrateForm
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
            this.inputPathTextbox = new System.Windows.Forms.TextBox();
            this.browseButton = new MetroFramework.Controls.MetroButton();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.exitButton = new MetroFramework.Controls.MetroButton();
            this.migrateButton = new MetroFramework.Controls.MetroButton();
            this.label2 = new System.Windows.Forms.Label();
            this.targetListsComboBox = new System.Windows.Forms.ComboBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.metroProgressBar = new MetroFramework.Controls.MetroProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 112);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Xml Path";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // inputPathTextbox
            // 
            this.inputPathTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputPathTextbox.Location = new System.Drawing.Point(142, 112);
            this.inputPathTextbox.Margin = new System.Windows.Forms.Padding(1);
            this.inputPathTextbox.Name = "inputPathTextbox";
            this.inputPathTextbox.Size = new System.Drawing.Size(293, 20);
            this.inputPathTextbox.TabIndex = 1;
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(437, 112);
            this.browseButton.Margin = new System.Windows.Forms.Padding(1);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(27, 20);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "...";
            this.browseButton.UseSelectable = true;
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(24, 60);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(27, 13);
            this.linkLabel1.TabIndex = 9;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "web";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Location = new System.Drawing.Point(400, 204);
            this.exitButton.Margin = new System.Windows.Forms.Padding(2);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(85, 21);
            this.exitButton.TabIndex = 4;
            this.exitButton.Text = "E&xit";
            this.exitButton.UseSelectable = true;
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // migrateButton
            // 
            this.migrateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.migrateButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.migrateButton.Highlight = true;
            this.migrateButton.Location = new System.Drawing.Point(312, 204);
            this.migrateButton.Margin = new System.Windows.Forms.Padding(2);
            this.migrateButton.Name = "migrateButton";
            this.migrateButton.Size = new System.Drawing.Size(85, 21);
            this.migrateButton.TabIndex = 3;
            this.migrateButton.Text = "&Migrate";
            this.migrateButton.UseSelectable = true;
            this.migrateButton.UseVisualStyleBackColor = true;
            this.migrateButton.Click += new System.EventHandler(this.migrateButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 91);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Target Discussions List";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // targetListsComboBox
            // 
            this.targetListsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetListsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetListsComboBox.FormattingEnabled = true;
            this.targetListsComboBox.Location = new System.Drawing.Point(142, 89);
            this.targetListsComboBox.Margin = new System.Windows.Forms.Padding(1);
            this.targetListsComboBox.Name = "targetListsComboBox";
            this.targetListsComboBox.Size = new System.Drawing.Size(293, 21);
            this.targetListsComboBox.TabIndex = 0;
            // 
            // infoLabel
            // 
            this.infoLabel.Location = new System.Drawing.Point(143, 133);
            this.infoLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(292, 34);
            this.infoLabel.TabIndex = 13;
            this.infoLabel.Text = "...";
            // 
            // metroProgressBar
            // 
            this.metroProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroProgressBar.HideProgressText = false;
            this.metroProgressBar.Location = new System.Drawing.Point(22, 206);
            this.metroProgressBar.Maximum = 1;
            this.metroProgressBar.Name = "metroProgressBar";
            this.metroProgressBar.Size = new System.Drawing.Size(100, 16);
            this.metroProgressBar.Step = 1;
            this.metroProgressBar.TabIndex = 14;
            this.metroProgressBar.Visible = false;
            // 
            // MigrateForm
            // 
            this.AcceptButton = this.migrateButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.exitButton;
            this.ClientSize = new System.Drawing.Size(500, 240);
            this.Controls.Add(this.metroProgressBar);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.targetListsComboBox);
            this.Controls.Add(this.migrateButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.inputPathTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 240);
            this.Name = "MigrateForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.ShowIcon = false;
            this.Text = "Migrate Discussions List";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MigrateForm_FormClosed);
            this.Load += new System.EventHandler(this.MigrateForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputPathTextbox;
        private MetroFramework.Controls.MetroButton browseButton;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private MetroFramework.Controls.MetroButton exitButton;
        private MetroFramework.Controls.MetroButton migrateButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox targetListsComboBox;
        private System.Windows.Forms.Label infoLabel;
        private MetroFramework.Controls.MetroProgressBar metroProgressBar;
    }
}