using System;
using System.Collections.Generic;

using Rhino;
using Rhino.DocObjects;


namespace RhinoLSystem.Commands {
    public class SetLayer : ModelerCommand {

        public override string CommandName() {
            return "SetLayer";
        }

        public override int NumberOfArguments() {
            return 1;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 1) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}


            float l_id_f = args[0];
            int l_id = (int)l_id_f;

            if (l_id_f - l_id != 0) {
                throw new Exception("Invalid Layer Name number: " + l_id_f + " in: " + CommandName());
            }

            //layer name
            string l_name = RhinoTurtle.LAYER_NAME_PREFIX + l_id.ToString();

            //look for layer
            int l_index = document.Layers.Find(l_name, true);

            if (l_index < 0) {
                //create layer

                Random rnd = new Random();
                int r = rnd.Next(0, 255);
                int g = rnd.Next(0, 255);
                int b = rnd.Next(0, 255);
                System.Drawing.Color c = System.Drawing.Color.FromArgb(r, g, b);

                if (c == null) {
                    c = System.Drawing.Color.White;
                }

                //add the layer to the doc
                l_index = document.Layers.Add(l_name, c);

                if (l_index < 0) {
                    throw new Exception("Failed to create layer: " + l_name + " in: " + CommandName());
                }


            }

            //set the layer index in the turtle
            turtle.LayerIndex = l_index;

        }//end execute



    }
}
