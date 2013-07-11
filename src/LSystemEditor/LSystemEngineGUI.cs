using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Configuration;

using System.IO;

using LSystemEditor;

using LSystemEngine;


namespace LSystemEditor {

    public enum ExecutionThreadingModel : uint {

        SingleThread,
        ExecuteOnNewThread,
        RhinoSafe

    }


    public enum LSEColorScheme : uint {

        System,
        Light,
        Dark

    }



    public partial class LSystemEngineGUI : Form, ILSystemListener {

        private const string FORM_TITLE = "LSystem Engine";
        private const string EXECUTE_EMPTY = "<no LSystems to execute>";
        private const string EXECUTE_READY = "Execute LSystem";
        private const string LSYSTEM_FILE_FILTER = "LSystem Files (*.lsf)|*.lsf|Text files (*.txt)|*.txt|All files (*.*)|*.*";


        //implementation notes
        private string _implementation_notes;

        //implemented to be able to provide a thread safe version for rhino
        private ExecutionThreadingModel _threading_model;

        //reference to the lsystem engine
        private LSystemEvaluationEngine _lSystemEngine;
        
        //interface for executing lSystem
       private IEditorExecutionHandle _execution_handle;


        private string _current_file;

        private  int _find_index;
        //private string systemToExecute = String.Empty;

        private bool _editor_dirty;

        private int _current_line;

        private string _form_title;
        //private List<string> executionStack;

        private bool _system_executing;

        private LSEColorScheme _color_scheme;

        private bool _suppress_event;

        //General Delegates
        private delegate void LSEventSimple();
        private delegate void LSEventNamed(string name);

        private delegate void LSEventEvaluation(string name, int iteration, int ofIterations, int letter, int ofLetters);
        private delegate void LSEventResolver(string name, int letter, int ofLetters);

        private delegate void LSEventException(string msg, Exception ex);

        
        private LSEventSimple d_ParseBegin;
        private LSEventNamed d_ParseLoad;
        private LSEventSimple d_ParseEnd;

        private LSEventNamed d_EvaluationBegin;
        private LSEventEvaluation d_EvaluationProgess;
        private LSEventNamed d_EvaluationEnd;

        private LSEventNamed d_ResolverBegin;
        private LSEventResolver d_ResolverProgess;
        private LSEventNamed d_ResolverEnd;

        private LSEventNamed d_ExecutionBegin;
        private LSEventNamed d_ExecutionEnd;

        private LSEventNamed d_SubSystemExecutionBegin;
        private LSEventNamed d_SubSystemExecutionEnd;

        private LSEventException d_ParseError;
        private LSEventException d_ExecutionError;


        private System.Diagnostics.Stopwatch _stop_watch;


        #region "Constructors, Init, Loading"


        /// <summary>
        /// Constructor with Modeler provided.  Engine will be intialized internally.
        /// </summary>
        /// <param name="modeler"></param>
        /// <param name="threading_model"></param>
        /// <param name="title"></param>
        public LSystemEngineGUI(ILSystemModeler modeler, ExecutionThreadingModel threading_model, string title) {

             
            //init LSystem Engine with modelr and this for listener
            LSystemEvaluationEngine sysEngine = new LSystemEvaluationEngine(modeler, this);

            IEditorExecutionHandle executionHandle = new InternalExecutionHandle(sysEngine);

            InitUI(sysEngine, executionHandle, threading_model, title);

        }
        



        /// <summary>
        /// Constructor with Engine and Execution handle provided.  
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="executionHandle"></param>
        /// <param name="threading_model"></param>
        /// <param name="title"></param>
        public LSystemEngineGUI(LSystemEvaluationEngine engine, IEditorExecutionHandle executionHandle, ExecutionThreadingModel threading_model, string title) {


            InitUI(engine, executionHandle, threading_model, title);


        }



        /// <summary>
        /// Constructor with Modeler and ... this doesn't make sense anymore...
        /// </summary>
        /// <param name="modeler"></param>
        /// <param name="executionHandle"></param>
        /// <param name="threading_model"></param>
        /// <param name="title"></param>
        public LSystemEngineGUI(ILSystemModeler modeler, IEditorExecutionHandle executionHandle,  ExecutionThreadingModel threading_model, string title) {


            //init LSystem Engine with modelr and this for listener
            LSystemEvaluationEngine sysEngine = new LSystemEvaluationEngine(modeler, this);

            InitUI(sysEngine, executionHandle, threading_model, title);

        }



 

        private void InitUI(LSystemEvaluationEngine engine, IEditorExecutionHandle executionHandle, ExecutionThreadingModel threading_model, string title) {

            InitializeComponent();

            //ui / misc stuff...
            _implementation_notes = "";
            _color_scheme = LSEColorScheme.Dark;


            _execution_handle = executionHandle;

            //grab multi-threading flag (for rhino mainly)....
            _threading_model = threading_model;

            //set the title
           _form_title = title.Trim();
           if (String.IsNullOrEmpty(_form_title)) {
               _form_title = FORM_TITLE;
           }

            //grab reference to engine
           _lSystemEngine = engine;


            //initialize empty file
            _current_file = String.Empty;
            _editor_dirty = false;

            _suppress_event = false;

            _current_line = 0;
            _find_index = 0;

            //hook engine into progress display
            progressDisplayPanel.setEngine(_lSystemEngine);

            //create delegates to call

            d_ParseBegin = new LSEventSimple(OnParseBegin);
            d_ParseLoad = new LSEventNamed(OnParseLoaded);
            d_ParseEnd = new LSEventSimple(OnParseEnd);


            d_EvaluationBegin = new LSEventNamed(OnEvaluationBegin);
            d_EvaluationProgess = new LSEventEvaluation(OnEvaluationProgress);
            d_EvaluationEnd = new LSEventNamed(OnEvaluationEnd);

            d_ResolverBegin = new LSEventNamed(OnResolverBegin);
            d_ResolverProgess = new LSEventResolver(OnResolverProgress);
            d_ResolverEnd = new LSEventNamed(OnResolverEnd);

            d_ExecutionBegin = new LSEventNamed(OnLSystemExecutionBegin);
            d_ExecutionEnd = new LSEventNamed(OnLSystemExecutionEnd);

            d_SubSystemExecutionBegin = new LSEventNamed(OnSubSystemExecutionBegin);
            d_SubSystemExecutionEnd = new LSEventNamed(OnSubSystemExecutionEnd);

            d_ExecutionError = new LSEventException(OnLSystemExecutionError);
            d_ParseError = new LSEventException(OnParserError);


            _stop_watch = new System.Diagnostics.Stopwatch();

        }//end constructor

 
        /// <summary>
        /// Load the GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LSysEngineGUI_Load(object sender, EventArgs e) {


            //setup controls
            setSysListNone();

            //ExecuteMenu.Enabled = false;

            _editor_dirty = false;

            //systemToExecute = String.Empty;
            setTitle();

            //collapse progess container
            editorProgressContainer.Panel2Collapsed = true;

            //collapse find/replace
            textPanelContainer.Panel1Collapsed = true;


            LoadedSystemsList.Items.AddRange(new string[] { EXECUTE_EMPTY });
            LoadedSystemsList.SelectedIndex = 0;
            //LoadedSystemsList.Enabled = false;

            UpdateLinePosition();
            

            statusMessage.Text = "Started LSystem Editor";
            _system_executing = false;


            //use default dark solarized
            //applyColorScheme(LSEColorScheme.Standard);
            loadColorScheme();

            //set focus to text editing
            definitionBox.Focus();

        }//end form load event


        /// <summary>
        /// Sets teh Implementation specific notes in the About box
        /// </summary>
        /// <param name="notes"></param>
        public void SetImplementationNotes(string notes) {

            _implementation_notes = notes;

        }



        #endregion



        /// <summary>
        /// Helper functions for loading and setting the UI colors
        /// </summary>
        #region "Color Scheme"



        private void loadColorScheme() {

            uint cs = 0;

            try {

                LSystemEditor.Properties.Settings.Default.Reload();
                string val = LSystemEditor.Properties.Settings.Default.PreferredColorScheme;
                uint.TryParse(val, out cs);

            } catch {
            

            }
  
            //changing the color on the rich text box fires the text changed event
            //need to supress that
            _suppress_event = true;

            LSEColorScheme scheme = ((LSEColorScheme)cs);

            switch (scheme) {

                case LSEColorScheme.System:

                    applyColorScheme(LSEColorScheme.System);

                    break;


                case LSEColorScheme.Dark:

                    applyColorScheme(LSEColorScheme.Dark);

                    break;

                case LSEColorScheme.Light:

                    applyColorScheme(LSEColorScheme.Light);

                    break;

                default:

                    applyColorScheme(LSEColorScheme.System);

                    break;

            }



            //reset flag
            _suppress_event = false;

        }

        /// <summary>
        /// Set the Text editing components foreground and background colors
        /// </summary>
        /// <param name="scheme"></param>
        private void applyColorScheme(LSEColorScheme scheme) {

            _color_scheme = scheme;
            Color fore_color, back_color;


            switch (scheme) {

                case LSEColorScheme.System:


                    back_color = SystemColors.Window;
                    fore_color = SystemColors.WindowText;

                    break;

                case LSEColorScheme.Dark:

                    back_color = Color.FromArgb(31,31,31);
                    fore_color =  Color.WhiteSmoke; 

                    break;

                case LSEColorScheme.Light:

                    back_color = Color.FromArgb(255, 255, 224);
                    fore_color = Color.FromArgb(0,0,51);

                    break;

                default :

                    back_color = SystemColors.Window;
                    fore_color = SystemColors.WindowText;

                    break;

            }//end switch


            //main text editiing
            definitionBox.ForeColor = fore_color;
            definitionBox.BackColor = back_color;

            //find and replace
            findBox.ForeColor = fore_color;
            findBox.BackColor = back_color;

            replaceBox.ForeColor = fore_color;
            replaceBox.BackColor = back_color;

        }



        #endregion





        #region "Load LSystems"


        

        /// <summary>
        /// Parses the editor contents into LSystem objects.
        /// </summary>
        private void loadFromEditor() {

                Cursor dc = this.Cursor;
                this.Cursor = Cursors.WaitCursor;

            try {

                //clear out the loaded lSystems
                _lSystemEngine.Clear();

                LoadedSystemsList.Items.Clear();

                //parse the editor contents
                _lSystemEngine.ParseString(definitionBox.Text);


                string[] loadedSystems = _lSystemEngine.LoadedLSystems();


                if (loadedSystems != null) {

                    if (loadedSystems.Length > 0) {

                        LoadedSystemsList.Items.AddRange(_lSystemEngine.LoadedLSystems());
                        LoadedSystemsList.SelectedIndex = 0;

                        LoadedSystemsList.Enabled = true;
                        executeBttn.Enabled = true;

                    } else {

                        setSysListNone();

                    }//end if length

                } else {
                    setSysListNone();
                }//end if null



            } catch (Exception ex) {

                // _lSystemEngine.
                setSysListNone();
                MessageBox.Show(this.Owner, ex.Message, "Error Parsing text", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }//end try/catch


            this.Cursor = dc;

        }//end load editor


        #endregion


        #region MenuItems

        /// <summary>
        /// File button action - displays File popup menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileBttn_Click(object sender, EventArgs e) {

            Point sPoint = fileBttn.PointToScreen(new Point(0, fileBttn.Height));

            FilePopupMenuStrip.Show(sPoint);

        }

        /// <summary>
        /// Help Button action method - this displays the Help popup menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpBttn_Click(object sender, EventArgs e) {

            int right = HelpMenuContextStrip.PreferredSize.Width - helpBttn.Width;// HelpMenuContextStrip.Width - helpBttn.Width;
            
            Point sPoint = helpBttn.PointToScreen(new Point(-right, helpBttn.Height));

            HelpMenuContextStrip.Show(sPoint);

        }


        /// <summary>
        /// Menu File New action method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e) {

            fileNew();

        }



        /// <summary>
        /// Menu File Open action method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {

            openFile();

        }//end load text button


        /// <summary>
        /// Menu Save action method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {

            fileSave();
        }

        /// <summary>
        /// Menu SaveAs action method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {

            fileSaveAs();

        }



        /// <summary>
        /// Display the About Box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {

            AboutBox ab = new AboutBox();

            ab.ImplementationNotes = _implementation_notes;

            ab.ShowDialog(this);

        }




        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = Path.GetDirectoryName(path);
            path = Path.Combine(new string[] { path, "Help", "LSystemEngine" });
            path += ".html";

            try {

                System.Diagnostics.Process.Start(path);

            } catch (Exception ex) {

                MessageBox.Show("Unable to Open Help File:" + ex.Message + Environment.NewLine + "Please make sure the folder \"Help\" exists in the plugin directory.", "Help Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }


        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {


            this.Close();


        }



        private void openToolStripMenuItem2_Click(object sender, EventArgs e) {

            openFile();


        }

        private void SystemToolStripMenuItem_Click(object sender, EventArgs e) {

            applyColorScheme(LSEColorScheme.System);


        }

        private void LightToolStripMenuItem_Click(object sender, EventArgs e) {

            applyColorScheme(LSEColorScheme.Light);

        }

        private void DarkToolStripMenuItem_Click(object sender, EventArgs e) {

            applyColorScheme(LSEColorScheme.Dark);

        }


        #endregion



        #region FileOperations


        /// <summary>
        /// File New operation.  Resets the editor wit ha blank document.
        /// </summary>
        private void fileNew() {

            //prompt to save if editor is dirty
            if (editorChangeCheck()) {

                setSysListNone();

                definitionBox.Text = "";
                _current_file = String.Empty;
                _editor_dirty = false;
                setTitle();

                definitionBox.Focus();

            }//end if editorCheck


        }//end file new


        /// <summary>
        /// File Open operation.  Opens a file.
        /// </summary>
        private void openFile() {


            //prompt to save if editor is dirty
            if (editorChangeCheck()) {

                OpenFileDialog fd = new OpenFileDialog();

                fd.Title = "Select LSystem File to Open";
                fd.Filter = LSYSTEM_FILE_FILTER;
                //LSystem Specification Files (*.lsf)|*.lsf|

                fd.CheckFileExists = true;
                fd.CheckPathExists = true;

                fd.ShowDialog();




                string fileName = fd.FileName;

                if (!String.IsNullOrEmpty(fileName)) {

                    //LoadedFileLabel.Text = fileName;

                    try {

                        StreamReader sr = new StreamReader(fd.OpenFile());

                        definitionBox.Text = sr.ReadToEnd();

                        sr.Close();
                        sr.Dispose();
                        sr = null;

                        //record the current file name
                        _current_file = fileName;

                        _editor_dirty = false;
                        setTitle();

                        setSysListNone();

                         statusMessage.Text = "Opened: " + fileName;

                         definitionBox.Focus();

                    } catch (Exception ex) {

                        definitionBox.Text = "Error reading file: " + ex.Message;
                        _current_file = String.Empty;
                        _editor_dirty = false;
                        setTitle();

                    }
                }//end if file selected



            }//end if editordirtyCheck


        }//end open file

        /// <summary>
        /// File Save operation.  Saves the current editor text to disk.  IF a file has not been set, user is prompted for a new file.
        /// </summary>
        public void fileSave() {

            StreamWriter outFile;

            if (_current_file != String.Empty) {


                try {

                    //get a new stream writer
                    outFile = new StreamWriter(_current_file);

                    //write out the contents of the editor 
                    outFile.Write(definitionBox.Text);
                    outFile.Flush();
                    outFile.Close();
                    outFile.Dispose();
                    outFile = null;

                    _editor_dirty = false;
                    setTitle();

                    statusMessage.Text = "File Saved.";

                    definitionBox.Focus();

                } catch (Exception ex) {


                    MessageBox.Show("Error saving file: " + ex.Message, "Error Saving File", MessageBoxButtons.OK, MessageBoxIcon.Error);


                } //end try catch



            } else {
                //there was no file opened, so start new one
                fileSaveAs();


            }//end if/else



        }//end file save

        /// <summary>
        /// File Save As operation.  Saves the current editor text as a new file.
        /// </summary>
        public void fileSaveAs() {


            SaveFileDialog sa = new SaveFileDialog();
            sa.Filter = LSYSTEM_FILE_FILTER;
            sa.Title = "Save File As";
            sa.ShowDialog();

            if (!String.IsNullOrEmpty(sa.FileName)) {

                _current_file = sa.FileName;
                fileSave();

            } else {


            }//end if/else




        }//end file saveas


        #endregion



        #region UIEventsAndHelpers


        private void definitionBox_MouseClick(object sender, MouseEventArgs e) {
            _find_index = definitionBox.SelectionStart;
            UpdateLinePosition();

        }

        private void definitionBox_Click(object sender, EventArgs e) {
            _find_index = definitionBox.SelectionStart;
            UpdateLinePosition();
        }

        private void definitionBox_KeyPress(object sender, KeyPressEventArgs e) {
            _find_index = definitionBox.SelectionStart;
            UpdateLinePosition();
        }

        private void definitionBox_KeyUp(object sender, KeyEventArgs e) {
            _find_index = definitionBox.SelectionStart;
            UpdateLinePosition();
        }


        /// <summary>
        /// Sets the GUI elements for progress display
        /// </summary>
        private void setEditorDisplayMode() {

            //progressDisplayPanel.Visible = false;
            //definitionBox.Visible = true;
            //toolPanel.Enabled = true;

            ////unlock modeler configuration
            //editorProgressContainer.Panel2.Enabled = true;

            toolPanel.Enabled = true;
            //show editor
            editorProgressContainer.Panel2.Enabled = true;
            editorProgressContainer.Panel1Collapsed = false;


            //hide progess
            editorProgressContainer.Panel2Collapsed = true;
            editorProgressContainer.Panel2.Enabled = false;


        }


        /// <summary>
        /// Sets the GUI the elements for editing
        /// </summary>
        private void setExecutingDisplayMode() {

            //progressDisplayPanel.Visible = true;
            //definitionBox.Visible = false;
            //toolPanel.Enabled = false;
            //lock modeler configuration
            //editorProgressContainer.Panel2.Enabled = false;

            toolPanel.Enabled = false;
            //hide editor
            editorProgressContainer.Panel2.Enabled = false;
            editorProgressContainer.Panel1Collapsed = true;
            

            //show progess
            editorProgressContainer.Panel2Collapsed = false;
            editorProgressContainer.Panel2.Enabled = true;

        }

        /// <summary>
        /// Editor changed event method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditorBox_TextChanged(object sender, EventArgs e) {

            SetEditorDirty();

        }


        private void SetEditorDirty() {

            //since it's a rich text dialog, changing the color fires the change event
            if (!_suppress_event) {

                UpdateLinePosition();

                _editor_dirty = true;
                setTitle();
                statusMessage.Text = "";
            }

        }

        /// <summary>
        /// Helper to update the line number
        /// </summary>
        private void UpdateLinePosition() {

            int curSelect = definitionBox.SelectionStart;
            int line = definitionBox.GetLineFromCharIndex(curSelect);

           
            _current_line = line + 1;

            statusLineCount.Text = "Line: " + _current_line;

    

        }


        /// <summary>
        /// Helper method to set Loaded LSystem List.
        /// </summary>
        private void setSysListNone() {

            LoadedSystemsList.Items.Clear();
            LoadedSystemsList.Items.AddRange(new string[] { EXECUTE_EMPTY });
            LoadedSystemsList.SelectedIndex = 0;
            LoadedSystemsList.Enabled = false;

            executeBttn.Enabled = false;


        }

        /// <summary>
        /// Check to see if editor context has changed and user wants to save.
        ///  True to proceed with editor change, False to halt operation (don't change editor contents).
        /// </summary>
        /// <returns>Boolean to proceed (change contents) or halt operation (preserve contents)</returns>
        private bool editorChangeCheck() {


            if (_editor_dirty) {

                string dlgText = "Save Changes to:";

                if (_current_file == "") {
                    dlgText += "<unnamed>";
                } else {
                    dlgText += _current_file;
                }


                DialogResult dr = MessageBox.Show(dlgText, "Save Changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);

                if (dr == DialogResult.Yes) {
                    if (_current_file == "") {
                        fileSaveAs();
                    } else {
                        fileSave();
                    }

                } else if (dr == DialogResult.Cancel) {
                    //get out
                    return false;

                }//end if/else dialogReuslt save

            }//end if  editor dirty

            return true;

        }


        /// <summary>
        /// Routine to set the Window title
        /// </summary>
        private void setTitle() {

            if (_current_file != String.Empty) {

                string displayName = _current_file;

                if (_editor_dirty) {
                    displayName = "*" + displayName;
                }

                this.Text = _form_title + " | " + displayName;

            } else {

                this.Text = _form_title;

                if (_editor_dirty) {
                    this.Text += " | *";
                }

            }//end if/else


        }//end set title



        /// <summary>
        /// Form closing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LSysEngineGUI_FormClosing(object sender, FormClosingEventArgs e) {

            //need to make sure system is not executing....
            if (!_system_executing) {

                //check the contents of the editor on close
                if (!editorChangeCheck()) {
                    e.Cancel = true;
                } else {

                    try {
                        //save settings
                        uint temp = (uint)_color_scheme;
                        LSystemEditor.Properties.Settings.Default["PreferredColorScheme"] = temp.ToString();
                        LSystemEditor.Properties.Settings.Default.Save();
                    } catch {

                    }

                }

            } else {
                //if system is still executing don't close (this is the listener!)
                e.Cancel = true;
            }




        }//end form closing


        #endregion




        #region ListenerSection

        /*
         * Listener section is composed of Delegate/Method pairs
         * 
         */


        /// <summary>
        /// Called when string (file) parsing begins. Use a delegate call
        /// </summary>
        public void ParseBegin() {
            //invoke the delegate
            d_ParseBegin();
        }

        

        private void OnParseBegin() {

            //do some prep work
            //indicate system is busy...
            _system_executing = true;
        }

        /// <summary>
        /// Called during string (file) parsing when an LSystem is loaded.
        /// </summary>
        /// <param name="name"></param>
        public void ParseLoadedSystem(string name) {
            this.Invoke(d_ParseLoad, new object[] { name });

        }


        private void OnParseLoaded(string name) {

            //display the loaded string

        }

        /// <summary>
        /// Called when string (file) parsing ends.  Reports the number of loaded LSystems.
        /// </summary>
        /// <param name="systemCount"></param>
        public void ParseEnd() {
            //make delegate call
            this.Invoke(d_ParseEnd);
        }

        private void OnParseEnd() {

            //do some notification
            statusMessage.Text = "LSystems Loaded";

            //indicate system is done.
            _system_executing = false;

        }

        /// <summary>
        /// Called when the LSystem parser encounters an exception.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        /// <param name="ex">The exception encountered.</param>
        public void LSystemParseError(string msg, Exception ex) {
            //make delegate call
            this.Invoke(d_ParseError, new object[] {msg, ex});

        }

        private void OnParserError(string msg, Exception ex) {

            MessageBox.Show(msg + " : " + ex.Message, "Parser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusMessage.Text = "Parse error";

            //indicate system is done.
            _system_executing = false;

        }

        /// <summary>
        /// Delegate Wrapper for OnLSystemExecutionBegin
        /// </summary>
        /// <param name="name">The name of the LSystem to be evaluated.</param>
        public void LSystemExecutionBegin(string name) {

            //delegate call
            this.Invoke(d_ExecutionBegin, new object[] {name } );

        }

        /// <summary>
        /// Called when the execution of an LSystem begins.  This will also be called when a nested LSystem executes.
        /// </summary>
        /// <param name="name"></param>
        private void OnLSystemExecutionBegin(string name) {

            //System.Diagnostics.Debug.Print("ExecutionBegin: " + _stop_watch.Elapsed.ToString());

            //indicate system is busy....
            _system_executing = true;

            //setup
            setExecutingDisplayMode();

            //progress panel
            progressDisplayPanel.ExecutionBegin(name);

            

        }//end OnLSystemExecutionBegin


        /// <summary>
        /// Delegate Wrapper for OnLSystemExecutionBegin
        /// </summary>
        /// <param name="name">The name of the LSystem to be evaluated.</param>
        public void SubSystemExecutionBegin(string name) {

            //delegate call
            this.Invoke(d_SubSystemExecutionBegin, new object[] { name });

        }

        /// <summary>
        /// Called when the execution of an LSystem begins.  This will also be called when a nested LSystem executes.
        /// </summary>
        /// <param name="name"></param>
        private void OnSubSystemExecutionBegin(string name) {

            //progress panel
            progressDisplayPanel.SubSystemExecutionBegin(name);


        }//end OnLSystemExecutionBegin

        /// <summary>
        /// Delegate Wrapper for OnEvaluationBegin
        /// </summary>
        /// <param name="nam">The name of the LSytem being evaluated.</param>
        public void EvaluationBegin(string name) {
            //delegate call
            //d_EvaluationBegin(name);
            this.Invoke(d_EvaluationBegin, new object[] { name });

        }


        /// <summary>
        /// Called when the evaluation of an LSystem begins (start of production cycle).
        /// </summary>
        /// <param name="name"></param>
        private void OnEvaluationBegin(string name) {

            //prepare progress display
            //progressControl.SetProductionDisplay();

        }

        /// <summary>
        /// Delegate Wrapper for OnEvaluationProgress
        /// </summary>
        /// <param name="name">The name of the LSytem beign evaluated.</param>
        /// <param name="iteration">The current iteration beign processed.</param>
        /// <param name="ofIterations">The total number of iterations to process.</param>
        /// <param name="letter">The index number of the current letter being processed</param>
        /// <param name="ofLetters">The total number of letters to process.</param>
        public void EvaluationProgress(string name, int iteration, int ofIterations, int letter, int ofLetters) {
            //delegate call
            //d_EvaluationProgess(name, iteration, ofIterations, letter, ofLetters);
            this.Invoke(d_EvaluationProgess, new object[] { name, iteration, ofIterations, letter, ofLetters });

        }

        /// <summary>
        /// Called during the evaluation of an LSystem when a letter is being test against the productions.
        /// </summary>
        /// <param name="name">The name of the LSytem beign evaluated.</param>
        /// <param name="iteration">The current iteration beign processed.</param>
        /// <param name="ofIterations">The total number of iterations to process.</param>
        /// <param name="letter">The index number of the current letter being processed</param>
        /// <param name="ofLetters">The total number of letters to process.</param>
        public void OnEvaluationProgress(string name, int iteration, int ofIterations, int letter, int ofLetters) {
   
            //display progress
            //progressControl.ProductionUpdate(iteration, ofIterations, letter, ofLetters);
            progressDisplayPanel.ProductionUpdate(iteration, ofIterations, letter, ofLetters);

        }

        /// <summary>
        /// Delegate Wrapper for OnEvaluationEnd
        /// </summary>
        /// <param name="name">The name of the LSytem evaluated</param>
        public void EvaluationEnd(string name) {

            this.Invoke(d_EvaluationEnd, new object[] { name });

        }

        /// <summary>
        /// Called when the evaluation of an LSystem ends (end of production cycle)
        /// </summary>
        /// <param name="name">The name of the LSytem evaluated</param>
        private void OnEvaluationEnd(string name) {

            //clean up display
            //test to update status bars
            //do it here so it only happens once - slow
            progressDisplayPanel.Refresh();

        }


        /// <summary>
        /// Delegate Wrapper for OnResolverBegin
        /// </summary>
        /// <param name="name">The name of the LSystem to be resolved</param>
        public void ResolverBegin(string name) {
            //delegate call
           // d_ResolverBegin(name);
            this.Invoke(d_ResolverBegin, new object[] { name });

        }

        /// <summary>
        /// Called when the resolving of an LSystem begins (all final axim letters are resolved from groups, aliases, and nested LSystems.
        /// </summary>
        /// <param name="name">The name of the LSystem to be resolved</param>
        private void OnResolverBegin(string name) {

            // Console.WriteLine("resolver begin: " + name);
            //progressControl.SetResolverDisplay();

        }


        /// <summary>
        /// Delegate Wrapper for OnResolverProgress
        /// </summary>
        /// <param name="name">The name of the LSystem being resolved</param>
        /// <param name="letter">The index of the Letter currently being resolved</param>
        /// <param name="ofLetters">The number of Letters to process</param>
        public void ResolverProgress(string name, int letter, int ofLetters) {

            //delegate call
            this.Invoke(d_ResolverProgess, new object[] { name, letter, ofLetters });


        }

        /// <summary>
        /// Called during the resolver process.
        /// </summary>
        /// <param name="name">The name of the LSystem being resolved</param>
        /// <param name="letter">The index of the Letter currently being resolved</param>
        /// <param name="ofLetters">The number of Letters to process</param>
        private void OnResolverProgress(string name, int letter, int ofLetters) {

            //update progress display
            //progressControl.ResolverUpdate(letter, ofLetters);
             progressDisplayPanel.ResolverUpdate(letter, ofLetters);

        }
        /// <summary>
        /// Delegate Wrapper for OnResolverEnd
        /// </summary>
        /// <param name="name">The name of the LSystem resolved</param>
        public void ResolverEnd(string name) {

            //delegate call
            this.Invoke(d_ResolverEnd, new object[] { name });

        }

        /// <summary>
        /// Called when the resolving of an LSystem ends
        /// </summary>
        /// <param name="name">The name of the LSystem resolved</param>
        private void OnResolverEnd(string name) {

            //cleanup display after resolver is done
            //...nothing to do...
            //test to update status bars
            //do it here so it only happens once - slow
            progressDisplayPanel.Refresh();

        }

        /// <summary>
        /// Delegate Wrapper for OnLSystemExecutionEnd
        /// </summary>
        /// <param name="name">The name of the Lsystem executed</param>
        public void LSystemExecutionEnd(string name) {
            //delegate call
            //d_ExecutionEnd(name);
            this.Invoke(d_ExecutionEnd, new object[] { name });

        }

        /// <summary>
        /// Called when the Execution of an LSystem ends
        /// </summary>
        /// <param name="name">The name of the Lsystem executed</param>
        private void OnLSystemExecutionEnd(string name) {


            _stop_watch.Stop();
            //System.Diagnostics.Debug.Print("ExecutionEnd: " + _stop_watch.Elapsed.ToString());
            //Console.WriteLine("system execution complete: " + name);
            //see if last LSystem called
            progressDisplayPanel.ExecutionEnd(name);

            
            setEditorDisplayMode();

            string elapsed = "Completed in: ";
            int e_h = (int)_stop_watch.Elapsed.TotalHours;
            int e_m = (int)_stop_watch.Elapsed.TotalMinutes;
            int e_s = (int)_stop_watch.Elapsed.TotalSeconds;

            if ( e_h > 0) {
                elapsed += e_h + "h, ";
            }
            if (e_m > 0) {
                e_m = e_m % 60;
                elapsed += e_m + "m, ";
            }
            //show seconds even if 0
            e_s = e_s % 60;
            elapsed += e_s + "s";
            
         

            statusMessage.Text = "Execution Ended on: " + name + "   |  " +  elapsed;

            //indicate system is done.
            _system_executing = false;

        }//end OnLSystemExecutionEnd

        /// <summary>
        /// Delegate Wrapper for OnSubSystemExecutionEnd
        /// </summary>
        /// <param name="name">The name of the sub-system executed</param>
        public void SubSystemExecutionEnd(string name) {
            //delegate call
            //d_ExecutionEnd(name);
            this.Invoke(d_SubSystemExecutionEnd, new object[] { name });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the sub-system executed</param>
        private void OnSubSystemExecutionEnd(string name) {


            progressDisplayPanel.SubSystemExecutionEnd(name);


        }


        /// <summary>
        /// Delegate Wrapper for OnLSystemExecutionError
        /// </summary>
        /// <param name="msg">The exception message</param>
        /// <param name="ex">The exception encountered</param>
        public void LSystemExecutionError(string msg, Exception ex) {

            //delegate call
            this.Invoke(d_ExecutionError, new object[] {msg, ex});

        }

        /// <summary>
        /// Called when the LSystem encounters an exception.
        /// </summary>
        /// <param name="msg">The exception message</param>
        /// <param name="ex">The exception encountered</param>
        private void OnLSystemExecutionError(string msg, Exception ex) {

            System.Diagnostics.Debug.Print("ExecutionError: " + _stop_watch.Elapsed.ToString());

            setEditorDisplayMode();
            statusMessage.Text = "Execution Error";
            //display error
            MessageBox.Show(msg + " : " + ex.Message , "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //indicate system is done.
            _system_executing = false;
           
        }
        

        #endregion



       #region ParseAndExecute

       /// <summary>
       /// Parse button action method.  This parses the text in the editor into LSystem objects ready for execution.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       private void parseBttn_Click(object sender, EventArgs e) {

           //call the load function
           loadFromEditor();

       }



       /// <summary>
       /// Cancels the execution
       /// </summary>
       public void IssueCancel() {

           //is this thread safe!?...
           if (_lSystemEngine != null) {

               _lSystemEngine.CancelExecution();


           }


       }

       /// <summary>
       /// Execute button action method.  This starts the execution of the selected LSystem on a new thread.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       private void executeBttn_Click(object sender, EventArgs e) {



           int li = LoadedSystemsList.SelectedIndex;

           if (li != -1) {

               //pull the string from the list
               string systemToExecute = LoadedSystemsList.SelectedItem.ToString();
               //check for null string
               if (!String.IsNullOrWhiteSpace(systemToExecute)) {

                   //double check it's not the empty list constant
                   if (!systemToExecute.Equals(EXECUTE_EMPTY)) {

                       //reset progress display
                       progressDisplayPanel.ResetProgressDisplay();

                       _stop_watch.Reset();
                       _stop_watch.Start();

                       statusMessage.Text = "Executing LSystem: " + systemToExecute;

                       switch (_threading_model) {


                           case ExecutionThreadingModel.ExecuteOnNewThread:
                               //multi threaded version, starts execution on a new thread
                               //init and start new thread
                               Thread newExecuteThread = new Thread(new ParameterizedThreadStart(executeSystem));
                               //StartPosition thread
                               newExecuteThread.Start(systemToExecute);
                               break;


                           case ExecutionThreadingModel.SingleThread:
                               //single thread execution
                               //this should be fine as Application.DoEvents is called internally to LSystem
                               //execute the LSystem on the current thread
                               _execution_handle.ExecuteSystem(systemToExecute);
                               break;


                           default:

                               MessageBox.Show("Unsupported Threading Model: " + _threading_model + " System Will not Execute");
                               break;




                       }//edn switch



                   } else {

                       //systemToExecute = String.Empty;
                       //no system to execute
                       statusMessage.Text = "No LSystems to Execute - Load the Systems first";

                   }//end if/else

               } else {

                   //systemToExecute = String.Empty;
                   //no system to execute
                   statusMessage.Text = "No LSystems to Execute - Load the Systems first";

               }//end if/else empty


           } else {

               //no system to execute
               statusMessage.Text = "No LSystems to Execute - Load the Systems first";

           }//end if/else selected index check




       }//end execute bttn


 

        /// <summary>
        /// Target for Thread to Execute LSystem
        /// </summary>
        /// <param name="name"></param>
       private void executeSystem(object name) {

           string sysName = (string)name;
           _execution_handle.ExecuteSystem(sysName);
          // _lSystemEngine.Execute(sysName, null);

       }

       #endregion




       #region "Find and replace"

       private void findReplaceCB_CheckedChanged(object sender, EventArgs e) {

           if (findReplaceCB.Checked) {

               textPanelContainer.Panel1Collapsed = false;


           } else {

               textPanelContainer.Panel1Collapsed = true;

           }



       }

       private void findBox_TextChanged(object sender, EventArgs e) {

         //???  _find_index = 0;

       }


       private void findBttn_Click(object sender, EventArgs e) {

           int temp = findText();

       }


        private int findText() {

           string find = findBox.Text.Trim();
           //int lastPos = 0;


           if (!String.IsNullOrWhiteSpace(find)) {

               if (_find_index < 0) {

                   _find_index = 0;

               }

               RichTextBoxFinds rtbFind = RichTextBoxFinds.None;

               if (matchCaseCheck.Checked && wholeWordCheck.Checked) {
                   rtbFind = RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord;
               } else if (matchCaseCheck.Checked) {
                   rtbFind = RichTextBoxFinds.MatchCase;
               } else if (wholeWordCheck.Checked) {
                   rtbFind = RichTextBoxFinds.WholeWord;
               }

               _find_index = definitionBox.Find(find, _find_index, rtbFind);

               if (_find_index >= 0) {

                   definitionBox.Focus();
                   definitionBox.Select(_find_index, find.Length);

                   UpdateLinePosition();
                   _find_index += find.Length;

                   return _find_index;

               } else {
                   _find_index = 0;
                   MessageBox.Show("Text: " + find + " was not found" + Environment.NewLine + "Search will begin from begining.","Text not found",  MessageBoxButtons.OK,  MessageBoxIcon.Information);
                   return -1;
               }



           } else {

               MessageBox.Show("No text to find sepcified", "No text to find", MessageBoxButtons.OK, MessageBoxIcon.Information);
               return -3;
           }


       }

       private void replaceBttn_Click(object sender, EventArgs e) {


           int temp = replaceSelected();
           if (temp > 0 ) {
               findText(); 
           }

       }

       private int replaceSelected() {

           if (definitionBox.SelectionLength != 0) {

               string replace = replaceBox.Text.Trim();

               //if (!String.IsNullOrWhiteSpace(replace)) {

                   int ss = definitionBox.SelectionStart;
                   int se = ss + definitionBox.SelectionLength;
                   int tailLen = definitionBox.Text.Length - se;

                   string firstPart = definitionBox.Text.Substring(0, ss);
                   string secondPart = definitionBox.Text.Substring(se, tailLen);

                   string newText = firstPart + replace + secondPart;


                   definitionBox.Text = newText;

                   SetEditorDirty();

                   //return next position
                   return firstPart.Length + replace.Length -1;

               //} else {
               //    return -1;
               //}//end if

           } else {
               return -3;
           }//end if


       }


       private void replaceAllBttn_Click(object sender, EventArgs e) {

            replaceAll();


       }//end replace all button

       private void replaceAll() {

           RichTextBoxFinds rtbFind = RichTextBoxFinds.None;

           if (matchCaseCheck.Checked && wholeWordCheck.Checked) {
               rtbFind = RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord;
           } else if (matchCaseCheck.Checked) {
               rtbFind = RichTextBoxFinds.MatchCase;
           } else if (wholeWordCheck.Checked) {
               rtbFind = RichTextBoxFinds.WholeWord;
           }

           string find = findBox.Text.Trim();
           string replace = replaceBox.Text.Trim();
           int findIndex = 0;
           int replace_count = 0;

           if (!String.IsNullOrWhiteSpace(replace) && !String.IsNullOrWhiteSpace(find)) {

               while (findIndex >= 0) {

                   findIndex = definitionBox.Find(find, findIndex, rtbFind);

                   if (findIndex >= 0) {

                       int find_end = findIndex + find.Length;
                       int tailLen = definitionBox.Text.Length - find_end;

                       string firstPart = definitionBox.Text.Substring(0, findIndex);
                       string secondPart = definitionBox.Text.Substring(find_end, tailLen);

                       definitionBox.Text = firstPart + replace + secondPart;

                       findIndex = find_end;
                       _find_index = find_end;

                       replace_count += 1;

                   } else {
                       _find_index = 0;

                   }

               }//end while


               MessageBox.Show("Replaced " + replace_count + " instances of: " + find + " with: " + replace);


           }//end if

       }

       #endregion




        #region "Internal Execution Handle"


        class InternalExecutionHandle : IEditorExecutionHandle {

           LSystemEvaluationEngine _engine;

           public InternalExecutionHandle(LSystemEvaluationEngine engine) {

               _engine = engine;

           }

           /// <summary>
           /// Handle for exditor to begin execution
           /// </summary>
           /// <param name="system"></param>
           public void ExecuteSystem(string system) {

               _engine.Execute(system, null);

           }


       }


        #endregion


    }//end class

}//end namespace
