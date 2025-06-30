namespace RuneFleet
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListView listViewAccounts;
        private System.Windows.Forms.ColumnHeader columnHeaderDisplayName;
        private System.Windows.Forms.ColumnHeader columnHeaderPID;
        private System.Windows.Forms.Button buttonLaunchAll;
        private System.Windows.Forms.FlowLayoutPanel flowPanelProcesses;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            listViewAccounts = new ListView();
            columnHeaderDisplayName = new ColumnHeader();
            columnHeaderPID = new ColumnHeader();
            buttonLaunchAll = new Button();
            flowPanelProcesses = new FlowLayoutPanel();
            refreshPane = new Button();
            groupSelection = new ComboBox();
            buttonWatchCharacters = new Button();
            checkTopMost = new CheckBox();
            pictureLoading = new PictureBox();
            labelLoading = new Label();
            numericClientScale = new NumericUpDown();
            checkBoxScale = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureLoading).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericClientScale).BeginInit();
            SuspendLayout();
            // 
            // listViewAccounts
            // 
            listViewAccounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listViewAccounts.Columns.AddRange(new ColumnHeader[] { columnHeaderDisplayName, columnHeaderPID });
            listViewAccounts.FullRowSelect = true;
            listViewAccounts.Location = new Point(12, 44);
            listViewAccounts.MultiSelect = false;
            listViewAccounts.Name = "listViewAccounts";
            listViewAccounts.Size = new Size(174, 409);
            listViewAccounts.TabIndex = 0;
            listViewAccounts.UseCompatibleStateImageBehavior = false;
            listViewAccounts.View = View.Details;
            listViewAccounts.ItemActivate += listViewAccounts_ItemActivate;
            listViewAccounts.Click += listViewAccounts_ItemActivate;
            listViewAccounts.DoubleClick += buttonLaunchSelected_Click;
            // 
            // columnHeaderDisplayName
            // 
            columnHeaderDisplayName.Text = "Character";
            columnHeaderDisplayName.Width = 95;
            // 
            // columnHeaderPID
            // 
            columnHeaderPID.Text = "Process";
            columnHeaderPID.Width = 55;
            // 
            // buttonLaunchAll
            // 
            buttonLaunchAll.Location = new Point(443, 12);
            buttonLaunchAll.Name = "buttonLaunchAll";
            buttonLaunchAll.Size = new Size(94, 26);
            buttonLaunchAll.TabIndex = 2;
            buttonLaunchAll.Text = "Launch Group";
            buttonLaunchAll.Click += buttonLaunchAll_Click;
            // 
            // flowPanelProcesses
            // 
            flowPanelProcesses.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowPanelProcesses.AutoScroll = true;
            flowPanelProcesses.AutoSize = true;
            flowPanelProcesses.BorderStyle = BorderStyle.FixedSingle;
            flowPanelProcesses.Location = new Point(192, 44);
            flowPanelProcesses.Name = "flowPanelProcesses";
            flowPanelProcesses.Size = new Size(406, 409);
            flowPanelProcesses.TabIndex = 3;
            // 
            // refreshPane
            // 
            refreshPane.Location = new Point(543, 12);
            refreshPane.Name = "refreshPane";
            refreshPane.Size = new Size(55, 26);
            refreshPane.TabIndex = 4;
            refreshPane.Text = "Refresh";
            refreshPane.UseVisualStyleBackColor = true;
            refreshPane.Click += buttonLoadPreviews_Click;
            // 
            // groupSelection
            // 
            groupSelection.DropDownStyle = ComboBoxStyle.DropDownList;
            groupSelection.FormattingEnabled = true;
            groupSelection.Location = new Point(302, 14);
            groupSelection.Name = "groupSelection";
            groupSelection.Size = new Size(135, 23);
            groupSelection.TabIndex = 6;
            groupSelection.SelectedValueChanged += groupSelection_SelectedValueChanged;
            // 
            // buttonWatchCharacters
            // 
            buttonWatchCharacters.Location = new Point(64, 12);
            buttonWatchCharacters.Name = "buttonWatchCharacters";
            buttonWatchCharacters.Size = new Size(122, 26);
            buttonWatchCharacters.TabIndex = 7;
            buttonWatchCharacters.Text = "Start Import Helper";
            buttonWatchCharacters.UseVisualStyleBackColor = true;
            buttonWatchCharacters.Click += buttonWatchCharacters_Click;
            // 
            // checkTopMost
            // 
            checkTopMost.AutoSize = true;
            checkTopMost.Checked = true;
            checkTopMost.CheckState = CheckState.Checked;
            checkTopMost.Location = new Point(13, 17);
            checkTopMost.Name = "checkTopMost";
            checkTopMost.Size = new Size(45, 19);
            checkTopMost.TabIndex = 8;
            checkTopMost.Text = "Top";
            checkTopMost.UseVisualStyleBackColor = true;
            checkTopMost.CheckedChanged += checkTopMost_CheckedChanged;
            // 
            // pictureLoading
            // 
            pictureLoading.Image = Properties.Resources.Dots_Loader;
            pictureLoading.Location = new Point(13, 172);
            pictureLoading.Name = "pictureLoading";
            pictureLoading.Size = new Size(171, 122);
            pictureLoading.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureLoading.TabIndex = 9;
            pictureLoading.TabStop = false;
            pictureLoading.Visible = false;
            // 
            // labelLoading
            // 
            labelLoading.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            labelLoading.Location = new Point(13, 45);
            labelLoading.MaximumSize = new Size(171, 0);
            labelLoading.MinimumSize = new Size(0, 407);
            labelLoading.Name = "labelLoading";
            labelLoading.Size = new Size(171, 407);
            labelLoading.TabIndex = 10;
            labelLoading.Text = resources.GetString("labelLoading.Text");
            labelLoading.Visible = false;
            // 
            // numericClientScale
            // 
            numericClientScale.DecimalPlaces = 1;
            numericClientScale.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericClientScale.Location = new Point(251, 14);
            numericClientScale.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numericClientScale.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numericClientScale.Name = "numericClientScale";
            numericClientScale.Size = new Size(45, 23);
            numericClientScale.TabIndex = 11;
            numericClientScale.Value = new decimal(new int[] { 10, 0, 0, 65536 });
            // 
            // checkBoxScale
            // 
            checkBoxScale.AutoSize = true;
            checkBoxScale.Location = new Point(192, 17);
            checkBoxScale.Name = "checkBoxScale";
            checkBoxScale.Size = new Size(53, 19);
            checkBoxScale.TabIndex = 12;
            checkBoxScale.Text = "Scale";
            checkBoxScale.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            ClientSize = new Size(608, 461);
            Controls.Add(checkBoxScale);
            Controls.Add(numericClientScale);
            Controls.Add(pictureLoading);
            Controls.Add(labelLoading);
            Controls.Add(checkTopMost);
            Controls.Add(buttonWatchCharacters);
            Controls.Add(groupSelection);
            Controls.Add(refreshPane);
            Controls.Add(listViewAccounts);
            Controls.Add(buttonLaunchAll);
            Controls.Add(flowPanelProcesses);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(624, 500);
            Name = "MainForm";
            Text = "RuneFleet";
            TopMost = true;
            FormClosing += MainForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureLoading).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericClientScale).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private Button refreshPane;
        private ComboBox groupSelection;
        private Button buttonWatchCharacters;
        private CheckBox checkTopMost;
        private PictureBox pictureLoading;
        private Label labelLoading;
        private NumericUpDown numericClientScale;
        private CheckBox checkBoxScale;
    }
}
