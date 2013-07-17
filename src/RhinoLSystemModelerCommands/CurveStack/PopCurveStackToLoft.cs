using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class PopCurveStackToLoft : ModelerCommand {

       public override string CommandName() {
           return "PopCurveStackToLoft";
       }

       public override int NumberOfArguments() {
           return 2;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


           float loft_t_f = args[0];
           float norm_s = args[1];

           Rhino.Geometry.LoftType loft_type = ModelerCommand.LoftTypeFromArgument(loft_t_f, CommandName());
           bool normalize_seam = ModelerCommand.BooleanValueFromArgument(norm_s, CommandName());


           List<Curve> curves = turtle.PopCurveStack();

           if (normalize_seam) {
               curves = NormalizeSeam(curves);
           }



           if (curves.Count < 2) {

               //collapse gracefully...
               return;
               throw new Exception("Not enough curves to create Loft: " + curves.Count + " provided, in: " + CommandName());
           }

           Brep[] loftSrfs = Brep.CreateFromLoft(curves, Point3d.Unset, Point3d.Unset, loft_type, false);

           if (loftSrfs != null) {
               if (loftSrfs.Length == 1) {

                   if (document.Objects.AddBrep(loftSrfs[0], turtle.Attributes) == Guid.Empty) {
                       throw new Exception("Failed to create loft object in" + CommandName());
                   }

               } else {

                   throw new Exception("Failed to create loft from curves in" + CommandName());
               }
           } else {

               throw new Exception("Failed to create loft from curves in" + CommandName());
           }
 


       }//end execute



        #region "Normalize Seam"



       private List<Curve> NormalizeSeam(List<Curve> loft_curves) {


           //List<Point3d> ints = new List<Point3d>();

           List<Point3d> centers = new List<Point3d>();
           List<Plane> planes = new List<Plane>();

           //find all centers and hold
           foreach (Curve c in loft_curves) {

               Plane curvePlane;
               if (!c.TryGetPlane(out curvePlane)) {

                   //Plane norm;
                   BoundingBox cbb = c.GetBoundingBox(false);
                   centers.Add(cbb.Center);

                   curvePlane = new Plane(cbb.Center, Vector3d.ZAxis);

                   //attempt best fit...
                   double[] t = c.DivideByCount(10, true);
                   List<Point3d> sp = new List<Point3d>();
                   foreach (double tv in t) {
                       sp.Add(c.PointAt(tv));
                   }

                   Plane fitPlane;
                   PlaneFitResult pfr = Plane.FitPlaneToPoints(sp, out fitPlane);
                   if (pfr == PlaneFitResult.Success) {
                       curvePlane = fitPlane;
                       curvePlane.Origin = cbb.Center;
                   } else {
                       //System.Diagnostics.Debug.Print("Fit Plane Fail!");

                   }


               } else {

                   //find the center of the curve and move origin of plane
                   BoundingBox t_bb = c.GetBoundingBox(curvePlane);
                   if (t_bb.IsValid) {
                       t_bb.Transform(Transform.PlaneToPlane(Plane.WorldXY, curvePlane));
                       curvePlane.Origin = t_bb.Center;
                   }


                   centers.Add(curvePlane.Origin);
               }

               planes.Add(curvePlane);

           }//end foreach


           Curve axialCurve = null;
           //A = planes;
           int p_cnt = centers.Count;

           //0 or 1 curves, no dice...
           if (p_cnt == 0 || p_cnt ==1) {
               return loft_curves;
           }

           if (p_cnt == 2) {
               //for two points make a line
               axialCurve = new LineCurve(centers[0], centers[1]);
           } else if (p_cnt == 3) {

               Rhino.Geometry.Arc arc = new Rhino.Geometry.Arc(centers[0], centers[1], centers[2]);
               axialCurve = new ArcCurve(arc);

           } else {

               //build interpolated curve from centers
               axialCurve = Curve.CreateInterpolatedCurve(centers, 3);

           }

           //created alignment curve?
           if (axialCurve == null) {
               return loft_curves;
           }

           //proceed...

           //init list for replacement curves
           List<Curve> final_curves = new List<Curve>();

           int cCnt = loft_curves.Count;

           //normalize curve seam
           for (int i = 0; i < cCnt; i++) {


               Curve c = loft_curves[i];
               Plane p = planes[i];
               Point3d cen = centers[i];

               double t;
               bool cp = axialCurve.ClosestPoint(cen, out t);
               Plane ctPlane;
               //this needs check/handling
               axialCurve.PerpendicularFrameAt(t, out ctPlane);

               //pretend it's ok...
               if (!ctPlane.IsValid) {
                   ctPlane = p;
                   //System.Diagnostics.Debug.Print("Invalid curve perp Frame");
               }

               ctPlane.Transform(Transform.Rotation(Math.PI * 0.5, ctPlane.XAxis, ctPlane.Origin));

               //alPlanes.Add(ctPlane);


               //now that we have fit plane, adjust seam
               Rhino.Geometry.Intersect.CurveIntersections c_ints = Rhino.Geometry.Intersect.Intersection.CurvePlane(c, ctPlane, 0.01);
               //should be two points...
               //grab local x axis
               Vector3d l_x = ctPlane.XAxis;
               //set artificially high for first pass
               double theta = 100000;
               Point3d seamPoint = new Point3d();
               double seam_t = 0;



               foreach (Rhino.Geometry.Intersect.IntersectionEvent ie in c_ints) {

                   Point3d ip_a = ie.PointA;
                   Vector3d va = ip_a - ctPlane.Origin;
                   va.Unitize();

                   //look for vector with smallest angle to x-axis
                   double v_angle = Vector3d.VectorAngle(l_x, va);

                   //if this is more closely aligned with x axis - use it
                   if (v_angle < theta) {
                       theta = v_angle;
                       seam_t = ie.ParameterA;
                       seamPoint = ie.PointA;
                   }


               }//end foreach


               //System.Diagnostics.Debug.Print("seam = " + seam_t);
               //now that we have the seam...
               //need a rebuild?...
               if (seam_t != 0) {


                   //attempt to adjust
                   if (!c.ChangeClosedCurveSeam(seam_t)) {
                      // System.Diagnostics.Debug.Print("seam failure!  adding original...");
                   } else {
                       //System.Diagnostics.Debug.Print("adding adjusted...");
                   }

                   final_curves.Add(c);


               } else {

                   //System.Diagnostics.Debug.Print("adding original...");
                   final_curves.Add(c);

               }//end if rebuild


           }//end for

           //curves all gathered...
           if (final_curves.Count < 2) {
               System.Diagnostics.Debug.Print("not enough curves..");
           }

           return final_curves;

       }






        #endregion






    }//end class
}
