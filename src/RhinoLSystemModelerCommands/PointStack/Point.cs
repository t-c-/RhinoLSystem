using System;


using Rhino;
using RhinoLSystem;

namespace RhinoLSystem.Commands {
    public class Point : RhinoLSystem.ModelerCommand {


        public override string CommandName() {
            return "Point";
        }

        public override int NumberOfArguments() {
            return 2;
        }


        /// <summary>
        /// Create a point.  This takes two arguments: destination and front(bool).
        /// Destination controls where the point is created: Current point list of current stack, document, or both.
        /// Front is a boolean indicating if the point is added to the front or end of the current point list.
        /// true(1) = add at front of list, false(0) = add at end of list
        /// </summary>
        /// <param name="turtle">Modeler Turtle</param>
        /// <param name="document">Current Document</param>
        /// <param name="args">Arguments</param>
        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            float d_f = args[0];
            float a_f = args[1];

            bool front = ModelerCommand.BooleanValueFromArgument(a_f, CommandName() );

            PointCurveDestination d = ModelerCommand.CurveAndPointDestination(d_f, CommandName());

            //put the curve on the curve stack
            if (d == PointCurveDestination.StackList || d == PointCurveDestination.Both) {
                turtle.PushPoint(front);
            }

            //add the point to the document
            if (d == PointCurveDestination.Document || d == PointCurveDestination.Both) {
                //create the point
                if (document.Objects.AddPoint(turtle.Position, turtle.Attributes) == Guid.Empty) {
                    throw new Exception("Failed to create point " + CommandName());
                }
            }            


        }//end execute




    }//end class
}
