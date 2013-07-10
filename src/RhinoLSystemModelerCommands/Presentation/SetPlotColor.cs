using System;

using Rhino;

namespace RhinoLSystem.Commands {
    public class SetPlotColor : ModelerCommand {


        public override string CommandName() {
            return "SetPlotColor";
        }

        public override int NumberOfArguments() {
            return 3;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 3) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}


            double dr = args[0];
            double dg = args[1];
            double db = args[2];

            int r = (int)dr;
            int g = (int)dg;
            int b = (int)db;

            if (dr - r != 0 || r < 0 || r > 255) {
                throw new Exception("Invalid red value of: " + dr + " in : " + CommandName());
            }
            if (dg - g != 0 || g < 0 || g > 255) {
                throw new Exception("Invalid green value of: " + dg + " in : " + CommandName());
            }
            if (db - b != 0 || b < 0 || b > 255) {
                throw new Exception("Invalid blue value of: " + db + " in : " + CommandName());
            }

            //set the display color
            turtle.PlotColor = System.Drawing.Color.FromArgb(r, g, b);


        }//end execute


    }
}

