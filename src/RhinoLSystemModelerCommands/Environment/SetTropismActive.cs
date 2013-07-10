using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;

namespace RhinoLSystem.Commands {
    public class SetTropismActive : ModelerCommand {

        public override string CommandName() {
            return "SetTropismActive";
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


            float on_off_f = args[0];
            int on = (int)on_off_f;

            if (on_off_f - on == 0) {

                if (on == 1) {
                    turtle.TropismActive = true;
                } else {
                    turtle.TropismActive = false;
                }

            } else {
                throw new Exception("Invalid on/off specification: " + on_off_f + " in: " + CommandName() + " , expectiong 0 or 1");
            }


        }//end execute


    }
}
