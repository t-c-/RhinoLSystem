using System;
using System.Collections.Generic;

using Rhino;

namespace RhinoLSystem.Commands {
    public class PushPointStack : ModelerCommand {


       public override string CommandName() {
           return "PushPointStack";
       }

       public override int NumberOfArguments() {
           return 0;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //if (args.Length != 0) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}

           //push point to stack in turtle
           turtle.PushPointStack();

       }//end execute

    }
}
