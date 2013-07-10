using System;
using Rhino;
using Rhino.Commands;

namespace RhinoLSystem {
    [System.Runtime.InteropServices.Guid("123b4333-2529-490e-adcb-2deeb65fa2e7")]
    public class RhinoCommandLSystem : Command {
        static RhinoCommandLSystem _instance;
        public RhinoCommandLSystem() {
            _instance = this;
        }

        ///<summary>The only instance of the RhinoCommandLSystemSafeModal command.</summary>
        public static RhinoCommandLSystem Instance {
            get { return _instance; }
        }

        public override string EnglishName {
            get { return "LSystem"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode) {
            // TODO: complete command.

            RhinoApp.WriteLine("Launching LSystem Editor.");

            //new form - uses Rhino Undo Stack properly
            RhinoLSystemPlugIn.Instance.ShowLSystemEditor(doc);



            return Result.Success;
        }
    }
}
