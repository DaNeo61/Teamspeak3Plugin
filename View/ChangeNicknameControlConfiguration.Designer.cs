namespace Teamspeak3Plugin.View
{
    partial class ChangeNicknameControlConfiguration
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
            chLabel = new Label();
            nickNameTextBox = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            SuspendLayout();
            // 
            // chLabel
            // 
            chLabel.AutoSize = true;
            chLabel.Location = new Point(16, 17);
            chLabel.Name = "chLabel";
            chLabel.Size = new Size(99, 23);
            chLabel.TabIndex = 3;
            chLabel.Text = "Nickname:";
            // 
            // nickNameTextBox
            // 
            nickNameTextBox.BackColor = Color.FromArgb(65, 65, 65);
            nickNameTextBox.Font = new Font("Tahoma", 9F);
            nickNameTextBox.Icon = null;
            nickNameTextBox.Location = new Point(164, 14);
            nickNameTextBox.MaxCharacters = 32767;
            nickNameTextBox.Multiline = false;
            nickNameTextBox.Name = "nickNameTextBox";
            nickNameTextBox.Padding = new Padding(8, 5, 8, 5);
            nickNameTextBox.PasswordChar = false;
            nickNameTextBox.PlaceHolderColor = Color.Gray;
            nickNameTextBox.PlaceHolderText = "";
            nickNameTextBox.ReadOnly = false;
            nickNameTextBox.ScrollBars = ScrollBars.None;
            nickNameTextBox.SelectionStart = 0;
            nickNameTextBox.Size = new Size(302, 25);
            nickNameTextBox.TabIndex = 4;
            nickNameTextBox.TextAlignment = HorizontalAlignment.Left;
            // 
            // ChangeNicknameControlConfiguration
            // 
            AutoScaleDimensions = new SizeF(10F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(nickNameTextBox);
            Controls.Add(chLabel);
            Name = "ChangeNicknameControlConfiguration";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label chLabel;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox nickNameTextBox;
    }
}
