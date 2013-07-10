using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class StraightTube : ModelerCommand {


        public override string CommandName() {
            return "StraightTube";
        }

        public override int NumberOfArguments() {
            return 4;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


            //if (args.Length != 4) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}

            double minRadius = 0.001;
            double endRadius = 1;
            double radius = args[0];
            double length = args[1];
            double radiusFactor = args[2];
            int cap = (int)args[3];
            bool createCap = false;

            if (cap != 1 && cap != 0) {
                throw new Exception("Invalid boolean value for cap in : " + CommandName());
            } 
            
            if (cap == 1) {
                createCap = true;
            }

            if (radius < minRadius) {
                radius = minRadius;
            }

            if (radiusFactor == 0) {
                radiusFactor = 0.00001;
            }


            endRadius = radius * radiusFactor;

            //orient plane along heading
            Plane turtlePlane = new Plane(turtle.Plane.Origin, turtle.Plane.YAxis, turtle.Plane.ZAxis);

            Rhino.Geometry.Circle startCircle = new Rhino.Geometry.Circle(turtlePlane, radius);
            Plane endPlane = turtlePlane;
            endPlane.Origin = endPlane.PointAt(0, 0, length);
            Rhino.Geometry.Circle endCircle = new Rhino.Geometry.Circle(endPlane, endRadius);

            List<Curve> curves = new List<Curve>();
            curves.Add(new ArcCurve(startCircle));
            curves.Add(new ArcCurve(endCircle));

            Brep[] lofts = Brep.CreateFromLoft(curves, Point3d.Unset, Point3d.Unset, LoftType.Normal, false);

            if (lofts.Length == 1) {
                document.Objects.AddBrep(lofts[0], turtle.Attributes);
            }


            if (createCap) {
                Point3d ras = endPlane.PointAt(-endRadius, 0, 0);
                Point3d rae = endPlane.PointAt(endRadius, 0, 0);
                Rhino.Geometry.Line revLine = new Rhino.Geometry.Line(ras, rae);
                Rhino.Geometry.Arc arc = new Rhino.Geometry.Arc(endCircle, Math.PI);
                RevSurface rev = RevSurface.Create(new ArcCurve(arc), revLine, 0, Math.PI);
                Brep bCap =  Brep.CreateFromRevSurface(rev, false, false);

                if (bCap != null) {
                    document.Objects.Add(bCap, turtle.Attributes);
                }

            }//end cap

        }//end execute


    }
}
