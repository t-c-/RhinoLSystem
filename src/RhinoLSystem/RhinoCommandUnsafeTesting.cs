using System;

using System.Windows.Forms;

using Rhino;
using Rhino.Commands;

namespace RhinoLSystem {
    [System.Runtime.InteropServices.Guid("44cb3fda-d3c0-4bf4-adad-7f1a277963b3")]
    public class RhinoCommandUnsafeTesting : Command {
        static RhinoCommandUnsafeTesting _instance;
        public RhinoCommandUnsafeTesting() {
            _instance = this;
        }

        ///<summary>The only instance of the TestBadCommand command.</summary>
        public static RhinoCommandUnsafeTesting Instance {
            get { return _instance; }
        }

        public override string EnglishName {
            get { return "LSystemUnsafeTesting"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode) {
            // TODO: complete command.

            string warn = "This command is an experimental multi-threaded execution of the LSystem.   ";
            warn += "Rhino is not Thread-Safe and this command may result in unexpected behaviour!  ";
            warn += "No guarantee can be made of what may or may not result from it's use and it is no way supported.";
         

            DialogResult dr = MessageBox.Show(warn, "Unsafe Command Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (dr == DialogResult.OK) {

                RhinoLSystemModeler _modeler = new RhinoLSystemModeler(doc);

                if (_modeler.LoadResolved) {


                    //init editor
                    LSystemEditor.LSystemEngineGUI _editor = new LSystemEditor.LSystemEngineGUI(_modeler, LSystemEditor.ExecutionThreadingModel.ExecuteOnNewThread, "Unsafe Testing");
                    //show the editor
                    _editor.Show(RhinoApp.MainWindow());

                } else {

                    //load di not go well!
                    RhinoApp.Write(_modeler.LoadReport);

                    RhinoApp.WriteLine("RhinoLSystem will close now.");

                    _modeler = null;


                }//end if else


            }




            return Result.Success;
        }
    }
}
