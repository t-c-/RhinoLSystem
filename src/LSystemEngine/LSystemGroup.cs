using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LSystemExpressions;

namespace LSystemEngine {

    /// <summary>
    /// group of letters, letters may resolve into a group, alias, or lsystem 
    /// </summary>

    public class LSystemGroup {


        private LSystemLetter _key;
        private LSystemLetterSequence _letters;


        public LSystemGroup() {

            _key = new LSystemLetter('\0');
            _letters = null;

        }//end constructor

        /// <summary>
        /// parses string into a group
        /// </summary>
        /// <param name="tokens">tokens from input string, split by deliminator</param>
        public void Parse(string[] tokens) {

            LSystemLetterSequence key_ls = new LSystemLetterSequence();

            LSystemLetterSequence grp_ls = new LSystemLetterSequence();

            //clean 
            string keyStr = tokens[1];
            keyStr = keyStr.Trim();

            if (String.IsNullOrEmpty(keyStr)) {
                throw new Exception("Invalid empty string for key letter in group");
            }

            //parse first token as keyLetter
            key_ls.Parse(keyStr);

            if (key_ls.Count == 1) {
                _key = key_ls.LetterAt(0);
            } else {
                throw new Exception("Invalid number of key letters in group: " + tokens[1] + ", expecting 1 letter");
            }//end if

            string groupStr = tokens[2];
            groupStr = groupStr.Trim();

            if (String.IsNullOrEmpty(groupStr )) {
                throw new Exception("Invalid empty string for letters in group:" + _key.key);
            }


            //parse the string into letters
            _letters = new LSystemLetterSequence();
            _letters.Parse(groupStr);
            
           
            //validate the group
            Validate();


        }//end parse


        /// <summary>
        /// Validates the group.  Throws an error on any invalidating condition found: such as an empty key letter, or empty letter sequence.
        /// </summary>
        public void Validate() {
            

            if (_key.key == '\0') {
              
                throw new Exception("Invalid null key in group");
            }

            if (_letters == null || _letters.Count == 0) {
          
                throw new Exception("Invalid empty letters parameter for group: " + _key);
            }
            
        }//end validate
        
        //return the key letter
        //public LsysLetter getKeyLetter() {
        //    return _key;
        //}

        /// <summary>
        /// Property for key letter.
        /// </summary>
        public LSystemLetter KeyLetter {
            get { return _key; }
        }

        /// <summary>
        /// Wrapper that evaluates letters in object's letter sequence
        /// </summary>
        /// <param name="lang"></param>
        public void EvaluateLetters(ILSystemExpressionEvaluator lang) {

            _letters.EvaluateLetters(lang);
 
        }//end evaluateLetters

        /// <summary>
        /// Wrapper around Letter Sequence Count.
        /// </summary>
        /// <returns>Number of letters in Letter Sequence</returns>
        public int Count() {
            return _letters.Count;
        }


        /// <summary>
        /// Wrapper around Letter Sequence letters.
        /// </summary>
        /// <param name="index">Index of Letter</param>
        /// <returns>Letter at Index or Throws an Exception on invalid Index</returns>
        public LSystemLetter LetterAt(int index) {

            return _letters.LetterAt(index);

        }

        /// <summary>
        /// not used - old
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
        /// string representation of group
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            string aoStr = String.Empty;
            aoStr = _key + " = " + _letters;
            return aoStr;

        }//end ToString



    }//end class
}//end namespace
