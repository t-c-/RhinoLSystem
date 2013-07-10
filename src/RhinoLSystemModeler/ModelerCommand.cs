using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;


namespace RhinoLSystem {

    public enum PointCurveDestination : uint {

        StackList,
        Document,
        Both

    }

     /// <summary>
    /// Base class to define Modeler Commands.
    /// </summary>
   public class ModelerCommand {


       /// <summary>
       /// Default Empty Constructor
       /// </summary>
        protected ModelerCommand() {


        }


       /// <summary>
       /// DO NOT OVERRIDE this method.  Only one Command needs to override this and it has handled as part of the Standard Library.
       /// </summary>
       /// <returns></returns>
        public virtual bool ReleasesPrune() {
            return false;
        }

        /// <summary>
        /// DO NOT OVERRIDE this method.  Only one Command needs to override this and it has handled as part of the Standard Library.
        /// </summary>
        /// <returns></returns>
        public virtual bool PruneDeeper() {
            return false;
        }

       /// <summary>
       /// Name of the Command.  This should always be unique name.
       /// </summary>
       /// <returns>Name of the command.  Ths should match the name referenced by an alias in the LSytem definition file.</returns>
        public virtual string CommandName() {
            return String.Empty;
        }

       /// <summary>
       /// Number of arguments that the command takes
       /// </summary>
       /// <returns></returns>
        public virtual int NumberOfArguments() {
            return Int32.MinValue;
        }

       /// <summary>
       /// Execute performs the command withprovided arguements.
       /// </summary>
       /// <param name="args">Array of floats passed from the LSystem Engine as argumetns for the command.</param>
       public virtual void Execute(RhinoTurtle refTurtle , RhinoDoc doc, float[] args) { }


       #region "Static Helpers"

       /// <summary>
       /// Helper function to parse a float as an integer that specifies a Local Plane
       /// 0=Local XY Plane
       /// 1=Local YZ Plane
       /// 2=Local XZ Plane
       /// </summary>
       /// <param name="turtle"></param>
       /// <param name="arg"></param>
       /// <param name="cmdName"></param>
       public static Plane GetPlaneFromArgument(RhinoTurtle turtle, float arg, string cmdName) {


           int plane_index = (int)arg;
           Plane local_plane = turtle.Plane;

           if (arg - plane_index == 0) {

               switch (plane_index) {
                   case 0:
                       //do nothing use local xy which is the turtle plane
                       break;
                   case 1:
                       //local yz plane
                       local_plane = new Plane(local_plane.Origin, local_plane.YAxis, local_plane.ZAxis);
                       break;
                   case 2:
                       //local xz plane
                       local_plane = new Plane(local_plane.Origin, local_plane.XAxis, local_plane.ZAxis);
                       break;
                   default:
                       string err_msg = "Invalid plane index: " + plane_index + " in: " + cmdName;
                       err_msg += Environment.NewLine + "Expecting: 0=XY, 1=YZ, 2=XZ";
                       throw new Exception(err_msg);
               }//end switch

               return local_plane;

           } else {

               string err_msg = "Invalid plane specification: " + arg + " in: " + cmdName;
               err_msg += Environment.NewLine + "Expecting: 0=XY, 1=YZ, 2=XZ";

               throw new Exception(err_msg);

           }//end if/else


       }




       /// <summary>
       /// Checks the destination index of provided argument.  This integer represents where geometry is created:
       /// The current appropriate stack list (Point or Curve), the current document, or both.
       /// </summary>
       /// <param name="d">destination 0= list (current stack list), 1=current document, 2= both</param>
       ///  <param name="cmdName">calling command</param>
       /// <returns>PointCurveDestination enum</returns>
       static public PointCurveDestination CurveAndPointDestination(float destination, string cmdName) {

           int d_code = (int)destination;
           
           //check for abs(arg) condition...
           if (destination - d_code  == 0 && (d_code>=0 && d_code <= 2)) {

               PointCurveDestination d_e = PointCurveDestination.Both;

               switch (d_code) {

                   case 0:
                       d_e = PointCurveDestination.StackList;
                       break;

                   case 1:
                       d_e = PointCurveDestination.Document;
                       break;
                   case 2:
                       //already set..
                       d_e = PointCurveDestination.Both;
                       break;
               }
               return d_e;

           } else {

               string err_msg = "Invalid destination specification: " + destination + " in: " + cmdName;
               err_msg += Environment.NewLine + "Expecting: 0=Curve List (current Curve Stack), 1=current Document, 2= Both";

               throw new Exception(err_msg);

           }//end if/else


       }


       /// <summary>
       /// Helper function to do the checking on a passed boolean.
       /// </summary>
       /// <param name="bool_v">Boolean value 1.0 = true, 0 = false</param>
       /// <param name="cmdName">calling command</param>
       /// <returns>true or false</returns>
       public static bool BooleanValueFromArgument(float bool_v, string cmdName) {

           if (bool_v == 1.0f) {
               return true;
           } else if (bool_v == 0) {
               return false;
           } else {
               throw new Exception("Invalid boolean value of: " + bool_v + ", in: " + cmdName);
           }

       }

       /// <summary>
       /// Helper function normalizing radians
       /// Anything with an absolute value larger than 2 * PI is normalized to within 2 * PI.
       /// Sign is preserved.
       /// </summary>
       /// <param name="rad"></param>
       /// <returns>normalized radians</returns>
       static public float NormalizeRadians(float rad) {

           float tpi = (float)Math.PI * 2;
           float a_rad = Math.Abs(rad);
           float theta = rad;

           //abs greater than  pi?
           //not 0 is filtered out -sign will return 0 on 0
           if (a_rad > tpi) {
               theta = (a_rad % tpi) * Math.Sign(rad);
           }


           return theta;

       }

       #endregion



   }//end class
}
