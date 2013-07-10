using System;

using RhinoLSystem;
using Rhino;
using Rhino.Geometry;


namespace ExampleCustomTurtleCommand {
    public class ExampleCustomTurtleCommandClass : RhinoLSystem.ModelerCommand {

        /// <summary>
        /// Name the command is referenced by.
        /// </summary>
        /// <returns></returns>
        public override string CommandName() {
            return "ExampleCustomTurtleCommand";
        }

        /// <summary>
        /// Always override this or their will be an error.
        /// This is the number of arguments that will be passed to your command. 
        /// </summary>
        /// <returns></returns>
        public override int NumberOfArguments() {
            return 0;
        }



        public override void Execute(RhinoTurtle turtle, Rhino.RhinoDoc document, float[] args) {

            TextDot td = new TextDot("Hello Rhino", turtle.Position);

            //always use the current turlte attributes when adding objects
            //if you want current decoration/layer state
            //this is done by default on all other commands
            document.Objects.AddTextDot(td, turtle.Attributes);



        }



    }
}
