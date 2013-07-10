using System;


using Rhino;
using Rhino.Geometry;

using RhinoLSystem;


namespace RhinoLSystem.Commands {
    public class Ellipse : ModelerCommand {


        public override string CommandName() {
            return "Ellipse";
        }

        public override int NumberOfArguments() {
            return 4;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


            float planeF = args[0];
            float d_f = args[1];
            float r_max = args[2];
            float r_min = args[3];

            int d = (int)d_f;

            //this will throw an error if incorrect
            Plane tPlane = ModelerCommand.GetPlaneFromArgument(turtle, planeF, CommandName());


            //build the ellipse
            Rhino.Geometry.Ellipse ellipse = new Rhino.Geometry.Ellipse(tPlane, r_max, r_min);
            Curve ec = ellipse.ToNurbsCurve();

            if (ec != null) {

                //put the curve on the curve stack
                if (d == 0 || d == 2) {
                    turtle.DropCurve(ec);
                }

                //add the curve to the document
                if (d == 1 || d == 2) {
                    //create the polyline
                    if (document.Objects.AddCurve(ec, turtle.Attributes) == Guid.Empty) {
                        throw new Exception("Failed to create circle curve " + CommandName());
                    }
                }

            } else {

                throw new Exception("Failed to create circle curve " + CommandName());
            }




        }//end execute


    }//end class
}

