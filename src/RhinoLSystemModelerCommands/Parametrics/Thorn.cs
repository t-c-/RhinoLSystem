using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;



namespace RhinoLSystem.Commands {
    public class Thorn : ModelerCommand {

        public override string CommandName() {
            return "Thorn";
        }

        public override int NumberOfArguments() {
            return 7;
        }

        public override void Execute(RhinoTurtle turtle, Rhino.RhinoDoc document, float[] args) {

            double length = args[0];
            double width = args[1];
            double height = args[2];

            double height_bias = args[3];
            double width_bias = args[4];
            double curl = args[5];

            float loft_type = args[6];

            //make four curves and crete closed loft
            Rhino.Geometry.LoftType loft_type_e = ModelerCommand.LoftTypeFromArgument( loft_type, CommandName() );



            curl = Constrain(curl, -1, 1);
            height_bias = Constrain(height_bias, 0.15, 1);
            width_bias = Constrain(width_bias, 0.15, 1);

            double t_inc = RhinoMath.ToRadians(45 * curl);


            double theta = 0;
            double dist = length / 3;
            //double edge_t = thickness * edge_thickness;
            double x = 0;
            double y = 0;
            //build control curve
            Point3d origin = new Point3d(0, 0, 0);


            // double h_wid = width * 0.5;
            double w_inc = width / 3;
            double w1 = w_inc * 3 * width_bias;
            double w2 = w_inc * 2 * width_bias;


            double h_inc = height / 3;
            double h1 = h_inc * 3 * height_bias;
            double h2 = h_inc * 2 * height_bias;

            //increment values
            theta += t_inc;
            x += dist * Math.Cos(theta);
            y += dist * Math.Sin(theta);

            //first control point
            Point3d cp1 = new Point3d(x, 0, y);
            //belly curve points inner/outer
            Point3d ib1 = PerpendicularPoint(cp1, h1, theta);
            Point3d ob1 = PerpendicularPoint(cp1, -h1, theta);
            //right/left points
            Point3d rp1 = cp1 + new Point3d(0, -w1, 0);
            Point3d lp1 = cp1 + new Point3d(0, w1, 0);



            theta += t_inc;
            x += dist * Math.Cos(theta);
            y += dist * Math.Sin(theta);

            Point3d cp2 = new Point3d(x, 0, y);
            //belly curve points inner/outer
            Point3d ib2 = PerpendicularPoint(cp2, h2, theta);
            Point3d ob2 = PerpendicularPoint(cp2, -h2, theta);
            //right/left points
            Point3d rp2 = cp2 + new Point3d(0, -w2, 0);
            Point3d lp2 = cp2 + new Point3d(0, w2, 0);




            theta += t_inc;
            x += dist * Math.Cos(theta);
            y += dist * Math.Sin(theta);

            Point3d end_point = new Point3d(x, 0, y);


            List<Point3d> points_oc = new List<Point3d>();
            List<Point3d> points_ic = new List<Point3d>();
            List<Point3d> points_re1 = new List<Point3d>();
            //List<Point3d> points_re2 = new List<Point3d>();
            List<Point3d> points_le1 = new List<Point3d>();
            //List<Point3d> points_le2 = new List<Point3d>();
            //outer curve
            points_oc.Add(origin + new Point3d(0, 0, -height));
            points_oc.Add(ob1);
            points_oc.Add(ob2);
            points_oc.Add(end_point);

            //inner curve
            points_ic.Add(origin + new Point3d(0, 0, height));
            points_ic.Add(ib1);
            points_ic.Add(ib2);
            points_ic.Add(end_point);

            //right edge curves
            points_re1.Add(origin + new Point3d(0, -width, 0));
            points_re1.Add(rp1);
            points_re1.Add(rp2);
            points_re1.Add(end_point);

            //left edge curve
            points_le1.Add(origin + new Point3d(0, width, 0));
            points_le1.Add(lp1);
            points_le1.Add(lp2);
            points_le1.Add(end_point);




            //create curves
            Curve oc_crv = Curve.CreateInterpolatedCurve(points_oc, 3);
            Curve ic_crv = Curve.CreateInterpolatedCurve(points_ic, 3);

            Curve re1_crv = Curve.CreateInterpolatedCurve(points_re1, 3);

            Curve le1_crv = Curve.CreateInterpolatedCurve(points_le1, 3);



            List<Curve> curves = new List<Curve>();

            curves.Add(oc_crv);
            curves.Add(re1_crv);

            curves.Add(ic_crv);

            curves.Add(le1_crv);

            Brep[] breps = Brep.CreateFromLoft(curves, Point3d.Unset, Point3d.Unset, loft_type_e, true);

            if (breps != null) {

                if (breps.Length == 1) {

                    Plane w_xy_plane = Plane.WorldXY;
                    Brep l_brep = breps[0];
                    l_brep.Transform(Transform.PlaneToPlane(w_xy_plane, turtle.Plane));

                    if (document.Objects.AddBrep(breps[0], turtle.Attributes) == Guid.Empty) {
                        throw new Exception("Failed to Create Brep object in: " + CommandName());
                    }
                } else {

                    throw new Exception("Too many surfaces Loft geometry in: " + CommandName());

                }


            } else {

                throw new Exception("Failed to Create Loft geometry in: " + CommandName());
            }
            //L1 = breps[0];





        }//end execute


        /// <summary>
        /// Helper function to constrain values - part of the checking process.
        /// </summary>
        /// <param name="val">value to constrain</param>
        /// <param name="min">minimum allowed value</param>
        /// <param name="max">maximum allowed value</param>
        /// <returns></returns>
        private double Constrain(double val, double min, double max) {

            if (val > max) {
                return max;
            }

            if (val < min) {
                return min;
            }

            return val;

        }

        /// <summary>
        /// Helper function for the building of profile curves,.
        /// </summary>
        /// <param name="point">base point</param>
        /// <param name="dist">distance from base point</param>
        /// <param name="ang">base angle in XZ Plane</param>
        /// <returns>Point perpendicular to base point in XZ Plane</returns>
        private Point3d PerpendicularPoint(Point3d point, double dist, double ang) {

            Point3d rPoint = new Point3d();
            double hpi = Math.PI * 0.5;
            rPoint.X = dist * Math.Cos(ang + hpi);
            rPoint.Z = dist * Math.Sin(ang + hpi);

            rPoint += point;

            return rPoint;

        }



    }//end class
}
