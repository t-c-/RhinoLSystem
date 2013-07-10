using System;
using System.Threading;


using Rhino;
using Rhino.Commands;



namespace RhinoLSystem {
    [System.Runtime.InteropServices.Guid("370df2da-c28b-4374-9c47-33d2d5c7a4b6"), CommandStyle(Style.Hidden)]
    public class RhinoCommandExecuteLSystem : Command {
        static RhinoCommandExecuteLSystem _instance;


        public RhinoCommandExecuteLSystem() {
            _instance = this;
        }

        ///<summary>The only instance of the ExecuteLSystemCommand command.</summary>
        public static RhinoCommandExecuteLSystem Instance {
            get { return _instance; }
        }

        public override string EnglishName {
            get { return "ExecuteLSystem"; }
        }





        /// <summary>
        /// This doesn't work so hot.....
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode) {



            //prompt for the name (really so it can be passed from RunScript()
            Rhino.Input.Custom.GetString getName = new Rhino.Input.Custom.GetString();
            getName.SetCommandPrompt("LSystem to execute");
            getName.AcceptNothing(false);
            getName.Get();
            if (getName.CommandResult() != Rhino.Commands.Result.Success) {
                Rhino.RhinoApp.WriteLine("Failed to get LSystem name");
                return Rhino.Commands.Result.Failure;
            }

            //get the system name
            string system_name = getName.StringResult().Trim();
            if (string.IsNullOrEmpty(system_name)) {
                Rhino.RhinoApp.WriteLine("No LSystem specified (empty name).");
                return Rhino.Commands.Result.Failure;
            }

            //grab a ref to the Engine
            LSystemEngine.LSystemEvaluationEngine engine = RhinoLSystemPlugIn.Instance.Engine;



            if (engine != null) {



                EventHandler evh = new EventHandler(RhinoApp_EscapeKeyPressed);
                RhinoApp.EscapeKeyPressed += evh;

                doc.Views.RedrawEnabled = false;

                //execute the system
                engine.Execute(system_name, null);

                //redraw
                doc.Views.RedrawEnabled = true;
                doc.Views.Redraw();

                //remove handler
                RhinoApp.EscapeKeyPressed -= evh;

                return Result.Success;

            } else {
                Rhino.RhinoApp.WriteLine("Failed to retrieve LSystem Engine from Plugin.");
                return Rhino.Commands.Result.Failure;
            }



        }//end run command


        /// <summary>
        /// Handler for Escape Key Pressed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RhinoApp_EscapeKeyPressed(Object sender, System.EventArgs args) {

            Rhino.RhinoApp.WriteLine("ESC Cancelling execution...");
            //grab a ref to the Engine
            LSystemEngine.LSystemEvaluationEngine engine = RhinoLSystemPlugIn.Instance.Engine;


            if (engine != null) {

                //execute the system
                engine.CancelExecution();


            } else {
                Rhino.RhinoApp.WriteLine("Failed to send cancel to LSystem Engine from Plugin.");

            }

        }
    }//end class
}
