using System;
using System.Collections.Generic;



using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
    public class Polygon : ModelerCommand  {


         public override string CommandName() {
             return "Polygon";
         }

         public override int NumberOfArguments() {
             return 4;
         }


         public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


             float p_f = args[0];
             float d_f = args[1];
             double radius = args[2];
             int sides = (int)args[3];
             int dest = (int)d_f;

             //gets plane or throws error
             Plane tPlane = ModelerCommand.GetPlaneFromArgument(turtle, p_f, CommandName());
            

             double theta = (Math.PI * 2) / sides;
             List<Point3d> points = new List<Point3d>();
             double px,  py;
             Point3d pp;

             for (int i = 0; i < sides; i++) {

                 px = radius * Math.Cos(i * theta);
                 py = radius * Math.Sin(i * theta);
                 pp = tPlane.PointAt(px, py);
                 points.Add(pp);

             }//end for

             //Close the polyline by reusing first point
             Point3d close = points[0];
             points.Add(close);

             PolylineCurve pc = new PolylineCurve(points);

             if (pc != null) {
                 if (dest == 0 || dest == 2) {
                     turtle.DropCurve(pc);
                 }

                 if (dest == 1 || dest == 2) {
                     if (document.Objects.AddCurve(pc, turtle.Attributes) == Guid.Empty) {
                         throw new Exception("Failed to create polygon in: " + CommandName());
                     }
                 }
             } else {

                 throw new Exception("Failed to create polygon in: " + CommandName());

             }


             

         }//end execute

    }

}//end namespace