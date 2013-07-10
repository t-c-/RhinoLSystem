using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
   public class Line : ModelerCommand {

        public override string CommandName() {
            return "Line";
        }

        public override int NumberOfArguments() {
            return 5;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            int argCnt = args.Length;


            float p_f = args[0];
            float d_f = args[1];
            float x = args[2];
            float y = args[3];
            float z = args[4];

            int d = (int)d_f;
            int p_index = (int)p_f;
            double dif = p_f - p_index;
            Plane linePlane = turtle.Plane;

            //was there a clean conversion?
            if (p_f - p_index == 0) {

                //get a copy of current position (about to move)
                Point3d sp = turtle.Position;

                switch (p_index) {
                    case 0:
                        //do nothing here, use turtle xy plane
                        break;
                    case 1:
                        linePlane = new Plane(linePlane.Origin, linePlane.YAxis, linePlane.ZAxis);
                        break;
                    case 2:
                        linePlane = new Plane(linePlane.Origin, linePlane.XAxis, linePlane.ZAxis);
                        break;
                    default:
                        throw new Exception("Invalid plane index: " + p_f + " in: " + CommandName() + " , expecting 0, 1, or 2.");
                }//end switch

                //get the end point
                Point3d ep = linePlane.PointAt(x,y,z);

                //build line
                //Rhino.Geometry.Line line = new Rhino.Geometry.Line(sp, ep);
                Rhino.Geometry.LineCurve line = new Rhino.Geometry.LineCurve(sp, ep);

                if (line != null) {

                    //put the curve on the curve stack
                    if (d == 0 || d == 2) {
                        turtle.DropCurve(line);
                    }

                    //add the curve to the document
                    if (d == 1 || d == 2) {
                        //create the polyline
                        if (document.Objects.AddCurve(line, turtle.Attributes) == Guid.Empty) {
                            throw new Exception("Failed to create line " + CommandName());
                        }
                    }

                } else {

                    throw new Exception("Failed to create line " + CommandName());
                }



                //add to doc
                //document.Objects.AddLine(line, turtle.Attributes);

            } else {

                throw new Exception("Invalid axis specification: " + p_f + " , expecting 0, 1, or 2.");

            }//end if/else

        }//end execute


    }
}
