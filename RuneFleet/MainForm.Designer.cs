namespace RuneFleet
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListView listViewAccounts;
        private System.Windows.Forms.ColumnHeader columnHeaderDisplayName;
        private System.Windows.Forms.ColumnHeader columnHeaderPID;
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
            flowPanelProcesses = new FlowLayoutPanel();
            groupSelection = new ComboBox();
            labelLoading = new Label();
            numericClientScale = new NumericUpDown();
            toolStrip1 = new ToolStrip();
            toolStripSeparator6 = new ToolStripSeparator();
            toolStripButtonTop = new ToolStripButton();
            toolStripSeparator8 = new ToolStripSeparator();
            toolStripButtonHot = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            toolStripButtonImport = new ToolStripButton();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButtonLaunch = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripButtonRefresh = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripButtonScale = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripButtonHelp = new ToolStripButton();
            toolStripSeparator9 = new ToolStripSeparator();
            toolStripButtonBorderless = new ToolStripButton();
            toolStripSeparator7 = new ToolStripSeparator();
            columnHeaderId = new ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)numericClientScale).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listViewAccounts
            // 
            listViewAccounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listViewAccounts.Columns.AddRange(new ColumnHeader[] { columnHeaderDisplayName, columnHeaderPID, columnHeaderId });
            listViewAccounts.FullRowSelect = true;
            listViewAccounts.Location = new Point(11, 28);
            listViewAccounts.MultiSelect = false;
            listViewAccounts.Name = "listViewAccounts";
            listViewAccounts.Size = new Size(174, 425);
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
            // flowPanelProcesses
            // 
            flowPanelProcesses.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowPanelProcesses.AutoScroll = true;
            flowPanelProcesses.AutoSize = true;
            flowPanelProcesses.BorderStyle = BorderStyle.FixedSingle;
            flowPanelProcesses.Location = new Point(192, 28);
            flowPanelProcesses.Name = "flowPanelProcesses";
            flowPanelProcesses.Size = new Size(432, 425);
            flowPanelProcesses.TabIndex = 3;
            // 
            // groupSelection
            // 
            groupSelection.DropDownStyle = ComboBoxStyle.DropDownList;
            groupSelection.FormattingEnabled = true;
            groupSelection.Location = new Point(135, 322);
            groupSelection.Name = "groupSelection";
            groupSelection.Size = new Size(88, 23);
            groupSelection.TabIndex = 6;
            groupSelection.SelectedValueChanged += groupSelection_SelectedValueChanged;
            // 
            // labelLoading
            // 
            labelLoading.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            labelLoading.Location = new Point(12, 29);
            labelLoading.MaximumSize = new Size(171, 0);
            labelLoading.MinimumSize = new Size(0, 407);
            labelLoading.Name = "labelLoading";
            labelLoading.Size = new Size(171, 423);
            labelLoading.TabIndex = 10;
            labelLoading.Text = resources.GetString("labelLoading.Text");
            labelLoading.Visible = false;
            // 
            // numericClientScale
            // 
            numericClientScale.DecimalPlaces = 1;
            numericClientScale.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericClientScale.Location = new Point(168, 293);
            numericClientScale.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numericClientScale.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numericClientScale.Name = "numericClientScale";
            numericClientScale.Size = new Size(45, 23);
            numericClientScale.TabIndex = 11;
            numericClientScale.TextAlign = HorizontalAlignment.Center;
            numericClientScale.Value = new decimal(new int[] { 10, 0, 0, 65536 });
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripSeparator6, toolStripButtonTop, toolStripSeparator8, toolStripButtonHot, toolStripSeparator5, toolStripButtonImport, toolStripProgressBar1, toolStripSeparator1, toolStripButtonLaunch, toolStripSeparator2, toolStripButtonRefresh, toolStripSeparator3, toolStripButtonScale, toolStripSeparator4, toolStripButtonHelp, toolStripSeparator9, toolStripButtonBorderless, toolStripSeparator7 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(634, 25);
            toolStrip1.TabIndex = 13;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 25);
            // 
            // toolStripButtonTop
            // 
            toolStripButtonTop.Checked = true;
            toolStripButtonTop.CheckOnClick = true;
            toolStripButtonTop.CheckState = CheckState.Checked;
            toolStripButtonTop.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonTop.Image = (Image)resources.GetObject("toolStripButtonTop.Image");
            toolStripButtonTop.ImageTransparentColor = Color.Magenta;
            toolStripButtonTop.Margin = new Padding(0, 1, 0, 1);
            toolStripButtonTop.Name = "toolStripButtonTop";
            toolStripButtonTop.Size = new Size(23, 23);
            toolStripButtonTop.Text = "Keep Window On Top";
            toolStripButtonTop.ToolTipText = "Keep this window on top.";
            toolStripButtonTop.Click += toolStripButtonTop_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(6, 25);
            // 
            // toolStripButtonHot
            // 
            toolStripButtonHot.Checked = true;
            toolStripButtonHot.CheckOnClick = true;
            toolStripButtonHot.CheckState = CheckState.Checked;
            toolStripButtonHot.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonHot.Image = (Image)resources.GetObject("toolStripButtonHot.Image");
            toolStripButtonHot.ImageTransparentColor = Color.Magenta;
            toolStripButtonHot.Margin = new Padding(0, 1, 0, 1);
            toolStripButtonHot.Name = "toolStripButtonHot";
            toolStripButtonHot.Size = new Size(23, 23);
            toolStripButtonHot.Text = "Hotkey";
            toolStripButtonHot.ToolTipText = "Enable or disable Hotkeys.";
            toolStripButtonHot.Click += toolStripButtonHot_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // toolStripButtonImport
            // 
            toolStripButtonImport.CheckOnClick = true;
            toolStripButtonImport.Image = (Image)resources.GetObject("toolStripButtonImport.Image");
            toolStripButtonImport.ImageTransparentColor = Color.Magenta;
            toolStripButtonImport.Margin = new Padding(0, 1, 0, 1);
            toolStripButtonImport.Name = "toolStripButtonImport";
            toolStripButtonImport.Size = new Size(63, 23);
            toolStripButtonImport.Text = "Import";
            toolStripButtonImport.ToolTipText = "Imports Jagex Launcher character launch actions to this utility.";
            toolStripButtonImport.Click += toolStripButtonImport_Click;
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Margin = new Padding(1);
            toolStripProgressBar1.MarqueeAnimationSpeed = 25;
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 23);
            toolStripProgressBar1.Step = 1;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripButtonLaunch
            // 
            toolStripButtonLaunch.Image = (Image)resources.GetObject("toolStripButtonLaunch.Image");
            toolStripButtonLaunch.ImageTransparentColor = Color.Magenta;
            toolStripButtonLaunch.Margin = new Padding(0, 1, 0, 1);
            toolStripButtonLaunch.Name = "toolStripButtonLaunch";
            toolStripButtonLaunch.Size = new Size(102, 23);
            toolStripButtonLaunch.Text = "Launch Group";
            toolStripButtonLaunch.ToolTipText = "Launches selected client groups based on configuration of accounts.csv file.";
            toolStripButtonLaunch.Click += toolStripButtonLaunch_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // toolStripButtonRefresh
            // 
            toolStripButtonRefresh.Image = (Image)resources.GetObject("toolStripButtonRefresh.Image");
            toolStripButtonRefresh.ImageTransparentColor = Color.Magenta;
            toolStripButtonRefresh.Margin = new Padding(0, 1, 0, 1);
            toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            toolStripButtonRefresh.Size = new Size(66, 23);
            toolStripButtonRefresh.Text = "Refresh";
            toolStripButtonRefresh.ToolTipText = "Refreshes below client preview pane.";
            toolStripButtonRefresh.Click += toolStripButtonRefresh_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // toolStripButtonScale
            // 
            toolStripButtonScale.CheckOnClick = true;
            toolStripButtonScale.Image = (Image)resources.GetObject("toolStripButtonScale.Image");
            toolStripButtonScale.ImageTransparentColor = Color.Magenta;
            toolStripButtonScale.Margin = new Padding(0, 1, 0, 1);
            toolStripButtonScale.Name = "toolStripButtonScale";
            toolStripButtonScale.Size = new Size(54, 23);
            toolStripButtonScale.Text = "Scale";
            toolStripButtonScale.ToolTipText = "Overrides scale settings for RuneLite.";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // toolStripButtonHelp
            // 
            toolStripButtonHelp.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonHelp.Image = (Image)resources.GetObject("toolStripButtonHelp.Image");
            toolStripButtonHelp.ImageTransparentColor = Color.Magenta;
            toolStripButtonHelp.Margin = new Padding(0, 1, 0, 1);
            toolStripButtonHelp.Name = "toolStripButtonHelp";
            toolStripButtonHelp.Size = new Size(23, 23);
            toolStripButtonHelp.Text = "Help";
            toolStripButtonHelp.ToolTipText = "Displays credits and assistance info.";
            toolStripButtonHelp.Click += toolStripButtonHelp_Click;
            //
            // toolStripSeparator9
            //
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(6, 25);
            //
            // toolStripButtonBorderless
            //
            toolStripButtonBorderless.CheckOnClick = true;
            toolStripButtonBorderless.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonBorderless.ImageTransparentColor = Color.Magenta;
            toolStripButtonBorderless.Margin = new Padding(0, 1, 0, 1);
            toolStripButtonBorderless.Name = "toolStripButtonBorderless";
            toolStripButtonBorderless.Size = new Size(72, 23);
            toolStripButtonBorderless.Text = "Borderless";
            toolStripButtonBorderless.ToolTipText = "Toggle borderless mode for clients.";
            toolStripButtonBorderless.Click += toolStripButtonBorderless_Click;
            //
            // toolStripSeparator7
            //
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(6, 25);
            // 
            // columnHeaderId
            // 
            columnHeaderId.Text = "Id";
            columnHeaderId.Width = 0; // Hidden column for internal use
            // 
            // MainForm
            // 
            ClientSize = new Size(634, 461);
            Controls.Add(numericClientScale);
            Controls.Add(groupSelection);
            Controls.Add(toolStrip1);
            Controls.Add(labelLoading);
            Controls.Add(listViewAccounts);
            Controls.Add(flowPanelProcesses);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(650, 500);
            Name = "MainForm";
            Text = "RuneFleet";
            TopMost = true;
            FormClosing += MainForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)numericClientScale).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private ComboBox groupSelection;
        private Label labelLoading;
        private NumericUpDown numericClientScale;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonRefresh;
        private ToolStripButton toolStripButtonLaunch;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonImport;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonScale;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolStripButtonTop;
        private ToolStripButton toolStripButtonHelp;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripButton toolStripButtonBorderless;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton toolStripButtonHot;
        private ToolStripSeparator toolStripSeparator8;
        private ColumnHeader columnHeaderId;
    }
}
