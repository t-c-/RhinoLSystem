using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

using System.Windows;
using System.ComponentModel;
using System.Runtime.InteropServices;



using LSystemExpressions;




namespace LSystemEngine {


    /// <summary>
    /// 
    /// </summary>

    public class LSystem {

        private static string DERIVATION_KEY = "derivations";
        private static string NULL_COMMAND_NAME = "null";


       // private bool _preserve_context;

        private ILSystemExpressionEvaluator _lang;

        private string _name;

        private int _derivations;

        //flag for user cancel
        private static volatile bool _user_issued_cancel = false;

        //
        //  Internal storage of axiom parsed from file.
        //  it is saved as a letter sequence until Execute is called
        //  where the letters are evaluated and transfered to _axiomList
        private LSystemLetterSequence _axiom;

        //List used for axiom when executing Lsystem
        private LSystemAxiomList _axiomList;

        //these are sued by productions
        private string _ignore;
        private string _branchSymbols;

        //List of Productions
        private List<LSystemProduction> _productions;

        //group dictionary
        private Dictionary<Char,LSystemGroup> _groups;

        //prepare statemnts - turtle setup
        private List<LSystemModelerPrepare> _prepare;

        //alias dictionary
        private Dictionary<Char, LSystemAlias> _aliases;

        //Dictionary of SubSystems
        private Dictionary<Char, LSystemSubSystem> _subsystems;


        //reference to modeler
        private ILSystemModeler _modeler;

        //reference to engine for calling sub l-system
        private LSystemEvaluationEngine _engine;


        //reference to listener
        private ILSystemListener _listener;

 

        //used for circular reference checking when resolving group
        private List<char> groupCallStack;





        /// <summary>
        /// Constructor - Initializes the LSystem
        /// </summary>
        /// <param name="lse">Reference to Engine above - used for calling Subsystems</param>
        /// <param name="sysModeler">Reference to the Modeler</param>
        /// <param name="listener">Listener for notification of Parsing and Execution progress or errors</param>
        public LSystem(LSystemEvaluationEngine lse, ILSystemModeler sysModeler,  ILSystemListener listener) {

            _engine = lse;

            _modeler = sysModeler;

            _derivations = -1;

            _axiomList = null;

            _axiom = new LSystemLetterSequence();

            _productions = new List<LSystemProduction>();

            _lang = new LSystemExpressionEvaluator();

            _groups = new Dictionary<Char, LSystemGroup>();

            _prepare = new List<LSystemModelerPrepare>();

            _aliases = new Dictionary<Char, LSystemAlias>();

            //testing
            _subsystems = new Dictionary<Char, LSystemSubSystem>();

            _listener = listener;

            groupCallStack = null;

            _ignore = null;
            _branchSymbols = null;

            //special switch for preserving context
            //_preserve_context = false;


            //testing this helper feature...
            SetConvenienceVariables();

        }//end constructor


        /// <summary>
        /// Testing for convenience variables.
        /// These must be coordinated with the Modeler command static functions!
        /// Should keep these and expand listing...
        /// </summary>
        private void SetConvenienceVariables() {

            //plane indexes
            _lang.SetVariable("PLANE_XY", 0);
            _lang.SetVariable("PLANE_YZ", 1);
            _lang.SetVariable("PLANE_XZ", 2);

            //axis indexes
            _lang.SetVariable("AXIS_X", 0);
            _lang.SetVariable("AXIS_Y", 1);
            _lang.SetVariable("AXIS_Z", 2);

            //destination indexes
            _lang.SetVariable("D_STACK", 0);
            _lang.SetVariable("D_DOC", 1);
            _lang.SetVariable("D_BOTH", 2);

            //boolean values
            _lang.SetVariable("TRUE", 1);
            _lang.SetVariable("FALSE", 0);

        }


        /// <summary>
        /// Name of the LSystem.
        /// </summary>
        public string Name {

            get {
                return _name;
            }
            set {
                _name = value;
            }

        }//end name property

        /// <summary>
        /// Cleans up necessary items upon Cancel.  Notifies Modeler of Cancel.
        /// </summary>
        private void CancelCleanup() {

            if (_modeler != null) {
                //throw an error for the nodeler
                _modeler.ExecutionError();
            }


        }//end cancel cleanup

        /// <summary>
        /// Issues Cancel on running LSystem.
        /// </summary>
        public static void Cancel() {

            _user_issued_cancel = true;

        }

         /// <summary>
        /// Clears a user issued cancel.
        /// </summary>
        public static void ClearCancel() {

            _user_issued_cancel = false;

        }

        /// <summary>
        /// Helper function to dynamically set update rate to help performance for DoEvents calls
        /// </summary>
        /// <param name="count">current list count to adapt to</param>
        /// <returns>update refresh rate</returns>
        private int UpdateEventsAndCheckRate(int count) {

            int _cr_low = 100;
            int _cr_mid = 10000;
            int rate = Convert.ToInt32(count * 0.1);

            if (count <= _cr_mid && count > _cr_low) {
                //use 1% updates
                rate = Convert.ToInt32(count * 0.01);

            } else if (count > _cr_mid) {
                //use 0.1% updates
                rate = Convert.ToInt32(count * 0.001);
            }

            return rate;

        }


        /// <summary>
        /// Evaulate the LSystem.
        /// </summary>
        public void Evaluate(  string[] args) {


            int _cancel_check_rate = 1;
            int _cancel_check_count = 0;

            //inform listener
            OnEvaluationBegin();

            LSystemAxiomList _temp_axiom = new LSystemAxiomList();



            //set the language to default variable state
            _lang.RestoreVariableState();

            //push all args to language
            //  these will all be global overrides
            if (args != null) {

                for (int i = 0; i < args.Length; i += 2) {

                    string var = args[i];
                    string exp = args[i + 1];
                    _lang.SetVariable(var, exp);

                }//end for

            }//end if args


            //System.Diagnostics.Debug.Print("evaluating: " + _name );
            //System.Diagnostics.Debug.Print("axiom: " + _axiom);
            //use this iteration value holder for Evaluate
            int evalDerivations = 0;

            //check for passed iteration value
            if (_lang.IsVariable(DERIVATION_KEY)) {
                evalDerivations = (int)_lang.GetVariable(DERIVATION_KEY);
            } else {
                //otherwise use defined value
                evalDerivations = _derivations;
            }//end if

            //first step is to run axiom through interpreter to set values...
            _axiom.EvaluateLetters(_lang);

            //now that the letters have been evaluated, create the linked list used for
            //  the rest of the operations on the axiom list
            _axiomList = new LSystemAxiomList(_axiom);



            //main loop for derivations
            for (int i = 0; i < evalDerivations; i++) {

                //check for cancel flag at begining of each iteration
                System.Windows.Forms.Application.DoEvents();
                if (_user_issued_cancel) { CancelCleanup(); return; }
      

                //debugging file
                //WriteDebugFile(_name + "-it" + i, _axiomList.ToString());

                //add iterations to lanuage for access
                //_d = derivation - make sure it starts as 1
                _lang.SetVariable("_d", i + 1);

                //pop the first letter
                LSystemAxiomListNode letterNode = _axiomList.First;

                int lCount = _axiomList.Count;
                int cLtr = 1;

                //setup the update/display process rate 
                _cancel_check_rate = UpdateEventsAndCheckRate(lCount);

                //set this to trigger on first run
                _cancel_check_count = _cancel_check_rate + 1;

                //
                //   ======  Check Each Letter Against Productions =====
                //
                //  processing loop for letters in axiom list
                //    if it's the last node, node.Next = null
                while (letterNode != null) {

                    //testing
                    //this helps out
                    if (_cancel_check_count >= _cancel_check_rate) {

                        //do update here.. 
                        OnEvaluationProgress(_name, i + 1, evalDerivations, cLtr, lCount);
                        //optimized for speed by doing this only periodically - it's slow
                        //check for cancel
                        System.Windows.Forms.Application.DoEvents();
                        if (_user_issued_cancel) { CancelCleanup(); return; }
                        
                        _cancel_check_count = 0;
                    }

                    _cancel_check_count += 1;

                    //grab current letter to test against productions
                    LSystemLetter curLtr = letterNode.Letter;



                    //   ******* Production Section **********

                    // *** remember to clear out temporary 
                    //     variables after production testing

                    //run through productions
                    int p = 0;
                    int pCount = _productions.Count;
                    //write current letter
                    //Console.WriteLine("current letter=" + curLtr);

                    //look for succesor
                    //while (p < pCount ) {
                    for (p = 0; p < pCount; p++) {

                        //get current production
                        LSystemProduction curP = _productions[p];

                        //Console.WriteLine("checking production: " + curP);
  
                        //test current letter against production
                        //this will set local variables defined in production
                        bool isMatch = curP.Matches(letterNode);


                        if (isMatch) {

                            //System.Diagnostics.Debug.Print("Production Matches!");

                            //successor
                            LSystemLetterSequence suc = curP.Successor;

                            //evaluate the letters against the language
                            //  local conditional variables have been set in .Matches(letterNode)
                            suc.EvaluateLetters(_lang);

                            //  apply the successor
                            //  this is why context matching is working!!!!
                            //  successors and letters need to go to new list!

                          //  if (_preserve_context) {

                                //  this the new (correct) method!
                               // _temp_axiom.AddSequenceToEnd(suc);

                           // } else {

                                //  this is changeing the context!!!!
                                _axiomList.ReplaceNode(letterNode, suc);


                           // }



                            //exit the loop of productions
                            break;

                            //new else statemetn - instead of letter falling through, push to temp list
                        } else {

                            //check to see if preserve context is on
                            //otherwise let the letter fall through (stays in axiom list)
                            //if (_preserve_context) {
                            //    //put a copy on end of list
                            //    _temp_axiom.AddLast(letterNode.Clone());
                            //}

                        }//end if


                        // *** clear out temporary variables ***
                        _lang.ClearTemporaryVariables();

                        //increment production index
                        //p += 1;

                    }//end  for

                    
                    //iterate next
                    //  this is still a valid operation 
                    //  even if the letter node has been replaced
                    letterNode = letterNode.Next;
                    cLtr += 1;

                }//end while - processing letters in axiomList

                //check flag for preserve context
                //if (_preserve_context) {
                //    //holy shit batman!  This kills it! out of RAM, End Task, arghhh..
                //    //new corrected testing for proper context matching!!
                //    //swap list references here on each iteration....
                //    _axiomList.Clear();
                //    _axiomList = _temp_axiom;
                //    _temp_axiom = new LSystemAxiomList();
                //}


               //do this once here outside of loop 
               // at the end of each iteration since not all updates sent:
                OnEvaluationProgress(_name, evalDerivations, evalDerivations, lCount, lCount);

            }//end for derivations

            //notify monitor
            OnEvaluationEnd();

            //lowest level commands are instructions to the modeler
           // modeler.command("test", null);


            /*
             * 
             *    Evaluate final axiom sequence.
             * 
             */

            //check for user cancel flag
            //this call is slowing things down, but necessary for a single threaded execution
            //  to allow proper updates of Progress bars in UI...
            //  multi threaded execution runs didn't need this call...
            System.Windows.Forms.Application.DoEvents();
            if (_user_issued_cancel) { CancelCleanup(); return; }


            //entry point to implement "prepare" statements
            //for future...  allow preset turtle preparation calls 
            // to set tropism, alignment, default decoration, etc..

            /*
             *          Modeler Prepare Commands
             * */

            foreach (LSystemModelerPrepare mp in _prepare) {

                string[] cmdArgs = mp.Arguments;

                //evaluate all arguments in command
                float[] argVals = new float[cmdArgs.Length];

                //evaluate the srguments
                for (int n = 0; n < cmdArgs.Length; n++) {
                    argVals[n] = _lang.Evaluate(cmdArgs[n]);

                }//end for 

                //issue command to modeler
                _modeler.Command(mp.Command, argVals);

            }//end foreach

            //prepare object caller stack
            //this is used for circular reference check since objects can be reference other objects
            groupCallStack = new List<char>();

            LSystemAxiomListNode resolveNode = _axiomList.First;


            int resolve_count = _axiomList.Count;
            int ltrs_resolved = 0;


            //setup the cancel rate 
            _cancel_check_rate = UpdateEventsAndCheckRate(resolve_count); 
            //trigger first update on start
            _cancel_check_count = _cancel_check_rate + 1;

            //notify monitor
            OnResolverBegin();

            //axiom is completed - start resolving letters...
            while (resolveNode != null) {

                //this helps out
                if (_cancel_check_count >= _cancel_check_rate) {

                    //send update
                    OnResolverProgress(_name, ltrs_resolved, resolve_count);
                    //this is getting called for all letters! no wonder it's so slow....
                    //check for cancel
                    System.Windows.Forms.Application.DoEvents();
                    //previously break was used here.... use return !?
                    if (_user_issued_cancel) { CancelCleanup(); return; }

                    _cancel_check_count = 0;
                }

                _cancel_check_count += 1;


                 //resolve the current letter
                //old way - check to see if copy of letter is needed...
                //ResolveLetter( new LSystemLetter (resolveNode.Letter));
                ResolveLetter(new LSystemLetter(resolveNode.Letter));
                //increment to next
                resolveNode = resolveNode.Next;

                ltrs_resolved += 1;

            }//end while


            //testing for progress update - make sure all are counted
            OnResolverProgress(_name, ltrs_resolved, resolve_count);

            //notify monitor
            OnResolverEnd();

            //let the events through
            System.Windows.Forms.Application.DoEvents();

        }//end evaluate


        /// <summary>
        /// new version
        /// Resolves a letter down through the groups to it's final Alias call.
        /// </summary>
        /// <param name="letter">Letter to resolve</param>
        private void ResolveLetter(LSystemLetter letter) {

            //System.Windows.Forms.Application.DoEvents();
            //  *** this needs to be done here! ***
            // if it isn't, the turtle can throw errors as commands still get through
            if (_user_issued_cancel) { CancelCleanup(); return; }

            //grab the current letter
            char currentKey = letter.key;


            /*
             *   Check Groups for Letter
             */

            if (_groups.ContainsKey(currentKey)) {

                //check for circular references since recursion is permitted here
                if (!groupCallStack.Contains(currentKey)) {

                    //push this group to stack record
                    groupCallStack.Add(currentKey);

                    LSystemGroup curGroup = _groups[currentKey];
                    LSystemLetter grpKeyLetter = curGroup.KeyLetter;

                    //resolve template object parameters
                    PutTemplateToLang(grpKeyLetter, letter);

                    //evaluate the object letters
                    curGroup.EvaluateLetters(_lang);

                    //loop through group letters and resolve them
                    for (int gl = 0; gl < curGroup.Count(); gl++) {

                        //start recursive resolve process
                        //allows objects to point to other objects
                        LSystemLetter grpLetter = curGroup.LetterAt(gl);

                        //  +++++++  Resolve letter ++++++
                        //  recursive entry point
                        ResolveLetter(grpLetter);

                    }//end for letters in object

                    //remove this group in the call stack (last one)
                    groupCallStack.RemoveAt(groupCallStack.Count - 1);


                    //*** Clear Temporary Variables
                    //    These are set by PutTemplateToLang ***
                    _lang.ClearTemporaryVariables();

                    //exit the function cleanly to avoid error
                    return;

                } else {

                    throw new Exception("Circular Reference detected resolving group: " + currentKey);
                } //end if/ else call stack check



            }//end if letter in groups


            /*
             *   Check in Aliases
             */

            if (_aliases.ContainsKey(currentKey)) {

                LSystemAlias alias = _aliases[currentKey];
                string mCmd = alias.Command;

                //intercept null here ignoring parameters
                if (mCmd != NULL_COMMAND_NAME) {

                    PutTemplateToLang(alias.KeyLetter, letter);
                    string[] cmdArgs = alias.Arguments;

                    //evaluate all arguments in command
                    float[] argVals = new float[cmdArgs.Length];

                    for (int n = 0; n < cmdArgs.Length; n++) {
                        argVals[n] = _lang.Evaluate(cmdArgs[n]);

                    }//end for 

                    //issue command to modeler
                    _modeler.Command(alias.Command, argVals);

                }//end if

                //*** Clear Temporary Variables
                //    These are set by PutTemplateToLang ***
                _lang.ClearTemporaryVariables();

                //exit the function on success to avoid error
                return;

            }//end if letter in aliases


            /*
             * Check in Sub-Systems
             */

            if (_subsystems.ContainsKey(currentKey)) {

                //check for pruning - shouldn't execute 
                //  subsystem while pruning is active
                if (_modeler.IsPruningOn()) {
                    //clean exit...
                    return;
                }
                //grab current
                LSystemSubSystem subSys = _subsystems[currentKey];

                //resolve template  parameters
                PutTemplateToLang(subSys.KeyLetter, letter);

                //grab the argument values
                string[] arguments = subSys.Arguments;

                for (int i = 0; i < arguments.Length; i += 2) {

                    float result = _lang.Evaluate(arguments[i + 1]);
                    //repack the result as string to pass on
                    arguments[i + 1] = result.ToString();

                }//end for

                //*** Clear Temporary Variables
                //    These are set by PutTemplateToLang ***
                _lang.ClearTemporaryVariables();

                //execute the sub-system
                _engine.ExecuteSubSystem(subSys.Name, arguments);

                //exit cleanly to avoid error
                return;

            }//end if letter in subsystems


            /*
             *  If Letter passes to here wiothout being found throw an error 
             * 
             */

            throw new Exception("Unresolvable letter: " + letter +  ",  Definition does not exist.  Letter must be defined as Group, Alias, or SubSystem.");


        }///end resolve letter


        ///<summary >
        ///Takes the values stored in value and assigns the variables defined in the template in the current Language (Expression Evaluator).
        ///This is the main mechanism for assigning the parametric values.  This uses the temporay variable section of the lang.
        /// </summary>
        /// <param name="template">Template of vairable names</param>
        /// <param name="value" >Values of variables</param>
        private void PutTemplateToLang(LSystemLetter template, LSystemLetter value) {

            int tp = template.parameters;
            int vp = value.parameters;

            //check number of parameters
            if (tp == vp) {

                //check for parameters to process F(a b)+(w)
                //no parameters is legit for simple letters: Ff+-, etc..
                if (tp != 0 && vp != 0) {

                    //parse templates to values
                    string[] lt = template.template;
                    float[] lv = value.values;

                    for (int pt = 0; pt < lt.Length; pt++) {
                        // set temporary variables
                        _lang.SetTemporaryVariable(lt[pt], lv[pt]);
                    }//end for


                }//end if parameters

            } else {
                throw new Exception("Mixed matched number of parameters in letter: " + template + " compared against object: " + value);
            }



        }//end putTemplateToLang




        #region "ListenerCalls"


        private void OnEvaluationBegin() {
            if (_listener != null) {
                _listener.EvaluationBegin( _name);
            }
        }


        private void OnEvaluationProgress(string name, int iteration, int ofIterations, int letter, int ofLetters) {
            if (_listener != null) {
                _listener.EvaluationProgress(_name, iteration, ofIterations, letter, ofLetters);
            }
        }


        private void OnEvaluationEnd() {
            if (_listener != null) {
                _listener.EvaluationEnd( _name );
            }
        }

        private void OnResolverBegin() {
            if (_listener != null) {
                _listener.ResolverBegin(_name);
            }
        }


        private void OnResolverProgress(string name, int letter, int ofLetters) {
            if (_listener != null) {
                _listener.ResolverProgress(_name, letter, ofLetters);
            }
        }


        private void OnResolverEnd() {
            if (_listener != null) {
                _listener.ResolverEnd(_name);
            }
        }

        #endregion


        #region "Parsing"


        /// <summary>
        /// Parse a file.  Lines are passed as tokens split() by deliminator- currently ":" (colon)
        /// (key) : (value) : (value) : (value)...
        /// </summary>
        /// <param name="fileText"></param>
        public void Parse(string[] tokens) {



            string keyToken = tokens[0].Trim();


            //check token
            if (keyToken.Equals("derivations")) {

                if (tokens.Length == 2) {

                    //set iterations to 0
                    _derivations = 0;

                    if (! Int32.TryParse(tokens[1], out _derivations)) {
                        throw new Exception("Invalid derivations specification: missing or invalid integer value.");
                    } 

                } else {

                    throw new Exception("Invalid derivations specification, expecting: derivations : <integer>");

                }
            }//end if iterations
            else if (keyToken.Equals("axiom")) {

                if (tokens.Length == 2) {

                    string aToken = tokens[1].Trim();


                    if (aToken == string.Empty) {
                        throw new Exception("Invalid empty axiom specification: missing specification.");
                    } else {
                        _axiom.Parse(aToken);
                    }

                } else {

                    throw new Exception("Invalid axiom specification, expecting: axiom : <letter sequence>");

                }


            }//end axiom
            //set variables
            else if (keyToken.Equals("define")) {


                if (tokens.Length == 3) {

                    string varName = tokens[1].Trim();
                    string varExpression = tokens[2].Trim();

                    if (varName == string.Empty) {
                        throw new Exception("Invalid empty define specification: missing variable name.");
                    }

                    if (varExpression == string.Empty) {
                        throw new Exception("Invalid empty define specification: missing variable expression.");
                    }

                    //define sets the variable as a global variable in the lang
                    //  all otehr places should only use temporary variables
                    _lang.SetVariable(varName, varExpression);

                } else {

                    throw new Exception("Invalid define statement, expecting: define : <variable name> : <variable expression> ");

                }

            }//end define
            else if (keyToken.Equals("ignore")) {

                if (tokens.Length == 2) {

                    _ignore = tokens[1].Trim();

                    if (_ignore == string.Empty) {
                        throw new Exception("Invalid empty ignore specification: missing specification.");
                    }
                    
                } else {

                    throw new Exception("Invalid ignore specification, expecting: ignore : <string>");

                }
            
                

            }//end ignore section
            else if (keyToken.Equals("branchsymbols")) {

                if (tokens.Length == 2) {

                    _branchSymbols = tokens[1].Trim();

                    if (_branchSymbols == String.Empty) {

                        throw new Exception("Invalid empty branchSymbols specification: missing specification.");
                    }

                    if (_branchSymbols.Length != 2) {

                        throw new Exception("Invalid branchSymbols specification: must be only two characters.");
                    }

                } else {

                    throw new Exception("Invalid branchSymbols specification, expecting: branchSymbols : <string>");

                }
            }//end branchSymbols section
            else if (keyToken.Equals("production")) {

                LSystemProduction pObj = new LSystemProduction(_lang);
                pObj.Parse(tokens);
                _productions.Add(pObj);

            }  else if (keyToken.Equals("group")) {
                LSystemGroup tmpGrp = new LSystemGroup();
                tmpGrp.Parse(tokens);

                if (!_groups.ContainsKey(tmpGrp.KeyLetter.key)) {
                    _groups.Add(tmpGrp.KeyLetter.key, tmpGrp);
                } else {
                    throw new Exception("Group already defined: " + tmpGrp);
                }

            } else if (keyToken.Equals("prepare")) {

                LSystemModelerPrepare tmpPrepare = new LSystemModelerPrepare();
                tmpPrepare.Parse(tokens);

                int args = tmpPrepare.Arguments.Length;
                string name = tmpPrepare.Command;

                //check the command, make sure it is valid
                VerifyCommand(name, args, false);

                //add the prepare statement
                _prepare.Add(tmpPrepare);
            
        } else if (keyToken.Equals("alias")) {

                LSystemAlias tmpAlias = new LSystemAlias();
                tmpAlias.Parse(tokens);
                
                int args =  tmpAlias.Arguments.Length;
                string name = tmpAlias.Command;

                //check the command, make sure it is valid
                VerifyCommand(name, args, true);


                if (!_aliases.ContainsKey(tmpAlias.KeyLetter.key)) {
                    _aliases.Add(tmpAlias.KeyLetter.key, tmpAlias);
                } else {
                    throw new Exception("Alias already defined: " + tmpAlias);
                }

            } else if (keyToken.Equals("subsystem")) {

                LSystemSubSystem subSys = new LSystemSubSystem();

                subSys.Parse(tokens);
                //add the subsystem
                _subsystems.Add(subSys.KeyLetter.key, subSys);


            } else {

                throw new Exception("Unknown key in parse LSystem: " + keyToken);

            }



        }//end parseString


        /// <summary>
        /// Verify the command is specified correctly in it's basic form
        /// </summary>
        /// <param name="name">command name</param>
        /// <param name="args">number of srguemnts provided</param>
        /// <param name="is_alias">is context alias? (or prepare=false)</param>
        private void VerifyCommand(string name, int args, bool is_alias) {

            bool allow_null = false;
            string context = "<undef>";
            if (is_alias) {
                allow_null = true;
                context = "alias";
            } else {
                context = "prepare";

            }

            //verify the alias against the modeler command
            //this is so we don't have to wait for a runtime error in the Resolver cycle
            //null commands are not verified (they don't exit in modeler)
            if (name != NULL_COMMAND_NAME) {

                int verify = _modeler.VerifyCommand(name);

                if (verify == -1) {

                    throw new Exception("Invalid " + context + ": Modeler Command: " + name + " is not defined in the current modeler.");

                } else if (verify == Int32.MinValue) {

                    throw new Exception("Invalid " + context + ": Modeler Command: " + name + " has an error in it's definition: it has not defined the number of arguments.");

                } else if (verify < 0) {

                    throw new Exception("Invalid " + context + ": Modeler Command: " + name + " has an error in it's definition: invalid number of arguments: " + verify);

                } else if (verify != args) {


                    throw new Exception("Invalid " + context + ": Modeler Command: " + name + " is expecting: " + verify + " arguments.  ");

                }

            } else {
                //are nulls permitted in context?
                if (!allow_null) {
                    throw new Exception("Invalid " + context + ": " + NULL_COMMAND_NAME + " , null commands are not allowed in a prepare statements");
                }

            }//end if verify non-null

        }



        /// <summary>
        /// TODO - check the LSystem after loading from file - basic checking currently </br>
        /// Currently this just propagates the Ignore and Branch Symbols to all the productions
        ///
        /// </summary>
        public void PostLoadValidation() {

            //there must be an axiom
            if (_axiom.Count == 0) {

                throw new Exception("No axiom defined for LSystem: " + _name);

            }

            int alias_cnt = _aliases.Count;
            int subsys_cnt = _subsystems.Count;

            //there must be at least one alias or subsystem defined
            if (alias_cnt == 0 && subsys_cnt == 0) {
                throw new Exception("No aliases or subsystems  defined for LSystem: " + _name + ".  LSystem must have at least one alias  or subsystem defined.");
            }



            //propagate branchsymbols and ignore
            foreach (LSystemProduction p in _productions) {

                //set branch or leave production default
                if (_branchSymbols != null) {
                    p.SetBranchSymbols(_branchSymbols);
                }

                //set ignore or leave production default
                if (_ignore != null) {
                    p.SetIgnoreSymbols(_ignore);
                }


                //special case for left context!
                //if there is a search for left context
                // then preserve context must be turned on?  Researching correct implementation...
                // currently this slows the system down severely!!!!
                if (p.HasLeftContext) {
                   // _preserve_context = true;
                }



            }//end foreach


            //capture variable state in lang
            //this saves a snapshot of the defined varaibles and default lang variables.
            _lang.SaveVariableState();



        }//end PostLoadValidation


        /// <summary>
        /// Stub for future....
        /// </summary>
        /// <param name="key"></param>
        /// <param name="found"></param>
        /// <param name="parameters"></param>
        private void LocateLetterDefinition(char key, ref bool found, ref int parameters) {


            //look for letter in groups
            if (_groups.ContainsKey(key)) {


                LSystemLetter ltr = _groups[key].KeyLetter;
                parameters = ltr.parameters;
                found = true;
                return;


            }

            //look for letter in aliases
            if (_aliases.ContainsKey(key)) {

               
                LSystemLetter ltr = _aliases[key].KeyLetter;
                parameters = ltr.parameters;
                found = true;
                return;

            }


            //look for letter in subsystems
            if (_subsystems.ContainsKey(key)) {


                LSystemLetter ltr = _subsystems[key].KeyLetter;
                parameters = ltr.parameters;
                found = true;
                return;


            }//end if

            //made it this far, nothing found...
            found = false;
            parameters = -1;


    }



        #endregion


        #region "To String"
        /// <summary>
        /// Prints a description of the LSystem
        /// </summary>
        /// <returns></returns>
        public override string ToString() {


            string sysText = "lsystem: " + _name + "\n";


            sysText += "derivations: " + _derivations + "\n";
            sysText += "axiom: " + _axiom + "\n";

            //productions
            foreach (LSystemProduction p in _productions) {
                sysText += p + "\n";
            }

            foreach (LSystemGroup grp in _groups.Values) {

                sysText += "group: " + grp + "\n";

            }

            foreach (LSystemModelerPrepare mp in _prepare) {

                sysText += "prepare: " + mp + "\n";
            }

            foreach (LSystemAlias sa in _aliases.Values) {

                sysText += "alias: " + sa + "\n";
            }


            //print lang
            sysText += "\n" + _lang + "\n";

            return sysText;
        }//end toString


        #endregion



    }//end class
}//end namespace
