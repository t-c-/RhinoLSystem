using System;


using Rhino;

namespace RhinoLSystem.Commands {
    public class SetLineWidth : ModelerCommand {

        public override string CommandName() {
            return "SetLineWidth";
        }

        public override int NumberOfArguments() {
            return 1;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 1) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}


            double lw = args[0];

            //make sure greater than or equal to zero, zero = non-plot
            if (lw < 0 ) {
                throw new Exception("Invliad Line Width of: " + lw + " in: " + CommandName());
            }

            //set the plot line width
            turtle.PlotLineWidth = lw;

        }//end execute

    }//end classs
}
