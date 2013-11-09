namespace OverlayDropper
{
    partial class OverlayDropper
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverlayDropper));
            this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
            this.GameSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ATEMIpAddressTextBox = new System.Windows.Forms.TextBox();
            this.ATEMConnectButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.HotKeyLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ProgLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxEnabled
            // 
            this.checkBoxEnabled.AutoSize = true;
            this.checkBoxEnabled.Location = new System.Drawing.Point(204, 101);
            this.checkBoxEnabled.Name = "checkBoxEnabled";
            this.checkBoxEnabled.Size = new System.Drawing.Size(65, 17);
            this.checkBoxEnabled.TabIndex = 0;
            this.checkBoxEnabled.Text = "Enabled";
            this.checkBoxEnabled.UseVisualStyleBackColor = true;
            this.checkBoxEnabled.CheckedChanged += new System.EventHandler(this.checkBoxEnabled_CheckedChanged);
            // 
            // GameSelector
            // 
            this.GameSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GameSelector.FormattingEnabled = true;
            this.GameSelector.Items.AddRange(new object[] {
            "",
            "League of Legends",
            "Starcraft 2"});
            this.GameSelector.Location = new System.Drawing.Point(53, 99);
            this.GameSelector.Name = "GameSelector";
            this.GameSelector.Size = new System.Drawing.Size(121, 21);
            this.GameSelector.TabIndex = 1;
            this.GameSelector.SelectedIndexChanged += new System.EventHandler(this.GameSelector_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Game:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "ATEM Software v4.2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "IP Address:";
            // 
            // ATEMIpAddressTextBox
            // 
            this.ATEMIpAddressTextBox.Location = new System.Drawing.Point(76, 31);
            this.ATEMIpAddressTextBox.Name = "ATEMIpAddressTextBox";
            this.ATEMIpAddressTextBox.Size = new System.Drawing.Size(100, 20);
            this.ATEMIpAddressTextBox.TabIndex = 5;
            this.ATEMIpAddressTextBox.TextChanged += new System.EventHandler(this.ATEMIpAddressTextBox_TextChanged);
            // 
            // ATEMConnectButton
            // 
            this.ATEMConnectButton.Location = new System.Drawing.Point(197, 28);
            this.ATEMConnectButton.Name = "ATEMConnectButton";
            this.ATEMConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ATEMConnectButton.TabIndex = 6;
            this.ATEMConnectButton.Text = "Connect";
            this.ATEMConnectButton.UseVisualStyleBackColor = true;
            this.ATEMConnectButton.Click += new System.EventHandler(this.ATEMConnectButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 2);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Hotkey Binding";
            // 
            // HotKeyLabel
            // 
            this.HotKeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HotKeyLabel.Location = new System.Drawing.Point(12, 132);
            this.HotKeyLabel.Name = "HotKeyLabel";
            this.HotKeyLabel.Size = new System.Drawing.Size(257, 23);
            this.HotKeyLabel.TabIndex = 10;
            this.HotKeyLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::OverlayDropper.Properties.Resources.ACLProLogo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 158);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(260, 101);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // ProgLabel
            // 
            this.ProgLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgLabel.Location = new System.Drawing.Point(121, 9);
            this.ProgLabel.Name = "ProgLabel";
            this.ProgLabel.Size = new System.Drawing.Size(151, 16);
            this.ProgLabel.TabIndex = 12;
            this.ProgLabel.Text = "Not Connected";
            this.ProgLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // OverlayDropper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.ProgLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.HotKeyLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ATEMConnectButton);
            this.Controls.Add(this.ATEMIpAddressTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GameSelector);
            this.Controls.Add(this.checkBoxEnabled);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OverlayDropper";
            this.Text = "Overlay Dropper by ACLPro";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxEnabled;
        private System.Windows.Forms.ComboBox GameSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ATEMIpAddressTextBox;
        private System.Windows.Forms.Button ATEMConnectButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label HotKeyLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label ProgLabel;

    }
}

