using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RuneFleet.Models;

namespace RuneFleet
{
    internal class EditAccountForm : Form
    {
        private readonly Account account;
        private TextBox txtDisplayName;
        private TextBox txtGroup;
        private TextBox txtClient;
        private TextBox txtArguments;
        private Button btnSave;
        private Button btnCancel;

        public EditAccountForm(Account account)
        {
            this.account = account;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Edit Account";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(400, 170);

            Label lblDisplayName = new Label() { Text = "Display Name", Left = 10, Top = 15, AutoSize = true };
            txtDisplayName = new TextBox() { Left = 110, Top = 12, Width = 270, Text = account.DisplayName ?? string.Empty };

            Label lblGroup = new Label() { Text = "Group", Left = 10, Top = 45, AutoSize = true };
            txtGroup = new TextBox() { Left = 110, Top = 42, Width = 270, Text = account.Group != null ? string.Join(";", account.Group) : string.Empty };

            Label lblClient = new Label() { Text = "Client", Left = 10, Top = 75, AutoSize = true };
            txtClient = new TextBox() { Left = 110, Top = 72, Width = 270, Text = account.Client ?? string.Empty };

            Label lblArguments = new Label() { Text = "Arguments", Left = 10, Top = 105, AutoSize = true };
            txtArguments = new TextBox() { Left = 110, Top = 102, Width = 270, Text = account.Arguments ?? string.Empty };

            btnSave = new Button() { Text = "Save", Left = 224, Width = 75, Top = 130, DialogResult = DialogResult.None };
            btnSave.Click += btnSave_Click;
            btnCancel = new Button() { Text = "Cancel", Left = 305, Width = 75, Top = 130, DialogResult = DialogResult.Cancel };

            this.Controls.AddRange(new Control[] { lblDisplayName, txtDisplayName, lblGroup, txtGroup, lblClient, txtClient, lblArguments, txtArguments, btnSave, btnCancel });
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            var clientPath = txtClient.Text.Trim();
            if (!string.IsNullOrWhiteSpace(clientPath) && !File.Exists(clientPath))
            {
                MessageBox.Show("Client path does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var groupText = txtGroup.Text.Trim();
            if (groupText.Contains(','))
            {
                MessageBox.Show("Groups must be separated using ';'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            account.DisplayName = txtDisplayName.Text.Trim();
            account.Group = groupText.Split(';', StringSplitOptions.RemoveEmptyEntries);
            account.Client = clientPath;
            account.Arguments = txtArguments.Text.Trim();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
