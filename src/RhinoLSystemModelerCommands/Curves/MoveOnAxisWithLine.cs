using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
   public class MoveOnAxisWithLine : ModelerCommand {

        public override string CommandName() {
            return "MoveOnAxisWithLine";
        }

        public override int NumberOfArguments() {
            return 2;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            int argCnt = args.Length;

            float axis = args[0];
            float dist = args[1];
            int axisIndex = (int)axis;
            double dif = axis - axisIndex;

            //was there a clean conversion?
            if (dif == 0) {

                //get a copy of current position (about to move)
                Point3d sp = turtle.Position;

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
                        throw new Exception("Invalid axis index: " + axisIndex + " in: " + CommandName() + " , expecting 0, 1, or 2.");
                }//end switch

                //get the end point
                Point3d ep = turtle.Position;


                //build line
                Rhino.Geometry.Line line = new Rhino.Geometry.Line(sp, ep);
                //add to doc
                document.Objects.AddLine(line, turtle.Attributes);

            } else {

                throw new Exception("Invalid axis specification: " + axis + " , expecting 0, 1, or 2.");

            }//end if/else

        }//end execute


    }
}
