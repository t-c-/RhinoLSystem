using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystemEngine {




    /// <summary>
    /// Object to hold call to a nested LSystem
    /// i.e.: subsystem : X(a,b,c,d) : abop_pg9_b : iterations : a*b : theta : rad(c) : L : 12 + d
    /// minimum of three tokens present
    /// </summary>
    public class LSystemSubSystem {

        private LSystemLetter _key;

        private string _name;

        private List<String> _arguments;

        public LSystemSubSystem() {

            _name = String.Empty;
            _key = new LSystemLetter('\0');
            _arguments = null;

        }

        /// <summary>
        /// Name of the Subsystem to Execute
        /// </summary>
        public string Name {
            get {
                return _name;
            }

        }//end name


        /// <summary>
        /// Key Letter of SubSystem
        /// </summary>
        public LSystemLetter KeyLetter {
            get {
                return _key;
            }
        }//end key letter

        /// <summary>
        /// Returns the Arguments
        /// </summary>
        public string[] Arguments {
            get {
                //ifthere are sruemnts - push out as array
                if (_arguments != null) {
                    return _arguments.ToArray();
                } else {
                    return null;
                }

            }//end get
        }//end arguments

        /// <summary>
        /// Parse the SubSystem from the tokens
        /// </summary>
        /// <param name="tokens"></param>
        public void Parse(string[] tokens) {

            LSystemLetterSequence key_ls;
            string keyStr;
            string nameStr;
            int argLen = tokens.Length - 3;

            if (tokens.Length < 3) {

                throw new Exception("Invalid number of tokens in subsystem");
            }

            //initialize arguments
            _arguments = new List<String>();

            //resolve the key
            key_ls = new LSystemLetterSequence();
            keyStr = tokens[1];
            keyStr = keyStr.Trim();
            key_ls.Parse(keyStr);

            if (key_ls.Count == 1) {
                _key = key_ls.LetterAt(0);
            } else {
                throw new Exception("Invalid number of key letters in subsystem: " + tokens[1] + ", expecting 1 letter");
            }//end if

            //resolve the name
            nameStr = tokens[2].Trim();
            //nameStr.Trim();

            if (!String.IsNullOrWhiteSpace(nameStr)) {
                _name = nameStr;
            } else {
                throw new Exception("Invalid empty name in subsystem: " + tokens[2] );
            }



            //grab all the arguments (variable/expression pairs)
            if (argLen != 0) {
                if (argLen % 2 == 0) {


                    for (int n = 3; n < argLen + 3; n++) {

                        string arg = tokens[n].Trim();

                        if (!String.IsNullOrWhiteSpace(arg)) {

                            _arguments.Add(arg);

                        } else {

                            throw new Exception("Invalid empty parameter in subsystem: " + _name);

                        }

                    }//end for


                } else {

                    throw new Exception("Invalid number of variable/value pairs in subsystem, must be even: subsystem : <letter> : <system> : <variable1> : <expression1> : <variable2> : <expression2> : ...");
                }
            }//end if zero arguments..



        }//end parse




    }//end class
}//end namespace
