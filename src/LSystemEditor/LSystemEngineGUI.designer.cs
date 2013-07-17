namespace LSystemEditor {
    partial class LSystemEngineGUI {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LSystemEngineGUI));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.ProgressBar2 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolPanel = new System.Windows.Forms.Panel();
            this.LoadedSystemsList = new System.Windows.Forms.ComboBox();
            this.FilePopupMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.replaceBox = new System.Windows.Forms.TextBox();
            this.wholeWordCheck = new System.Windows.Forms.CheckBox();
            this.replaceBttn = new System.Windows.Forms.Button();
            this.matchCaseCheck = new System.Windows.Forms.CheckBox();
            this.findBttn = new System.Windows.Forms.Button();
            this.findBox = new System.Windows.Forms.TextBox();
            this.replaceAllBttn = new System.Windows.Forms.Button();
            this.HelpMenuContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLineCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.editorProgressContainer = new System.Windows.Forms.SplitContainer();
            this.textPanelContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.definitionBox = new System.Windows.Forms.RichTextBox();
            this.progressDisplayPanel = new LSystemEditor.LSystemProgressContainer();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.findReplaceCB = new System.Windows.Forms.CheckBox();
            this.helpBttn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.fileBttn = new System.Windows.Forms.Button();
            this.parseBttn = new System.Windows.Forms.Button();
            this.executeBttn = new System.Windows.Forms.Button();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.plainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solarizedLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solarizedDarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolPanel.SuspendLayout();
            this.FilePopupMenuStrip.SuspendLayout();
            this.HelpMenuContextStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorProgressContainer)).BeginInit();
            this.editorProgressContainer.Panel1.SuspendLayout();
            this.editorProgressContainer.Panel2.SuspendLayout();
            this.editorProgressContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textPanelContainer)).BeginInit();
            this.textPanelContainer.Panel1.SuspendLayout();
            this.textPanelContainer.Panel2.SuspendLayout();
            this.textPanelContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(100, 20);
            // 
            // ProgressBar2
            // 
            this.ProgressBar2.Name = "ProgressBar2";
            this.ProgressBar2.Size = new System.Drawing.Size(100, 20);
            // 
            // toolPanel
            // 
            this.toolPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toolPanel.BackColor = System.Drawing.SystemColors.Control;
            this.toolPanel.Controls.Add(this.pictureBox2);
            this.toolPanel.Controls.Add(this.findReplaceCB);
            this.toolPanel.Controls.Add(this.helpBttn);
            this.toolPanel.Controls.Add(this.pictureBox1);
            this.toolPanel.Controls.Add(this.fileBttn);
            this.toolPanel.Controls.Add(this.parseBttn);
            this.toolPanel.Controls.Add(this.executeBttn);
            this.toolPanel.Controls.Add(this.LoadedSystemsList);
            this.toolPanel.Location = new System.Drawing.Point(0, 0);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(634, 33);
            this.toolPanel.TabIndex = 9;
            // 
            // LoadedSystemsList
            // 
            this.LoadedSystemsList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LoadedSystemsList.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadedSystemsList.FormattingEnabled = true;
            this.LoadedSystemsList.Location = new System.Drawing.Point(139, 4);
            this.LoadedSystemsList.MaxDropDownItems = 12;
            this.LoadedSystemsList.Name = "LoadedSystemsList";
            this.LoadedSystemsList.Size = new System.Drawing.Size(208, 23);
            this.LoadedSystemsList.TabIndex = 0;
            this.toolTip1.SetToolTip(this.LoadedSystemsList, "Loaded LSystems - Select the LSystem to Execute");
            // 
            // FilePopupMenuStrip
            // 
            this.FilePopupMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripSeparator3,
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem1,
            this.saveAsToolStripMenuItem1,
            this.toolStripSeparator2,
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.FilePopupMenuStrip.Name = "contextMenuStrip1";
            this.FilePopupMenuStrip.Size = new System.Drawing.Size(149, 154);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(145, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(145, 6);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // replaceBox
            // 
            this.replaceBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(163)))), ((int)(((byte)(170)))));
            this.replaceBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.replaceBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replaceBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replaceBox.Location = new System.Drawing.Point(163, 3);
            this.replaceBox.Name = "replaceBox";
            this.replaceBox.Size = new System.Drawing.Size(74, 21);
            this.replaceBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.replaceBox, "Text used in Replace");
            // 
            // wholeWordCheck
            // 
            this.wholeWordCheck.AutoSize = true;
            this.wholeWordCheck.BackColor = System.Drawing.SystemColors.Control;
            this.wholeWordCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.wholeWordCheck.Location = new System.Drawing.Point(83, 30);
            this.wholeWordCheck.Name = "wholeWordCheck";
            this.wholeWordCheck.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.wholeWordCheck.Size = new System.Drawing.Size(74, 17);
            this.wholeWordCheck.TabIndex = 5;
            this.wholeWordCheck.Text = "Whole Word";
            this.toolTip1.SetToolTip(this.wholeWordCheck, "Match the whole word when searching");
            this.wholeWordCheck.UseVisualStyleBackColor = false;
            // 
            // replaceBttn
            // 
            this.replaceBttn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replaceBttn.Location = new System.Drawing.Point(243, 3);
            this.replaceBttn.Name = "replaceBttn";
            this.replaceBttn.Size = new System.Drawing.Size(75, 21);
            this.replaceBttn.TabIndex = 3;
            this.replaceBttn.Text = "Replace";
            this.toolTip1.SetToolTip(this.replaceBttn, "Replaces the selected text and finds the next instance");
            this.replaceBttn.UseVisualStyleBackColor = true;
            this.replaceBttn.Click += new System.EventHandler(this.replaceBttn_Click);
            // 
            // matchCaseCheck
            // 
            this.matchCaseCheck.AutoSize = true;
            this.matchCaseCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.matchCaseCheck.Location = new System.Drawing.Point(3, 30);
            this.matchCaseCheck.Name = "matchCaseCheck";
            this.matchCaseCheck.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.matchCaseCheck.Size = new System.Drawing.Size(74, 17);
            this.matchCaseCheck.TabIndex = 4;
            this.matchCaseCheck.Text = "Match Case";
            this.toolTip1.SetToolTip(this.matchCaseCheck, "Match the case of the text when searching");
            this.matchCaseCheck.UseVisualStyleBackColor = true;
            // 
            // findBttn
            // 
            this.findBttn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.findBttn.Location = new System.Drawing.Point(83, 3);
            this.findBttn.Name = "findBttn";
            this.findBttn.Size = new System.Drawing.Size(74, 21);
            this.findBttn.TabIndex = 0;
            this.findBttn.Text = "Find";
            this.toolTip1.SetToolTip(this.findBttn, "Finds the specified text starting from the cursor position");
            this.findBttn.UseVisualStyleBackColor = true;
            this.findBttn.Click += new System.EventHandler(this.findBttn_Click);
            // 
            // findBox
            // 
            this.findBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(163)))), ((int)(((byte)(170)))));
            this.findBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.findBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.findBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.findBox.Location = new System.Drawing.Point(3, 3);
            this.findBox.Name = "findBox";
            this.findBox.Size = new System.Drawing.Size(74, 21);
            this.findBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.findBox, "Text to Find");
            this.findBox.TextChanged += new System.EventHandler(this.findBox_TextChanged);
            // 
            // replaceAllBttn
            // 
            this.replaceAllBttn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replaceAllBttn.Location = new System.Drawing.Point(243, 30);
            this.replaceAllBttn.Name = "replaceAllBttn";
            this.replaceAllBttn.Size = new System.Drawing.Size(75, 21);
            this.replaceAllBttn.TabIndex = 6;
            this.replaceAllBttn.Text = "Replace All";
            this.toolTip1.SetToolTip(this.replaceAllBttn, "Replaces all found text");
            this.replaceAllBttn.UseVisualStyleBackColor = true;
            this.replaceAllBttn.Click += new System.EventHandler(this.replaceAllBttn_Click);
            // 
            // HelpMenuContextStrip
            // 
            this.HelpMenuContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.toolStripMenuItem3,
            this.aboutToolStripMenuItem});
            this.HelpMenuContextStrip.Name = "HelpMenuContextStrip";
            this.HelpMenuContextStrip.Size = new System.Drawing.Size(108, 54);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(104, 6);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLineCount,
            this.statusMessage});
            this.statusStrip.Location = new System.Drawing.Point(0, 468);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(634, 24);
            this.statusStrip.TabIndex = 11;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLineCount
            // 
            this.statusLineCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusLineCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusLineCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 2);
            this.statusLineCount.Name = "statusLineCount";
            this.statusLineCount.Size = new System.Drawing.Size(80, 19);
            this.statusLineCount.Text = "<line count>";
            // 
            // statusMessage
            // 
            this.statusMessage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusMessage.Margin = new System.Windows.Forms.Padding(4, 3, 0, 2);
            this.statusMessage.Name = "statusMessage";
            this.statusMessage.Size = new System.Drawing.Size(69, 19);
            this.statusMessage.Text = "<message>";
            // 
            // editorProgressContainer
            // 
            this.editorProgressContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.editorProgressContainer.BackColor = System.Drawing.SystemColors.Control;
            this.editorProgressContainer.Location = new System.Drawing.Point(4, 35);
            this.editorProgressContainer.Name = "editorProgressContainer";
            // 
            // editorProgressContainer.Panel1
            // 
            this.editorProgressContainer.Panel1.Controls.Add(this.textPanelContainer);
            // 
            // editorProgressContainer.Panel2
            // 
            this.editorProgressContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.editorProgressContainer.Panel2.Controls.Add(this.progressDisplayPanel);
            this.editorProgressContainer.Size = new System.Drawing.Size(624, 430);
            this.editorProgressContainer.SplitterDistance = 321;
            this.editorProgressContainer.SplitterWidth = 6;
            this.editorProgressContainer.TabIndex = 13;
            // 
            // textPanelContainer
            // 
            this.textPanelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textPanelContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.textPanelContainer.IsSplitterFixed = true;
            this.textPanelContainer.Location = new System.Drawing.Point(0, 0);
            this.textPanelContainer.Name = "textPanelContainer";
            this.textPanelContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // textPanelContainer.Panel1
            // 
            this.textPanelContainer.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // textPanelContainer.Panel2
            // 
            this.textPanelContainer.Panel2.Controls.Add(this.definitionBox);
            this.textPanelContainer.Size = new System.Drawing.Size(321, 430);
            this.textPanelContainer.SplitterDistance = 54;
            this.textPanelContainer.SplitterWidth = 1;
            this.textPanelContainer.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.replaceBox, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.replaceBttn, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.findBttn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.findBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.matchCaseCheck, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.wholeWordCheck, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.replaceAllBttn, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(321, 54);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // definitionBox
            // 
            this.definitionBox.AcceptsTab = true;
            this.definitionBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(163)))), ((int)(((byte)(170)))));
            this.definitionBox.DetectUrls = false;
            this.definitionBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.definitionBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.definitionBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.definitionBox.Location = new System.Drawing.Point(0, 0);
            this.definitionBox.Name = "definitionBox";
            this.definitionBox.Size = new System.Drawing.Size(321, 375);
            this.definitionBox.TabIndex = 0;
            this.definitionBox.TabStop = false;
            this.definitionBox.Text = resources.GetString("definitionBox.Text");
            this.definitionBox.WordWrap = false;
            this.definitionBox.Click += new System.EventHandler(this.definitionBox_Click);
            this.definitionBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.definitionBox_MouseClick);
            this.definitionBox.TextChanged += new System.EventHandler(this.EditorBox_TextChanged);
            this.definitionBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.definitionBox_KeyPress);
            this.definitionBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.definitionBox_KeyUp);
            // 
            // progressDisplayPanel
            // 
            this.progressDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressDisplayPanel.Location = new System.Drawing.Point(0, 0);
            this.progressDisplayPanel.Name = "progressDisplayPanel";
            this.progressDisplayPanel.Size = new System.Drawing.Size(297, 430);
            this.progressDisplayPanel.TabIndex = 0;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::LSystemEditor.Properties.Resources.sep02;
            this.pictureBox2.Location = new System.Drawing.Point(355, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(3, 24);
            this.pictureBox2.TabIndex = 15;
            this.pictureBox2.TabStop = false;
            // 
            // findReplaceCB
            // 
            this.findReplaceCB.Appearance = System.Windows.Forms.Appearance.Button;
            this.findReplaceCB.Image = global::LSystemEditor.Properties.Resources.find;
            this.findReplaceCB.Location = new System.Drawing.Point(34, 4);
            this.findReplaceCB.Name = "findReplaceCB";
            this.findReplaceCB.Size = new System.Drawing.Size(24, 24);
            this.findReplaceCB.TabIndex = 14;
            this.toolTip1.SetToolTip(this.findReplaceCB, "Show/Hide: Find & Replace Text");
            this.findReplaceCB.UseVisualStyleBackColor = true;
            this.findReplaceCB.CheckedChanged += new System.EventHandler(this.findReplaceCB_CheckedChanged);
            // 
            // helpBttn
            // 
            this.helpBttn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpBttn.Image = global::LSystemEditor.Properties.Resources.help2;
            this.helpBttn.Location = new System.Drawing.Point(604, 4);
            this.helpBttn.Name = "helpBttn";
            this.helpBttn.Size = new System.Drawing.Size(24, 24);
            this.helpBttn.TabIndex = 10;
            this.toolTip1.SetToolTip(this.helpBttn, "Help Menu - Click to display Help menu");
            this.helpBttn.UseVisualStyleBackColor = true;
            this.helpBttn.Click += new System.EventHandler(this.helpBttn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LSystemEditor.Properties.Resources.sep02;
            this.pictureBox1.Location = new System.Drawing.Point(66, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(3, 24);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // fileBttn
            // 
            this.fileBttn.Image = global::LSystemEditor.Properties.Resources.file;
            this.fileBttn.Location = new System.Drawing.Point(4, 4);
            this.fileBttn.Name = "fileBttn";
            this.fileBttn.Size = new System.Drawing.Size(24, 24);
            this.fileBttn.TabIndex = 7;
            this.toolTip1.SetToolTip(this.fileBttn, "File Menu - Click to display File menu");
            this.fileBttn.UseVisualStyleBackColor = true;
            this.fileBttn.Click += new System.EventHandler(this.fileBttn_Click);
            // 
            // parseBttn
            // 
            this.parseBttn.Image = global::LSystemEditor.Properties.Resources.parse;
            this.parseBttn.Location = new System.Drawing.Point(76, 4);
            this.parseBttn.Name = "parseBttn";
            this.parseBttn.Size = new System.Drawing.Size(24, 24);
            this.parseBttn.TabIndex = 6;
            this.toolTip1.SetToolTip(this.parseBttn, "Load LSystems (Parse Editor contents)");
            this.parseBttn.UseVisualStyleBackColor = true;
            this.parseBttn.Click += new System.EventHandler(this.parseBttn_Click);
            // 
            // executeBttn
            // 
            this.executeBttn.Image = global::LSystemEditor.Properties.Resources.execute1;
            this.executeBttn.Location = new System.Drawing.Point(106, 4);
            this.executeBttn.Name = "executeBttn";
            this.executeBttn.Size = new System.Drawing.Size(24, 24);
            this.executeBttn.TabIndex = 5;
            this.toolTip1.SetToolTip(this.executeBttn, "Execute LSystem (Executes the selected LSystem)");
            this.executeBttn.UseVisualStyleBackColor = true;
            this.executeBttn.Click += new System.EventHandler(this.executeBttn_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::LSystemEditor.Properties.Resources.new_icon;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem2.Text = "New";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Image = global::LSystemEditor.Properties.Resources.open_icon;
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Image = global::LSystemEditor.Properties.Resources.save_icon;
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem1
            // 
            this.saveAsToolStripMenuItem1.Image = global::LSystemEditor.Properties.Resources.saveas_icon;
            this.saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            this.saveAsToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.saveAsToolStripMenuItem1.Text = "Save As";
            this.saveAsToolStripMenuItem1.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plainToolStripMenuItem,
            this.solarizedLightToolStripMenuItem,
            this.solarizedDarkToolStripMenuItem});
            this.toolStripMenuItem1.Image = global::LSystemEditor.Properties.Resources.colors_icon;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem1.Text = "Color Scheme";
            // 
            // plainToolStripMenuItem
            // 
            this.plainToolStripMenuItem.Image = global::LSystemEditor.Properties.Resources.color_sys;
            this.plainToolStripMenuItem.Name = "plainToolStripMenuItem";
            this.plainToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.plainToolStripMenuItem.Text = "System";
            this.plainToolStripMenuItem.Click += new System.EventHandler(this.SystemToolStripMenuItem_Click);
            // 
            // solarizedLightToolStripMenuItem
            // 
            this.solarizedLightToolStripMenuItem.Image = global::LSystemEditor.Properties.Resources.color_light;
            this.solarizedLightToolStripMenuItem.Name = "solarizedLightToolStripMenuItem";
            this.solarizedLightToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.solarizedLightToolStripMenuItem.Text = "Light";
            this.solarizedLightToolStripMenuItem.Click += new System.EventHandler(this.LightToolStripMenuItem_Click);
            // 
            // solarizedDarkToolStripMenuItem
            // 
            this.solarizedDarkToolStripMenuItem.Image = global::LSystemEditor.Properties.Resources.color_dark;
            this.solarizedDarkToolStripMenuItem.Name = "solarizedDarkToolStripMenuItem";
            this.solarizedDarkToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.solarizedDarkToolStripMenuItem.Text = "Dark";
            this.solarizedDarkToolStripMenuItem.Click += new System.EventHandler(this.DarkToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::LSystemEditor.Properties.Resources.close_icon3;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Image = global::LSystemEditor.Properties.Resources.help_icon;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::LSystemEditor.Properties.Resources.about_icon;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // LSystemEngineGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(634, 492);
            this.Controls.Add(this.editorProgressContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(410, 200);
            this.Name = "LSystemEngineGUI";
            this.Text = "LSystem Engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LSysEngineGUI_FormClosing);
            this.Load += new System.EventHandler(this.LSysEngineGUI_Load);
            this.toolPanel.ResumeLayout(false);
            this.FilePopupMenuStrip.ResumeLayout(false);
            this.HelpMenuContextStrip.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.editorProgressContainer.Panel1.ResumeLayout(false);
            this.editorProgressContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.editorProgressContainer)).EndInit();
            this.editorProgressContainer.ResumeLayout(false);
            this.textPanelContainer.Panel1.ResumeLayout(false);
            this.textPanelContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textPanelContainer)).EndInit();
            this.textPanelContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar1;
        //private System.Windows.Forms.ToolStripStatusLabel statusLabel1;
        //private System.Windows.Forms.ToolStripStatusLabel statusLabel2;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar2;
        private System.Windows.Forms.Panel toolPanel;
        private System.Windows.Forms.ComboBox LoadedSystemsList;
        private System.Windows.Forms.ContextMenuStrip FilePopupMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem1;
        private System.Windows.Forms.Button parseBttn;
        private System.Windows.Forms.Button executeBttn;
        private System.Windows.Forms.Button fileBttn;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button helpBttn;
        private System.Windows.Forms.ContextMenuStrip HelpMenuContextStrip;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLineCount;
        private System.Windows.Forms.ToolStripStatusLabel statusMessage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer editorProgressContainer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox findReplaceCB;
        private System.Windows.Forms.SplitContainer textPanelContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox replaceBox;
        private System.Windows.Forms.CheckBox wholeWordCheck;
        private System.Windows.Forms.Button replaceBttn;
        private System.Windows.Forms.CheckBox matchCaseCheck;
        private System.Windows.Forms.Button findBttn;
        private System.Windows.Forms.TextBox findBox;
        private System.Windows.Forms.RichTextBox definitionBox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button replaceAllBttn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem plainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solarizedLightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solarizedDarkToolStripMenuItem;
        private LSystemProgressContainer progressDisplayPanel;
    }
}