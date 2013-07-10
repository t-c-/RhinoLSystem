using System;


using Rhino;
using Rhino.Geometry;




namespace RhinoLSystem.Commands {
    public class Arc : ModelerCommand {


        public override string CommandName() {
            return "Arc";
        }

        public override int NumberOfArguments() {
            return 5;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


            float planeF = args[0];
            float d_f = args[1];
            float radius = args[2];  //radius
            float s_ang = args[3];   //start angle
            float e_ang = args[4];   //end angle


            PointCurveDestination d = ModelerCommand.CurveAndPointDestination(d_f, CommandName());

            //this will throw an error if incorrect
            Plane tPlane = ModelerCommand.GetPlaneFromArgument(turtle, planeF, CommandName());

            s_ang = ModelerCommand.NormalizeRadians(s_ang);
            e_ang = ModelerCommand.NormalizeRadians(e_ang);
            


            //build the circle
            Rhino.Geometry.Circle circle = new Rhino.Geometry.Circle(tPlane, radius);
            Interval arc_r = new Interval(s_ang, e_ang);
            Rhino.Geometry.Arc arc = new Rhino.Geometry.Arc(circle, arc_r);


            if (arc.IsValid) {
                Rhino.Geometry.ArcCurve ac = new ArcCurve(arc);

                if (ac != null) {

                    //put the curve on the curve stack
                    if (d == PointCurveDestination.StackList || d == PointCurveDestination.Both) {
                        turtle.DropCurve(ac);
                    }

                    //add the curve to the document
                    if (d == PointCurveDestination.Document || d == PointCurveDestination.Both) {
                        //create the polyline
                        if (document.Objects.AddCurve(ac, turtle.Attributes) == Guid.Empty) {
                            throw new Exception("Failed to create circle curve object" + CommandName());
                        }
                    }

                } else {

                    throw new Exception("Failed to create arc curve object" + CommandName());
                }

            } else {

                throw new Exception("Failed to create valid arc geometry" + CommandName());
            }




        }//end execute




    }//end class
}

