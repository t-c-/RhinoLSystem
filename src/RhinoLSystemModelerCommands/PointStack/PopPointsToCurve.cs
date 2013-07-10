using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class PopPointsToCurve : ModelerCommand {


       public override string CommandName() {
           return "PopPointsToCurve";
       }

       public override int NumberOfArguments() {
           return 2;
       }

        /// <summary>
        /// Creates a Curve form the current points in the point list.
       /// Destination for the curve is specified by passing an integer from 0-2
       /// 0 = Drop Curve onto current curve stack
       /// 1 = Create Cruve in Document
       /// 2 = Perform both actions
        /// </summary>
        /// <param name="turtle"></param>
        /// <param name="document"></param>
        /// <param name="args"></param>
       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //if (args.Length != 2) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}

           //resolve the destination for the curve
           float d_f = args[0];
           float close_f = args[1];

           int d = (int)d_f;
           int closeCurve = (int)close_f;
           float tol = 0.01F;


           if (d_f - d != 0 || d < 0 || d > 2) {
               throw new Exception("Invalid destination code: " + d_f + " in: " + CommandName() + " ,expecting 0, 1, or 2");
           }

           if (close_f - closeCurve != 0 || !(closeCurve == 1 || closeCurve == 0)) {
               throw new Exception("Invalid close curve value: " + close_f + " in: " + CommandName() + " , expecting 0 or 1");
           }


           //pop the points
           List<Point3d> points = turtle.PopPointStack();

           int pCnt = points.Count;
           Curve curve = null;


           if (pCnt < 2) {
               throw new Exception("Too Few Points (" + pCnt + ") in Point Stack calling: " + CommandName());
           }

           if (closeCurve == 1 && pCnt < 3) {
               throw new Exception("Too few points in point stack to close curve in: " + CommandName() + " , need at least 3 points");
           }

           if (pCnt == 2) {
               //for two points make a line
               curve = new LineCurve(points[0], points[1]);
 

           } else if (pCnt == 3) {
               //if only three points, make 3 point arc
               Rhino.Geometry.Arc arc = new Rhino.Geometry.Arc(points[0], points[1], points[2]);
               
               if (arc.IsValid) {
                   if (closeCurve == 1) {
                       Rhino.Geometry.Circle circle = new Rhino.Geometry.Circle(arc);
                       curve = new ArcCurve(circle);
                   } else {
                       curve = new ArcCurve(arc);
                   }
               } else {

                   throw new Exception("Failed to create three point arc in " + CommandName());

               }

           } else {

               if (closeCurve == 1) {


                   Point3d sp = points[0];
                   Point3d ep = points[pCnt - 1];

                   if (sp.DistanceTo(ep) > tol) {
                       points.Add(sp);
                   }

                   curve = Curve.CreateInterpolatedCurve(points, 3);
                   if (!curve.MakeClosed(tol)) {
                       throw new Exception("Failed to create closed interpolated curve" + CommandName());
                   }

               } else {

                   //create interpolated curve
                   curve = Curve.CreateInterpolatedCurve(points, 3);

               }


               if (curve == null) {
                   throw new Exception("Failed to create interpolated curve" + CommandName());
               } 
           }//end if /else 3 points


           if (curve != null) {
               //put the curve on the curve stack
               if (d== 0 || d== 2) {
                   turtle.DropCurve(curve);
               }

               //add the curve to the document
               if (d==1 || d== 2) {
                   if (document.Objects.AddCurve(curve, turtle.Attributes) == Guid.Empty) {
                       RhinoApp.WriteLine("Failed to create interpolated curve " + CommandName());
                   }
               }
  

           } else {
               throw new Exception("Failed to create curve in: " + CommandName() );
           }



       }//end Execute


    }//end class
}
