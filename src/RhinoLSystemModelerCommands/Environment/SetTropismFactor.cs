using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
    public class SetTropismFactor : ModelerCommand {

        public override string CommandName() {
            return "SetTropismFactor";
        }

        public override int NumberOfArguments() {
            return 1;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            int argCnt = args.Length;

            //two arguments
            //if (argCnt != 1) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}

            float factor = args[0];
            turtle.TropismFactor = factor;

        }//end execute


    }
}
