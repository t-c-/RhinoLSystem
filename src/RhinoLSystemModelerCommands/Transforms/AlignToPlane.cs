using System;
using System.Text;


using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class AlignToPlane : ModelerCommand {

       public override string CommandName() {
           return "AlignToPlane";
       }

       public override int NumberOfArguments() {
           return 1;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


           //if (args.Length != 1) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}

           float planeF = args[0];

           int planeIndex = (int)planeF;
           float dif = planeF - planeIndex;

           Plane plane = new Plane();

           if (dif == 0) {


               switch (planeIndex) {
                   case 0:
                       plane = Plane.WorldXY;
                       break;
                   case 1:
                       plane = Plane.WorldYZ;
                       break;
                   case 2:
                       plane = Plane.WorldZX;
                       break;
                   default:
                       throw new Exception("Invalid plane index: " + planeIndex + " , expecting 0, 1, or 2.");
               }//end switch


               turtle.AlignToPlane(plane);


           } else {

               throw new Exception("Invalid plane specification: " + planeF + " , expecting 0, 1, or 2.");

           }//end if/else


       }//end execute




    }
}
