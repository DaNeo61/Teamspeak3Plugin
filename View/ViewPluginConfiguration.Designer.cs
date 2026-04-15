namespace Teamspeak3Plugin.View
{
    partial class ViewPluginConfiguration
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            QueryLabel = new Label();
            QueryInputTextBox = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            SaveButton = new SuchByte.MacroDeck.GUI.CustomControls.ButtonPrimary();
            label1 = new Label();
            RefreshRateInput = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            SuspendLayout();
            // 
            // QueryLabel
            // 
            QueryLabel.AutoSize = true;
            QueryLabel.Location = new Point(14, 14);
            QueryLabel.Name = "QueryLabel";
            QueryLabel.Size = new Size(181, 16);
            QueryLabel.TabIndex = 0;
            QueryLabel.Text = "Teamspeak 3 Client Query API";
            // 
            // QueryInputTextBox
            // 
            QueryInputTextBox.BackColor = Color.FromArgb(65, 65, 65);
            QueryInputTextBox.Font = new Font("Tahoma", 9F);
            QueryInputTextBox.Icon = null;
            QueryInputTextBox.Location = new Point(300, 14);
            QueryInputTextBox.MaxCharacters = 32767;
            QueryInputTextBox.Multiline = false;
            QueryInputTextBox.Name = "QueryInputTextBox";
            QueryInputTextBox.Padding = new Padding(8, 5, 8, 5);
            QueryInputTextBox.PasswordChar = false;
            QueryInputTextBox.PlaceHolderColor = Color.Gray;
            QueryInputTextBox.PlaceHolderText = "";
            QueryInputTextBox.ReadOnly = false;
            QueryInputTextBox.ScrollBars = ScrollBars.None;
            QueryInputTextBox.SelectionStart = 0;
            QueryInputTextBox.Size = new Size(491, 25);
            QueryInputTextBox.TabIndex = 1;
            QueryInputTextBox.TextAlignment = HorizontalAlignment.Left;
            // 
            // SaveButton
            // 
            SaveButton.BorderRadius = 8;
            SaveButton.FlatAppearance.BorderSize = 0;
            SaveButton.FlatStyle = FlatStyle.Flat;
            SaveButton.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SaveButton.ForeColor = Color.White;
            SaveButton.HoverColor = Color.Empty;
            SaveButton.Icon = null;
            SaveButton.Location = new Point(641, 144);
            SaveButton.Name = "SaveButton";
            SaveButton.Progress = 0;
            SaveButton.ProgressColor = Color.FromArgb(0, 103, 205);
            SaveButton.Size = new Size(150, 40);
            SaveButton.TabIndex = 2;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.UseWindowsAccentColor = true;
            SaveButton.WriteProgress = true;
            SaveButton.Click += SaveButtonClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 53);
            label1.Name = "label1";
            label1.Size = new Size(112, 16);
            label1.TabIndex = 3;
            label1.Text = "Query Refreshrate";
            // 
            // RefreshRateInput
            // 
            RefreshRateInput.BackColor = Color.FromArgb(65, 65, 65);
            RefreshRateInput.Font = new Font("Tahoma", 9F);
            RefreshRateInput.Icon = null;
            RefreshRateInput.Location = new Point(300, 53);
            RefreshRateInput.MaxCharacters = 32767;
            RefreshRateInput.Multiline = false;
            RefreshRateInput.Name = "RefreshRateInput";
            RefreshRateInput.Padding = new Padding(8, 5, 8, 5);
            RefreshRateInput.PasswordChar = false;
            RefreshRateInput.PlaceHolderColor = Color.Gray;
            RefreshRateInput.PlaceHolderText = "";
            RefreshRateInput.ReadOnly = false;
            RefreshRateInput.ScrollBars = ScrollBars.None;
            RefreshRateInput.SelectionStart = 0;
            RefreshRateInput.Size = new Size(491, 25);
            RefreshRateInput.TabIndex = 4;
            RefreshRateInput.TextAlignment = HorizontalAlignment.Left;
            // 
            // ViewPluginConfiguration
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(802, 195);
            Controls.Add(RefreshRateInput);
            Controls.Add(label1);
            Controls.Add(SaveButton);
            Controls.Add(QueryInputTextBox);
            Controls.Add(QueryLabel);
            Name = "ViewPluginConfiguration";
            Text = "Teamspeak 3 Integration Configuration";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label QueryLabel;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox QueryInputTextBox;
        private SuchByte.MacroDeck.GUI.CustomControls.ButtonPrimary SaveButton;
        private Label label1;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox RefreshRateInput;
    }
}
