using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;
using Rhino.Render;


namespace RhinoLSystem.Commands {
    public class PopPointsToPolygons : ModelerCommand {


       public override string CommandName() {
           return "PopPointsToPolygons";
       }

       public override int NumberOfArguments() {
           return 0;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //if (args.Length != 0) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}


           //pop the points
           List<Point3d> points = turtle.PopPointStack();

           int pCnt = points.Count;

           if (pCnt < 3) {
               throw new Exception("Too Few Points (" + pCnt + ") in Point Stack calling: " + CommandName());
           }

           Mesh rhinoMesh = new Mesh();

           //System.Diagnostics.Debug.Print("points on stack = " + pCnt);

           Point3d first = points[0];
           Point3d second =new Point3d();
           List<Point3d> plane_points = new List<Point3d>();

           int curIndex = 1;
           int secondIndex = -1;
           int thirdIndex = -1;
           bool secondFound = false;

           int vIndex = 0;
           //add first vertex
           rhinoMesh.Vertices.Add(first);
           plane_points.Add(first);

           while (curIndex < pCnt) {

               if (!secondFound) {
                   //looking for second point
                   second = points[curIndex];
                   if (second.DistanceTo(first) != 0) {
                       
                       rhinoMesh.Vertices.Add(second);
                       vIndex += 1;
                       plane_points.Add(second);
                       //System.Diagnostics.Debug.Print("added second, index = " + vIndex);

                       secondIndex = curIndex;
                       secondFound = true;
                   }
                   curIndex += 1;

               } else {

                   //looking for third point
                   Point3d third = points[curIndex];

                   if (third.DistanceTo(second) != 0) {

                       thirdIndex = curIndex;
                       rhinoMesh.Vertices.Add(third);
                       plane_points.Add(third);
                       //add  the face
                       rhinoMesh.Faces.AddFace(0, vIndex, vIndex + 1);
                       vIndex += 1;

                       //System.Diagnostics.Debug.Print("added third, index = " + vIndex);
                       secondIndex = thirdIndex;
                       second = third;
                       
                   }

                   curIndex += 1;
               }//end if/else

               

           }//end while


           rhinoMesh.Compact();
           rhinoMesh.Normals.ComputeNormals();

           Plane bbPlane;
           //Plane.FitPlaneToPoints(plane_points, out bbPlane );

           
           BoundingBox bb = rhinoMesh.GetBoundingBox( true);
           Interval dx, dy, dz;

           if (bb.IsValid) {
               //System.Diagnostics.Debug.Print("using bb for texture");
               dx = new Interval(bb.Min.X, bb.Max.X);
               dy = new Interval(bb.Min.Y, bb.Max.Y);
               dz = new Interval(bb.Min.Z, bb.Max.Z);

           } else {
                dx = new Interval(0, 1);
                dy = new Interval(0, 1);
                dz = new Interval(0, 1);
           }

           
     
           bbPlane = new Plane(bb.Min, new Vector3d(0,0,1));
           TextureMapping tm = TextureMapping.CreatePlaneMapping(bbPlane, dx, dy, dz);
           

           if (tm != null) {

               if (!rhinoMesh.TextureCoordinates.SetTextureCoordinates(tm)) {
                   RhinoApp.WriteLine("failed to apply texture coords");
               }


           }


           //add the mesh
           if (document.Objects.AddMesh(rhinoMesh, turtle.Attributes) == Guid.Empty) {
               throw new Exception("Failed to generate Mesh Object in " + CommandName());
               //RhinoApp.WriteLine("Failed to generate mesh - Invalid mesh in " + CommandName());
           }

       }//end execute


    }//end class
}
