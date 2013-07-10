using System;
using System.Collections.Generic;
using System.Text;

///
///
///  New version - new version brought over from Rhino Expression Evaluator
///


namespace LSystemExpressions {

    /// <summary>
    /// Enum for expression token type
    /// </summary>
    public enum ExpressionTokenType : int {
        UnAssigned,
        Variable,
        Number,
        Expression,
        UnaryOperator,
        MultiplicitiveOperator,
        AdditiveOperator,
        LogicalOperator,
        ConditionalOperator,
        Assignment,
        GeneralOperator,
        Function,
        Null
    };


    public struct ExpressionToken {

        public string Id;
        public ExpressionTokenType Type;
        public float Value;

        //constructor
        public ExpressionToken(string identity, ExpressionTokenType tokenType) {
            Id = identity;
            Type = tokenType;
            Value = float.NaN;
        }

        //constructor
        public ExpressionToken(string identity, ExpressionTokenType tokenType, float intialValue) {
            Id = identity;
            Type = tokenType;
            Value = intialValue;
        }

        //pretty print for debug
        public override string ToString() {
            string text = "token: '" + Id + "', of type: " + Type.ToString();
            if (Type == ExpressionTokenType.Number || Type == ExpressionTokenType.Variable) {
                text += ", value= " + Value;
            }

            return text;
        }//end ToString

    }//end struct


    public struct ExpressionTokenRecord {

        public ExpressionTokenType TokenType;
        public string Key;
        public UInt16 Rank;

        public ExpressionTokenRecord(string key, ExpressionTokenType type, UInt16 rank) {

            Key = key;
            TokenType = type;
            Rank = rank;

        }

        public override string ToString() {
            return Key + " :: " + TokenType.ToString() + " [" + Rank + "]";
        }

    }//end struct



    /// <summary>
    /// Main class for evaluating expressions
    /// </summary>
    public class LSystemExpressionEvaluator  : ILSystemExpressionEvaluator {

        private static string _operatorChars = "*/+-%=><!&|";

       //internal random number generator
       // always intialize so function doesn't need to
        private Random _random;

        private int _stack_depth;

        //main variable dictionary
        private Dictionary<string,float> _variables;

       //backup variable dictionary
       //need to maintain thevariable state as it was originally defined
        private Dictionary<string, float> _original_variables;

        //temporary variables
        private Dictionary<string, float> _temp_variables;

        private Dictionary<string, ExpressionTokenRecord> _token_table;

        private Dictionary<string, ExpressionTokenRecord> _function_table;

        private Dictionary<string, ExpressionTokenRecord> _unary_table;

        /// <summary>
        /// Enum for tracking state of stack.  This tells what is on the stack: a number or symbol or nothing
        /// </summary>
        private enum EvalStackType : int {

            Empty = 0,
            Symbol = 1,
            Number = 2

        };


        /// <summary>
        /// Standard Constructor.  Variable dictionary is intialized here.
        /// </summary>
        public LSystemExpressionEvaluator() {

            //random number generator
            _random = new Random();


            //intialize variable storage
            _variables = new Dictionary<string,float>();

            _original_variables = null; // new Dictionary<string, float>();

            //temporary variable dictionary
            _temp_variables = new Dictionary<string, float>();

            _token_table = new Dictionary<string, ExpressionTokenRecord>();

            _function_table = new Dictionary<string, ExpressionTokenRecord>();

            _unary_table = new Dictionary<string, ExpressionTokenRecord>();

            //set teh defaults: pi,e,phi, etc
            SetDefaultVariables();


            //stack depth debugging
            _stack_depth = -1;


            BuildTables();

        }//end constructor


       /// <summary>
       /// Sets some common mathematical constants
       /// </summary>
        private void SetDefaultVariables() {

            //set some initial variables
            //pi, half pi, and quater pi = 180/90/45
            SetVariable("pi", (float)Math.PI);
            SetVariable("hpi", (float)(Math.PI * 0.5));
            SetVariable("qpi", (float)(Math.PI * 0.25));
            //2 pi
            SetVariable("tau", (float)(Math.PI * 2.0));

            //e
            SetVariable("e", (float)Math.E);

            //define golden ratio
            SetVariable("phi", (float)((1 + Math.Sqrt(5)) / 2));



        }//end set default variables


       /// <summary>
       /// Build the token, unary, and function tables
       /// </summary>
        private void BuildTables() {


            //additive
            RegisterToken(new ExpressionTokenRecord("+", ExpressionTokenType.AdditiveOperator, 80));
            RegisterToken(new ExpressionTokenRecord("-", ExpressionTokenType.AdditiveOperator, 80));

            //multiplicative operators
            RegisterToken(new ExpressionTokenRecord("*", ExpressionTokenType.MultiplicitiveOperator, 90));
            RegisterToken(new ExpressionTokenRecord("/", ExpressionTokenType.MultiplicitiveOperator, 90));
            RegisterToken(new ExpressionTokenRecord("%", ExpressionTokenType.MultiplicitiveOperator, 90));

            //assignment
            RegisterToken(new ExpressionTokenRecord("=", ExpressionTokenType.Assignment, 10));
            RegisterToken(new ExpressionTokenRecord("+=", ExpressionTokenType.Assignment, 10));
            RegisterToken(new ExpressionTokenRecord("-=", ExpressionTokenType.Assignment, 10));
            RegisterToken(new ExpressionTokenRecord("*=", ExpressionTokenType.Assignment, 10));
            RegisterToken(new ExpressionTokenRecord("/=", ExpressionTokenType.Assignment, 10));
            RegisterToken(new ExpressionTokenRecord("%=", ExpressionTokenType.Assignment, 10));

            //logical
            RegisterToken(new ExpressionTokenRecord("<", ExpressionTokenType.LogicalOperator, 70));
            RegisterToken(new ExpressionTokenRecord(">", ExpressionTokenType.LogicalOperator, 70));
            RegisterToken(new ExpressionTokenRecord("<=", ExpressionTokenType.LogicalOperator, 70));
            RegisterToken(new ExpressionTokenRecord(">=", ExpressionTokenType.LogicalOperator, 70));
            RegisterToken(new ExpressionTokenRecord("==", ExpressionTokenType.LogicalOperator, 40));
            RegisterToken(new ExpressionTokenRecord("!=", ExpressionTokenType.LogicalOperator, 40));

            RegisterToken(new ExpressionTokenRecord("&", ExpressionTokenType.ConditionalOperator, 30));
            RegisterToken(new ExpressionTokenRecord("|", ExpressionTokenType.ConditionalOperator, 20));

            //_unary_operators operators
            RegisterUnary(new ExpressionTokenRecord("-", ExpressionTokenType.UnaryOperator, 100));
            RegisterUnary(new ExpressionTokenRecord("!", ExpressionTokenType.UnaryOperator, 100));
            RegisterUnary(new ExpressionTokenRecord("++", ExpressionTokenType.UnaryOperator, 100));
            RegisterUnary(new ExpressionTokenRecord("--", ExpressionTokenType.UnaryOperator, 100));

            //functions
            RegisterFunction(new ExpressionTokenRecord("sqrt", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("pow", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("mod", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("ceil", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("floor", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("exp", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("log", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("log10", ExpressionTokenType.Function, 0));
            //trig
            RegisterFunction(new ExpressionTokenRecord("cos", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("sin", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("tan", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("cosh", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("sinh", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("tanh", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("acos", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("asin", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("atan", ExpressionTokenType.Function, 0));

            //angular degrees
            RegisterFunction(new ExpressionTokenRecord("deg", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("rad", ExpressionTokenType.Function, 0));

            //round & random
            RegisterFunction(new ExpressionTokenRecord("round", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("rnd", ExpressionTokenType.Function, 0));
            RegisterFunction(new ExpressionTokenRecord("rndint", ExpressionTokenType.Function, 0));


        }//end BuildTokenTable

       /// <summary>
       /// Helper wrapper to add to dictionary
       /// </summary>
       /// <param name="tokenRecord"></param>
        private void RegisterToken(ExpressionTokenRecord tokenRecord) {

            if (_token_table.ContainsKey(tokenRecord.Key)) {
                // throw new Exception("Token Record: " + tokenRecord.Key + " is already defined.");
                System.Diagnostics.Debug.Print("Token Record: " + tokenRecord.Key + " is already defined.");
            } else {
                _token_table.Add(tokenRecord.Key, tokenRecord);
            }

        }

       /// <summary>
        /// Helper wrapper to register unary operator in dictionary
       /// </summary>
       /// <param name="tokenRecord"></param>
        private void RegisterUnary(ExpressionTokenRecord tokenRecord) {

            if (_unary_table.ContainsKey(tokenRecord.Key)) {
                // throw new Exception("Token Record: " + tokenRecord.Key + " is already defined.");
                System.Diagnostics.Debug.Print("Token Record: " + tokenRecord.Key + " is already defined as Unary Operator.");
            } else {
                _unary_table.Add(tokenRecord.Key, tokenRecord);
            }

        }


       /// <summary>
       /// Helper wrapper to register function
       /// </summary>
       /// <param name="tokenRecord"></param>
        private  void RegisterFunction(ExpressionTokenRecord tokenRecord) {

            if (_function_table.ContainsKey(tokenRecord.Key)) {
                //throw new Exception("Token Record: " + tokenRecord.Key + " is already defined.");
                System.Diagnostics.Debug.Print("Token Record: " + tokenRecord.Key + " is already defined as Function.");
            } else {
                _function_table.Add(tokenRecord.Key, tokenRecord);
            }

        }//end RegisterFunction

        /// <summary>
        /// Checks the variable name for validity and throws an error if not.
        /// </summary>
        /// <param name="name">name of the variable to check</param>
        /// <returns></returns>
        private bool IsValidVariableName(string name) {

            bool name_ok = true;

            //variable names can only start with letter or underscore
            if (!Char.IsLetter(name[0]) && name[0] != '_') {
                throw new EvaluateException("illegal start of variable name: " + name[0].ToString() + " in: " + name);
            }

            for (int l = 1; l < name.Length; l++) {
                char cur = name[l];
                if (!(Char.IsLetter(cur) || Char.IsDigit(cur) || cur == '_')) {
                    throw new EvaluateException("invalid character: " + cur + " in variable name: " + name);
                }

            }//end for

            return name_ok;

        }


        /// <summary>
        /// Internal call to set a variable.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetVariableIntern(string name, float value) {

            //see if variable is already defined as a temporary varaible
            //if so set that varaible, otherwise set as new global (or overwrite global)
            if (_temp_variables.ContainsKey(name)) {
                SetTemporaryVariable(name, value);
            } else  {
                SetVariable(name, value);
            } 

        }//end set intern

        /// <summary>
        /// Set a variable in the dictionary
        /// </summary>
        /// <param name="key">variable name</param>
        /// <param name="value">value of variable</param>
        public void SetVariable(string key, float value) {


            //if variable doesn't exist, do test to make sure name is valide before we create it
            if (!_variables.ContainsKey(key)) {



                ////variable names can only start with letter or underscore
                //if (!Char.IsLetter(key[0]) && key[0] != '_') {
                //    throw new EvaluateException("illegal start of variable name: " + key[0].ToString() + " in: " + key);
                //} 
               
                
                //for (int l = 1; l < key.Length; l++) {
                //    char cur = key[l];
                //    if (!(Char.IsLetter(cur) || Char.IsDigit(cur) || cur == '_')) {
                //        throw new EvaluateException("invalid character: " + cur + " in variable name: " + key);
                //    }

                //}//end for

                //_variables.Add(key, value);

                if (IsValidVariableName(key)) {
                    _variables.Add(key, value);
                }

            } else {
                //set the key to new value, it will be re-added
                _variables[key] = value;
            }


            

        }//end set variable



       /// <summary>
       /// Sets a variable to the result of the expression
       /// </summary>
       /// <param name="name">variable name</param>
       /// <param name="expression">expression for value of variable</param>
        public void SetVariable(string name, string expression) {


            SetVariable(name, Evaluate(expression));


        }//end setVariable


       /// <summary>
       /// Get a variable from the dictionary or the temp variable dictionary
       /// 
       /// </summary>
       /// <param name="key">Name of variable to retrive</param>
       /// <returns>Value of varaible or NaN if variable is undefined</returns>
        public float GetVariable(string key) {

            float value = float.NaN;
            //see if it exists
            if (_variables.ContainsKey(key)) {
                value = (float)_variables[key];
            } else if (_temp_variables.ContainsKey(key)) {
                //pull from temporary variables
                value = (float)_temp_variables[key];
            } else {
                throw new EvaluateException("reference error, undefined variable: " + key);
            }//end if

            return value;

        }//end getVariable

        /// <summary>
        /// Sets a temporary variable to the result of an expression
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <param name="expression">expression to evaluate</param>
        public void SetTemporaryVariable(string name, string expression) {

            SetTemporaryVariable(name, Evaluate(expression));

        }

        /// <summary>
        /// Sets a temporary variable to a value
        /// </summary>
        /// <param name="name">name of the variable </param>
        /// <param name="value">value of variable</param>
        public void SetTemporaryVariable(string name, float value) {

            //make sure it's not defined as a global
            if (_variables.ContainsKey(name)) {
                throw new EvaluateException("Illegal temporary variable: " + name + " is defined as global variable");
            }

            //if variable doesn't exist create it - check name
            if (!_temp_variables.ContainsKey(name)) {

                if (IsValidVariableName(name)) {
                    _temp_variables.Add(name, value);
                }


            } else {

                //set teh variable to value if it already exists
                _temp_variables[name] = value;

            }

        }//end set temporary variable


       /// <summary>
       /// This is used to store the original defined variables, so if the execution of an lsystem changes any variables, they can be reset.
       /// 
       /// </summary>
       /// <returns></returns>
        public void SaveVariableState() {

            _original_variables = new Dictionary<string, float>();

            foreach (KeyValuePair<string, float> entry in _variables) {
                _original_variables.Add(entry.Key, (float)entry.Value);
            }


        }

        /// <summary>
        /// Clear the Temporary Variables
        /// </summary>
        public void ClearTemporaryVariables() {

            //clear temporary variables...
            _temp_variables.Clear();

        }

       /// <summary>
       /// Restores the variable table from saved state.  The variable state should be reste after an LSystem
       /// is executed as the production sequence can change variables.  This will reset the variable table first,
       /// then copy all the key value pairs into the variable dictionary.
       /// 
       /// </summary>
       /// <param name="vars"></param>
        public void RestoreVariableState() {

            if (_original_variables != null) {

                _variables.Clear();

                //note that original_variables should contain default variables
                foreach (KeyValuePair<string, float> entry in _original_variables) {
                    _variables.Add(entry.Key, (float)entry.Value);
                }

                //clear temporary variables...
                _temp_variables.Clear();

            } else {
                //oh crap...
                SetDefaultVariables();
            }



        }




       
       

        /// <summary>
        /// Evaluate an expression
        /// </summary>
        /// <param name="text">expression to evaluate, assumed to be clean, so it should be trimmed/cleaned before passing</param>
        /// <returns>value of expression, 1 or 0 in terms of logic</returns>
        public float Evaluate(string text) {

            //if (_stack_depth == -1) {
            //    Debug.Print(Environment.NewLine + Environment.NewLine + "+++++++++  Entering New Evaluate +++++++++");
            //}
            _stack_depth += 1;
            int d = _stack_depth;
            int depth = d + 1;
            //indent for debug print
            string spc = "";
            for (int s = 0; s < depth; s++) { spc += "\t"; }
           
            //Debug.Print(spc + ">> recursion depth=" + depth);
            //Debug.Print(spc + ">> entered evaluate with: " + text);


            //tokenize the string
            List<ExpressionToken> tokens = new List<ExpressionToken>();

            //first step is to look for parentheses
            const char openP = '(';
            const char closeP = ')';
            string stack = "";
            EvalStackType stackType = EvalStackType.Empty;
            char current = '\0';
            int index = 0;
            int last = text.Length;


            if (text == "") {
                throw new EvaluateException("empty string to evaluate");
            }

            //loop through string chars
           while(index < last) {
               //current character
               current = text[index];

               /*
                *  ====== check for whitespace ======
                * */
               if (Char.IsWhiteSpace(current)) {

                   //process stack if not empty- whitespace is seperator
                   if (stackType != EvalStackType.Empty) {
                       AddToTokens(ref tokens, stack, stackType);
                       stack = "";
                       stackType = EvalStackType.Empty;

                   }//end if

                   index += 1;


                   /*
                    *  ====== opening parentheses '(' ======
                    *  
                    * 
                    * this will precede either a function or an expression based on stack contents
                    * 
                    * */
               } else if (current == openP) {

                   string args = "";
                   //find closing parentheses starting from current position: '('
                   int closing = ParenthesesMatch(text, index, ref args);

                   if (closing != -1) {

                       //  ---- functions -----
                       //assume stack is function mae
                       if (stackType == EvalStackType.Symbol) {
                           //this should be a function
                           if (IsFunction(stack)) {

                               //Debug.Print(spc +  "Evaluating function: " + stack + " with arguments: " + args);


                                // parse the args into parts
                                string[] arguments = SplitArgs(args);
                                float[] values = new float[arguments.Length];

                                for (int i = 0; i < arguments.Length; i++) {
                                    //Debug.Print(spc + "evaluating argument " + (i+1) + " of " + arguments.Length + " in function: " + stack);
                                    //evaluate each argument
                                    values[i] = Evaluate(arguments[i]);

                                }//end for

                                //call the function and capture result
                                float result = CallFunction(stack, values);

                                //add the result as number token
                                tokens.Add(new ExpressionToken(result.ToString(), ExpressionTokenType.Number, result));

                                stack = "";
                                stackType = EvalStackType.Empty;
                                index = closing + 1;

                               //}


                           } else {

                               throw new EvaluateException("unknown function (or missing operator?): " + stack + "(" + args + ")");

                           }

                           //  ---- expressions (a + (c - d * (b / 3)) ----- 
                           //if there is nothing on the stack assume valid context for expression
                       } else if (stackType == EvalStackType.Empty) {

                           //ExpressionToken expToken = new ExpressionToken(args, ExpressionTokenType.Expression);

                           //Debug.Print(spc +  "entering evaluate with expression: " + args);
                           //evaluate expression here
                           float result = Evaluate(args);

                           //Debug.Print(spc + "expression evaluated to: " + result);
                           //create number token from result
                           ExpressionToken nt = new ExpressionToken(result.ToString(), ExpressionTokenType.Number, result);

                           //add to tokens
                           tokens.Add(nt);

                           //tokens.Add(expToken);
                           index = closing + 1;


                       } else if (stackType == EvalStackType.Number) {

                           throw new EvaluateException("invalid context for expression (missing operator?): " + stack + "(" + args + ")");

                       }//end if stack type symbol


                   } else {
                       throw new EvaluateException("missing right parentheses in:" + text);
                   }

                   /*
                    *  ====== closing parentheses ')' ======
                    *  
                    *  this should be picked up be left parentheses...
                    * 
                    */

               } else if (current == closeP) {
                   throw new EvaluateException("extra right parentheses in:" + text);

                  
                   /*
                    *  ======  operators:  general flag  ======
                    *  
                    * Asssign general operator flag here - specifics are discovered 
                    *   when context can be queried right/left context
                    *   
                    * */
               } else if (IsOperatorChar(current)) {
                   //if the first character matches, gobble up the string
                   int newIndex = index;
                   string op = GetOperatorString(ref newIndex, text);

                   //process stack if not empty
                   if (stackType != EvalStackType.Empty) {
                       AddToTokens(ref tokens, stack, stackType);
                   }

                   //create the operator token
                   ExpressionToken opToken = new ExpressionToken(op, ExpressionTokenType.GeneralOperator);
                   //add the token
                   tokens.Add(opToken);
                   //update index
                   index = newIndex;
                   //clear the stack
                   stack = "";
                   stackType = EvalStackType.Empty;

                  
                   /*
                    *  ====== digit 0-9 ======
                    *  
                    * */
               } else if (Char.IsDigit(current) ) {

                   //check for empty - otherwise append number or 
                   // allow numbers after aplha for variable names i.e.: a2, b34
                   if (stackType == EvalStackType.Empty) {
                       stackType = EvalStackType.Number;
                   }
                   //append stack
                   stack += current;
                   index += 1;


                   /*
                    *  ====== dot (.) for decimals ======
                    *  
                    * */
               } else if (current == '.') {

                   if (stackType == EvalStackType.Number) {
                       stack += current;
                       index += 1;
                   } else {
                       throw new EvaluateException("invalid context for dot (.), context = " + stack);
                   }


                   /*
                    *  ====== exponential format numbers 1.2e+3, 5.6E-12, etc ======
                    *  
                    * */
               } else if (current == 'E' || current == 'e') {

                   //check for exponential format
                   if (stackType == EvalStackType.Number) {

                       if (index < last - 2) {
                           char sign = text[index + 1];
                           char digit = text[index + 2];
                           //patter must be 'E+1' or 'E-1'
                           if ((sign == '+' || sign == '-') && Char.IsNumber(digit)) {

                               stack += current.ToString() + sign.ToString() + digit.ToString();
                               index += 3;

                           } else {
                               throw new EvaluateException("invalid exponentional format: " + current.ToString() + sign.ToString() + digit.ToString());
                           }

                       } else {

                           throw new EvaluateException("invalid exponentional format: " + current.ToString() + " <missing exponent>");
                       }//end if next

                   } else {

                       //assume symbol or empty - either is valid
                       stack += current;
                       stackType = EvalStackType.Symbol;
                       index += 1;

                   }//end if stackType number


                   /*
                    *  ====== alphas a-zA-Z ======
                    *  
                    * */
               } else if (Char.IsLetter(current)) {

                   stack += current;
                   stackType = EvalStackType.Symbol;
                   index += 1;


                   /*
                    * 
                    *  ====== underscore (for variable names) =========
                    *  
                    * 
                    * */

               } else if (current == '_') {


                   if (stackType == EvalStackType.Empty || stackType == EvalStackType.Symbol) {

                       stack += current;
                       stackType = EvalStackType.Symbol;
                       index += 1;

                   } else {
                       throw new EvaluateException("invalid context for underscore '_': " + text);
                   }


                   /*
                    *  ====== !error - unknown  ======
                    *  
                    * */
               } else {

                   throw new EvaluateException("invalid symbol: " + current);

               }//end if else whitespace

           }//end while

            //do final post loop check on stack
           if (stackType != EvalStackType.Empty) {
               AddToTokens(ref tokens, stack, stackType);
           }



            //debugging....
           //Debug.Print(spc + "--- tokens ---");
           //foreach (ExpressionToken et in tokens) {
           //    Debug.Print(spc + et.ToString());
           //}//end foreach

           /*
            *    Process token list into RPN stacks
            * 
            * 
            *   iterate over tokens and run equation based on operator precedence
            *   
            * */

           List<ExpressionToken> opStack = new List<ExpressionToken>();
           List<ExpressionToken> varStack = new List<ExpressionToken>();

            int tIndex = 0;
            int tCount = tokens.Count;

            
           while (tIndex < tCount) {

               ExpressionToken expTok = tokens[tIndex];

               ExpressionTokenType etType = expTok.Type;


               //push numbers and variables directly to varStack
               if (etType == ExpressionTokenType.Number || etType == ExpressionTokenType.Variable) {

                   varStack.Add(expTok);
                   tIndex += 1;

                   //check GeneralOperators and refine operator type
               } else if (etType == ExpressionTokenType.GeneralOperator) {

                   string opStr = expTok.Id;

                   if (IsUnaryOperator(opStr)) {
                       ExpressionToken lt = tIndex > 0 ? (ExpressionToken)tokens[tIndex - 1] : new ExpressionToken("<>", ExpressionTokenType.Null);
                       ExpressionToken rt = tIndex < tokens.Count - 1 ? (ExpressionToken)tokens[tIndex + 1] : new ExpressionToken("<>", ExpressionTokenType.Null);
                       
                       //Debug.Print(spc + "processing unary: " + et + ", right= " + rt + ", left= " + lt);

                       //there has to be a right context regardless
                       if (rt.Type == ExpressionTokenType.Null) {
                           throw new EvaluateException("missing right operand for operator: " + expTok);
                       } else if (!(rt.Type == ExpressionTokenType.Number || rt.Type == ExpressionTokenType.Variable)) {
                           throw new EvaluateException("invalid context for unary operator: " + expTok + " expecting number or variable.");
                       }

                       //first make sure that it is not confused with '-' in proper context
                       if (opStr.Equals("-") && (lt.Type == ExpressionTokenType.Number || lt.Type == ExpressionTokenType.Variable) &&
                            (rt.Type == ExpressionTokenType.Number || rt.Type == ExpressionTokenType.Variable)) {
                           //left hand and right hand are numbers - this should be standard subtract
                           ExpressionToken ot = GetSpecificOperatorToken(expTok);
                           opStack.Add(ot);
                            tIndex += 1;

                           //if left is null, and right is number/variable - apply unary
                       } else if (lt.Type == ExpressionTokenType.Null && (rt.Type == ExpressionTokenType.Number || rt.Type == ExpressionTokenType.Variable)) {

                          ExpressionToken ut = ApplyUnary(rt, expTok);
                          varStack.Add(ut);
                          tIndex += 2;

                           //if left is operator this should be unary
                       } else if (lt.Type != ExpressionTokenType.Null && lt.Type == ExpressionTokenType.GeneralOperator) {

                           ExpressionToken ut = ApplyUnary(rt, expTok);
                           varStack.Add(ut);
                           tIndex += 2;

                       } else {
                           throw new EvaluateException("invalid unary operator context for: " + expTok );
                       }

                       //non-unary operators are processed here
                   } else {

                       if (varStack.Count == 0) {
                           throw new EvaluateException("invalid operator before value or variable :" + expTok);
                       }

                       //process operator to stack
                       //resolve type
                       ExpressionToken ot = GetSpecificOperatorToken(expTok);
                       opStack.Add(ot);
                       tIndex += 1;
                   }//end unary check

               } else {
                   throw new EvaluateException("unexpected token type found parsing token list: " + expTok);
               }//end if else - token type


           }//end while - tokens



           //debugging....
            //=====================================
           //Debug.Print(spc + "--- operators ---");
           //foreach (ExpressionToken et in opStack) {
           //    Debug.Print(spc + et.ToString());
           //}//end foreach

           //Debug.Print(spc + "--- values ---");
           //foreach (ExpressionToken et in varStack) {
           //    Debug.Print(spc + et.ToString());
           //}//end foreach
            //========================================


            //there should always be one less operator than value by this point
           if (opStack.Count != varStack.Count - 1) {
               throw new EvaluateException("incorrect number of operators and values in: " + text);
           }

           //Debug.Print(spc + "--- executing ---");
            //process any operators on operator stack
           if (opStack.Count > 0) {

               /*
                *    =========  process all operators ==========
                *    
                *    at the end of this cycle there should be only one token
                *    in the variable stack and all operators have been processed
                */

    
               //- inner loop can skip over elements based on precedence
               while (opStack.Count > 0) {

                   int opIndex = 0;

                   //process all operators
                   while (opIndex < opStack.Count) {

                       bool doOp = false;

                       ExpressionToken opToken = (ExpressionToken)opStack[opIndex]; //operator

                       //check precedence first
                       //see if there is another operator on the stack
                       //check for next operator ahead on stack and compare precedence rank
                       if (opIndex < opStack.Count - 1) {
                           //  get next operator
                           ExpressionToken nextOpToken = (ExpressionToken)opStack[opIndex + 1]; //operator

                           //  test precedence using pre-stored operator rank
                           //  rank is discovered and stored in GetSpecificOperatorToken

                           //if associativity is right to left, precedence must be greater
                           // otherwise, for left to right associativity, precedence can be greater or equal
                           if (opToken.Type == ExpressionTokenType.Assignment) {
                               if (opToken.Value > nextOpToken.Value) { doOp = true; }
                           } else {
                               if (opToken.Value >= nextOpToken.Value) { doOp = true; }
                           }


                       } else {

                           //last operator so just process it
                           doOp = true;
                       }

                       //check flag to performa operation
                       if (doOp) {
                           //pull out operands
                           ExpressionToken oaToken = (ExpressionToken)varStack[opIndex];
                           ExpressionToken obToken = (ExpressionToken)varStack[opIndex + 1];

                           //Debug.Print(spc + "executing operation: " + opToken + ", with: " + oaToken + ", and " + obToken);

                           //do operation
                           ExpressionToken opResult = DoOperation(opToken, oaToken, obToken);

                           //remove operator from stack
                           opStack.RemoveAt(opIndex);

                           //remove values
                           varStack.RemoveAt(opIndex);
                           //remove second var at same index since it shifted left
                           varStack.RemoveAt(opIndex);

                           //insert the result of operation
                           varStack.Insert(opIndex, opResult);

                           //!do not increment opIndex - stay at current position

                       } else {
                           //go to next operator
                           opIndex += 1;
                       }

                   }//end while 

               }//while operators exist to process


           } //end if else operator stack count > 0

            //reset stack depth
           _stack_depth -= 1;

           //there should only be one value left on value stack
           if (varStack.Count == 1) {
               ExpressionToken fTok = (ExpressionToken)varStack[0];
               //this could slip through if just evaluating a single variable
               if (fTok.Type == ExpressionTokenType.Variable) {
                   fTok.Value = GetVariable(fTok.Id );
               }

               //Debug.Print(spc + "*** evaluation complete ***" );
               //Debug.Print(spc + "result= " + fTok);

               return fTok.Value;
           } else if (varStack.Count > 1) {
               throw new EvaluateException("Too many values to evaluate on final value stack: " + text);
           } else  {
              throw new EvaluateException("No values to evaluate on final value stack: " + text);
           }



        }//end eval


       /// <summary>
       /// Helper function so we don't repeat this code
       /// </summary>
       /// <param name="tokenList">list to add token to</param>
       /// <param name="curStack">current 'string stack', symbol name of token</param>
       /// <param name="curType">current 'string stack' type, variable or symbol (variable) </param>
        private void AddToTokens(ref List<ExpressionToken> tokenList, string curStack, EvalStackType curType) {

            if (String.IsNullOrWhiteSpace(curStack)) {
                throw new EvaluateException("Attempted to add emtpy token: " + curStack + ", of type:" + curType.ToString() );
            }//end if empty check

            switch (curType) {

                case EvalStackType.Number:

                    //attmept to parse number
                    float value;
                    bool ok = float.TryParse(curStack, out value);

                    if (ok) {
                        tokenList.Add(new ExpressionToken(curStack, ExpressionTokenType.Number, value));
                    } else {
                        throw new EvaluateException("Failed to parse number: " + curStack);
                    }

                    break;

                case EvalStackType.Symbol:

                    //try this with lazy evaluation, instead of chaecking...
                    tokenList.Add(new ExpressionToken(curStack, ExpressionTokenType.Variable));

                    //if (IsVariable(curStack)) {
                    //    tokenList.Add(new ExpressionToken(curStack, ExpressionTokenType.Variable));
                    //} else {
                    //    throw new EvaluateException("Undefined variable: " + curStack);
                    //}
                    
                   break;

                default:
                    throw new EvaluateException("Invalid token type for list: " + curType.ToString());
   
            }//end switch


        }//end AddToTokens


       /// <summary>
       /// Applies unary operator 
       /// </summary>
       /// <param name="token"></param>
       /// <param name="unary"></param>
       /// <returns></returns>
        private ExpressionToken ApplyUnary(ExpressionToken token, ExpressionToken unary) {

            ExpressionToken rToken;
            rToken.Type = ExpressionTokenType.Number;

            if (token.Type == ExpressionTokenType.Variable) {

                float var = GetVariable(token.Id);
                token.Value = var;

            }//end variable check

            float tokenValue = token.Value;

            switch (unary.Id ) {

                case "!":

                    if (tokenValue == 0) {
                        tokenValue = 1;
                    } else if (tokenValue == 1) {
                        tokenValue = 0;
                    } else {
                        throw new EvaluateException("Invalid application of unary not '!': token value is not boolean: " + token.ToString());
                    }

                    //return new ExpressionToken(tokenValue.ToString(), ExpressionTokenType.Number, tokenValue);
                    break;


                case "-":
                    tokenValue *= -1.0f;
                    //return new ExpressionToken(tokenValue.ToString(), ExpressionTokenType.Number, tokenValue);
                    break;

                case "++":
                    tokenValue += 1.0f;
                    //handle variable cases for decrement
                    if (token.Type == ExpressionTokenType.Variable) {
                        //SetVariable(token.Id, tokenValue);
                        SetVariableIntern(token.Id, tokenValue);
                    }
                    //return new ExpressionToken(tokenValue.ToString(), ExpressionTokenType.Number, tokenValue);
                    break;

                case "--":
                    tokenValue -= 1.0f;
                    //handle vairable cases for decrement
                    if (token.Type == ExpressionTokenType.Variable) {
                        //SetVariable(token.Id, tokenValue);
                        SetVariableIntern(token.Id, tokenValue);
                    }
                    //return new ExpressionToken(tokenValue.ToString(), ExpressionTokenType.Number, tokenValue);
                    break;

                default:
                    throw new EvaluateException("Unable to apply unknown unary operator: " + unary.ToString());
     
            }//end switch

            //apply result to token to return
            rToken.Id = tokenValue.ToString();
            rToken.Value = tokenValue; 

            return rToken;

        }//end applyUnary



        /// <summary>
        /// Return a specific operator token from general Operator Token
        /// </summary>
        /// <param name="token">GeneralOperator token to parse</param>
        private ExpressionToken GetSpecificOperatorToken(ExpressionToken token) {

            ExpressionToken et = new ExpressionToken();

            if ( _token_table.ContainsKey(token.Id)) {

                ExpressionTokenRecord etr = _token_table[token.Id];
                et.Id = token.Id;
                et.Type = etr.TokenType;
                et.Value = etr.Rank;

            } else {


                //invalid operator - should never be reached
                throw new EvaluateException("Invalid operator: " + token);

            }


            return et;

        }


       /// <summary>
       /// Calls the spcified function with arguments provided.  This handles all math functions,
       ///   except for define, which is handled as a special case before variable values are resolved
       /// </summary>
       /// <param name="name">name of function to call</param>
       /// <param name="functArgs">array of argument values to pass to function</param>
       /// <returns></returns>
        private float CallFunction(string name, float[] functArgs) {

            float result = float.NaN;
            int nArgs = functArgs.Length;

            /*
             * Use one big switch for all functions
             * 
             * 
             * */

            switch (name) {
                    //start with basics...
                    //square root (x)
                case "sqrt":

                    if (nArgs == 1) {
                        result = (float)Math.Sqrt(functArgs[0]);
                    }
                    break;

                    //power (x , y)
                case "pow":
                    if (nArgs == 2) {
                        result = (float)Math.Pow(functArgs[0], functArgs[1]);
                    }
                    break;

                    //modulus
                case "mod":
                    if (nArgs ==2) {
                        result = functArgs[0] % functArgs[1];
                    }
                    break;

                    //ceiling
                case "ceil":
                    if (nArgs == 1) {
                        result = (float)Math.Ceiling(functArgs[0]);
                    }
                    break;

                    //floor
                case "floor":
                    if (nArgs == 1) {
                        result = (float)Math.Floor(functArgs[0]);
                    }
                    break;
                    //exponetial
                case "exp":
                    if (nArgs == 1) {
                        result = (float)Math.Exp(functArgs[0]);
                    }
                    break;
                    //log
                case "log":
                    if (nArgs == 1) {
                        result = (float)Math.Log(functArgs[0]);
                    }
                    break;
                    //log10
                case "log10":
                    if (nArgs == 1) {
                        result = (float)Math.Log10(functArgs[0]);
                    }
                    break;

                    //-----  Trig Functions ------
                    //cosine
                case "cos":
                    if (nArgs == 1) {
                        result = (float)Math.Cos(functArgs[0]);
                    }
                    break;
                    //sin
                case "sin":
                    if (nArgs == 1) {
                        result = (float)Math.Sin(functArgs[0]);
                    }
                    break;
                    //tangent
                case "tan":
                    if (nArgs == 1) {
                        result = (float)Math.Tan(functArgs[0]);
                    }
                    break;
                    //hyperbolics
                case "cosh":
                    if (nArgs == 1) {
                        result = (float)Math.Cosh(functArgs[0]);
                    }
                    break;
                case "sinh":
                    if (nArgs == 1) {
                        result = (float)Math.Sinh(functArgs[0]);
                    }
                    break;
                case "tanh":
                    if (nArgs == 1) {
                        result = (float)Math.Tanh(functArgs[0]);
                    }
                    break;
                    //arc
                case "acos":
                    if (nArgs == 1) {
                        result = (float)Math.Acos(functArgs[0]);
                    }
                    break;
                case "asin":
                    if (nArgs == 1) {
                        result = (float)Math.Asin(functArgs[0]);
                    }
                    break;
                    //special case - handle both versions of atan
                case "atan":
                    if (nArgs == 1) {
                        result = (float)Math.Atan(functArgs[0]);
                    } else if (nArgs == 2) {
                        result = (float)Math.Atan2(functArgs[0],functArgs[1]);
                    }
                    break;

                    //angle conversions
                    //degrees from radians (radToDeg)
                case "deg":
                    if (nArgs == 1) {
                        result = (float)(functArgs[0] * (180 / Math.PI));
                    }
                    break;

                    //radians from degrees (degToRad)
                case "rad":
                    if (nArgs ==1) {
                        result = (float)(functArgs[0] / 180.0 * Math.PI);
                    }
                    break;

                    //round a number
                case "round":
                    if (nArgs == 1) {
                        result = (float)Math.Round(functArgs[0]);
                    } else if (nArgs == 2) {
                        result = (float)Math.Round(functArgs[0], (int)functArgs[1]);
                    }
                    break;

                case "rnd":
                    if (nArgs == 0) {

                        result = (float)_random.NextDouble();
                    } else if (nArgs == 2) {

                        float min = functArgs[0];
                        float max = functArgs[1];
                        float rnd = (float)_random.NextDouble();
                        result = ((max - min ) * rnd) + min;

                    }
                    break;

                case "rndint":
                    if (nArgs == 2) {

                        int min = (int)functArgs[0];
                        int max = (int)functArgs[1];
                        result = _random.Next(min, max);

                    }
                    break;
                    //throw error on unkown 
                 default:
                    throw new EvaluateException("Unknown function: " + name);

            }//end switch

            //corect number of arguments?
            if (float.IsNaN(result)) {
                throw new EvaluateException("Invalid Number of Arguments for function: " + name);
            }

            return result;

        }//end call Function

       /// <summary>
       /// Apply the Operation with the operation token and two expression tokens ( a & b)
       /// </summary>
       /// <param name="op">Operation Token</param>
       /// <param name="ea">Expression a</param>
       /// <param name="eb">Expression b</param>
       /// <returns></returns>
        private ExpressionToken DoOperation(ExpressionToken op, ExpressionToken ea, ExpressionToken eb) {

            //handle all assignment operations differently
            //  they will preserve varaibles on stack
            if (op.Type == ExpressionTokenType.Assignment) {

                float result = float.NaN;
                string opStr = op.Id;

                //make sure left hand is variable token
                if (ea.Type != ExpressionTokenType.Variable) {
                    throw new EvaluateException("invalid left token: " + ea + ", for operator: " + op + ", expecting variable");
                }

                //for all assignment operators other than '=', get the variable's value
                if (!opStr.Equals("=")) {
                    ea.Value = GetVariable(ea.Id);
                }

                //if operand b is varaible, resolve variable reference
                if (eb.Type == ExpressionTokenType.Variable) {
                    eb.Value = GetVariable(eb.Id);
                }


                switch (op.Id ) {
                    //set a variable here - variable token ignores  value, just references name
                    case "=":
                        SetVariableIntern(ea.Id, eb.Value);
                        break;

                    case "+=":
                        SetVariableIntern(ea.Id, ea.Value + eb.Value);
                        break;

                    case "-=":
                        SetVariableIntern(ea.Id, ea.Value - eb.Value);
                        break;

                    case "*=":
                        SetVariableIntern(ea.Id, ea.Value * eb.Value);
                        break;

                    case "/=":
                        if (eb.Value == 0) { throw new EvaluateException("divide by zero: " + eb); }
                        SetVariableIntern(ea.Id, ea.Value / eb.Value);
                        break;

                    case "%=":
                        SetVariableIntern(ea.Id, ea.Value % eb.Value);
                        break;
                 
                    default:
                        throw new EvaluateException("unknown assignment operator reached: " + op);

                }//end switch

                //aquire result through get variable
                result = GetVariable(ea.Id);
                return new ExpressionToken(ea.Id, ExpressionTokenType.Variable, result);

            } else {


                //resolve variables here
                if (ea.Type == ExpressionTokenType.Variable) {
                    ea.Value = GetVariable(ea.Id);
                }

                if (eb.Type == ExpressionTokenType.Variable) {
                    eb.Value = GetVariable(eb.Id);
                }

                float result = float.NaN;
                float a = ea.Value;
                float b = eb.Value;

                switch (op.Id) {

                    //multiplicitive operators
                    case "*":
                        result = a * b;
                        break;

                    case "/":
                        if (b == 0) { throw new EvaluateException("divide by zero:" + ea + ", divided by: " + eb); }
                        result = a / b;
                        break;

                    case "%":
                        result = a % b;
                        break;

                    //additive operators
                    case "+":
                        result = a + b;
                        break;

                    case "-":
                        result = a - b;
                        break;

                    //logical opperators
                    //return 1 or zero only
                    case "<":
                        result = a < b ? 1 : 0;
                        break;

                    case ">":
                        result = a > b ? 1 : 0;
                        break;

                    case "<=":
                        result = a <= b ? 1 : 0;
                        break;

                    case ">=":
                        result = a >= b ? 1 : 0;
                        break;

                    case "==":
                        result = a == b ? 1 : 0;
                        break;

                    case "!=":
                        result = a != b ? 1 : 0;
                        break;

                    case "&":
                        result = a == 1 && b == 1 ? 1 : 0;
                        break;

                    case "|":
                        result = a == 1 || b == 1 ? 1 : 0;
                        break;



                }//end switch


                return new ExpressionToken(result.ToString(), ExpressionTokenType.Number, result);
  

            }//end if else operators/assignment

        }//end do operation


        #region "String_Tests"

       /// <summary>
       /// Check if string is defined variable
       /// </summary>
       /// <param name="key"></param>
       /// <returns></returns>
        public bool IsVariable(string key) {

            return _variables.ContainsKey(key);

        }//end isVariable

        /// <summary>
        /// tests to see if current char is operator - treated as seperator
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        static bool IsOperatorChar(char test) {
            return _operatorChars.IndexOf(test) != -1 ? true : false;
        }//end IsOperatorChar



        /// <summary>
        /// Check if string is a unary operator
        /// </summary>
        /// <param name="test">string to test</param>
        /// <returns>true if matches, false otherwise</returns>
        private bool IsUnaryOperator(string test) {

            if (_unary_table.ContainsKey(test)) {
                return true;
            } else {
                return false;
            }


        }


       /// <summary>
       /// Checks ik token is a function
       /// </summary>
       /// <param name="test"></param>
       /// <returns></returns>
       private bool IsFunction(string test) {

           return _function_table.ContainsKey(test);

       }

       #endregion


 

       #region String_Parsing

       /// <summary>
       /// Gobble up all operator characters using first one as start.  Only read two chars, as we know no operator is longer than two chars.
       /// </summary>
       /// <param name="start"></param>
       /// <param name="text"></param>
       /// <returns></returns>
       private string GetOperatorString(ref int start, string text) {
           string op = text[start].ToString();
           start += 1;
           char next = text[start];

           if (IsOperatorChar(next)) {
               op += next.ToString();
               start +=1;
           }


           return op;
       }//end operatorString

       /// <summary>
        /// Splits the string int coomma seperated parts, honoring only top level commas
        /// 
        /// </summary>
        /// <param name="args">String to split</param>
        /// <returns>array of string arguments</returns>
        private string[] SplitArgs(string args) {


            int index = 0;
            int last = args.Length;
            //if there is at least one char
            if (args.Length != 0) {

                string stack = "";
                string[] argArray = new string[] { };
                int pStack = 0;

                while (index < last) {

                    char cur = args[index];
                    if (cur == '(') {
                        pStack += 1;
                    } else if (cur == ')') {
                        pStack -= 1;
                    } else if (cur == ',') {
                        //see if it is not nested in (... , ...)
                        if (pStack == 0) {

                            if (stack != "") {
                                //declare a new temporary array
                                string[] newArgArray = null;
                                newArgArray = new string[argArray.Length + 1];
                                //copy array values
                                System.Array.Copy(argArray, newArgArray, argArray.Length);
                                //push arg onto end
                                newArgArray[argArray.Length] = stack;
                                argArray = newArgArray;
                            }//end if stack not empty
                            stack = "";
                            cur = '\0';

                        }//end if pStack depth
                    }//end if

                    //update stack and index
                    if (cur != '\0') {
                        stack += cur;
                    }

                    index += 1;

                }//end while

                if (stack != "") {
                    //declare a new temporary array
                    string[] newArgArray = null;
                    newArgArray = new string[argArray.Length + 1];
                    //copy array values
                    System.Array.Copy(argArray, newArgArray, argArray.Length);
                    //push arg onto end
                    newArgArray[argArray.Length] = stack;
                    argArray = newArgArray;
                }//end if stack not empty

                return argArray;

            } else {
                //return empty array for no inputs
                return new string[] { };
            }//end if/else

        }//end split args



       /// <summary>
       /// Find text enclosed within parentheses.  This honors nesting of parantheses and will find the enclosing parentheses at the same level.
       /// </summary>
       /// <param name="text">String to search</param>
       /// <param name="start">Index of opening parentheses '('</param>
       /// <param name="contents">String contained within parentheses '(contents)'</param>
       /// <returns>Index of closing parentheses ')' or -1 on failure</returns>
        private static int ParenthesesMatch(string text, int start, ref string contents) {
            const char op = '(';
            const char cp = ')';

            int index = start + 1;
            int last = text.Length;
            int found = -1;
            int stackDepth = 1;

            while (index < last) {

                char current = text[index];

                if (current == op) {
                    stackDepth += 1;
                } else if (current == cp) {
                    stackDepth -= 1;
                    if (stackDepth == 0) {
                        //found matching parentheses
                        contents = text.Substring(start + 1, index  - (start + 1));
                        found = index;
                        //index = last + 1;//this gets us out of while...
                        break;
                    }
                }//end else/if

                //increment index
                index += 1;

            }//end while

            return found;

        }// end ParenthesesMatch

#endregion


 
        public override string ToString() {

            string appdata = "<<<< C-Style ExpressionEvaluator >>>>" + Environment.NewLine;

            appdata += "=== operators ===" + Environment.NewLine;
            foreach (string k in _token_table.Keys) {

                ExpressionTokenRecord er = _token_table[k];
                appdata += er.ToString() + Environment.NewLine;


            }

            appdata += "=== unary operators ===" + Environment.NewLine;
            foreach (string k in _unary_table.Keys) {
                ExpressionTokenRecord er = _unary_table[k];
                appdata += er.ToString() + Environment.NewLine;
            }


            appdata += "=== functions ===" + Environment.NewLine;
            foreach (string k in _function_table.Keys) {

                ExpressionTokenRecord er = _function_table[k];
                appdata += er.ToString() + Environment.NewLine;

            }

            appdata += "=== variables ===" + Environment.NewLine;
            //print all the variables...
            foreach (KeyValuePair<string, float> pair in _variables) {
                appdata += pair.Key + " = " + pair.Value.ToString() + Environment.NewLine;
            }

            return appdata;

        }//end ToString()


        public string PrintVariables() {

            string appdata = "LS_ExpressionEvaluator:Variables::" + Environment.NewLine;

            //print all the variables...
            foreach (KeyValuePair<string, float> pair in _variables) {
                appdata += pair.Key + " = " + pair.Value.ToString() + Environment.NewLine;
            }

            return appdata;

        }//end ToString()


   }//end class


    /// <summary>
    /// Exception Class for ExpressionEvaluator
    /// </summary>
   public class EvaluateException : System.Exception {

       public EvaluateException() {}
          
       public EvaluateException(string message) : base(message) {}


   }//end class

}//end namespace

