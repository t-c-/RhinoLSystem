using System;


using Rhino;
using Rhino.Geometry;


namespace RhinoLSystem.Commands {
    public class Sphere : ModelerCommand {

       public override string CommandName() {
           return "Sphere";
       }

       public override int NumberOfArguments() {
           return 1;
       }

       public override void Execute(RhinoTurtle turtle, RhinoDoc document, float[] args) {

           //check args
           //if (args.Length != 1) {
           //    throw new Exception("Invalid number of args in : " + CommandName());
           //}


           float radius = args[0];

           //check radius
           if (radius <= 0) {
               throw new Exception("Invalid radius of: " + radius + " in: " + CommandName());
           }


           //create the sphere
           Rhino.Geometry.Sphere sphere = new Rhino.Geometry.Sphere(turtle.Plane, radius);

           if (sphere.IsValid) {
               if (document.Objects.AddSphere(sphere, turtle.Attributes) == Guid.Empty) {
                   throw new Exception("Failed to create Sphere Object in: " + CommandName());
               }
           } else {
               throw new Exception("Failed to create Sphere Geoemetry in: " + CommandName());
           }

           


       }//end execute

    }
}
