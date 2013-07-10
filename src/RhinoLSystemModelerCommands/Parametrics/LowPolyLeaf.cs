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


            float f_plane = args[0];
            float f_length = args[1];
            float f_width = args[2];
            float f_curl = args[3];

            double step1 = f_length * 0.6;
            double step2 = f_length * 0.4;
            double step_s = step1 * 0.5;

            double h_width = f_width * 0.5;

            Plane tPlane = ModelerCommand.GetPlaneFromArgument(turtle, f_plane, CommandName());

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
            cp1.X = step_s * Math.Cos(f_curl);
            cp1.Z = step_s * Math.Sin(f_curl);

            //second point
            cp2.X = (step_s + step2) * Math.Cos(f_curl * 2);
            cp2.Z = (step_s + step2) * Math.Sin(f_curl * 2);

            //third point
            cp3.X = (step2 + step1) * Math.Cos(f_curl * 3);
            cp3.Z = (step2 + step1) * Math.Sin(f_curl * 3);


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
                Transform tpt = Transform.PlaneToPlane(Plane.WorldXY, tPlane);

                l_mesh.Transform(tpt);

                BoundingBox bb = l_mesh.GetBoundingBox(tPlane);

                Interval ix = new Interval(bb.Min.X, bb.Max.X);
                Interval iy = new Interval(bb.Min.Y, bb.Max.Y);
                Interval iz = new Interval(bb.Min.Z, bb.Max.Z);

                //create texture mapping
                Rhino.Render.TextureMapping tm = Rhino.Render.TextureMapping.CreatePlaneMapping(tPlane, ix, iy, iz);

                l_mesh.TextureCoordinates.SetTextureCoordinates(tm);

                //add teh leaf to the document
                document.Objects.AddMesh(l_mesh, turtle.Attributes);


            }




        }//endexecute



    }//end class
}
