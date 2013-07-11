using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystemEngine {
    class LSystemModelerPrepare {


        private string _command;

        private string[] _arguments;



        public LSystemModelerPrepare() {


            _command = string.Empty;
            _arguments = null; 


        }//end constructor

        #region "Properties"

        /// <summary>
        /// The command to issue to the modeler
        /// </summary>
        public string Command {
            get {
                return _command;
            }
        }

        /// <summary>
        /// Command arguments.
        /// </summary>
        public string[] Arguments {
            get {
                return _arguments;
            }
        }//end Arguments property


        #endregion


        /// <summary>
        /// Parse the prepare statement from the string tokens.
        /// The form is: prepare : command : arg : arg : arg : ...
        /// </summary>
        /// <param name="tokens"></param>
        public void Parse(string[] tokens) {

            LSystemLetterSequence ls = new LSystemLetterSequence();

            if (tokens.Length < 2) {
                throw new Exception("Invalid number of tokens to specify prepare, expecting 2");
            }


            //grab the command
            string cmdStr = tokens[1];
            cmdStr = cmdStr.Trim();

            if (String.IsNullOrEmpty(cmdStr)) {
                throw new Exception("invalid empty string for command in prepare: " + cmdStr);
            }

            _command = cmdStr;


            //transfer arguments - skip first two tokens "prepare  : <turtle cmd> : ... : ..."
            if (tokens.Length >= 2) {
                _arguments = new string[tokens.Length - 2];

                for (int a = 2; a < _arguments.Length + 2; a++) {

                    string argStr = tokens[a];
                    argStr = argStr.Trim();

                    if (String.IsNullOrEmpty(argStr)) {
                        throw new Exception("invalid empty string for argument: " + (a - 2) + ", in prepare: " + _command);
                    }
                    _arguments[a - 2] = argStr;

                }//end for


            }//end if argument tokens


        }//end parse


        /// <summary>
        /// Prints the prepare statement
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            string prepStr = _command + " : ";

            if (_arguments != null) {
                for (int i = 0; i < _arguments.Length; i++) {
                    prepStr += " : " + _arguments[i];
                }
            }//end if args present


            return prepStr;

        }//end to string


    }//end class
}
