namespace Teamspeak3Plugin.View
{
    partial class SwitchChannelControlConfiguration
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
            AppComboBox = new SuchByte.MacroDeck.GUI.CustomControls.RoundedComboBox();
            refreshButton = new SuchByte.MacroDeck.GUI.CustomControls.ButtonPrimary();
            SuspendLayout();
            // 
            // AppComboBox
            // 
            AppComboBox.BackColor = Color.FromArgb(65, 65, 65);
            AppComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            AppComboBox.Font = new Font("Tahoma", 9F);
            AppComboBox.Icon = null;
            AppComboBox.Location = new Point(140, 14);
            AppComboBox.Name = "AppComboBox";
            AppComboBox.Padding = new Padding(8, 2, 8, 2);
            AppComboBox.SelectedIndex = -1;
            AppComboBox.SelectedItem = null;
            AppComboBox.Size = new Size(302, 26);
            AppComboBox.TabIndex = 0;
            // 
            // refreshButton
            // 
            refreshButton.BorderRadius = 8;
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.FlatStyle = FlatStyle.Flat;
            refreshButton.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            refreshButton.ForeColor = Color.White;
            refreshButton.HoverColor = Color.Empty;
            refreshButton.Icon = null;
            refreshButton.Location = new Point(448, 14);
            refreshButton.Name = "refreshButton";
            refreshButton.Progress = 0;
            refreshButton.ProgressColor = Color.FromArgb(0, 103, 205);
            refreshButton.Size = new Size(110, 26);
            refreshButton.TabIndex = 1;
            refreshButton.Text = "Refresh";
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.UseWindowsAccentColor = true;
            refreshButton.WriteProgress = true;
            refreshButton.Click += refreshButton_Click;
            // 
            // SwitchChannelControlConfiguration
            // 
            AutoScaleDimensions = new SizeF(10F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(refreshButton);
            Controls.Add(AppComboBox);
            Name = "SwitchChannelControlConfiguration";
            ResumeLayout(false);
        }

        #endregion

        private SuchByte.MacroDeck.GUI.CustomControls.RoundedComboBox AppComboBox;
        private SuchByte.MacroDeck.GUI.CustomControls.ButtonPrimary refreshButton;
    }
}
