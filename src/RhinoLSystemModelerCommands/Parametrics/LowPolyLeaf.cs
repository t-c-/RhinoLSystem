using System;
using System.Collections.Generic;



using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class LowPolyLeaf : ModelerCommand {


        public override string CommandName() {
            return "LowPolyLeaf";
        }

        public override int NumberOfArguments() {
            return 4;
        }

        /// <summary>
        /// Creates a a simple Leaf from two triangles with a rectangle in between.
        /// Uses a total of four faces and applies a simple planar mapping
        /// </summary>
        /// <param name="turtle"></param>
        /// <param name="document"></param>
        /// <param name="args"></param>
        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {



            float f_length = args[0];
            float f_width = args[1];
            float f_w_bias = args[2];
            float f_curl = args[3];



            List<Point3d> points = new List<Point3d>();

            f_w_bias = ModelerCommand.FloatValueFromArgument(f_w_bias, 0.1f, 0.9f, CommandName() );


            double dist1 = 0.7 * f_w_bias * f_length;
            double dist2 = f_length * 0.3;
            double dist3 = f_length - dist1 - dist2;

            double h_width = f_width * 0.5;

            f_curl  = ModelerCommand.FloatValueFromArgument(f_w_bias, -1f, 1f, CommandName() );
            double curl_inc = RhinoMath.ToRadians(30 * f_curl);

            double x = 0;
            double y = 0;


            Point3d origin = new Point3d(0, 0, 0);
            Point3d cp1 = new Point3d();
            Point3d cp2 = new Point3d();
            Point3d cp3 = new Point3d();
            Point3d rep1 = new Point3d();
            Point3d rep2 = new Point3d();
            Point3d lep1 = new Point3d();
            Point3d lep2 = new Point3d();

            //double x, y = 0 ;

            //first point
            x += dist1 * Math.Cos(curl_inc);
            y += dist1 * Math.Sin(curl_inc);
            curl_inc += curl_inc;
            cp1 = new Point3d(x, 0, y);

            //second point
            x += dist2 * Math.Cos(curl_inc);
            y += dist2 * Math.Sin(curl_inc);
            curl_inc += curl_inc;
            cp2 = new Point3d(x, 0, y);

            //third point
            x += dist3 * Math.Cos(curl_inc);
            y += dist3 * Math.Sin(curl_inc);


            cp3 = new Point3d(x, 0, y);

            // first edge points (looking down x)
            rep1 = cp1;
            lep1 = cp1;

            rep1.Y = -h_width;
            lep1.Y = h_width;

            // first edge points (looking down x)
            rep2 = cp2;
            lep2 = cp2;

            rep2.Y = -h_width;
            lep2.Y = h_width;


            Mesh l_mesh = new Mesh();

            l_mesh.Vertices.Add(origin);  //0
            l_mesh.Vertices.Add(rep1); //1
            l_mesh.Vertices.Add(rep2); //2

            l_mesh.Vertices.Add(cp3); //3

            l_mesh.Vertices.Add(lep2); //4
            l_mesh.Vertices.Add(lep1); //5

            //start face
            l_mesh.Faces.AddFace(0, 1, 5);
            //first section
            l_mesh.Faces.AddFace(1, 2, 4);
            l_mesh.Faces.AddFace(1, 4, 5);
            //end face
            l_mesh.Faces.AddFace(2, 3, 4);


            l_mesh.Normals.ComputeNormals();
            l_mesh.Compact();


            if (l_mesh.IsValid) {

                //build transform
                Transform tpt = Transform.PlaneToPlane(Plane.WorldXY, turtle.Plane);

                l_mesh.Transform(tpt);

                BoundingBox bb = l_mesh.GetBoundingBox(turtle.Plane);

                Interval ix = new Interval(bb.Min.X, bb.Max.X);
                Interval iy = new Interval(bb.Min.Y, bb.Max.Y);
                Interval iz = new Interval(bb.Min.Z, bb.Max.Z);

                //create texture mapping
                Rhino.Render.TextureMapping tm = Rhino.Render.TextureMapping.CreatePlaneMapping(turtle.Plane, ix, iy, iz);

                l_mesh.TextureCoordinates.SetTextureCoordinates(tm);

                //add teh leaf to the document
                document.Objects.AddMesh(l_mesh, turtle.Attributes);


            }




        }//endexecute



    }//end class
}
