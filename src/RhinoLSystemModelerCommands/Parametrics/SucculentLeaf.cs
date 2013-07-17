using System;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class SucculentLeaf : ModelerCommand {


        public override string CommandName() {
            return "SucculentLeaf";
        }

        public override int NumberOfArguments() {
            return 8;
        }


        /// <summary>
        /// Creates a bulbous leaf - inspired some from succulents on the front porch.
        /// </summary>
        /// <param name="turtle">turtle to act on</param>
        /// <param name="document">current Rhino document</param>
        /// <param name="args">command arguments: length/width/thickness/edge thickness factor/curl/cup/width bias/loft type</param>
        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


            float length = args[0];
            float width = args[1];
            float thickness = args[2];
            float edge_thickness = args[3];
            float curl = args[4];
            
            float cup = args[5];
            float width_bias = args[6];

            float lt_f = args[7];

            //make four curves and crete closed loft
            Rhino.Geometry.LoftType loft_type_e = ModelerCommand.LoftTypeFromArgument(lt_f, CommandName());



            Plane tPlane = Plane.WorldXY;
            double h_wid = width * 0.5;

            curl = ModelerCommand.FloatValueFromArgument(curl, -1, 1, CommandName() );

            double t_inc = RhinoMath.ToRadians(45 * curl);

            width_bias = ModelerCommand.FloatValueFromArgument(width_bias, 0.15f, 0.85f, CommandName() );

            cup = ModelerCommand.FloatValueFromArgument(cup, -1, 1, CommandName() );
            double c_inc = RhinoMath.ToRadians(60 * cup);

            double theta = 0;
            double dist = length / 4;
            double edge_t = thickness * edge_thickness;
            double x = 0;
            double y = 0;
            //build control curve
            Point3d origin = new Point3d(0, 0, 0);

            double len_per = 0.7;
            double len2 = 1 - len_per;
            double len1 = len_per * width_bias;
            double len3 = len_per - len1;


            //increment values
            theta += t_inc;
            x += length * len1 * Math.Cos(theta);
            y += length * len1 * Math.Sin(theta);

            //first control point
            Point3d cp1 = new Point3d(x, 0, y);
            //belly curve points inner/outer
            Point3d ib1 = PerpendicularPoint(cp1, thickness, theta);
            Point3d ob1 = PerpendicularPoint(cp1, -thickness, theta);
            //right/left points
            Point3d rp1 = cp1 + new Point3d(0, -h_wid, 0);
            Point3d lp1 = cp1 + new Point3d(0, h_wid, 0);

            Point3d rp1i = PerpendicularPoint(rp1, edge_t, theta);
            Point3d rp1o = PerpendicularPoint(rp1, -edge_t, theta);

            Point3d lp1i = PerpendicularPoint(lp1, edge_t, theta);
            Point3d lp1o = PerpendicularPoint(lp1, -edge_t, theta);

            theta += t_inc;
            x += length * len2 * Math.Cos(theta);
            y += length * len2 * Math.Sin(theta);

            Point3d cp2 = new Point3d(x, 0, y);
            //belly curve points inner/outer
            Point3d ib2 = PerpendicularPoint(cp2, thickness, theta);
            Point3d ob2 = PerpendicularPoint(cp2, -thickness, theta);
            //right/left points
            Point3d rp2 = cp2 + new Point3d(0, -h_wid, 0);
            Point3d lp2 = cp2 + new Point3d(0, h_wid, 0);


            Point3d rp2i = PerpendicularPoint(rp2, edge_t, theta);
            Point3d rp2o = PerpendicularPoint(rp2, -edge_t, theta);

            Point3d lp2i = PerpendicularPoint(lp2, edge_t, theta);
            Point3d lp2o = PerpendicularPoint(lp2, -edge_t, theta);


            theta += t_inc;
            x += length * len3 * Math.Cos(theta);
            y += length * len3 * Math.Sin(theta);

            Point3d end_point = new Point3d(x, 0, y);


            List<Point3d> points_oc = new List<Point3d>();
            List<Point3d> points_ic = new List<Point3d>();
            List<Point3d> points_re1 = new List<Point3d>();
            List<Point3d> points_re2 = new List<Point3d>();
            List<Point3d> points_le1 = new List<Point3d>();
            List<Point3d> points_le2 = new List<Point3d>();
            //outer curve
            points_oc.Add(origin);
            points_oc.Add(ob1);
            points_oc.Add(ob2);
            points_oc.Add(end_point);
            //inner curve
            points_ic.Add(origin);
            points_ic.Add(ib1);
            points_ic.Add(ib2);
            points_ic.Add(end_point);
            //right edge curves
            points_re1.Add(origin);
            points_re1.Add(rp1o);
            points_re1.Add(rp2o);
            points_re1.Add(end_point);

            points_re2.Add(origin);
            points_re2.Add(rp1i);
            points_re2.Add(rp2i);
            points_re2.Add(end_point);

            //left edge curve
            points_le1.Add(origin);
            points_le1.Add(lp1o);
            points_le1.Add(lp2o);
            points_le1.Add(end_point);

            points_le2.Add(origin);
            points_le2.Add(lp1i);
            points_le2.Add(lp2i);
            points_le2.Add(end_point);


            //create curves
            Curve oc_crv = Curve.CreateInterpolatedCurve(points_oc, 3);
            Curve ic_crv = Curve.CreateInterpolatedCurve(points_ic, 3);

            Curve re1_crv = Curve.CreateInterpolatedCurve(points_re1, 3);
            Curve re2_crv = Curve.CreateInterpolatedCurve(points_re2, 3);

            Curve le1_crv = Curve.CreateInterpolatedCurve(points_le1, 3);
            Curve le2_crv = Curve.CreateInterpolatedCurve(points_le2, 3);

            //cup the edges by rotating edge curves
            Vector3d r_axis = new Vector3d(end_point);
            r_axis.Unitize();
            Transform trans = Transform.Rotation(-c_inc, r_axis, origin);

            re1_crv.Transform(trans);
            re2_crv.Transform(trans);

            trans = Transform.Rotation(c_inc, r_axis, origin);

            le1_crv.Transform(trans);
            le2_crv.Transform(trans);

            List<Curve> curves = new List<Curve>();

            curves.Add(oc_crv);
            curves.Add(re1_crv);
            curves.Add(re2_crv);
            curves.Add(ic_crv);
            curves.Add(le2_crv);
            curves.Add(le1_crv);

            Brep[] breps = Brep.CreateFromLoft(curves, Point3d.Unset, Point3d.Unset, loft_type_e, true);

            if (breps != null) {
                if (breps.Length == 1) {

                    Brep sl = breps[0];
                    sl.Transform(Transform.PlaneToPlane(Plane.WorldXY, turtle.Plane));

                    if (document.Objects.AddBrep(breps[0], turtle.Attributes) == Guid.Empty) {
                        throw new Exception("Failed to Create Brep object in: " + CommandName());
                    }
                } else {
                    throw new Exception("Too many surfaces Loft geometry in: " + CommandName());
                }

            } else {
                throw new Exception("Failed to Create Loft geometry in: " + CommandName());
            }


        }//end execute



        /// <summary>
        /// Creates a point perpendicular to provided point in world xz plane.
        /// </summary>
        /// <param name="point">base point</param>
        /// <param name="dist">distance from base</param>
        /// <param name="ang">base angle - perpendicular to this angle</param>
        /// <returns></returns>
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
