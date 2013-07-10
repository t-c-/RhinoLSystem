using System;
using System.Collections.Generic;



using RhinoLSystem;
using Rhino.Geometry;



namespace RhinoLSystem.Commands {
    public class PopCurveStackToExtrude : ModelerCommand {


        public override string CommandName() {
            return "PopCurveStackToExtrude";
        }

        public override int NumberOfArguments() {
            return 2;
        }

        /// <summary>
        /// Takes a single Curve off of the Curve stack and extrudes it.
        /// Expecting two arguments:
        /// extrusion height = float
        /// Cap = on or off (1 or 0)
        /// </summary>
        /// <param name="turtle"></param>
        /// <param name="document"></param>
        /// <param name="args"></param>
        public override void Execute(RhinoTurtle turtle, Rhino.RhinoDoc document, float[] args) {

            int argCnt = args.Length;


            float ext_height = args[0];
            float cap_f = args[1];

            int cap = (int)cap_f;
            bool create_cap = false;

            if (cap_f - cap != 0 || !(cap == 0 || cap == 1)) {
                throw new Exception("Invalid cap specification of: " + cap_f + " in: " + CommandName() + " expecting 1 or 0");
            } else if (cap == 1) {
                create_cap = true;
            }




            List<Curve> curves = turtle.PopCurveStack();

            if (curves.Count != 1) {
                throw new Exception("Invalid number of curves for extrude: " + curves.Count + " in: " + CommandName() + " expecting 1.");
            }


            Rhino.Geometry.Extrusion extr = Rhino.Geometry.Extrusion.Create(curves[0], ext_height, create_cap);

            if (extr != null) {

                if (document.Objects.AddExtrusion(extr, turtle.Attributes) == Guid.Empty) {
                    throw new Exception("Failed to create extrusion object in: " + CommandName());
                }


            } else {
                throw new Exception("Failed to create extrusion geometry in: " + CommandName());
            }


        }//end execute



    }
}

