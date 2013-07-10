using System;
using System.Collections.Generic;
using System.Text;


using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {

    /// <summary>
    /// Rotates the Y Axis around the X Axis until the Y Axis lays in the World XY Plane.
    /// This is for the '$' command described on Page 57 (Figure 2.6, page 56).
    /// </summary>
    public class RollToLevel : ModelerCommand {

        public override string CommandName() {
            return "RollToLevel";
        }

        public override int NumberOfArguments() {
            return 0;
        }

        public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

            if (args.Length != 0) {
                throw new Exception("Invalid number of args in : " + CommandName());
            }


            //turtle x axis (H) and origin
            Vector3d x_axis = turtle.Plane.XAxis;
            Point3d t_origin = turtle.Plane.Origin;

            //use standard WorldXY plane Z axis for world as universal "up" vector
            //world z, heading cross product
            Vector3d wz_h_cpv = Vector3d.CrossProduct(Plane.WorldXY.ZAxis, x_axis);


            //wz_h_cpv.Unitize();
            //same as unitize
            wz_h_cpv = wz_h_cpv / wz_h_cpv.Length;

            //heading description from book
            //U = H X L  try using new left...
            Vector3d newUp = Vector3d.CrossProduct(turtle.Plane.XAxis, wz_h_cpv);

            //new y axis 
            Vector3d new_y_axis = Vector3d.CrossProduct (turtle.Plane.XAxis , newUp);

            //new turtle plane
            Plane new_t_plane = new Plane(t_origin, x_axis, new_y_axis);

            //check the plane - maybe throw error?
            if (new_t_plane.IsValid) {
                //set the turtle plane
                turtle.Plane = new_t_plane;
            } else {
                RhinoApp.WriteLine("Invalid Turtle Plane generated in " + CommandName());
            }


        }//end execute


    }//end class
}
