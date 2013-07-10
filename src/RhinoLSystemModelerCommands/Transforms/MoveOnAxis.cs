using System;



using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class MoveOnAxis : ModelerCommand {


        public override string CommandName() {
            return "MoveOnAxis";
        }

        public override int NumberOfArguments() {
            return 2;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 2) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}

            float axis = args[0];
            float dist = args[1];
            int axisIndex = (int)axis;
            double dif = axis - axisIndex;

            //was there a clean conversion?
            if (dif == 0) {

                switch (axisIndex) {
                    case 0:
                        turtle.MoveOnXAxis(dist);
                        break;
                    case 1:
                        turtle.MoveOnYAxis(dist);
                        break;
                    case 2:
                        turtle.MoveOnZAxis(dist);
                        break;
                    default:
                        throw new Exception("Invalid axis index: " + axisIndex + " , expecting 0, 1, or 2.");
                }//end switch

            } else {

                throw new Exception("Invalid axis specification: " + axis + " , expecting 0, 1, or 2.");

            }//end if/else
        }//end execute

    }
}
