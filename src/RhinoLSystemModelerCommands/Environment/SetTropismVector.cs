using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
    public class SetTropismVector : ModelerCommand {

        public override string CommandName() {
            return "SetTropismVector";
        }

        public override int NumberOfArguments() {
            return 3;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            int argCnt = args.Length;

            //two arguments
            //if (argCnt != 3) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}


            float x = args[0];
            float y = args[1];
            float z = args[2];

            Vector3d tVec = new Vector3d(x, y, z);

            if (tVec.IsValid) {

                turtle.TropismVector = tVec;

            } else {

                throw new Exception("Invalid vector specification: " + x + ", " + y + ", " + z + ", in: " + CommandName());
            }



        }//end execute


    }
}
