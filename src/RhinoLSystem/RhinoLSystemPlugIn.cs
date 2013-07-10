using System;
using System.Windows.Forms;
using Rhino;

namespace RhinoLSystem {
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class RhinoLSystemPlugIn : Rhino.PlugIns.PlugIn {
        static RhinoLSystemPlugIn _instance;

        private LSystemEngine.LSystemEvaluationEngine _lsystem_engine;
        private LSystemEditor.LSystemEngineGUI _editor;
        private RhinoLSystemModeler _modeler;

        private System.EventHandler<DocumentEventArgs> _docCloseEventH;
        private System.EventHandler<DocumentOpenEventArgs> _docOpenEventH;

        public RhinoLSystemPlugIn() {
            _instance = this;

            _lsystem_engine = null;
            _editor = null;
            _modeler = null;

            _docCloseEventH = null;
            _docOpenEventH = null;


        }

        ///<summary>Gets the only instance of the RhinoLSystemPlugIn plug-in.</summary>
        public static RhinoLSystemPlugIn Instance {
            get { return _instance; }
        }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and mantain plug-in wide options in a document.


        
        /// <summary>
        /// To comply with Rhino Plugin Installer instructions.  McNeel provided... 
        /// http://wiki.mcneel.com/developer/rhinoinstallerengine/dotnet
        /// </summary>
        /// <returns></returns>
        public System.Guid PlugInID() {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            object[] idattr = a.GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), true);
            System.Guid id = new System.Guid(((System.Runtime.InteropServices.GuidAttribute)idattr[0]).Value);
            return id;
        }

        /// <summary>
        /// To comply with Rhino Plugin Installer instructions.  McNeel provided... 
        /// http://wiki.mcneel.com/developer/rhinoinstallerengine/dotnet
        /// </summary>
        /// <returns></returns>
        public string PlugInName() {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            object[] idattr = a.GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), true);
            string name = ((System.Reflection.AssemblyTitleAttribute)idattr[0]).Title;
            return name;
        }

        /// <summary>
        /// To comply with Rhino Plugin Installer instructions.  McNeel provided... 
        /// http://wiki.mcneel.com/developer/rhinoinstallerengine/dotnet
        /// </summary>
        /// <returns></returns>
        public  string PlugInVersion() {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string version = a.GetName().Version.ToString();
            return version;
        }


        /// <summary>
        /// Initializes the Editor and Engine Objects and displays the GUI.
        /// </summary>
        /// <param name="doc"></param>
        private void InitLSystem(Rhino.RhinoDoc doc) {

            //since the modeler needs to load the commands dynamically
            //load it first and check it before proceeding
            _modeler = new RhinoLSystemModeler(doc);

            if (_modeler.LoadResolved) {


                _docCloseEventH = new System.EventHandler<DocumentEventArgs>(DocumentClose);
                _docOpenEventH = new System.EventHandler<DocumentOpenEventArgs>(DocumentOpen);
                RhinoDoc.CloseDocument += _docCloseEventH;
                RhinoDoc.EndOpenDocument += _docOpenEventH;
                //RhinoDoc.BeginOpenDocument += _docOpenEventH;

                //new portability testing
                EditorExecutionHandle eeh = new EditorExecutionHandle();

                _lsystem_engine = new LSystemEngine.LSystemEvaluationEngine(_modeler, null);

                //init editor
                _editor = new LSystemEditor.LSystemEngineGUI(_lsystem_engine, eeh, LSystemEditor.ExecutionThreadingModel.SingleThread, "LSystem Engine for Rhino");

                //set the implementation specific notes for the about box
                _editor.SetImplementationNotes(RhinoImplementationNotes());

                //happens interanlly to editor
                _lsystem_engine.SetListener(_editor);

                //add the handler to dispose of the form since references are being kept here
                _editor.FormClosed += new FormClosedEventHandler(EditorWindowClosed);

                //show the editor
                _editor.Show(RhinoApp.MainWindow());

            } else {

                //load did not go well!
                RhinoApp.Write(_modeler.LoadReport);

                RhinoApp.WriteLine("RhinoLSystem will close now.");

                _modeler = null;


            }//end if else

        }//end InitSystem

        /// <summary>
        /// Document close event - push null document ref to modeler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="a"></param>
        private void DocumentClose(object sender, DocumentEventArgs a) {

            //this will throw an error if execution attempted now
            //shouldn't happen, but error should be handled...
            //do a check for rhino close...
            if (_modeler != null) {

                _modeler.PushDocReference(null);

            }
            

        }

        /// <summary>
        /// Document Open Event - Update the Modeler with new document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="a"></param>
        private void DocumentOpen(object sender, DocumentOpenEventArgs a) {

            //set reference to new document
            if (_modeler != null) {
                _modeler.PushDocReference(a.Document);
            }

        }

        /// <summary>
        /// Cleanup Engine and GUI Objects when the window is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditorWindowClosed(object sender, FormClosedEventArgs e) {

            //yes - being reached
            RhinoApp.WriteLine("Closing LSystem...");
            

            //remove handlers...
            if (_docCloseEventH != null) {
                RhinoDoc.CloseDocument -= _docCloseEventH;
            }
            if (_docOpenEventH != null) {
                RhinoDoc.EndOpenDocument -= _docOpenEventH;
            }

            //deref it all...
            if (_editor != null) {
                _editor.Dispose();
            }

            _editor = null;

            _lsystem_engine = null;
            _modeler = null;

        }


        /// <summary>
        /// Shows the LSystem Editor
        /// </summary>
        /// <param name="doc"></param>
        public void ShowLSystemEditor(Rhino.RhinoDoc doc) {

            if (_editor == null) {
                InitLSystem(doc);
            } else if (_editor.IsDisposed) {
                _editor = null;
                InitLSystem(doc);
            }
            //otherwise it should already be showing...

        }//end showLSystemEditor




        /// <summary>
        /// Returns a reference to the engine.  This may return a null, so it needs to be checked before accessing!
        /// </summary>
        public LSystemEngine.LSystemEvaluationEngine Engine {
            get {

                return _lsystem_engine;
            }
        }//end Property Engine


        private string RhinoImplementationNotes() {

            string notes = "LSystem Engine Implementation for Rhino." + Environment.NewLine;
            notes += "Rhino LSystem PlugIn: 2012~2013" + Environment.NewLine + Environment.NewLine;
            notes += "Thanks To Robert McNeel & Associates for their continued support of the Rhino community. " +Environment.NewLine;


            return notes;

        }
        


    }//end class
}