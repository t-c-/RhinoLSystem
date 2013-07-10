using System;


//using LSystemEditor;

using Rhino;


namespace RhinoLSystem {
    class EditorExecutionHandle : LSystemEditor.IEditorExecutionHandle   {


        /// <summary>
        /// Handle for exditor to begin execution
        /// </summary>
        /// <param name="system"></param>
        public void ExecuteSystem(string system) {

            //rhino command name
            string cmd_name = RhinoCommandExecuteLSystem.Instance.EnglishName;

            try {

                
                //ignore the result, as listener will pick up any messages
                Rhino.RhinoApp.RunScript("!_" + cmd_name + " " + system, true);


            } catch (Exception ex) {

                RhinoApp.WriteLine("An error occured launching: " + cmd_name + ": " + ex.Message );

            }



        }
    


    }
}
