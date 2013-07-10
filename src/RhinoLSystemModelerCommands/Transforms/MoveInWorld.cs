using System;

using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
   public class MoveInWorld : ModelerCommand {


       public override string CommandName() {
           return "MoveInWorld";
       }


       public override int NumberOfArguments() {
           return 3;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //if (args.Length != 3) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}

           float x = args[0];
           float y = args[1];
           float z = args[2];

           //move the turtle
           turtle.TranslateWorld(x, y, z);
    

       }//end execute




    }
}
