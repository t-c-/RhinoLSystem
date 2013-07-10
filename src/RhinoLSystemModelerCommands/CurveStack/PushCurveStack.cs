using System;
using System.Collections.Generic;

using Rhino;


namespace RhinoLSystem.Commands {
    public class PushCurveStack : ModelerCommand {

       public override string CommandName() {
           return "PushCurveStack";
       }


       public override int NumberOfArguments() {
           return 0;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //if (args.Length != 0) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}

           //push the loft stack in the turtle
           turtle.PushCurveStack();


       }//end execute



    }
}
