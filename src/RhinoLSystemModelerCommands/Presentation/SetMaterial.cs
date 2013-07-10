using System;
using System.Collections.Generic;

using Rhino;
using Rhino.DocObjects;

namespace RhinoLSystem.Commands {
    public class SetMaterial : ModelerCommand  {


        public override string CommandName() {
            return "SetMaterial";
        }

        public override int NumberOfArguments() {
            return 1;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            //if (args.Length != 1) {
            //    throw new Exception("Invalid number of args in : " + CommandName());
            //}


            float m_id_f = args[0];
            int m_id = (int)m_id_f;

            if (m_id_f - m_id != 0) {
                throw new Exception("Invalid Material ID: " + m_id_f + " in: " + CommandName());
            }

            //material name
            string m_name = RhinoTurtle.MATERIAL_NAME_PREFIX + m_id.ToString();

            int m_index = document.Materials.Find(m_name, true);

            if (m_index < 0) {
                //create material

                m_index = document.Materials.Add();
                Material m = document.Materials[m_index];

                //set the name
                m.Name = m_name;
                //setup some basics
                Random rnd = new Random();
                int r = rnd.Next(0, 255);
                int g = rnd.Next(0, 255);
                int b = rnd.Next(0, 255);
                System.Drawing.Color c = System.Drawing.Color.FromArgb(r, g, b);
                m.DiffuseColor = c;
              
                //update material
                m.CommitChanges();

            }

            //set the material index in the turtle
            turtle.MaterialIndex = m_index;

        }//end execute


    }
}
