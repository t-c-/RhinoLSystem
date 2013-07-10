using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

using System.Reflection;
using System.IO;



using Rhino;
using Rhino.Geometry;

using LSystemEngine;


namespace RhinoLSystem {
    public class RhinoLSystemModeler : ILSystemModeler {

        private const string EXTERNAL_COMMAND_DIR = "Commands";

        private RhinoTurtle turtle;
        private Dictionary<string, ModelerCommand> commands;

        private RhinoDoc _rDoc;


        private string _load_report;
        private bool _load_ok;

        private int _prune_depth;

        public RhinoLSystemModeler() : this(null) {

        }

        /// <summary>
        /// Default constructor fo Modeler Object.
        /// </summary>
        public RhinoLSystemModeler(RhinoDoc doc) {

            _rDoc = doc;

            //new turtle
            turtle = new RhinoTurtle();


            //init command table
            commands = new Dictionary<string, ModelerCommand>();

             _load_report = "";
            _load_ok = true;

            //testing 
            LoadCommandsFromDirectory();

            _prune_depth = 0;

        }//end constructor


        #region "Command Loading"

        /// <summary>
        /// Load the commands from a directory at start up.
        /// This will facilitate adding of commands without having to re-compile
        /// </summary>
        private void LoadCommandsFromDirectory() {


            _load_report = "";
            _load_ok = true;


            string assmbDir = AssemblyDirectory;
            string commandsDir = Path.Combine(new string[] { AssemblyDirectory, EXTERNAL_COMMAND_DIR });

            //System.Diagnostics.Debug.Print("current dir= " + commandsDir);

  

            if (Directory.Exists(commandsDir)) {

                try {

                    string[] files = Directory.GetFiles(commandsDir, "*.dll");

                    foreach (string file in files) {


                        //System.Diagnostics.Debug.Print("current file= " + file);
                        Assembly cmdAssembly = Assembly.LoadFile(file);

                        Type[] types = cmdAssembly.GetExportedTypes();

                        //this should only expose only one type...

                        foreach (Type t in types) {

                            if (t.BaseType.Equals(typeof(ModelerCommand))) {

                                ModelerCommand mc = (ModelerCommand)cmdAssembly.CreateInstance(t.FullName);

                                if (mc != null) {

                                    RegisterCommand(mc);

                                    //System.Diagnostics.Debug.Print("loaded command: " + mc.CommandName());

                                } else {


                                    throw new Exception("Failed to create instance of: " + t.FullName);

                                }//end if/else null

                            }//end if extends modelerCommand

                        }//end foreach


                    }//end foreach

                } catch (System.UnauthorizedAccessException ex) {

                    _load_ok = false;
                    _load_report += "You may not the currect file permissions for this PlugIn Location, check your system settings" + Environment.NewLine;
                    _load_report += ex.Message + Environment.NewLine;



                } catch (Exception ex) {

                    _load_ok = false;
                    _load_report += "An unknown error has occured loading the commands: " + Environment.NewLine;
                    _load_report += ex.Message + Environment.NewLine;

                }

            } else {


                _load_ok = false;
                _load_report = "Cannot Locate Commands directory: " + Environment.NewLine;
                _load_report += Path.GetFullPath(commandsDir) + Environment.NewLine;
                //_load_report += commandsDir + Environment.NewLine;
                _load_report += "The Modeler Command library DLL should be in this location." + Environment.NewLine;

            }//end if


            //System.Diagnostics.Debug.Print("Load report:" + _load_report);

        }//end LoadCommandsFromDirectory


        /// <summary>
        /// Used this example from stackoverflow
        /// http://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
        /// </summary>
        static public string AssemblyDirectory {
            get {
                //implementing a little safety...
                try {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    return Path.GetDirectoryName(path);
                } catch {
                    return "";
                }
            }
        }


        /// <summary>
        /// Register the modeler command in the internal dictionary object.  
        /// This serves as a convenience wrapper for adding ommands to the dictionary using CommandName() as key.
        /// </summary>
        /// <param name="command"></param>
        private void RegisterCommand(ModelerCommand command) {

            string cmdName = command.CommandName();

            //malformed command name check...?
            if (!cmdName.Equals(String.Empty)) {

                //no overwrite allowed
                if (commands.ContainsKey(cmdName)) {
                    throw new Exception("Command already defined: " + cmdName);
                } else {
                    //add command to dictionary
                    commands.Add(cmdName, command);
                }//end if/else already registered


            } else {

                throw new Exception("Empty Command Name!");

            }//end if / else


        }//end registerCommand
        
        
        
        #endregion

        #region "Interface Methods"

        /// <summary>
        /// Verify's the command.  Returns -1 if the command does not exist, otherwise it returns the number of arguments for the command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public int VerifyCommand(string command) {


            if (commands.ContainsKey(command)) {
                //pull the commmand from dict
                ModelerCommand mc = commands[command];
                return mc.NumberOfArguments();

            } else {
                return -1;
            }//end if/else 


        }

        /// <summary>
        /// Main interface for LSystem.  LSystem calls this method with 'command' and 'args[]' (LSystem Alias). 
        /// </summary>
        /// <param name="command">Name of command to execute.</param>
        /// <param name="args">Array of arguments(floats)  to pass to command.</param>
        public void Command(string command, float[] args) {

            //trim the input string
            command = command.Trim();

            if (commands.ContainsKey(command)) {
                //pull the commmand from dict
                ModelerCommand mc = commands[command];
                

                //branch pruning check 
                //in pruning mode?
                if (turtle.PruneBranch) {
                    //go deeper on PushStack - otherwise mixed matched pairs of push/pop
                    //only PushStack should return true on this..
                    if (mc.PruneDeeper() ) {
                        _prune_depth += 1;
                        return;
                    }

                    if (mc.ReleasesPrune()) {
                        if (_prune_depth == 0) {
                            //release pruning here and fall through...
                            turtle.PruneBranch = false;
                        } else {
                            //go up in pruning level - need to match pairs
                            _prune_depth -= 1;
                            return;
                        }

                    } else {
                        //exit out...
                        return;
                    }
                }


                //execute command
                //doc, could be null (unlikely), but this should always be
                //executed from try/catch above, so let it slide...
                mc.Execute(turtle, _rDoc, args);


            } else {
                throw new Exception("Unknown Modeler command: " + command);
            }//end if/else registered command

        }//end command


        /// <summary>
        /// Intializes the Turtle.
        /// </summary>
        public void Initialize() {


            turtle.Reset();

            _prune_depth = 0;
            //System.Diagnostics.Debug.Print("turtle intialized");

        }

        /// <summary>
        /// Closes down the turtle.
        /// </summary>
        public void CloseDown() {

            turtle.Reset();
            _prune_depth = 0;
            //turtle = null;
            //this is handled above
            //_rDoc.Views.Redraw();

        }

        /// <summary>
        /// Part of the Modeler interface.  This is called in the event of an error
        /// During the execution cycle.  It is intended to give the modeler a chance to clenup.
        /// </summary>
        public void ExecutionError() {

            //reset turtle on error..
            turtle.Reset();
            _prune_depth = 0;

            //this is handled above
           // _rDoc.Views.Redraw();


        }

        /// <summary>
        /// Lets the Engine know if pruning is on.
        /// </summary>
        /// <returns></returns>
        public bool IsPruningOn() {

            if (turtle != null) {
                return turtle.PruneBranch;
            } else {
                //if turtle is null for some reason, don't continue...
                return true;
            }

        }

         #endregion





        /// <summary>
        /// Utility Method for pushing the doc when changed.
        /// 
        /// </summary>
        /// <param name="doc">Current Rhino Document or null</param>
        public void PushDocReference(RhinoDoc doc) {

            _rDoc = doc;

        }

        /// <summary>
        /// Confirms that the Commands were able to be loaded from the "commands" directory.
        /// </summary>
        public bool LoadResolved {
            get {
                return _load_ok;
            }

        }

        /// <summary>
        /// Report about the command load attempt.  
        /// If the Modeler fails to load the commands from disk, this will contain the error messages.
        /// </summary>
        public string LoadReport {
            get {
                return _load_report;
            }
        }


    }//end class
}//end namespace
