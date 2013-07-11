using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using LSystemExpressions;

namespace LSystemEngine {


    /// <summary>
    ///  LSystem engine revision 1
    /// </summary>
    public class LSystemEvaluationEngine {

        private static string COMMENT_SYMBOL = "#";


        //internal list of loaded systems
        private Dictionary<string, LSystem> _systems;

        //reference to modeler
        private ILSystemModeler _modeler;


        //reference to monitor object
        private ILSystemListener _listener;


        //for use in implementing recursive calls to LSystems
        private List<string> _systemCallStack;


        public LSystemEvaluationEngine(ILSystemModeler modeler, ILSystemListener listener) {


            //initialize collection of lsystems
            _systems = new Dictionary<string, LSystem>();


            //reference modeler
            _modeler = modeler;

            //reference monitor
            _listener = listener;

            //system call stack
            _systemCallStack = new List<string>();

        }//end constructor


        /// <summary>
        /// Used to set the listener - added 
        /// </summary>
        /// <param name="listener"></param>
        public void SetListener(ILSystemListener listener) {

            _listener = listener;

        }


        #region "Execution"

        /// <summary>
        /// Cencels the Execution of any Running LSystem(s).
        /// </summary>
        public void CancelExecution() {

            LSystem.Cancel();


        }



        /// <summary>
        /// Execute a loaded LSystem.
        /// This should only ever be called by the Top-Level Object.  To call a subsystem, use ExecuteSubSystem(...).
        /// </summary>
        /// <param name="lsystemName"></param>
        /// <param name="args"></param>
        public void Execute(string lsystemName, string[] args) {

            LSystem activeLSys = null;
            _systemCallStack.Clear();


            try {

                //setup the modeler
                _modeler.Initialize();


                if (_systems.ContainsKey(lsystemName)) {

                    //in future, this will need to make a copy for recursive lsystems
                    //grab lsystem
                    activeLSys = _systems[lsystemName];

                    _systemCallStack.Add(activeLSys.Name);

                    //reset any user issued cancel
                    LSystemEngine.LSystem.ClearCancel();


                    //begin execute event for listener
                    OnExecutionBegin(lsystemName);

                    //evaluate the lsystem
                    activeLSys.Evaluate(args);

                    //doesn't really do anything here, but for posterity's sake....
                    _systemCallStack.Remove(activeLSys.Name);

                    //end execute event for listener
                    OnExecutionEnd(lsystemName);

                } else {

                    throw new Exception("LSystem undefined:" + lsystemName);

                }//end if/else


                //closedown the modeler
                _modeler.CloseDown();

            } catch (Exception ex) {

                //alert the modeler things have gone wrong
                _modeler.ExecutionError();

                //notify the listener
                OnExecutionError("Execution error: ", ex);


            }//end try/catch



        }//end execute



        /// <summary>
        /// there should be only one of these entry points - rework
        /// Executes a nested LSystem.
        /// </summary>
        /// <param name="name">name of the lsystem to execute</param>
        /// <param name="args"></param>
        public void ExecuteSubSystem(string name, string[] arguments) {

            //check the call stack for circular references
            if (!_systemCallStack.Contains(name)) {

                if (_systems.ContainsKey(name)) {

                    //in future, this will need to make a copy for recursive lsystems?...
                    //grab lsystem
                    LSystem lSys = _systems[name];

                    //add the subsystem to the call stack
                    _systemCallStack.Add(name);

                    //begin execute event for listener
                    OnSubSystemExecutionBegin(name);


                    //evaluate the lsystem
                    lSys.Evaluate(arguments);


                    //remove name from call stack
                    _systemCallStack.Remove(name);

                    //end execute event for listener
                    OnSubSystemExecutionEnd(name);

                } else {

                    throw new Exception("LSystem undefined:" + name);

                }//end if/else


            } else {

                throw new Exception("Circular LSystem call detected calling nested LSystem: " + name);

            }//end if/else



        }//end ExecuteSubSystem


        #endregion



        #region "Listener Broadcasts"

        private void OnParseError(string msg, Exception ex) {
            if (_listener != null) {
                _listener.LSystemParseError(msg, ex);
            }
        }

        private void OnParseBegin() {
            if (_listener != null) {
                _listener.ParseBegin();
            }
        }

        private void OnSystemLoaded(string name) {
            if (_listener != null) {
                _listener.ParseLoadedSystem(name);
            }
        }

        private void OnParseEnd() {
            if (_listener != null) {
                _listener.ParseEnd();
            }
        }


        private void OnExecutionBegin(string name) {
            if (_listener != null) {
                _listener.LSystemExecutionBegin(name);
            }
        }

        private void OnSubSystemExecutionBegin(string name) {
            if (_listener != null) {
                _listener.SubSystemExecutionBegin(name);
            }
        }


        private void OnSubSystemExecutionEnd(string name) {
            if (_listener != null) {
                _listener.SubSystemExecutionEnd(name);
            }
        }

        private void OnExecutionEnd(string name) {
            if (_listener != null) {
                _listener.LSystemExecutionEnd(name);
            }
        }



        private void OnExecutionError(string msg, Exception ex) {
            if (_listener != null) {
                _listener.LSystemExecutionError(msg, ex);
            }
        }



        #endregion


        #region "Loading And Parsing"

        /// <summary>
        /// <para> Load LSystems from a file, calls ParseString with file contents.</para>
        /// <para> Multiple LSystems can be defined in one file.</para>
        /// </summary>
        /// <param name="filename">Name of file to load.</param>
        public void LoadFile(string filename) {

            TextReader fileReader;
            fileReader = new StreamReader(filename);
            string fileText = fileReader.ReadToEnd();

            //close th file first: parse may throw error
            fileReader.Close();
            fileReader.Dispose();

            //parse the string
            ParseString(fileText);


        }//end load file

        /// <summary>
        /// Parse string into LSystem definitions, multiple LSystem definitions can be stored in one string.  
        /// String will be parsed into file lines.
        /// </summary>
        /// <param name="text"></param>
        public void ParseString(string text) {

            int lineIndex = 0;

            //rasie parserBegin event
            OnParseBegin();

            try {

                if (String.IsNullOrEmpty(text)) {
                    throw new Exception("Cannot parse empty string.");
                }

                //prepare the systems list
                _systems.Clear();

                //support unix style
                string newLine = "\n";  // Environment.NewLine;

                string[] fileLines = text.Split(newLine.ToCharArray());


                string fileLine;
                LSystem currentLSystem = null;

                for (int l = 0; l < fileLines.Length; l++) {
                    //file line count 
                    lineIndex = l + 1;
                    //get the trimmed line
                    fileLine = fileLines[l].Trim();
                    //not an empty line
                    if (!(fileLine.Equals(""))) {

                        string ls_statement = fileLine;

                        //check for line comments!
                        int com_pos = fileLine.IndexOf(COMMENT_SYMBOL);

                        //no comments on end
                        if (com_pos >= 0) {

                            ls_statement = fileLine.Substring(0, com_pos);

                        }

                        //check for break
                        if (!String.IsNullOrEmpty(ls_statement)) {



                            //System.Console.WriteLine("parsing: " + fileLine);

                            //grab statement from above...
                            string[] tokens = ls_statement.Split(':');
                            string keyToken = tokens[0].Trim();
                            //string currentKey = null;

                            if (keyToken.Equals("lsystem")) {
                                //if there is current lsystem, close it down
                                if (currentLSystem != null) {
                                    //push lsystem to list

                                    AddLSystem(currentLSystem.Name, currentLSystem);
                                    //currentLSystem.PostLoadValidation();
                                    //_systems.Add(currentLSystem.Name, currentLSystem);

                                    //raise the event
                                    OnSystemLoaded(currentLSystem.Name);

                                    currentLSystem = null;
                                } //end if open lsystem

                                string lsName = tokens[1].Trim();


                                currentLSystem = new LSystem(this, _modeler, _listener);
                                //currentLSystem.setName(lsName);
                                currentLSystem.Name = lsName.Trim();
                                //System.Console.WriteLine("New LSystem: " + lsName);

                            } else {

                                if (currentLSystem == null) {
                                    throw new Exception("Parse LSystem Error, no LSystem declared: " + fileLine);
                                } else {
                                    //pass the tokens down...
                                    currentLSystem.Parse(tokens);
                                }

                            }//end if/else lsystem key

                        }//end if empty string after comments


                        //}//end if length
                    }//end if not empty
                }//end for

                //make sure an open lsystem is captured
                //if there is current lsystem, close it down
                if (currentLSystem != null) {
                    //push lsystem to dictionary

                    AddLSystem(currentLSystem.Name, currentLSystem);
                    //currentLSystem.PostLoadValidation();
                    //_systems.Add(currentLSystem.Name, currentLSystem);

                    //raise loaded event
                    OnSystemLoaded(currentLSystem.Name);

                    currentLSystem = null;
                } //end if open lsystem                


                if (_systems.Count == 0) {
                    throw new Exception("No LSystems Loaded.");
                }


            } catch (Exception ex) {

                //clear the systems
                _systems.Clear();

                //notify listener
                OnParseError("Parser Error on line:" + lineIndex, ex);


            }//end try/catch



            //testing...
            //System.Diagnostics.Debug.Print( this.ToString() );

            //raise parserEnd event
            OnParseEnd();

        }//end parse text


        /// <summary>
        /// Clear the engine of all loaded LSystems.  All LSystems are unloaded.
        /// </summary>
        public void Clear() {

            _systems.Clear();

        }

        /// <summary>
        /// Internal helper stub to do some basic functions post load
        /// </summary>
        /// <param name="name">name of the LSystem to add to set</param>
        /// <param name="system">LSystem Object to add</param>

        private void AddLSystem(string name, LSystem system) {

            if (_systems != null) {

                if (!_systems.ContainsKey(name)) {

                    system.PostLoadValidation();

                    _systems.Add(name, system);

                } else {
                    throw new Exception("LSystem: " + name + " is already defined: use a different name.");
                }//end if else already defined

            } else {

                throw new Exception("Internal LSystem list has not been intialized!");

            }//end if else _systems not null



        }

        /// <summary>
        /// Get a list of loaded LSystems.  Designed for GUI List Elements.
        /// </summary>
        /// <returns>Array of strings containing names of loaded LSystems</returns>
        public string[] LoadedLSystems() {

            string[] k = _systems.Keys.ToArray<string>();
            return k;


        }


        #endregion



        /// <summary>
        /// Overrides ToString, prints all loaded LSystems or empty string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            string engText = "";

            foreach (LSystem ls in _systems.Values) {

                engText += ls.ToString() + "\r\n";
            }

            return engText;

        }//end toString()





    }//end class LSystemObject




}//end namespace
