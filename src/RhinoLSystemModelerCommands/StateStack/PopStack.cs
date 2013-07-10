using System;

using Rhino;

namespace RhinoLSystem.Commands {
   public class PopStack : ModelerCommand {

       public override string CommandName() {
           return "PopStack";
       }

       public override int NumberOfArguments() {
           return 0;
       }

       /// <summary>
       /// This is the only place where this is overridden
       /// </summary>
       /// <returns></returns>
       public override bool ReleasesPrune() {
           return true;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //if (args.Length != 0) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}

           //pop the turtle state stack
           turtle.PopStack();

       }//end execute


    }
}
