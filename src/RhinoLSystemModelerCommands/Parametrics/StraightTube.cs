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


            double minRadius = 0.001;
            double endRadius = 1;
            double radius = args[0];
            double length = args[1];
            double radiusFactor = args[2];
            int cap = (int)args[3];
            bool createCap = false;

            createCap = ModelerCommand.BooleanValueFromArgument(cap, CommandName());

            if (radius < minRadius) {
                radius = minRadius;
            }

            if (radiusFactor == 0) {
                radiusFactor = 0.00001;
            }


            endRadius = radius * radiusFactor;

            Point3d sp = new Point3d(0, radius, 0);
            Point3d ep = new Point3d(length, endRadius, 0);
            Curve profile = new LineCurve(sp, ep);

            if (createCap) {

                Point3d a_cen = new Point3d(length , 0, 0);
                Rhino.Geometry.Arc arc = new Rhino.Geometry.Arc(a_cen, endRadius, Math.PI * 0.5);
                Rhino.Geometry.ArcCurve ac = new ArcCurve(arc);

                List<Curve> j_list = new List<Curve>();
                j_list.Add(profile);
                j_list.Add(ac);

                Curve[] curves = Curve.JoinCurves(j_list);

                if (curves.Length == 1) {
                    profile = curves[0];
                }


            }//end if


            Rhino.Geometry.Line r_axis = new Rhino.Geometry.Line(new Point3d(0, 0, 0), new Point3d(1, 0, 0));

            RevSurface rev = RevSurface.Create(profile, r_axis, 0, Math.PI * 2);

            if (rev != null) {

                Brep bSurf = Brep.CreateFromRevSurface(rev, false, false);

                if (bSurf != null) {

                    bSurf.Transform(Transform.PlaneToPlane(Plane.WorldXY, turtle.Plane));

                    if (document.Objects.AddBrep(bSurf, turtle.Attributes) == Guid.Empty) {
                        throw new Exception("Failed to Create Brep object in: " + CommandName());
                    }
                } else {

                    throw new Exception("Failed to Create Brep from revolve geometry in: " + CommandName());

                }


            } else {

                throw new Exception("Failed to Create revolve geometry in: " + CommandName());

            }


        }//end execute


    }//end class
}
