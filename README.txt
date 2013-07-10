
About The LSystem Engine:


This work is based on the book The Algorithmic Beauty of Plants, by: Przemyslaw Prusinkiewicz & Aristid Lindenmayer.  This is currently working, but under development.  Some features may be not operate as fully expected, and some features are to be added.  Due to the nateure of the system it's difficult to test all permutations.  Documents and examples are provided with more to come in the future.



Rhino Implementation:
Robert McNeel and Associates's Rhinoceros 3D is used as the current implementation for modeling functions.  A plugin is provided, please see: "Rhino Plugin Installation"
This is a working copy of the Rhino Plugin with future improvements and refinements planned.


Standard Features (LSystem Engine Library):

Parametric (Letters can have n Parameters)
Context Sensitive (Productions are context sensitive) *
Conditional (Productions can have conditional statements)
Indirect Stocastic support (through the use of Conditional statements)

*Sub-Branch searches and branch skipping in left and right contexts seem to be working in testing, although the Hogeweg and Hesper LSystems from page35 of ABOP are not executing as shown...  (this may not perform as expected and is being investigated....)

Other Features:

User Defined Grammer (Letters are indirectly linked to Turtle Commands through the use of Aliases)
Letter Groups (Groups of Letters to represent compound instructions)
Letter Group Nesting (Call a Group from a Group)
LSystem Nesting (Start another LSytem from within the current LSystem, with parameter passing/overrides. Yes, please do be carefull...)


Modeling Features:

These features are specific to the Rhino Plugin 

-Lines, Arcs, Polylines, Free-Form curves
-Circles, Ellipses, Polygons
-Spheres, Cubes, Cones, CYlinders
-3D Faces, Meshes * (Some meshing functions are still under developemnt)
-Lofts, Sweeps, Revolves
-Parametric Objects * (more to come in future)
-Tropism * (WIP - this is working, but much stronger than illustrated in ABOP, use small values, this will be looked into...)
-User defined turtle Commands (interface for custom paramtetric objects and other functions)



Architectural Features:


This project is divided into four parts (3 Libraries - DLLs & Rhino Plugin):  
-Lsystem Engine (responsible for parsing and executing definitions)
-LSystem Editor (for editing definition files)
-Rhino LSystem Modeler (responsible for all Turtle functions - Rhino specific - Standard Commands are broken out into seperate Library)
-Rhino LSystem Plugin (Stitching it all together: UI/Engine/Modeler)

The design intent was to create an LSystem Engine that handled the grammer and productions, leaving the modeling aspects to a "host" environment (i.e.: Rhino).  The Engine and Editor can run in any .Net supported environment.  All that needs to be provided is a modeler that honors the Modeler Interface.  This means the system can be easily ported over to another CAD system or Application that supports .Net.  I.e.:  A simple stand-alone DXF writer was used for the majority of early development.  The Modeler component in this release is the only Rhino specific code (R5 RhinoCommon).  All that is required is that you write your own modeler for your application.  You can also provide your own GUI Editor and/or Expression Evaluator to replace those provided.  Note that the Expression Evaluator is not a seperate library at this time.  It uses an Interface, but the source must be re-compiled to implement a different one.


Open Source.

c# Code: BSD License
The source code will be cleaned as the project is developed, and not all classes and methods are fully documented.  This will be addressed in future releases.

