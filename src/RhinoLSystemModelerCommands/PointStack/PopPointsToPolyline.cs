using System;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class PopPointsToPolyline : ModelerCommand {


       public override string CommandName() {
           return "PopPointsToPolyline";
       }

       public override int NumberOfArguments() {
           return 2;
       }

        /// <summary>
        /// Creates a Polyline from points in current point list.
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
           int c = (int)close_f;
           float tol = 0.001F;

           if (d_f - d != 0 || d < 0 || d > 2) {
               throw new Exception("Invalid destination code: " + d_f + " in: " + CommandName() + " ,expecting 0, 1, or 2");
           }


           if (close_f - c != 0 || !(c == 1 || c == 0)) {
               throw new Exception("Invalid close curve value: " + close_f + " in: " + CommandName() + " , expecting 0 or 1");
           }

           

           //pop the points
           List<Point3d> points = turtle.PopPointStack();

           int pCnt = points.Count;
           Polyline polyline;

           if (pCnt < 2) {
               throw new Exception("Too Few Points (" + pCnt + ") in Point Stack calling: " + CommandName());
           }

           if (c == 1) {
               if (pCnt < 3) {
                   throw new Exception("Too few points in point stack to close curve in: " + CommandName() + " , need at least 3 points");
               }
               
               Point3d sp = points[0];
               Point3d ep = points[pCnt - 1];

               if (sp.DistanceTo(ep) > tol) {
                   points.Add(sp);
               }

               polyline = new Polyline(points);
              // polyline.
               
               
           } else {

               polyline = new Polyline(points);

           }



           if (polyline != null) {

               PolylineCurve plc = new PolylineCurve(polyline);
               //try to close curve
               if (c == 0) {
                   if (!plc.IsClosed) {
                       if (!plc.MakeClosed(tol)) {

                       }
                   }
               }

               if (plc != null) {

                   //put the curve on the curve stack
                   if (d == 0 || d == 2) {
                       turtle.DropCurve(plc);
                   }

                   //add the curve to the document
                   if (d == 1 || d == 2) {
                       //create the polyline
                       if (document.Objects.AddCurve(plc, turtle.Attributes) == Guid.Empty) {
                           throw new Exception("Failed to create Polyline Object " + CommandName());
                       }
                   }
               } else {
                   throw new Exception("Failed to build PolylineCurve in: " + CommandName());
               }



           } else {
               throw new Exception("Failed to create Polyline in: " + CommandName());
           }

       }//end Execute


    }//end class
}
