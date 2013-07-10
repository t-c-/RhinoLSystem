using System;

using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
    public class Cylinder : ModelerCommand {


        public override string CommandName() {
            return "Cylinder";
        }

        public override int NumberOfArguments() {
            return 3;
        }


        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 3) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}

            float plane_f = args[0];
            float radius = args[1];
            float height = args[2];

            //check radius
            if (radius <= 0) {
                throw new Exception("Invalid radius of: " + radius + " in: " + CommandName());
            }


            //grab the plane or throw exception...
            Plane cPlane = ModelerCommand.GetPlaneFromArgument(turtle, plane_f, CommandName());

            Rhino.Geometry.Circle circle = new Rhino.Geometry.Circle(cPlane, radius);


            if (circle.IsValid) {

                //build the cylinder
                Rhino.Geometry.Cylinder cylinder = new Rhino.Geometry.Cylinder(circle, height);

                if (cylinder.IsValid) {

                    Rhino.Geometry.Brep bCylinder = Brep.CreateFromCylinder(cylinder, true, true);

                    if (bCylinder != null) {

                        if (document.Objects.AddBrep(bCylinder, turtle.Attributes) == Guid.Empty) {
                            throw new Exception("Failed to create Cone Object in: " + CommandName());
                        }
                    } else {
                        throw new Exception("Failed to create Brep Cylinder Object in: " + CommandName());
                    }
                } else {
                    throw new Exception("Failed to create Cylinder Geometry in: " + CommandName());
                }
            } else {
                throw new Exception("Failed to create Circle Geometry in: " + CommandName());

            }


        }//end execute




    }
}
