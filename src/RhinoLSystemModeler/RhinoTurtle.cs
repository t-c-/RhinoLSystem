using System;
using System.Collections.Generic;
using System.Text;



using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;


namespace RhinoLSystem {
    public class RhinoTurtle {


        public static  string BLOCK_NAME_PREFIX = "LSystemBlock";
        public static  string LAYER_NAME_PREFIX = "LSystemLayer";
        public static  string MATERIAL_NAME_PREFIX = "LSystemMaterial";


        private Plane _plane;

        //private List<Plane> _stack;

        private List<TurtleStackRecord> _state_stack;

    

        //experiements...
        private List<Point3d> _points;

        private List<List<Point3d>> _point_stack;

        private List<List<Curve>> _curve_stack;

        private List<Curve> _curves;

        private ObjectAttributes _obj_attributes;

 

        private bool _tropism_active;
        private Vector3d _tropism_vector;
        private double _tropism_factor;

        //switches branch pruning on
        private bool _pruning_on;


        /// <summary>
        /// Constructor
        /// Intializes a Plane and Stack
        /// </summary>
        public RhinoTurtle() {


            _plane = new Plane(Plane.WorldXY);

           // _stack = new List<Plane>();

            _state_stack = new List<TurtleStackRecord>();



            _points = null;

            _point_stack = new List<List<Point3d>>();


            //loft stack
            _curve_stack = new List<List<Curve>>();
            _curves = null;

            _pruning_on = false;

            _tropism_active = false;
            _tropism_vector = new Vector3d(0, 0, -1);
            _tropism_factor = 1;

            _obj_attributes = new ObjectAttributes();



        }

        #region "Utility"

        /// <summary>
        /// This Resets the Turtle.  Since this reference is handed off to the commands, 
        /// this was needed as re-intializing a new Turtle(), broke the reference (Turtle would never "reset").
        /// </summary>
        public void Reset() {

            //reset the plane to world xy
            _plane = new Plane(Plane.WorldXY);

            //obj attributes by default inherit from the document
            _obj_attributes = RhinoDoc.ActiveDoc.CreateDefaultAttributes();
            

            //_stack.Clear();

            //reset the state stack
            _state_stack.Clear();

            //reset current point list
            _points = null;

            //reset point stack
            _point_stack.Clear();


            //clear loft stack and curves
            _curves = null;

            _curve_stack.Clear();


            _tropism_active = false;
            _tropism_factor = 0.1;
            _tropism_vector = new Vector3d(0, 0, -1);

            _pruning_on = false;

        }


        #endregion


        #region "Properties"


        /// <summary>
        /// Gets the Turtle Position
        /// </summary>
        public Point3d Position {
            get {
                return _plane.Origin;
            }
        }


        /// <summary>
        /// Gets the Turtle Plane.
        /// </summary>
        public Plane Plane {
            set {
                _plane = value;
            }

            get {
                return _plane;
            }

        }


        /// <summary>
        /// Sets the Turtle Display Color.  
        /// If this is set, the default color settings from the Rhino Document are overridden and this color is applied.
        /// Value is stored in the Turtle State, so it is recorded and restored with Push/Pop Stack.
        /// </summary>
        public System.Drawing.Color DisplayColor {

            set {

                _obj_attributes.ColorSource = ObjectColorSource.ColorFromObject;
                _obj_attributes.ObjectColor = value;

            }

            get {

                return _obj_attributes.ObjectColor;
            }
        }


        /// <summary>
        /// Sets the Turtle Plot Color.  
        /// If this is set, the default plot color settings from the Rhino Document are overridden and this color is applied.
        /// Value is stored in the Turtle State, so it is recorded and restored with Push/Pop Stack.
        /// </summary>
        public System.Drawing.Color PlotColor {

            set {

                _obj_attributes.PlotColorSource = ObjectPlotColorSource.PlotColorFromObject;
                _obj_attributes.PlotColor = value;

               
            }

            get {

                return _obj_attributes.PlotColor;
            }
        }

        /// <summary>
        /// Sets the Turtle Line width.  
        /// If this is set, the default plot color settings from the Rhino Document are overridden and this color is applied.
        /// Value is stored in the Turtle State, so it is recorded and restored with Push/Pop Stack.
        /// </summary>
        public double PlotLineWidth {

            set {

                _obj_attributes.PlotWeightSource = ObjectPlotWeightSource.PlotWeightFromObject;
                _obj_attributes.PlotWeight = value;

            }
            get {

                return _obj_attributes.PlotWeight;
            }

        }


        /// <summary>
        /// Sets the Layer Index which controls the Layer objects are created on.
        /// No Checking is done here, Layer Indexes are expected to be pre-checked in Command.
        /// </summary>
        public int LayerIndex {
            set {
                _obj_attributes.LayerIndex = value;
            }
            get {
                return _obj_attributes.LayerIndex;
            }
        }

        /// <summary>
        /// Sets the Material Index of the ObjectAttributes.
        /// After the the Material is set, all subsequent objects are created with MaterialFromObject.
        /// </summary>
        public int MaterialIndex {
            set {

                //if the material is being set, make sure to set to MaterialSource to object
                _obj_attributes.MaterialSource = ObjectMaterialSource.MaterialFromObject;
                _obj_attributes.MaterialIndex = value;

            }
            get {
                return _obj_attributes.MaterialIndex;
            }
        }



        /// <summary>
        /// Sets Tropism on or off.  When tropism is one, all movements will "bend" the Turtle Movements towards the Tropism Vector.
        /// </summary>
        public bool TropismActive {
            set {
                _tropism_active = value;
            }
            get {
                return _tropism_active;
            }


        }//end apply tropism

        /// <summary>
        /// The Tropism factor, or susceptibility to bending.
        /// </summary>
        public double TropismFactor {
            set {
                _tropism_factor = value;
            }
            get {
                return _tropism_factor;
            }
        }//end tropism magnitude


        /// <summary>
        /// The Tropism Vector.
        /// </summary>
        public Vector3d TropismVector {
            set {
                _tropism_vector = value;
            }
            get {
                return _tropism_vector;
            }

        }//end tropism vector

        /// <summary>
        /// DO NOT CHANGE this flag.  This is handled as part of the Standard Library.
        /// Turns branch pruning on.  When on, no commands will be issued until released.
        /// </summary>
        public bool PruneBranch {
            set {
                _pruning_on = value;
            }
            get {
                return _pruning_on;
            }
        }

        /// <summary>
        /// The currently used Rhino Object Attributes that controls the Turtles
        /// Color, Line Width, Layer, and Material
        /// </summary>
        public ObjectAttributes Attributes {

            get {
                return _obj_attributes; //.Duplicate();
            }

        }


        #endregion


        #region "Move on Axis, Translation, Plane Alignment"

 
        /// <summary>
        /// Move Turtle on X Axis
        /// </summary>
        /// <param name="distance">Distance to translate Turtle</param>
        public void MoveOnXAxis(double distance) {

            _plane.Origin += _plane.XAxis * distance;
            ApplyTropism();

        }//end move on x axis


        /// <summary>
        /// Move Turtle on Y Axis
        /// </summary>
        /// <param name="distance">Distance to translate Turtle</param>
        public void MoveOnYAxis(double distance) {

            _plane.Origin += _plane.YAxis * distance;
            ApplyTropism();

        }


        /// <summary>
        /// Move Turtle on Z Axis
        /// </summary>
        /// <param name="distance">Distance to translate Turtle</param>
        public void MoveOnZAxis(double distance) {

            _plane.Origin += _plane.ZAxis * distance;
            ApplyTropism();

        }


        /// <summary>
        /// Moves the Turtle in World Coordinates
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        public void TranslateWorld(double x, double y, double z) {

            _plane.Origin += new Point3d(x, y, z);

        }

        /// <summary>
        /// Moves the Turtle in the Local XY Plane.
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        public void TranslateLocalXY(double x, double y, double z) {

            Plane local = new Plane(Plane.WorldXY.Origin , _plane.XAxis, _plane.YAxis);
            _plane.Origin += local.PointAt(x, y, z); 

        }


        /// <summary>
        /// Moves the Turtle in the Local XZ Plane.
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        public void TranslateLocalXZ(double x, double y, double z) {

            Plane local = new Plane(Plane.WorldXY.Origin, _plane.XAxis, _plane.ZAxis);
            _plane.Origin += local.PointAt(x, y, z);

        }

        /// <summary>
        /// Moves the Turtle in the Local XY Plane.
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        public void TranslateLocalYZ(double x, double y, double z) {

            Plane local = new Plane(Plane.WorldXY.Origin, _plane.YAxis, _plane.ZAxis);
            _plane.Origin += local.PointAt(x, y, z); 

        }

        /// <summary>
        /// Aligns the Turtle to the plane specified.  The current Position (Plane origin) is preserved.
        /// </summary>
        /// <param name="plane"></param>
        public void AlignToPlane(Plane plane) {

            Point3d originalOrigin = _plane.Origin;
            _plane = plane;
            _plane.Origin = originalOrigin;

        }

        #endregion


        #region "Tropism"

        /// <summary>
        /// Applies Tropism Vector and weight (if active) to Turtle Heading Vector (X Axis)
        /// </summary>
        private void ApplyTropismFirstVersion() {

            if (_tropism_active) {

                Vector3d xAxis = _plane.XAxis;

                //do no adjustment if vectors are parallel or anti-parallel
                if (xAxis.IsParallelTo(_tropism_vector, 0.01) == 0) {

                    Vector3d crossHT = Vector3d.CrossProduct(xAxis, _tropism_vector);

                    //take the arc cosine of the angle between vectors
                    double dotAcos = Math.Acos(Vector3d.Multiply(xAxis, _tropism_vector));

                    //multiple this by e value to get rotation around cross product
                    double theta = _tropism_factor * dotAcos;

                    //new heading
                    Vector3d newXAxis = xAxis;
                    //rotate x axis around cross product by theta
                    newXAxis.Transform(Transform.Rotation(theta, crossHT, Plane.WorldXY.Origin));
                    newXAxis.Unitize();

                    //construct new z axis (used only for constructing new Y)
                    Vector3d newZAxis = Vector3d.CrossProduct(newXAxis, _plane.YAxis);
                    newZAxis.Unitize();

                    //construct new Y Axis from new z
                    Vector3d newYAxis = Vector3d.CrossProduct(newZAxis, newXAxis);

                    Plane newTurtlePlane = new Plane(_plane.Origin, newXAxis, newYAxis);

                    //make sure plane is valid...
                    if (newTurtlePlane.IsValid) {
                        _plane = newTurtlePlane;
                    } else {
                        throw new Exception("Invalid Turtle Plane generated applying Tropism");
                    }

                }//end if


            }//end if apply tropism


        }//end ApplyTropism




        /// <summary>
        /// Applies Tropism per ABOP: theta=e|hXt|
        /// fixed finally?.....
        /// Still seems stronger
        /// </summary>
        private void ApplyTropism() {

            if (_tropism_active) {

                Vector3d xAxis = _plane.XAxis;

                //do no adjustment if vectors are parallel or anti-parallel
                if (xAxis.IsParallelTo(_tropism_vector, 0.01) == 0) {

                    Vector3d crossHT = Vector3d.CrossProduct(xAxis, _tropism_vector);

                    //multiple this by e value to get rotation around cross product
                    //double theta =  _tropism_bending_factor * crossHT.Length;
                    double theta = Math.Sin(_tropism_factor * crossHT.Length);

                    //new heading
                    Vector3d newXAxis = xAxis;
                    //rotate x axis around cross product by theta
                    newXAxis.Transform(Transform.Rotation(theta, crossHT, Plane.WorldXY.Origin));
                    newXAxis.Unitize();

                    //construct new z axis (used only for constructing new Y)
                    Vector3d newZAxis = Vector3d.CrossProduct(newXAxis, _plane.YAxis);
                    newZAxis.Unitize();

                    //construct new Y Axis from new z
                    Vector3d newYAxis = Vector3d.CrossProduct(newZAxis, newXAxis);

                    Plane newTurtlePlane = new Plane(_plane.Origin, newXAxis, newYAxis);

                    //make sure plane is valid...
                    if (newTurtlePlane.IsValid) {
                        _plane = newTurtlePlane;
                    } else {
                        throw new Exception("Invalid Turtle Plane generated applying Tropism");
                    }

                }//end if


            }//end if apply tropism


        }//end ApplyTropism



#endregion


        #region "Rotation"



        /// <summary>
        /// Rotates the Turtle around X Axis
        /// </summary>
        /// <param name="theta">amount to rotate in radians</param>
        public void RotateOnXAxis(double theta) {

            Transform rTrans = Transform.Rotation(theta, _plane.XAxis, _plane.Origin);
            _plane.Transform(rTrans);

        }

        /// <summary>
        /// Rotates the Turtle around Y Axis
        /// </summary>
        /// <param name="theta">amount to rotate in radians</param>
        public void RotateOnYAxis(double theta) {

            Transform rTrans = Transform.Rotation(theta, _plane.YAxis, _plane.Origin);
            _plane.Transform(rTrans);

        }

        /// <summary>
        /// Rotates the Turtle around Z Axis
        /// </summary>
        /// <param name="theta">amount to rotate in radians</param>
        public void RotateOnZAxis(double theta) {

            Transform rTrans = Transform.Rotation(theta, _plane.ZAxis, _plane.Origin);
            _plane.Transform(rTrans);

        }




        #endregion


        #region "Stack Functions"


        /// <summary>
        /// Push current Turtle position and orientation to Stack for retrieval later
        /// </summary>
        public void PushStack() {

            //Plane curPlane = _plane;
            //_stack.Add(curPlane);

            //push the state to the stack
            _state_stack.Add(new TurtleStackRecord(_plane, _obj_attributes));

        }

        /// <summary>
        /// Pop last Turtle position and orientation off the stack
        /// </summary>
        public void PopStack() {

            int cnt = _state_stack.Count;

            if (cnt > 0) {
                //pop record off stack
                TurtleStackRecord tsr = _state_stack[cnt - 1];
                _plane = tsr.turtlePlane;
                _obj_attributes = tsr.attributes;
                //remove the record from the stack
                _state_stack.RemoveAt(cnt - 1);


            } else {

                //throw an error
                throw new Exception("Invalid PopStack called on empty Turtle state stack - missing PushStack");

            }
   
        }


        #endregion


        #region "Point Stack"

        /// <summary>
        /// Pushes the current list of Points onto the stack
        /// </summary>
        public void PushPointStack() {

            //see if if there are any other open stacks
            if (_points != null) {

                _point_stack.Add(_points);
                
            }
            //init current list
            _points = new List<Point3d>();

        }


        /// <summary>
        /// Pops the current list of DroppedPoints of the stack
        /// </summary>
        /// <returns>List of Points</returns>
        public List<Point3d> PopPointStack() {

            //do something with current points...
            List<Point3d> rtn_pnts = _points;
            //index check
            int cnt = _point_stack.Count;
            if (cnt > 0) {
                //set list to last on stack
                _points = _point_stack[cnt - 1];
                //remvoe record from stack
                _point_stack.RemoveAt(cnt - 1);
            } else {
                _points = null;
            }

            return rtn_pnts;

        }

 

        /// <summary>
        /// Pushes the current Turtle position to the point stack.
        /// Can add a point to begining or end of stack.
        /// </summary>
        /// <param name="front">true = point is added to front of list, false = point is added at end of list</param>
        public void PushPoint(bool front) {

            if (_points == null) {
                throw new Exception("PushPoint called on Turtle before PushPointStack");
            }

            if (front) {
                _points.Insert(0, _plane.Origin);
            } else {
                _points.Add(_plane.Origin);
            }


        }//end append point


        #endregion


        #region "Curve Stack"


        /// <summary>
        /// Starts a new list of Curves, and pushes List of curves onto the Loft stack.  
        /// </summary>
        public void PushCurveStack() {

            if (_curves != null) {
                _curve_stack.Add(new List<Curve>(_curves));
            } 


            _curves = new List<Curve>();


        }// end push loft stack

        /// <summary>
        /// Adds a curve to the Current Curve stack
        /// </summary>
        public void DropCurve(Curve crv) {

            if (_curves == null) {

                throw new Exception("DropCurve called on Turtle before PushCurveStack");
            }

            _curves.Add(crv);

           

        }//end drop loft curve

        /// <summary>
        /// Pops the List of curves from the Loft stack.
        /// </summary>
        /// <returns></returns>
        public List<Curve> PopCurveStack() {

            //return list of curves
            List<Curve> stackCurves = null;

            //if there is a current stack item
            if (_curves != null) {

                //grab current
                stackCurves = new List<Curve>(_curves);

                _curves = null;

                int sc = _curve_stack.Count;
                if (sc != 0) {

                    _curves = _curve_stack[sc - 1];
                    _curve_stack.RemoveAt(sc - 1);

                } else {

                    _curves = null;

                }//end if /eslee

            } else {
                throw new Exception("Error during PopLoftStack:  empty stack");
            }

            return stackCurves;

        }


        #endregion



        /// <summary>
        /// Stores the Plane representation of the Turtle and the ObjectAttributes applied to geometry upon creation.
        /// </summary>
        private struct TurtleStackRecord {

            public Plane turtlePlane;


            public ObjectAttributes attributes;

            public TurtleStackRecord(Plane tp,  Rhino.DocObjects.ObjectAttributes atts) {


                turtlePlane = tp;

                attributes = atts.Duplicate();


            }

        }


    }//end class
}//end namespace
