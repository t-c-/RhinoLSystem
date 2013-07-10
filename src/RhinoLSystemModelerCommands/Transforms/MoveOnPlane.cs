using System;
using System.Text;

using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
   public class MoveOnPlane : ModelerCommand {


       public override string CommandName() {
           return "MoveOnPlane";
       }

       public override int NumberOfArguments() {
           return 4;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //if (args.Length != 4) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}

           float plane = args[0];
           float x = args[1];
           float y = args[2];
           float z = args[3];
           int planeIndex = (int)plane;
           double dif = plane - planeIndex;

           if (dif == 0) {

               //get a copy of current position (about to move)
               Point3d sp = turtle.Position;

               switch (planeIndex) {
                   case 0:
                       turtle.TranslateLocalXY(x, y, z);
                       break;
                   case 1:
                       turtle.TranslateLocalYZ(x, y, z);
                       break;
                   case 2:
                       turtle.TranslateLocalXZ(x, y, z);
                       break;
                   default:
                       throw new Exception("Invalid plane index: " + planeIndex + " , expecting 0 (XY), 1 (YZ), or 2 (XZ).");
               }//end switch

               //use a reference here (not moving)
               Point3d ep = turtle.Position;

           } else {

               throw new Exception("Invalid plane specification: " + plane + " , expecting 0, 1, or 2.");

           }//end if/else

       }//end execute


    }
}
