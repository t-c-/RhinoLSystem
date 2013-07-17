using System;
using System.Collections.Generic;



using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
    public class Petal : ModelerCommand {

        public override string CommandName() {
            return "Petal";
        }

        public override int NumberOfArguments() {
            return 5;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

 
            double l_length = args[0];
            double l_width = args[1];
            double l_width_bias = args[2];
            double l_curl = args[3];
            int l_detail = (int)args[4];

            double w_bias = l_width_bias;
            //check bias range
            w_bias = Math.Min(l_width_bias, 1.0);
            w_bias = Math.Max(w_bias, 0);


            //leaf plane
            Plane l_plane = turtle.Plane;


            Point3d sp, ep, ct_p;
            //start point
            sp = new Point3d(0, 0, 0);
            //end point
            ep = new Point3d(l_length, 0, 0);
            //control point
            ct_p = new Point3d(l_length * w_bias, l_width,0);


            List<Point3d> points = new List<Point3d>();
            List<Point3d> left_pp = new List<Point3d>();
            List<Point3d> right_pp = new List<Point3d>();


            points.Add(sp); points.Add(ct_p); points.Add(ep);

            //Curve profile = Curve.CreateInterpolatedCurve(points, 2);
            Curve profile = Curve.CreateControlPointCurve(points);
            //Curve center = new LineCurve(sp, ep);


            int c_detail = Math.Max(3, l_detail);

            double c_inc = l_curl / c_detail;
            double l_inc = l_length / c_detail;
            double x, y, cx, cy;
            double theta = c_inc;

            x = 0;
            y = 0;

            points.Clear();


            double[] t_vals = profile.DivideByCount(c_detail, true);
      
            double curD = 0;
            double lastX = 0;
            int tvMax = t_vals.Length;

            for (int i = 0; i < tvMax; i++) {


                Point3d pp = profile.PointAt(t_vals[i]);

                double px = pp.X;
                curD = px - lastX;
                lastX = px;


                cx = curD * Math.Cos(theta);
                cy = curD * Math.Sin(theta);

                theta += i * c_inc;
                //theta += c_inc;

                x += cx;
                y += cy;

                Point3d lp = l_plane.PointAt(x, pp.Y, y);
                Point3d rp = l_plane.PointAt(x, -pp.Y, y);

                left_pp.Add(lp);
                right_pp.Add(rp);

                //left_pp.Add(new Point3d(x, pp.Y, y));
                //right_pp.Add(new Point3d(x, -pp.Y, y));


                Point3d cp = l_plane.PointAt(x, 0, y);

                points.Add(cp);

                //points.Add(new Point3d(x, 0, y));
  


            }//end for


            Curve c_curve = Curve.CreateInterpolatedCurve(points, 3);
            Curve l_curve = Curve.CreateInterpolatedCurve(left_pp, 3);
            Curve r_curve = Curve.CreateInterpolatedCurve(right_pp, 3);


            List<Curve> curves = new List<Curve>();

            curves.Add(l_curve); curves.Add(c_curve); curves.Add(r_curve);


            Brep[] srf = Brep.CreateFromLoft(curves, Point3d.Unset, Point3d.Unset, LoftType.Normal, false);

            if (srf.Length == 1) {

                document.Objects.Add(srf[0], turtle.Attributes);

            }

            //cleanup...
            curves.Clear();
            curves = null;

            c_curve = null;
            l_curve = null;
            r_curve = null;


            points.Clear();
            points = null;
            left_pp.Clear();
            left_pp = null;
            right_pp.Clear();
            right_pp = null;

        }// end execute

    }
}
