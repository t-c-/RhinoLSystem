using System;


using Rhino;
using Rhino.Geometry;




namespace RhinoLSystem.Commands {
    public class Circle : ModelerCommand {


        public override string CommandName() {
            return "Circle";
        }

        public override int NumberOfArguments() {
            return 3;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {



            float planeF = args[0];
            float d_f = args[1];
            float radius = args[2];


            PointCurveDestination d = ModelerCommand.CurveAndPointDestination(d_f, CommandName());

            //this will throw an error if incorrect
            Plane cPlane = ModelerCommand.GetPlaneFromArgument(turtle, planeF, CommandName());

            //only use the normal vector from the plane
            //this creates circles with more unified seams for loft operations
            //apparently automatic plane generation doesn't work so hot...
            //Plane cPlane = new Plane(tPlane.Origin, tPlane.Normal);


            //build the circle
            Rhino.Geometry.Circle circle = new Rhino.Geometry.Circle(cPlane, radius);

            if (circle.IsValid) {
                Rhino.Geometry.ArcCurve ac = new ArcCurve(circle);

                if (ac != null) {

                    //put the curve on the curve stack
                    if (d == PointCurveDestination.StackList || d == PointCurveDestination.Both) {
                        turtle.DropCurve(ac);
                    }

                    //add the curve to the document
                    if (d == PointCurveDestination.Document || d == PointCurveDestination.Both) {
                        //create the polyline
                        if (document.Objects.AddCurve(ac, turtle.Attributes) == Guid.Empty) {
                            throw new Exception("Failed to create circle curve " + CommandName());
                        }
                    }

                } else {

                    throw new Exception("Failed to create circle curve object" + CommandName());
                }

            } else {

                throw new Exception("Failed to create valid circle geometry" + CommandName());
            }


        }//end execute


    }//end class
}

