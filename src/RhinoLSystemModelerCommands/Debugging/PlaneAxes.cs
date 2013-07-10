using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
    public class PlaneAxes : ModelerCommand {


        public override string CommandName() {
            return "PlaneAxes";
        }

        public override int NumberOfArguments() {
            return 1;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            int argLen = args.Length;

            //0 or 1 arguments
            //if (argLen != 1  ) {
            //    throw new Exception("Invalid number of args in : " + CommandName() +  " , expecting 1");
            //}

            float vLen = args[0];

            if (vLen == 0.0f) {
                throw new Exception("Invalid zero length vector in : " + CommandName());
            }

            Vector3d xAxis = turtle.Plane.XAxis;
            Vector3d yAxis = turtle.Plane.YAxis;
            Vector3d zAxis = turtle.Plane.ZAxis;

            Point3d origin = turtle.Plane.Origin;
            Point3d sp, ep;


            //grab a copy of the current turtle attributes since it's being modified
            Rhino.DocObjects.ObjectAttributes atts = turtle.Attributes.Duplicate();
            atts.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
            atts.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.EndArrowhead;


            //set origin
            sp = turtle.Plane.Origin;

            //xAxis
            ep = sp + new Point3d ( xAxis * vLen);
            atts.ObjectColor = System.Drawing.Color.Red;
            document.Objects.AddLine(sp, ep, atts);


            //yaxis
            ep = sp + new Point3d(yAxis * vLen);
            atts.ObjectColor = System.Drawing.Color.Green;
            document.Objects.AddLine(sp, ep, atts);

            //zaxis
            ep = sp + new Point3d(zAxis * vLen);
            atts.ObjectColor = System.Drawing.Color.Blue;
            document.Objects.AddLine(sp, ep, atts);



        }//end Execute


    }//end class
}
