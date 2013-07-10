using System;
using System.Collections.Generic;


using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class PopCurveStackToRevolve : ModelerCommand {

        public override string CommandName() {
            return "PopCurveStackToRevolve";
        }


        public override int NumberOfArguments() {
            return 3;
        }

        /// <summary>
        /// Pops the current curve stack to a revolve operation.  There can be only one curve in the curve
        /// stack for this operation.
        /// Arguments = axis, start angle, end angle
        /// </summary>
        /// <param name="turtle"></param>
        /// <param name="document"></param>
        /// <param name="args"></param>
       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {


        //if (args.Length != 3) {
        //  throw new Exception("Invalid number of arguments in: " + CommandName());
        //}



        float axis_f = args[0];
        float s_angle = args[1];
        float e_angle = args[2];

        int axis_i = (int)axis_f;

        if (axis_f - axis_i != 0) {
          throw new Exception("Invalid axis specification of: " + axis_f + " in: " + CommandName());
        }


        Vector3d axis = turtle.Plane.ZAxis;

        switch(axis_i ){

            case 0:
              axis = turtle.Plane.XAxis;
              break;

            case 1:
              axis = turtle.Plane.YAxis;
              break;

            case 2:
              axis = turtle.Plane.ZAxis;
              break;

            default:
                throw new Exception("Invalid axis specification of: " + axis_f + " in: " + CommandName() );


        }//end switch


        List<Curve> curves = turtle.PopCurveStack();


        if (curves.Count != 1) {
            throw new Exception("Invalid number of cuurves in curve stack for: " + CommandName() + " expecting 1");
        }


        Curve revCurve = curves[0];
        Point3d  sp = turtle.Position;
        Point3d  ep  = sp + (axis * 10);
        Rhino.Geometry.Line axis_line = new Rhino.Geometry.Line(sp,ep);


        RevSurface rs = RevSurface.Create(revCurve, axis_line, s_angle, e_angle);

        if (rs != null) {

            if(document.Objects.AddSurface(rs, turtle.Attributes) == Guid.Empty) {
                throw new Exception("Failed to create revolved surface in: " + CommandName());
            }

        } else {
          throw new Exception("Failed to create revolved surface in: " + CommandName());
        }






    }//end execute

    }//end class
}

