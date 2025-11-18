// 
// Copyright © 2010-2025, Sinclair Community College
// Licensed under the GNU General Public License, version 3.
// See the LICENSE file in the project root for full license information.  
//
// This file is part of Make Me Admin.
//
// Make Me Admin is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Make Me Admin is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Make Me Admin. If not, see <http://www.gnu.org/licenses/>.
//

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Windows.Forms;

    public partial class RequestReasonDialog : Form
    {
        public RequestReasonDialog()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.SecurityLock;
            this.reasonTextBox.MaxLength = Settings.MaximumReasonLength;

            if ((Settings.CannedReasons != null) && (Settings.CannedReasons.Length > 0))
            {
                this.responseComboBox.Items.AddRange(Settings.CannedReasons);
            }

            this.reasonTextBox.Enabled = Settings.AllowFreeTextReason;

            if (this.reasonTextBox.Enabled)
            {
                this.responseComboBox.Items.Add(Properties.Resources.OtherReason);
            }
            
            if (this.responseComboBox.Items.Count > 0)
            {
                this.responseComboBox.SelectedIndex = 0;
            }            

            if (this.responseComboBox.Items.Count <= 1)
            {
                this.responseComboBox.Enabled = false;
            }

            // Apply rounded corners to the dialog
            ApplyRoundedCorners();
        }

        /// <summary>
        /// Applies rounded corners to the dialog.
        /// </summary>
        private void ApplyRoundedCorners()
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 15;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            this.Region = new System.Drawing.Region(path);
        }

        /// <summary>
        /// Handles the Paint event for the dialog to draw rounded borders.
        /// </summary>
        private void Dialog_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, 2))
            {
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                int radius = 15;
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(this.Width - radius - 2, 0, radius, radius, 270, 90);
                path.AddArc(this.Width - radius - 2, this.Height - radius - 2, radius, radius, 0, 90);
                path.AddArc(0, this.Height - radius - 2, radius, radius, 90, 90);
                path.CloseFigure();
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void FormLoadHandler(object sender, EventArgs e)
        {
            if (this.reasonTextBox.Enabled && ((Settings.CannedReasons == null) || Settings.CannedReasons.Length == 0))
            {
                this.reasonTextBox.Focus();
            }
            else
            {
                this.responseComboBox.Focus();
            }
            SetOKButtonState();
        }

        private void ReasonTextBoxChangedHandler(object sender, EventArgs e)
        {
            this.responseComboBox.SelectedItem = Properties.Resources.OtherReason;
            SetOKButtonState();
            UpdateCharacterCount();
        }

        /// <summary>
        /// Updates the character count label.
        /// </summary>
        private void UpdateCharacterCount()
        {
            int currentLength = this.reasonTextBox.Text.Trim().Length;
            this.charCountLabel.Text = string.Format("{0} / 40 characters", currentLength);
            this.charCountLabel.ForeColor = currentLength >= 40 ? System.Drawing.Color.Green : System.Drawing.Color.Gray;
        }

        private void ResponseComboBoxSelectionChangeCommitted(object sender, EventArgs e)
        {
            SetOKButtonState();
        }

        private void SetOKButtonState()
        {
            //if (Settings.PromptForReason == ReasonPrompt.Required)
            //{
                if (this.responseComboBox.SelectedIndex >= 0)
                { // Something is selected in the combo box.
                    string selectedItemText = ((string)this.responseComboBox.SelectedItem).Trim();
                    if (string.Compare(selectedItemText, Properties.Resources.OtherReason, true) == 0)
                    { // The "Other" item is selected in the combo box.
                        // Enable the OK button if there is at least 40 characters in the text box.
                        this.okButton.Enabled = this.reasonTextBox.Text.Trim().Length >= 40;
                    }
                    else
                    {
                        // Canned reasons must also be at least 40 characters
                        this.okButton.Enabled = selectedItemText.Length >= 40;
                    }
                }
                else
                { // Nothing is selected in the combo box.
                    // Enable the OK button if there is at least 40 characters in the text box.
                    this.okButton.Enabled = this.reasonTextBox.Text.Trim().Length >= 40;
                }
            //}
        }

        private void CancelButtonClickHandler(object sender, EventArgs e)
        {
            this.Close();
        }

        public string Reason
        {
            get
            {
                if (this.responseComboBox.SelectedIndex >= 0)
                { // Something is selected in the combo box.
                    string selectedItemText = ((string)this.responseComboBox.SelectedItem).Trim();
                    if (string.Compare(selectedItemText, Properties.Resources.OtherReason, true) == 0)
                    {
                        return string.Format("{0}: {1}", Properties.Resources.OtherReason, this.reasonTextBox.Text.Trim());
                    }
                    else
                    {
                        return selectedItemText;
                    }
                }
                else
                { // Nothing is selected in the combo box. Return the contents of the text box, even if empty.
                    return this.reasonTextBox.Text.Trim();
                }
            }
        }

    }
}
