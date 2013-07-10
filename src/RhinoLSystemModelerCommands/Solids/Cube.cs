using System;

using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class Cube : ModelerCommand {

        public override string CommandName() {
            return "Cube";
        }

        public override int NumberOfArguments() {
            return 4;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 4) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}

            float plane_f = args[0];

            float cx = args[1];
            float cy = args[2];
            float cz = args[3];



            //grab the plane or throw exception...
            Plane cPlane = ModelerCommand.GetPlaneFromArgument(turtle, plane_f, CommandName());

            Rhino.Geometry.Interval xInt, yInt, zInt;


            //center on 0,0 in xy 
            xInt = new Rhino.Geometry.Interval(cx * -0.5, cx * 0.5);
            yInt = new Rhino.Geometry.Interval(cy * -0.5, cy * 0.5);
            zInt = new Rhino.Geometry.Interval(cz * -0.5, cz * 0.5);
        

            Rhino.Geometry.Box cube = new Rhino.Geometry.Box(cPlane, xInt, yInt, zInt);

            if (cube.IsValid) {

                Rhino.Geometry.Brep bCube = Rhino.Geometry.Brep.CreateFromBox(cube);

                if (bCube != null) {

                    if (document.Objects.AddBrep(bCube, turtle.Attributes) == Guid.Empty) {
                        throw new Exception("Failed to create Cube Object in: " + CommandName());
                    }
                } else {
                    throw new Exception("Failed to create Brep Box Object in: " + CommandName());
                }

            } else {
                throw new Exception("Failed to create Box Geometry in: " + CommandName());

            }


        }//end execute

    }//end class
}
