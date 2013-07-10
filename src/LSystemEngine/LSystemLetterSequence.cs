using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LSystemExpressions;

namespace LSystemEngine {

 
    public class LSystemLetterSequence {


        private List<LSystemLetter> _letters;

        public LSystemLetterSequence() {


            _letters = new List<LSystemLetter>();


        }//end constructor


        /// <summary>
        /// Copy Constructor - reference safe - performs deep copy
        /// </summary>
        /// <param name="sequence"></param>
        public LSystemLetterSequence(LSystemLetterSequence sequence) {


            _letters = new List<LSystemLetter>();

            for (int l = 0; l < sequence.Count; l++) {
                _letters.Add(sequence.LetterAtCopy(l));
            }//end for

        }//end constructor

        #region IndexingQuerying


        /// <summary>
        /// Number of letters in sequence
        /// </summary>
        /// <returns></returns>
        public int Count {
            get {
                return _letters.Count;
            }
        }//end count


        /// <summary>
        /// Letter at specified index
        /// </summary>
        /// <param name="index">index of letter</param>
        /// <returns>letter as a reference</returns>
        public LSystemLetter LetterAt(int index) {


            //index check...
            if (index >= 0 && index <= _letters.Count - 1) {
                return _letters[index];
            } else {
                throw new Exception("invalid letter sequence index: " + index + ", for list of: " + _letters.Count + " letters");
            }


        }//end LetterAt

        /// <summary>
        /// Letter at specified index.  Creates a safe copy.
        /// </summary>
        /// <param name="index">index of letter</param>
        /// <returns>letter as a copy</returns>
        public LSystemLetter LetterAtCopy(int index) {

            //index check...
            if (index >= 0 && index <= _letters.Count - 1) {

                return new LSystemLetter(_letters[index]);
            } else {
                throw new Exception("invalid letter sequence index: " + index + ", for list of: " + _letters.Count + " letters");
            }

        }


        /// <summary>
        /// Not used presently
        /// Replace letter at index with sequence of letters
        /// </summary>
        /// <param name="index">index of letter to replace</param>
        /// <param name="ls">sequence of letters to replace letter with</param>
        public void ReplaceLetter(int index, LSystemLetterSequence ls) {

            //remove the old letter
            _letters.RemoveAt(index);
            //insert lettersequence
            _letters.InsertRange(index, ls.LettersCopy());
            //_letters.InsertRange(index, ls.Letters());

        }//end replace letter

        /// <summary>
        /// Get the letters in the sequence.
        /// </summary>
        /// <returns>Letters in list as a reference </returns>
        public List<LSystemLetter> Letters() {

            return _letters;

        }//end Letters

        /// <summary>
        /// Creates a copy of letter sequence
        /// </summary>
        /// <returns>Letters in list as a copy </returns>
        public List<LSystemLetter> LettersCopy() {

            List<LSystemLetter> clone = new List<LSystemLetter>(_letters.Count);

            //use copy constructor to generate reference safe copy
            foreach (LSystemLetter l in _letters) {
                clone.Add(new LSystemLetter(l));
            }//end foreach

            return clone;

        }//end LettersCopy

        /// <summary>
        /// Add the letter to the list
        /// 
        /// </summary>
        /// <param name="letter"></param>
        public void Add(LSystemLetter letter) {

            _letters.Add(letter);

        }

        /// <summary>
        /// Appends list to letters
        /// </summary>
        /// <param name="appendList">list to append onto letters</param>
        public void Append(List<LSystemLetter> appendList) {
            _letters.AddRange(appendList);
        }//end append


        /// <summary>
        /// Append using a copy of sequence
        /// </summary>
        /// <param name="appendList"></param>
        public void AppendCopy(List<LSystemLetter> appendList) {

            //copy each letter - copy using constructor
            for (int a = 0; a < appendList.Count; a++) {
                _letters.Add(new LSystemLetter(appendList[a]));
            }//end for

        }//end append

        /// <summary>
        /// Clears all letters fron the list
        /// </summary>
        public void Clear() {
            _letters.Clear();
        }


        

        #endregion

        #region Evaluation

        /// <summary>
        /// evaluates parameters stored in parameter template against language instance
        /// </summary>
        /// <param name="lang"></param>
        public void EvaluateLetters(ILSystemExpressionEvaluator lang) {

            //go through all letters
            for (int l = 0; l < _letters.Count(); l++) {
                //pop current letter
                LSystemLetter cl = _letters[l];
                byte plen = cl.parameters;
                //if there are parameters
                if (plen != 0) {
                    //process all parameters
                    for (int p = 0; p < plen; p++) {

                        string param = cl.template[p];
                        cl.values[p] = lang.Evaluate(param);

                    }//end for

                    //do we need to do this because of value type struct!?
                    //push back to list
                    _letters[l] = cl;

                }//end if parameters

            }//end for

        }//end evalParameters




        #endregion



        #region StringUtils


        /// <summary>
        /// Parse text into letters.
        /// Sequence can look like this:
        /// F+F+F+ 
        /// F(23)+(12, 4)F(12)+(3, 8)F(53)+(78, 34)
        /// </summary>
        /// <param name="text"></param>
        public void Parse(string text) {

            //clean the string
            text = text.Trim();

            int tLen = text.Length;
            int index = 0;

            //initialize null char
            char curLetterKey = '\0';


            while (index < tLen) {

                char cur = text[index];

                //ignore whitespace between chars 
                if (!Char.IsWhiteSpace(cur)) {


                    switch (cur) {

                        case '(':

                            //int closing = findClosingParen(text, index);
                            string contents = "";
                            int closing = ParenthesesMatch(text, index, ref contents);

                            if (closing != -1) {

                                //string argStr = text.Substring(index + 1, closing - index - 1);
                                //use on string between parentheses
                                string[] args = SplitArguments(contents);
                                int argL = args.Length;
                                if (argL > 255) {
                                    throw new Exception("Maximum number of arguments exceeded in letter:");
                                }
                                float[] vals = new float[argL];
                                byte np = (byte)argL;

                                if (curLetterKey != '\0') {
                                    _letters.Add(new LSystemLetter(curLetterKey, args, vals, np));
                                } else {
                                    throw new Exception("Letter parameters present without letter: " + text);
                                }//end if else

                                //set index
                                index = closing + 1;

                                //reset current letter "stack"
                                curLetterKey = '\0';


                            } else {
                                throw new Exception("Mising right parentheses in letter parameter: " + text);
                            }//end if

                            break;


                        case ')':

                            //extra right parentheses
                            throw new Exception("Extra right parentheses in letter parameter: " + text);


                        default:

                            //see if there is a letter on "stack"
                            if (curLetterKey != '\0') {

                                _letters.Add(new LSystemLetter(curLetterKey));

                            }//end if

                            //capture char
                            curLetterKey = cur;
                            index += 1;
                            break;

                    }//end switch

                } else {
                    //skip whitespace
                    index += 1;
                }//end if whitespace

            }//end while

            //make sure no letter is left on "stack"
            //it won't be picked up above if there are no args
            if (curLetterKey != '\0') {

                _letters.Add(new LSystemLetter(curLetterKey, new string[]{}, new float[]{},0));

            }//end if


        }//end parse


        /// <summary>
        /// Splits the string int coomma seperated parts, honoring only top level commas
        /// 
        /// </summary>
        /// <param name="args">String to split</param>
        /// <returns>array of string arguments</returns>
        private static string[] SplitArguments(string args ) {


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
                                stack = stack.Trim();
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
                    stack = stack.Trim();
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
        /// Find text enclosed within parentheses.
        /// </summary>
        /// <param name="text">String to search</param>
        /// <param name="start">Index of opening parentheses '('</param>
        /// <param name="contents">String contained within '(...)'</param>
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
                        contents = text.Substring(start + 1, index - (start + 1));
                        found = index;
                        index = last + 1;//this gets us out of while...
                    }
                }//end else/if

                //increment index
                index += 1;

            }//end while

            return found;

        }// end ParenthesesMatch


        /// <summary>
        /// Standard ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            string lsStr = "";

            foreach (LSystemLetter l in _letters) {
                lsStr += l.ToString() + " ";
            }//end foreach


            return lsStr;

        }//end ToString()


        #endregion



    }//end class
}//end namespace
