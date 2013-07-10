

using System;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;
using Rhino.Render;


namespace RhinoLSystem.Commands {
    public class PopPointsToMeshBorder : ModelerCommand {


        public override string CommandName() {
            return "PopPointsToMeshBorder";
        }

        public override int NumberOfArguments() {
            return 0;
        }

        /// <summary>
        /// Creates a Mesh from the closed polyline border curve.
        /// </summary>
        /// <param name="turtle"></param>
        /// <param name="document"></param>
        /// <param name="args"></param>
        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 0) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}



            //pop the points
            List<Point3d> points = turtle.PopPointStack();

            int pCnt = points.Count;
            Polyline polyline;
            float tol = 0.00001F;

            if (pCnt < 2) {
                throw new Exception("Too Few Points (" + pCnt + ") in Point Stack calling: " + CommandName());
            }


            if (pCnt < 3) {
                throw new Exception("Too few points in point stack to close curve in: " + CommandName() + " , need at least 3 points");
            }

            Point3d sp = points[0];
            Point3d ep = points[pCnt - 1];

            //check to see if another point is needed to close
            if (sp.DistanceTo(ep) >= tol) {
                points.Add(sp);
            }

            polyline = new Polyline(points);
            
            if (polyline != null) {

                //Mesh mesh = Mesh.CreateFromClosedPolyline(polyline);
                MeshFace[] faces = polyline.TriangulateClosedPolyline();

                if (faces == null) {
                    throw new Exception("failed to generate faces from polyline in: " + CommandName());
                }

                Mesh mesh = new Mesh();
                mesh.Vertices.AddVertices(points);
                mesh.Faces.AddFaces(faces);
                mesh.Faces.CullDegenerateFaces();

                mesh.Normals.ComputeNormals();
                mesh.Compact();


                if (!mesh.IsValid) {
                    throw new Exception("Failed to build Mesh from polyline in: " + CommandName());
                }


                Interval dx, dy, dz;
                Plane bbPlane;
                Plane.FitPlaneToPoints(points, out bbPlane);

                if (!bbPlane.IsValid) {
                    bbPlane = Plane.WorldXY;
                    bbPlane.Origin = points[0];
                }

                BoundingBox meshBB = mesh.GetBoundingBox(bbPlane);

                if (meshBB.IsValid) {
                    //System.Diagnostics.Debug.Print("using bb for texture");
                    dx = new Interval(meshBB.Min.X, meshBB.Max.X);
                    dy = new Interval(meshBB.Min.Y, meshBB.Max.Y);
                    dz = new Interval(meshBB.Min.Z, meshBB.Max.Z);

                } else {
                    dx = new Interval(0, 1);
                    dy = new Interval(0, 1);
                    dz = new Interval(0, 1);
                }

                TextureMapping tm = TextureMapping.CreatePlaneMapping(bbPlane, dx, dy, dz);

                if (tm.IsValid) {

                    if (!mesh.TextureCoordinates.SetTextureCoordinates(tm)) {
                        RhinoApp.WriteLine("failed to apply texture coords");
                    }


                } else {
                    RhinoApp.WriteLine("failed to apply texture coords");
                }


                if (mesh.IsValid ) {

                    if (document.Objects.AddMesh(mesh, turtle.Attributes) == Guid.Empty) {
                        throw new Exception("Failed to create Mesh Object " + CommandName());
                    }

                } else {
                    throw new Exception("Failed to build Mesh from polyline in: " + CommandName());
                }



            } else {
                throw new Exception("Failed to create polyline in: " + CommandName());
            }

        }//end Execute


    }//end class
}
