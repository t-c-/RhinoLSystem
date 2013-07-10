using System;

using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
    public class Cone : ModelerCommand {


        public override string CommandName() {
            return "Cone";
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

            //adjust plane to compenstate for cone definition
            cPlane.Flip();
            cPlane.Origin = cPlane.PointAt(0, 0, -height);

            //build the cone
            Rhino.Geometry.Cone cone = new Rhino.Geometry.Cone(cPlane, height, radius);
            

            if (cone.IsValid) {

                Rhino.Geometry.Brep bCone = Brep.CreateFromCone(cone, true);

                if (bCone != null) {

                    if (document.Objects.AddBrep(bCone, turtle.Attributes) == Guid.Empty) {
                        throw new Exception("Failed to create Cone Object in: " + CommandName());
                    }
                } else {
                    throw new Exception("Failed to create Brep Cone Object in: " + CommandName());
                }

            } else {
                throw new Exception("Failed to create Cone Geometry in: " + CommandName());

            }


        }//end execute




    }
}
