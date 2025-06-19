namespace OSRSClientManager
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
            setWorlds = new CheckBox();
            groupSelection = new ComboBox();
            SuspendLayout();
            // 
            // listViewAccounts
            // 
            listViewAccounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listViewAccounts.Columns.AddRange(new ColumnHeader[] { columnHeaderDisplayName, columnHeaderPID });
            listViewAccounts.FullRowSelect = true;
            listViewAccounts.Location = new Point(12, 12);
            listViewAccounts.Name = "listViewAccounts";
            listViewAccounts.Size = new Size(174, 428);
            listViewAccounts.TabIndex = 0;
            listViewAccounts.UseCompatibleStateImageBehavior = false;
            listViewAccounts.View = View.Details;
            listViewAccounts.ItemActivate += listViewAccounts_ItemActivate;
            listViewAccounts.Click += listViewAccounts_ItemActivate;
            listViewAccounts.DoubleClick += buttonLaunchSelected_Click;
            // 
            // columnHeaderDisplayName
            // 
            columnHeaderDisplayName.Text = "Account";
            columnHeaderDisplayName.Width = 95;
            // 
            // columnHeaderPID
            // 
            columnHeaderPID.Text = "Process";
            columnHeaderPID.Width = 55;
            // 
            // buttonLaunchAll
            // 
            buttonLaunchAll.Location = new Point(371, 12);
            buttonLaunchAll.Name = "buttonLaunchAll";
            buttonLaunchAll.Size = new Size(130, 30);
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
            flowPanelProcesses.Location = new Point(192, 48);
            flowPanelProcesses.Name = "flowPanelProcesses";
            flowPanelProcesses.Size = new Size(406, 392);
            flowPanelProcesses.TabIndex = 3;
            // 
            // refreshPane
            // 
            refreshPane.Location = new Point(507, 12);
            refreshPane.Name = "refreshPane";
            refreshPane.Size = new Size(91, 30);
            refreshPane.TabIndex = 4;
            refreshPane.Text = "Load Previews";
            refreshPane.UseVisualStyleBackColor = true;
            refreshPane.Click += buttonLoadPreviews_Click;
            // 
            // setWorlds
            // 
            setWorlds.AutoSize = true;
            setWorlds.Location = new Point(192, 19);
            setWorlds.Name = "setWorlds";
            setWorlds.Size = new Size(99, 19);
            setWorlds.TabIndex = 5;
            setWorlds.Text = "Default World";
            setWorlds.UseVisualStyleBackColor = true;
            setWorlds.CheckedChanged += setWorlds_CheckedChanged;
            // 
            // groupSelection
            // 
            groupSelection.DropDownStyle = ComboBoxStyle.DropDownList;
            groupSelection.FormattingEnabled = true;
            groupSelection.Location = new Point(297, 17);
            groupSelection.Name = "groupSelection";
            groupSelection.Size = new Size(68, 23);
            groupSelection.TabIndex = 6;
            groupSelection.SelectedValueChanged += groupSelection_SelectedValueChanged;
            // 
            // MainForm
            // 
            ClientSize = new Size(608, 448);
            Controls.Add(groupSelection);
            Controls.Add(setWorlds);
            Controls.Add(refreshPane);
            Controls.Add(listViewAccounts);
            Controls.Add(buttonLaunchAll);
            Controls.Add(flowPanelProcesses);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(624, 487);
            Name = "MainForm";
            Text = "OSRS Client Manager";
            TopMost = true;
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }
        private Button refreshPane;
        private CheckBox setWorlds;
        private ComboBox groupSelection;
    }
}
