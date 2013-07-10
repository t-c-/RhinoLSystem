using System;

using Rhino;

namespace RhinoLSystem.Commands {
   public class PushStack : ModelerCommand {

       public override string CommandName() {
           return "PushStack";
       }

       public override int NumberOfArguments() {
           return 0;
       }

       /// <summary>
       /// This is the only place where this is overriden.
       /// This tells the modeler to push the prune stack deeper : once pruning is turned on the modeler issues
       /// no commands until it finds the command that releases the prune: ModelerCommand.ReleasesPrune() which is PopStack.
       /// PruneDeeper sets the matching depth to PopStack deeper so mis-matched pairs are retruned - error!
       /// </summary>
       /// <returns></returns>
       public override bool PruneDeeper() {
           return true;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //push the state of the turtle to it's state stack
           turtle.PushStack();


       }//end execute

    }
}
