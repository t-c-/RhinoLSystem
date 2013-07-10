using System;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class PopCurveStackToSwep : ModelerCommand {


        public override string CommandName() {
            return "PopCurveStackToSweep";
        }

        public override int NumberOfArguments() {
            return 0;
        }

        /// <summary>
        /// Creates A Sweep 1 or 2 Rail Surface.
        /// If two curves are on th stack, the first is used as the rail and the second (last) is used as the section curve.
        /// If three curves are provided, the cond is the scond rail and the last is the section curve.
        /// Any other number of curves will result in an error.
        /// </summary>
        /// <param name="turtle"></param>
        /// <param name="document"></param>
        /// <param name="args"></param>
        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 0) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}


            List<Curve> curves = turtle.PopCurveStack();
            int cCnt = curves.Count;

            if ( !(cCnt == 2 || cCnt == 3)) {
                throw new Exception("Invalid number of curves to create Sweep: " + cCnt + " provided, in: " + CommandName() + ", expecting 2 or 3");
            }

            Brep[] sweepSrfs = null;

            if (cCnt == 2) {
               sweepSrfs = Brep.CreateFromSweep( curves[0], curves[1], false, 0.001);
            } else if (cCnt == 3) {
               sweepSrfs = Brep.CreateFromSweep(curves[0], curves[1], curves[2] , false, 0.001);
            }

            
            

            if (sweepSrfs == null) {
                throw new Exception("Failed to create Sweep from curves in" + CommandName());
            }
            if (sweepSrfs.Length == 1) {
                //there should only be one surface
                document.Objects.AddBrep(sweepSrfs[0], turtle.Attributes);

            } else {
                throw new Exception("Failed to create loft from curves in" + CommandName());
            }


        }//end execute



    }
}
