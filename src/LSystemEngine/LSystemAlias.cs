using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LSystemExpressions;

namespace LSystemEngine {

    public class LSystemAlias {

        private LSystemLetter _key;

        private string _command;

        private string[] _arguments;


        public LSystemAlias() {

            _key = new LSystemLetter('\0');
            _command = string.Empty;
            _arguments = null; 


        }//end constructor


        /// <summary>
        /// Property for key letter
        /// 
        /// </summary>
        public LSystemLetter KeyLetter {
            get { return _key; }
        }

        /// <summary>
        /// See if letters match, used in resolving final axiom
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public bool Matches(LSystemLetter test) {

            if (_key.key == test.key) {
                return true;
            } else {
                return false;
            }

        }//end Matches


        /// <summary>
        /// Parses Alias definition from Text.
        /// </summary>
        /// <param name="tokens"></param>
        public void Parse(string[] tokens) {

            LSystemLetterSequence ls = new LSystemLetterSequence();

            if (tokens.Length < 3) {
                throw new Exception("invalid number of tokens to specify alias, expecting 3");
            }


            
            //clean 
            string keyStr = tokens[1];
            keyStr = keyStr.Trim();

            if (String.IsNullOrEmpty(keyStr)) {
                throw new Exception("invalid empty string for key letter in alias");
            }

            //parse first token as keyLetter
            ls.Parse(keyStr);

            if (ls.Count == 1) {
                _key = ls.LetterAt(0);
            } else {
                throw new Exception("invalid number of key letters in alias: " + tokens[1] + ", expecting 1 letter");
            }//end if

            //grab the command
            string cmdStr = tokens[2];
            cmdStr = cmdStr.Trim();

            if (String.IsNullOrEmpty(cmdStr)) {
                throw new Exception("invalid empty string for command in alias: " + _key.key);
            }

            _command = cmdStr;


            //transfer arguments
            if (tokens.Length >= 3) {
                _arguments = new string[tokens.Length - 3];

                for (int a = 3; a < _arguments.Length + 3; a++) {

                    string argStr = tokens[a];
                    argStr = argStr.Trim();

                    if (String.IsNullOrEmpty(argStr)) {
                        throw new Exception("invalid empty string for argument: " + (a - 2) + ", in alias: " + _key.key);
                    }
                    _arguments[a - 3] = argStr;

                }//end for


            }//end if argument tokens


        }//end parse


        /// <summary>
        /// The command to issue to the modeler
        /// </summary>
        public string Command {
            get {
                return _command;
            }
        }


        public string[] Arguments {
            get {
                return _arguments;
            }
        }//end Arguments property

        /// <summary>
        /// Prints the Alias
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            string aliasStr = String.Empty;

            aliasStr = _key.ToString();
            aliasStr += " = " + _command;

            if (_arguments != null) {
                for (int i = 0; i < _arguments.Length; i++) {
                    aliasStr += " : " + _arguments[i];
                }
            }//end if args present


            return aliasStr;

        }//end to string

    }//end class
}//end namespace
