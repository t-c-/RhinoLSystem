using System;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;




namespace RhinoLSystem.Commands {
    public class InsertBlock : ModelerCommand  {



        public override string CommandName() {
            return "InsertBlock";
        }

        public override int NumberOfArguments() {
            return 5;
        }

        /// <summary>
        /// args = plane index, block id, x scale, y scale, zscale
        /// </summary>
        /// <param name="turtle"></param>
        /// <param name="document"></param>
        /// <param name="args"></param>
        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


            float plane_f = args[0];
            float block_f = args[1];
            float sx = args[2];
            float sy = args[3];
            float sz = args[4];

            int blockIndex = (int)block_f;

            string blockPrefix = "LSystemBlockRef";
            string blockName = blockPrefix + blockIndex;

            int planeIndex = (int)plane_f;
            double dif = plane_f - planeIndex;

            //this will throw an error if incorrect
            Plane blockPlane = ModelerCommand.GetPlaneFromArgument(turtle, plane_f, CommandName());

            if (block_f - blockIndex == 0) {

                //combine transforms: scaling then location/orientation 
                //order is verified and correct
                Transform bTrans = Transform.Scale(blockPlane, sx, sy, sz) * Transform.PlaneToPlane(Plane.WorldXY, blockPlane);

                Rhino.DocObjects.InstanceDefinition bdef = document.InstanceDefinitions.Find(blockName, true);
                int defIndex;

                if (bdef == null) {

                    string blkDesc = "lsystem generated placeholder : please define geometry";
                    //create block reference
                    TextEntity note = new TextEntity();
                    note.Text = blockName;
                    note.TextHeight = 1;
                    List<GeometryBase> geom = new List<GeometryBase>();
                    geom.Add(note);
                    defIndex = document.InstanceDefinitions.Add(blockName, blkDesc, new Point3d(0, 0, 0), geom);


                } else {

                    //grab the index from defined block
                    defIndex = bdef.Index;
                }


                //add to doc
                document.Objects.AddInstanceObject(defIndex, bTrans, turtle.Attributes);


            } else {

                throw new Exception("Invalid Block ID number: " + block_f + " , expecting an integer.");

            }



        }//end execute



    }
}
