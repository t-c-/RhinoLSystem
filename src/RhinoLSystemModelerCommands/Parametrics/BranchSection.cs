using System;
using System.Collections.Generic;



using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class BranchSection : ModelerCommand {

       public override string CommandName() {
           return "BranchSection";
       }

       public override int NumberOfArguments() {
           return 4;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


            //if (args.Length != 4) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}


            float planeF = args[0];
            int planeIndex = (int)planeF;
            float dif = planeF - planeIndex;

            Plane l_plane = new Plane();
            Plane tp = turtle.Plane;

            if (dif == 0) {

                switch (planeIndex) {
                    case 0:
                        l_plane = new Plane(tp.Origin, tp.XAxis, tp.YAxis);
                        break;
                    case 1:
                        l_plane = new Plane(tp.Origin, tp.YAxis, tp.ZAxis); //Plane.WorldYZ;
                        break;
                    case 2:
                        l_plane = new Plane(tp.Origin, tp.XAxis, tp.ZAxis); // Plane.WorldZX;
                        break;
                    default:
                        throw new Exception("Invalid plane index: " + planeIndex + " , expecting 0, 1, or 2.");
                }//end switch

            } else {

                throw new Exception("Invalid plane specification: " + planeF + " , expecting 0, 1, or 2.");

            }//end if/else

            //transfer location
            l_plane.Origin = turtle.Plane.Origin;

            double radius = args[1];
            double randomness = args[2];
            int detail = (int)args[3];

            if (detail < 3) { detail = 3; }
            //Plane l_plane = Plane.WorldXY;
            Random rnd = new Random();

            double tInc = (Math.PI * 2) / detail;
            double min = radius * -0.5;
            double max = radius * 0.5;
            double range = max - min;

            List<Point3d> points = new List<Point3d>();

            for (int i = 0; i < detail; i++) {

                double dist = radius + (randomness * range * rnd.NextDouble());
                double x = dist * Math.Cos(i * tInc);
                double y = dist * Math.Sin(i * tInc);

                points.Add(l_plane.PointAt(x, y));


            }//end for

            Point3d first = points[0];
            points.Add(first);

            Curve loftCurve = Curve.CreateControlPointCurve(points, 3);
            //Curve ring = Curve.CreateInterpolatedCurve(points, 3);
            loftCurve.MakeClosed(0.001);

            turtle.DropCurve(loftCurve);


        }//end execute

    }//end class
}
