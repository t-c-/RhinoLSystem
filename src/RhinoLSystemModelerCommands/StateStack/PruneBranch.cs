using System;

using Rhino;

namespace RhinoLSystem.Commands {
    public class PruneBranch : ModelerCommand {


        public override string CommandName() {
            return "PruneBranch";
        }

        public override int NumberOfArguments() {
            return 0;
        }


        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //engages the pruning switch
            turtle.PruneBranch = true;

        }//end execute



    }
}
