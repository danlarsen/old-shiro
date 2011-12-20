namespace ShiroChan
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Tabs = new System.Windows.Forms.TabControl();
            this.consolePage = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFrameMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toCurrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runConsoleMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.runSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runSelectionIsolatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runInIsolationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllSymbolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.symbolsWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.buildexeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildWithoutParseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.framesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nameFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ErrorToOutputMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.allowOutputSelectMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wordWrapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.turnOffIntellisenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter = new System.Windows.Forms.SplitContainer();
            this.RightText = new ScintillaNet.Scintilla();
            this.RightTitle = new System.Windows.Forms.Label();
            this.LeftText = new System.Windows.Forms.RichTextBox();
            this.argumentLabel = new System.Windows.Forms.Label();
            this.toNewEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Tabs.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RightText)).BeginInit();
            this.SuspendLayout();
            // 
            // Tabs
            // 
            this.Tabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Tabs.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.Tabs.Controls.Add(this.consolePage);
            this.Tabs.Location = new System.Drawing.Point(0, 27);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(879, 25);
            this.Tabs.TabIndex = 0;
            // 
            // consolePage
            // 
            this.consolePage.Location = new System.Drawing.Point(4, 25);
            this.consolePage.Name = "consolePage";
            this.consolePage.Padding = new System.Windows.Forms.Padding(3);
            this.consolePage.Size = new System.Drawing.Size(871, 0);
            this.consolePage.TabIndex = 0;
            this.consolePage.Text = "Console";
            this.consolePage.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.executeToolStripMenuItem,
            this.framesToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(879, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFrameMenu,
            this.saveEditorToolStripMenuItem,
            this.openEditorToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newFrameMenu
            // 
            this.newFrameMenu.Name = "newFrameMenu";
            this.newFrameMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newFrameMenu.Size = new System.Drawing.Size(177, 22);
            this.newFrameMenu.Text = "&New Frame";
            this.newFrameMenu.Click += new System.EventHandler(this.newFrameMenu_Click);
            // 
            // saveEditorToolStripMenuItem
            // 
            this.saveEditorToolStripMenuItem.Name = "saveEditorToolStripMenuItem";
            this.saveEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveEditorToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.saveEditorToolStripMenuItem.Text = "&Save Editor";
            this.saveEditorToolStripMenuItem.Click += new System.EventHandler(this.saveEditorToolStripMenuItem_Click);
            // 
            // openEditorToolStripMenuItem
            // 
            this.openEditorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toCurrToolStripMenuItem,
            this.toNewEditorToolStripMenuItem});
            this.openEditorToolStripMenuItem.Name = "openEditorToolStripMenuItem";
            this.openEditorToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.openEditorToolStripMenuItem.Text = "&Open";
            // 
            // toCurrToolStripMenuItem
            // 
            this.toCurrToolStripMenuItem.Name = "toCurrToolStripMenuItem";
            this.toCurrToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.toCurrToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.toCurrToolStripMenuItem.Text = "To &Current Editor";
            this.toCurrToolStripMenuItem.Click += new System.EventHandler(this.toCurrToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(174, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem2,
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.replaceToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.redoToolStripMenuItem.Text = "Re&do";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.cutToolStripMenuItem.Text = "C&ut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 6);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.findToolStripMenuItem.Text = "&Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.findNextToolStripMenuItem.Text = "Find &Next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.replaceToolStripMenuItem.Text = "&Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // executeToolStripMenuItem
            // 
            this.executeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runConsoleMenu,
            this.runSelectionToolStripMenuItem,
            this.runSelectionIsolatedToolStripMenuItem,
            this.runInIsolationToolStripMenuItem,
            this.toolStripMenuItem1,
            this.clearAllSymbolsToolStripMenuItem,
            this.symbolsWindowToolStripMenuItem,
            this.toolStripMenuItem4,
            this.buildexeToolStripMenuItem,
            this.buildWithoutParseToolStripMenuItem});
            this.executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            this.executeToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.executeToolStripMenuItem.Text = "E&xecute";
            // 
            // runConsoleMenu
            // 
            this.runConsoleMenu.Name = "runConsoleMenu";
            this.runConsoleMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.runConsoleMenu.Size = new System.Drawing.Size(247, 22);
            this.runConsoleMenu.Text = "Run &Current";
            this.runConsoleMenu.Click += new System.EventHandler(this.runConsoleMenu_Click);
            // 
            // runSelectionToolStripMenuItem
            // 
            this.runSelectionToolStripMenuItem.Name = "runSelectionToolStripMenuItem";
            this.runSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F6)));
            this.runSelectionToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.runSelectionToolStripMenuItem.Text = "&Run Selection";
            this.runSelectionToolStripMenuItem.Click += new System.EventHandler(this.runSelectionToolStripMenuItem_Click);
            // 
            // runSelectionIsolatedToolStripMenuItem
            // 
            this.runSelectionIsolatedToolStripMenuItem.Name = "runSelectionIsolatedToolStripMenuItem";
            this.runSelectionIsolatedToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.runSelectionIsolatedToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.runSelectionIsolatedToolStripMenuItem.Text = "Run Selection Isolate&d";
            this.runSelectionIsolatedToolStripMenuItem.Click += new System.EventHandler(this.runSelectionIsolatedToolStripMenuItem_Click);
            // 
            // runInIsolationToolStripMenuItem
            // 
            this.runInIsolationToolStripMenuItem.Name = "runInIsolationToolStripMenuItem";
            this.runInIsolationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runInIsolationToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.runInIsolationToolStripMenuItem.Text = "Run in &Isolation";
            this.runInIsolationToolStripMenuItem.Click += new System.EventHandler(this.runInIsolationToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(244, 6);
            // 
            // clearAllSymbolsToolStripMenuItem
            // 
            this.clearAllSymbolsToolStripMenuItem.Name = "clearAllSymbolsToolStripMenuItem";
            this.clearAllSymbolsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.clearAllSymbolsToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.clearAllSymbolsToolStripMenuItem.Text = "&Clear All Symbols";
            this.clearAllSymbolsToolStripMenuItem.Click += new System.EventHandler(this.clearAllSymbolsToolStripMenuItem_Click);
            // 
            // symbolsWindowToolStripMenuItem
            // 
            this.symbolsWindowToolStripMenuItem.Name = "symbolsWindowToolStripMenuItem";
            this.symbolsWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.symbolsWindowToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.symbolsWindowToolStripMenuItem.Text = "&Symbols Window...";
            this.symbolsWindowToolStripMenuItem.Click += new System.EventHandler(this.symbolsWindowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(244, 6);
            // 
            // buildexeToolStripMenuItem
            // 
            this.buildexeToolStripMenuItem.Name = "buildexeToolStripMenuItem";
            this.buildexeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.buildexeToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.buildexeToolStripMenuItem.Text = "&Build .exe";
            this.buildexeToolStripMenuItem.Click += new System.EventHandler(this.buildexeToolStripMenuItem_Click);
            // 
            // buildWithoutParseToolStripMenuItem
            // 
            this.buildWithoutParseToolStripMenuItem.Name = "buildWithoutParseToolStripMenuItem";
            this.buildWithoutParseToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.buildWithoutParseToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.buildWithoutParseToolStripMenuItem.Text = "Build &Without Parse";
            this.buildWithoutParseToolStripMenuItem.Click += new System.EventHandler(this.buildWithoutParseToolStripMenuItem_Click);
            // 
            // framesToolStripMenuItem
            // 
            this.framesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nextToolStripMenuItem,
            this.previousToolStripMenuItem,
            this.gotoConsoleToolStripMenuItem,
            this.nameFrameToolStripMenuItem});
            this.framesToolStripMenuItem.Name = "framesToolStripMenuItem";
            this.framesToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.framesToolStripMenuItem.Text = "F&rames";
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.nextToolStripMenuItem.Text = "&Next";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.nextToolStripMenuItem_Click);
            // 
            // previousToolStripMenuItem
            // 
            this.previousToolStripMenuItem.Name = "previousToolStripMenuItem";
            this.previousToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.previousToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.previousToolStripMenuItem.Text = "&Previous";
            this.previousToolStripMenuItem.Click += new System.EventHandler(this.previousToolStripMenuItem_Click);
            // 
            // gotoConsoleToolStripMenuItem
            // 
            this.gotoConsoleToolStripMenuItem.Name = "gotoConsoleToolStripMenuItem";
            this.gotoConsoleToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.gotoConsoleToolStripMenuItem.Text = "Goto &Console";
            this.gotoConsoleToolStripMenuItem.Click += new System.EventHandler(this.gotoConsoleToolStripMenuItem_Click);
            // 
            // nameFrameToolStripMenuItem
            // 
            this.nameFrameToolStripMenuItem.Name = "nameFrameToolStripMenuItem";
            this.nameFrameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.nameFrameToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.nameFrameToolStripMenuItem.Text = "Na&me Frame";
            this.nameFrameToolStripMenuItem.Click += new System.EventHandler(this.nameFrameToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ErrorToOutputMenu,
            this.allowOutputSelectMenu,
            this.toggleConsoleToolStripMenuItem,
            this.wordWrapToolStripMenuItem,
            this.turnOffIntellisenseToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // ErrorToOutputMenu
            // 
            this.ErrorToOutputMenu.Checked = true;
            this.ErrorToOutputMenu.CheckOnClick = true;
            this.ErrorToOutputMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ErrorToOutputMenu.Name = "ErrorToOutputMenu";
            this.ErrorToOutputMenu.Size = new System.Drawing.Size(215, 22);
            this.ErrorToOutputMenu.Text = "&Error Display in Output";
            this.ErrorToOutputMenu.Click += new System.EventHandler(this.ErrorToOutputMenu_Click);
            // 
            // allowOutputSelectMenu
            // 
            this.allowOutputSelectMenu.CheckOnClick = true;
            this.allowOutputSelectMenu.Name = "allowOutputSelectMenu";
            this.allowOutputSelectMenu.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.allowOutputSelectMenu.Size = new System.Drawing.Size(215, 22);
            this.allowOutputSelectMenu.Text = "Allow Output &Selection";
            this.allowOutputSelectMenu.Click += new System.EventHandler(this.allowOutputSelectMenu_Click);
            // 
            // toggleConsoleToolStripMenuItem
            // 
            this.toggleConsoleToolStripMenuItem.Name = "toggleConsoleToolStripMenuItem";
            this.toggleConsoleToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.toggleConsoleToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.toggleConsoleToolStripMenuItem.Text = "&Toggle Console";
            this.toggleConsoleToolStripMenuItem.Click += new System.EventHandler(this.toggleConsoleToolStripMenuItem_Click);
            // 
            // wordWrapToolStripMenuItem
            // 
            this.wordWrapToolStripMenuItem.Name = "wordWrapToolStripMenuItem";
            this.wordWrapToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.wordWrapToolStripMenuItem.Text = "&Word Wrap";
            this.wordWrapToolStripMenuItem.Click += new System.EventHandler(this.wordWrapToolStripMenuItem_Click);
            // 
            // turnOffIntellisenseToolStripMenuItem
            // 
            this.turnOffIntellisenseToolStripMenuItem.CheckOnClick = true;
            this.turnOffIntellisenseToolStripMenuItem.Name = "turnOffIntellisenseToolStripMenuItem";
            this.turnOffIntellisenseToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.turnOffIntellisenseToolStripMenuItem.Text = "Turn off Intellisense";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(118, 22);
            this.helpToolStripMenuItem1.Text = "&Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // splitter
            // 
            this.splitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitter.Location = new System.Drawing.Point(0, 52);
            this.splitter.Name = "splitter";
            this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitter.Panel1
            // 
            this.splitter.Panel1.Controls.Add(this.RightText);
            // 
            // splitter.Panel2
            // 
            this.splitter.Panel2.Controls.Add(this.RightTitle);
            this.splitter.Panel2.Controls.Add(this.LeftText);
            this.splitter.Size = new System.Drawing.Size(879, 630);
            this.splitter.SplitterDistance = 450;
            this.splitter.TabIndex = 2;
            // 
            // RightText
            // 
            this.RightText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RightText.Location = new System.Drawing.Point(9, 3);
            this.RightText.Name = "RightText";
            this.RightText.Size = new System.Drawing.Size(866, 444);
            this.RightText.Styles.BraceBad.FontName = "Verdana";
            this.RightText.Styles.BraceLight.FontName = "Verdana";
            this.RightText.Styles.ControlChar.FontName = "Verdana";
            this.RightText.Styles.Default.FontName = "Verdana";
            this.RightText.Styles.IndentGuide.FontName = "Verdana";
            this.RightText.Styles.LastPredefined.FontName = "Verdana";
            this.RightText.Styles.LineNumber.FontName = "Verdana";
            this.RightText.Styles.Max.FontName = "Verdana";
            this.RightText.TabIndex = 3;
            this.RightText.AutoCompleteAccepted += new System.EventHandler<ScintillaNet.AutoCompleteAcceptedEventArgs>(this.RightText_AutoCompleteAccepted);
            this.RightText.CharAdded += new System.EventHandler<ScintillaNet.CharAddedEventArgs>(this.RightText_CharAdded);
            this.RightText.DoubleClick += new System.EventHandler(this.RightText_DoubleClick);
            this.RightText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RightText_KeyDown);
            // 
            // RightTitle
            // 
            this.RightTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RightTitle.BackColor = System.Drawing.SystemColors.ControlDark;
            this.RightTitle.Location = new System.Drawing.Point(3, 4);
            this.RightTitle.Name = "RightTitle";
            this.RightTitle.Size = new System.Drawing.Size(871, 14);
            this.RightTitle.TabIndex = 2;
            this.RightTitle.Text = "Output";
            this.RightTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LeftText
            // 
            this.LeftText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LeftText.Location = new System.Drawing.Point(4, 21);
            this.LeftText.Name = "LeftText";
            this.LeftText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.LeftText.Size = new System.Drawing.Size(875, 155);
            this.LeftText.TabIndex = 2;
            this.LeftText.Text = "";
            this.LeftText.WordWrap = false;
            // 
            // argumentLabel
            // 
            this.argumentLabel.AutoSize = true;
            this.argumentLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.argumentLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.argumentLabel.ForeColor = System.Drawing.Color.DimGray;
            this.argumentLabel.Location = new System.Drawing.Point(396, 9);
            this.argumentLabel.Name = "argumentLabel";
            this.argumentLabel.Size = new System.Drawing.Size(21, 15);
            this.argumentLabel.TabIndex = 3;
            this.argumentLabel.Text = "F()";
            this.argumentLabel.Visible = false;
            // 
            // toNewEditorToolStripMenuItem
            // 
            this.toNewEditorToolStripMenuItem.Name = "toNewEditorToolStripMenuItem";
            this.toNewEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.O)));
            this.toNewEditorToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.toNewEditorToolStripMenuItem.Text = "To &New Editor";
            this.toNewEditorToolStripMenuItem.Click += new System.EventHandler(this.toNewEditorToolStripMenuItem_Click_1);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 682);
            this.Controls.Add(this.argumentLabel);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.Tabs);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Shiro-chan 0.8";
            this.Tabs.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel2.ResumeLayout(false);
            this.splitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RightText)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFrameMenu;
        private System.Windows.Forms.ToolStripMenuItem executeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runConsoleMenu;
        private System.Windows.Forms.TabPage consolePage;
        private System.Windows.Forms.SplitContainer splitter;
        private System.Windows.Forms.Label RightTitle;
        private System.Windows.Forms.ToolStripMenuItem saveEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toCurrToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runInIsolationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem clearAllSymbolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem framesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gotoConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ErrorToOutputMenu;
        private System.Windows.Forms.RichTextBox LeftText;
        private System.Windows.Forms.ToolStripMenuItem allowOutputSelectMenu;
        private System.Windows.Forms.ToolStripMenuItem nameFrameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem symbolsWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runSelectionIsolatedToolStripMenuItem;
        private ScintillaNet.Scintilla RightText;
        private System.Windows.Forms.ToolStripMenuItem toggleConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wordWrapToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem buildexeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildWithoutParseToolStripMenuItem;
        private System.Windows.Forms.Label argumentLabel;
        private System.Windows.Forms.ToolStripMenuItem turnOffIntellisenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toNewEditorToolStripMenuItem;
    }
}

