using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

using LSystemExpressions;


namespace LSystemEngine {

    /// <summary>
    /// Production object
    /// </summary>

   public class LSystemProduction {

       private string _branchSymbols;
       private string _ignoreSymbols;


        private LSystemLetterSequence _leftContext;
        private LSystemLetterSequence _rightContext;
        private LSystemLetter _predecessor;
        private string _condition;

        private LSystemLetterSequence _successor;

        private ILSystemExpressionEvaluator _lang;

       //these are used as tempory lists in the production
       // evaluation tests
        List<LSystemLetter> productionTemplates;
        List<LSystemLetter> productionValues;


        public LSystemProduction(ILSystemExpressionEvaluator lang) {

            _leftContext = null;
            _rightContext = null;
            _predecessor = new LSystemLetter('\0');
            _condition = String.Empty;

            //set the reference to the expression evaluator
            _lang = lang;

            _branchSymbols = string.Empty;
            _ignoreSymbols = string.Empty;

            //intialize lists for later use
            productionTemplates = new List<LSystemLetter>();
            productionValues = new List<LSystemLetter>();

        }//end constructor


       /// <summary>
       /// The successor of the production.  Returns a Copy, so it's reference safe.
       /// </summary>
        public LSystemLetterSequence Successor {
            get {
                return new LSystemLetterSequence(_successor);
            }
        }//end property successor


       /// <summary>
       /// Check to see if Production has left context as part of definition.
       /// </summary>
        public bool HasLeftContext {
            get {
                if (_leftContext == null) {
                    return false;
                } else {
                    return true;
                }
            }
        }//end HasLeftContext


       /// <summary>
       /// Tests wether the current axiom list letter matches the conditipos of this production.
       /// </summary>
       /// <param name="axiomNode">The current axiom letter being tested</param>
       /// <returns>True if the axiom letter matches the conditions of this production.</returns>
        public bool Matches(LSystemAxiomListNode axiomNode) {


            //bool isMatch = false;
            LSystemLetter curLtr = axiomNode.Letter;

            //Debug.Print("checking production: " + this.ToString() + " against letter: " + axiomNode.ToString());
            /*
             *  
             * Test to see if production matches letter 
             * 
             */
            //!start from scratch!!!!
            //first step - see if letter matches
            if (curLtr.key == _predecessor.key) {


                //lists to hold letter values and template values for language variables
                //fill the list from left to right: left context : predecessor : right context
                //prepare the lists for use
                //these are filled by the production context checks
                productionTemplates.Clear ();
                productionValues.Clear();

                //step two context matching
                /*
                 *   --- Left Context ---
                 * 
                 */

                if (_leftContext != null) {

                    LSystemLetterSequence lc = _leftContext;
                    //new version of context matching

                    if (!ContextCheckLeft(axiomNode, lc)) {
                        return false;
                    }

                    //Debug.Print("left context true");

                }//end if left context


                //add the current predeccessor to letter references
                productionValues.Add(curLtr);
                productionTemplates.Add(_predecessor);


                /*
                 *    --- Right Context ---
                 * 
                 */
                if (_rightContext != null) {
                    LSystemLetterSequence rc = _rightContext;
                    if (!ContextCheckRight(axiomNode, rc)) {
                        return false;
                    }

                   // Debug.Print("right context true");                  


                }//end if right context


                /*
                 * 
                 *   Evaluate variable template in letters
                 *   since we need to do this for the condition.
                 *   if production matches, then all these variables 
                 *   need to be set in the language anyways.
                 * 
                 * 
                 */

                //process language template for predeccessor,
                // and left/right contexts
                int tempCount = productionTemplates.Count;

                //for simple productions this will be a single letter
                //  with multiple left/right contexts, there will be more
                for (int n = 0; n < tempCount; n++) {
                    LSystemLetter tmplt_ltr = productionTemplates[n]; 
                    LSystemLetter value_ltr = productionValues[n]; 

                    int tp = tmplt_ltr.parameters;
                    int vp = value_ltr.parameters;
                    
                    if (tp != vp) {
                        throw new Exception("Mixed matched template and values in production evaluation for p:" + this.ToString());
                    }//end if

                    //check to see if there are parameters to process
                    if (vp > 0) {

                        //loop through parameters and values- set to lang
                        for (int t = 0; t < tp; t++) {
                            string v_name = tmplt_ltr.template[t];
                            float v_value = value_ltr.values[t];

                            //put the variable to lang
                            //_lang.SetVariable(v_name, v_value);

                            //new testing - temporary variables
                            _lang.SetTemporaryVariable(v_name, v_value);
                        }//end for
                    }//end if parameters to process


                }//end for letters


                /*
                 * 
                 *  Conditional testing
                 * 
                 */
                if (!String.IsNullOrEmpty(_condition)) {

                    float condResult = _lang.Evaluate(_condition);

                    if (condResult != 1 && condResult != 0) {
                        throw new Exception("production condition did not return 1 or 0, p:" + this.ToString());
                    } else {
                        //if condition is false, return 0
                        if (condResult == 0) {
                            return false;
                        } 
                    }//end if

                }//end if context


                /*
                 * 
                 * since it made it this far, everything passes, return true! 
                 * 
                 */

                return true;


            } else {

                //letters don't match
                return false;

            }//end if letter matches


        }//end matches






       /// <summary>
       /// Testing another new version...
       /// works!
       /// Perform Left Context Check
       /// </summary>
       /// <param name="curNode">The curent node being chcked for context, the previous node will be pulled</param>
       /// <param name="contextLeft">The left context specification</param>
       /// <returns></returns>
        private bool ContextCheckLeft(LSystemAxiomListNode curNode, LSystemLetterSequence leftContext) {

            bool contextOK = false;
            //int index = 0;

            //right most letter of left context
            //search happens right to left
            int contextIndex = leftContext.Count - 1;

            LSystemLetter curMatchSymb;
            char curMatchKey = '\0';
            bool search = true;

            //get the initial left node - need to do clone!?...
            //make sure psoition from level above is not all messed up...
            LSystemAxiomListNode leftNode = curNode.Clone();

            //search should be driven by context specification
            //this could be very simple if not looking for sub-branch
            while (search) {

                //grab the current left node
                leftNode = leftNode.Previous;



                //make sure the left node is not an ignored letter
                ignoreFilterLeft(ref leftNode);


                //if the current left node is null - beging has been reached
                //  context specification has not been met, return false
                if (leftNode == null) {
                    contextOK = false;
                    search = false;
                    break;
                }

                //pop the current context letter
                curMatchSymb = leftContext.LetterAt(contextIndex);
                curMatchKey = curMatchSymb.key;

                //Debug.Print("left context search \r\n");
                //Debug.Print("left letter = " + leftNode.ToString() + " \r\n");
                //Debug.Print("left context symbol = " + curMatchSymb.ToString() + " \r\n");

                //what are we looking for?
                //sub branch specification? ...[B]<n>...
                if (isCloseBranch(curMatchKey)) {
                    //looking for sub branch speicification...
                    //grab sub branch spec, recursivly check to resolve any sub branch levels below...
                    // A[B[CB[AA]]C]

                    //are we at a close branch "]" in axiom (is there a sub branch to search?)
                    if (isCloseBranch(leftNode.Letter.key)) {

                        //get sub-branch spec
                        int nextIndex = contextIndex;
                        LSystemLetterSequence subSpec = getLeftContextSubBranch(contextIndex, leftContext, ref nextIndex);
                        //move next left

                        //this needs to find begining of current level branch and check whole branch against
                        //sub-branch spec in left context
                        leftNode = findBranchOpenSymbol(leftNode);
                        if (leftNode == null) {
                            contextOK = false;
                            break;
                        }
                        //set ahead of "["
                        LSystemAxiomListNode branchNode = leftNode;
                        //leftNode = leftNode.Previous;
                        //enter the context checking recursively 
                        // - this actually needs to search right
                        //if (ContextCheckLeft(leftNode, subSpec)) {
                        if (ContextCheckRight(branchNode, subSpec)) {
                            //set index of left - don't do this now - left node already there
                            //leftNode = findBranchOpenSymbol(leftNode);
                            //fixed context index...
                            contextIndex = nextIndex;
                        } else {
                            //sub branch check pattern failed!
                            contextOK = false;
                            break;


                        }//end if else recursive sub-branch check


                    } else {
                        //looking for sub branch, but didn't find it
                        contextOK = false;
                        break;
                    }//end if /else is close branch



                } else {

                    //doing simple letter match - no sub branch matching
                    //check left context node from axiom
                    //is this the end of a branch?
                    if (isCloseBranch(leftNode.Letter.key)) {
                        //Debug.Print("left context close branch");
                        //skip over branch to next node
                        //make the left node the opening branch symbol
                        //and run from beging of loop (null/ignore/etc)
                        leftNode = findBranchOpenSymbol(leftNode);
                        //not incrementing cntxtIndex! keep same position

                    } else if (isOpenBranch(leftNode.Letter.key)) {
                        //go outside of branch if already inside branch per pg32 abop
                        //Debug.Print("left context open branch");
                        //don't do this:
                        //leftNode = leftNode.Previous;

                        //just fall through while not incrementing cntxtIndex! keep same position

                        //this should be !?:
                        //contextOK = false;
                        //break;


                    } else {

                        //do a simple match here

                        //looking for simple letter match
                        if (curMatchKey == leftNode.Letter.key) {
                            //this letter matches

                            //save this matched letter!
                            //do something!

                            //add template from letter stored in production left context
                            productionTemplates.Add(curMatchSymb);
                            //add value reference from letter in axiom
                            productionValues.Add(leftNode.Letter);                            

                            //go next left context letter (countdown)
                            contextIndex -= 1;
                            if (contextIndex == -1) {
                                //last letter was found successfully
                                //everything ok!
                                contextOK = true;
                                break;
                                //return true;
                            }

                            //fall through to begining of while - it pops next context letter

                        } else {
                            //context match has failed!

                            contextOK = false;
                            break;
                            //return false;


                        }//end if/else key match

                    }//end if else skip branch


                }//end if else sub-branch search



            }//end while

            //Debug.Print("left context result = " + contextOK);

            return contextOK;

        }//end ContextCheckLeft


       /// <summary>
       /// Perform Right Context Check
       /// </summary>
       /// <param name="curNode"></param>
       /// <param name="rightContext"></param>
       /// <returns></returns>
        private bool ContextCheckRight(LSystemAxiomListNode curNode, LSystemLetterSequence rightContext) {

            bool contextOK = false;

            //left most letter of right context
            //search happens left to right
            int contextIndex = 0;

            LSystemLetter curMatchSymb;
            char curMatchKey = '\0';
            bool search = true;

            //get the initial left node - need to do clone!?...
            //make sure psoition from level above is not all messed up...
            LSystemAxiomListNode rightNode = curNode.Clone();

            //search should be driven by context specification
            //this could be very simple if not looking for sub-branch
            while (search) {

                //grab the current left node
                rightNode = rightNode.Next;





                //make sure the left node is not an ignored letter
                ignoreFilterRight(ref rightNode);


                //if the current right node is null - end has been reached
                //  context specification has not been met, return false
                if (rightNode == null) {
                    contextOK = false;
                    search = false;
                    break;
                }

                //pop the current context letter
                curMatchSymb = rightContext.LetterAt(contextIndex);
                curMatchKey = curMatchSymb.key;

                //Debug.Print("right context search \r\n");
                //Debug.Print("right letter = " + rightNode.ToString() + " \r\n");

                //Debug.Print("right context symbol = " + curMatchSymb.ToString() + " \r\n");

                //what are we looking for?
                //sub branch specification? ...<n>[B]...
                if (isOpenBranch(curMatchKey)) {
                    //looking for sub branch speicification...
                    //grab sub branch spec, recursivly check to resolve any sub branch levels below...
                    // A[B[CB[AA]]C]

                    //are we at an open branch "[" in axiom (is there a sub branch to search?)
                    if (isOpenBranch(rightNode.Letter.key)) {

                        //get sub-branch spec
                        int nextIndex = contextIndex;
                        LSystemLetterSequence subSpec = getRightContextSubBranch(contextIndex, rightContext, ref nextIndex);
  
                        //enter the context checking recursively 
                        if (ContextCheckRight(rightNode, subSpec)) {
                            //set index of left
                            rightNode = findBranchCloseSymbol(rightNode);
                            //fixed context index...
                            contextIndex = nextIndex;
                        } else {
                            //sub branch check pattern failed!
                            contextOK = false;
                            break;

                        }//end if else recursive sub-branch check


                    } else {
                        //looking for sub branch, but didn't find it
                        contextOK = false;
                        break;
                    }//end if /else is close branch



                } else {

                    //doing simple letter match - no sub branch matching
                    //check left context node from axiom
                    //is this the end of a branch?
                    if (isOpenBranch(rightNode.Letter.key)) {
                        //Debug.Print("right context open branch");
                        //skip over branch to next node
                        //make the right node the closing branch symbol
                        //and run from beging of loop (null/ignore/etc)
                        rightNode = findBranchCloseSymbol(rightNode);
                        //not incrementing cntxtIndex! keep same position

                    } else if (isCloseBranch(rightNode.Letter.key)) {
                        //go outside of branch if already inside branch per pg32 abop
                        //Debug.Print("right context close branch");
                        //don't do this:
                        //rightNode = rightNode.Next;
                        //just fall through while not incrementing cntxtIndex! keep same position

                    } else {

                        //do a simple match here

                        //looking for simple letter match
                        if (curMatchKey == rightNode.Letter.key) {
                            //this letter matches

                            //save this matched letter!
                            //do something!

                            //add template from letter stored in production right context
                            productionTemplates.Add(curMatchSymb);
                            //add value reference from letter in axiom
                            productionValues.Add(rightNode.Letter);

                            //go next right context letter 
                            contextIndex += 1;
                            //past end?
                            if (contextIndex == rightContext.Count) {
                                //last letter was found successfully
                                //everything ok!
                                contextOK = true;
                                break;
                                //return true;
                            }

                            //fall through to begining of while - it pops next context letter

                        } else {
                            //context match has failed!

                            contextOK = false;
                            break;
                            //return false;


                        }//end if/else key match

                    }//end if else skip branch


                }//end if else sub-branch search



            }//end while

            //Debug.Print("right context result = " + contextOK);

            return contextOK;

        }//end ContextCheckLeft



        #region "Basic Branch Symbol Tests"

        /// <summary>
        /// Checks to see if char is same as open branch symbol ('[')
        /// </summary>
        /// <param name="key">Symbol to check</param>
        /// <returns>true if symbol is same as open branch, false otherwise and false if no bramch symbols were defined</returns>
        private bool isOpenBranch(char key) {

            if (_branchSymbols != string.Empty) {

                if (key == _branchSymbols[0]) {

                    return true;

                } else {
                    return false;
                }

            } else {
                //no branc symbols set
                return false;

            }//end if/else


        }//end isOpenBranch


        /// <summary>
        /// Checks to see if char is same as close branch symbol (']')
        /// </summary>
        /// <param name="key">Symbol to check</param>
        /// <returns>true if symbol is same as close branch, false otherwise and false if no bramch symbols were defined</returns>
        private bool isCloseBranch(char key) {

            if (_branchSymbols != string.Empty) {

                if (key == _branchSymbols[1]) {

                    return true;

                } else {
                    return false;
                }

            } else {
                //no branc symbols set
                return false;

            }//end if/else


        }//end isOpenBranch



        #endregion


        #region "Branch Searching for Axiom Nodes"


        /// <summary>
       /// Search left for next open branch symbol.
       /// </summary>
       /// <param name="curNode">This is the node containing the Close Branch symbol</param>
       /// <returns>The branch node containing the Open Branch symbol</returns>
        private LSystemAxiomListNode findBranchOpenSymbol(LSystemAxiomListNode curNode) {

            //prep with current node
            LSystemAxiomListNode nextNode = curNode;
            int depth = 0;
            bool search = true;
 
            //null check
              //ignore check - not relevant
              //close branch symbol (set depth)
              //open branch symbol (set depth)
                  //recheck after depth...
            while (search) {

                //get next node
                nextNode = nextNode.Previous;

                if (nextNode != null) {
                    //check for open branch
                    if (isOpenBranch(nextNode.Letter.key)) {

                        if (depth == 0) {
                            //found it!
                            //return nextNode;
                            search = false;
                        } else {
                            //pop depth - look for next open branch
                            depth -= 1;
                        }
                        //ccheck fior close branch
                    } else if (isCloseBranch(nextNode.Letter.key)) {
                        //push depth - go a level deeper
                        depth += 1;;

                    }//end if/else

                    

                } else {
                    //hit begining!
                    //this should really be an error condition
                    throw new Exception("Failed to find open branch, missing open branch: " + _branchSymbols);
                    //return null;
                }

            }//end while search

            //temp...
            return nextNode;

        }//findBranchOpenSymbol


       /// <summary>
       /// Search Right for next close branch symbol.
       /// </summary>
        /// <param name="curNode">The branch node containing the Close Branch symbol</param>
        /// <returns>The branch node containing the Close Branch symbol</returns>
        private LSystemAxiomListNode findBranchCloseSymbol(LSystemAxiomListNode curNode) {

            //prep with current node
            LSystemAxiomListNode nextNode = curNode;
            int depth = 0;
            bool search = true;

            //null check
            //ignore check - not relevant
            //close branch symbol (set depth)
            //open branch symbol (set depth)
            //recheck after depth...
            while (search) {

                //get next node
                nextNode = nextNode.Next;

                if (nextNode != null) {
                    //check for close branch
                    if (isCloseBranch(nextNode.Letter.key)) {

                        if (depth == 0) {
                            //found it!
                            //return nextNode;
                            search = false;
                        } else {
                            //pop depth - look for next open branch
                            depth -= 1;
                        }
                        //ccheck for open branch
                    } else if (isOpenBranch(nextNode.Letter.key)) {
                        //push depth - go a level deeper
                        depth += 1; ;

                    }//end if/else



                } else {
                    //hit end (sarching right)!
                    //this should really be an error condition
                    throw new Exception("Failed to find close branch, missing close branch: " + _branchSymbols);
                    //return null;
                }

            }//end while search

            //temp...
            return nextNode;

        }//findBranchOpenSymbol

        #endregion


        #region "Ignore Filters"

        /// <summary>
        /// Checks if Letter is to be ignored.  New version using standard string.IndexOf().
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if letter is to be ignored, false if not ignored or no ignore specification exists</returns>
        private bool ignoreLetter(char key) {

            if (_ignoreSymbols != string.Empty) {

                if (_ignoreSymbols.IndexOf(key) != -1) {
                    return true;
                } else {
                    return false;
                }

            } else {
                //no ignore specification
                return false;
            }

        }//end ignoreLetter



        /// <summary>
       /// Takes the current node and checks it against the ignore list.
       /// Searches Left To Right
       /// </summary>
       /// <param name="curNode"></param>
        private void ignoreFilterLeft(ref LSystemAxiomListNode curNode) {
            //init check ignore
            bool ignoreNode = false;
            //check for begining reached
            if (curNode != null) {
                //begining reached 
                ignoreNode = ignoreLetter(curNode.Letter.key);
            }
            
     
            //while left node is not ignore...
            //find the next non-ignored letter
            while (ignoreNode) {
                //get next left node
                curNode = curNode.Previous;
                //if it's not null, check it
                if (curNode == null) {
                    //begining reached 
                    break;
                }
                //perform check again
                ignoreNode = ignoreLetter(curNode.Letter.key);

            }//end while


        }//end ignoreFilterLeft


        /// <summary>
        /// Takes the current node and checks it against the ignore list.
        /// Seraches Right to Left
        /// </summary>
        /// <param name="curNode"></param>
        private void ignoreFilterRight(ref LSystemAxiomListNode curNode) {

            //init check ignore.
            bool ignoreNode = false;

            //check for end reached
            if (curNode != null) {
                ignoreNode = ignoreLetter(curNode.Letter.key);
            } 

            //while left node is not ignore...
            //find the next non-ignored letter
            while (ignoreNode) {
                //get next left node
                curNode = curNode.Next;
                //if it's not null, check it
                if (curNode == null) {
                    //begining reached 
                    break;
                }
                //perform check again
                ignoreNode = ignoreLetter(curNode.Letter.key);

            }//end while


        }//end ignoreFilterLeft

        #endregion


        #region "Context Sub-Branch Searches"


        /// <summary>
       /// Gets the Sub Branch Specification from the left context
       /// i.e.:  context = JK[H]M  returns "H"
       /// i.e.: context = JK[I[P]H]M returns "I[P]H"
       /// </summary>
       /// <param name="index"></param>
       /// <param name="context"></param>
       /// <returns></returns>
        private LSystemLetterSequence getLeftContextSubBranch(int index, LSystemLetterSequence context, ref int seqEnd) {

            LSystemLetterSequence subSeq = new LSystemLetterSequence();
            LSystemLetter curSymb;

            //initialize at next left context letter
            int n = 1;
            bool search = true;
            int depth = 0;

            while (search) {

                //search left
                curSymb = context.LetterAtCopy(index - n);

                //check for closing branch "]"
                if (isCloseBranch(curSymb.key)) {
                    //push depth & add symbol
                    depth += 1;
                    subSeq.Add(curSymb);

                    //check for opening branch "[" the target
                } else if (isOpenBranch(curSymb.key)) {
                    //check depth
                    if (depth == 0) {
                        //found it - sub-dequence complete
                        search = false;

                    } else {
                        //pop depth  & add symbol
                        depth -= 1;
                        subSeq.Add(curSymb);

                    }

                } else {
                    //not a branch, just add it
                    subSeq.Add(curSymb);

                }//end if/elseif close/open branch

                //go to next context letter
                n += 1;

            }//end while search

            //set the end of sequence marker
            seqEnd = index - n ;

            return subSeq;

        }//en getSubSequenceleft


       /// <summary>
       /// TODO: needs to be checked for seqEnd!
       /// Gets the Sub Branch Specification from the right context
       /// </summary>
       /// <param name="index"></param>
       /// <param name="context"></param>
       /// <param name="seqEnd"></param>
       /// <returns></returns>
        private LSystemLetterSequence getRightContextSubBranch(int index, LSystemLetterSequence context, ref int seqEnd) {

            LSystemLetterSequence subSequence = new LSystemLetterSequence();
            LSystemLetter curSymb;

            //initialize at next left context letter
            int n = 1;
            bool search = true;
            int depth = 0;

            while (search) {

                //search Right
                curSymb = context.LetterAtCopy(index + n);

                //check for opening branch "["
                if (isOpenBranch(curSymb.key)) {
                    //push depth & add symbol
                    depth += 1;
                    subSequence.Add(curSymb);

                    //check for closing branch "]" the target
                } else if (isCloseBranch(curSymb.key)) {
                    //check depth
                    if (depth == 0) {
                        //found it - sub-dequence complete
                        search = false;

                    } else {
                        //pop depth  & add symbol
                        depth -= 1;
                        subSequence.Add(curSymb);

                    }

                } else {
                    //not a branch, just add it
                    subSequence.Add(curSymb);

                }//end if/elseif close/open branch

                //go to next context letter
                n += 1;

            }//end while search

            //set the end of sequence marker
            seqEnd = index + n ;

            return subSequence;

        }//en getSubSequenceleft

        #endregion

        #region Production testing"
       



       /// <summary>
       /// Testing for new context matching...
       /// 
       /// </summary>
       /// <param name="c_letter">Current axiom letter to test against</param>
       /// <param name="l_context">Letter list representing left context</param>
       /// <returns></returns>
        private bool ContextMatchLeft(LSystemAxiomListNode c_letter, LSystemAxiomList l_context) {

            bool match = false;


            LSystemAxiomListNode cur_cntx_ltr = l_context.Last;

            //filter left ignore






            return match;

        }

       /// <summary>
       /// testing for sub branch matching..
       /// idea here is to pass letter position of opening branch "["
       /// </summary>
       /// <param name="cur_letter"></param>
       /// <param name="branch"></param>
       /// <returns></returns>
        private bool SubBranchMatches(LSystemAxiomListNode cur_letter, LSystemAxiomList branch) {


            return false;

        }

        #endregion


        #region "Parsing Functions"




        //new syntaxt parser
        //     0            1            2                3              4         5
        //production : left context : predecessor : right context : condition : successor
        //token[0] = "production"

        /// <summary>
        /// Parse the production from string. 
        /// <remarks>This is utilized by the file parser to read in productions.</remarks>
        /// <remarks>Production tokens are passed in the following order: </remarks>
        /// <remarks>0 production : 1 left context : 2 predecessor : 3 right context : 4 condition : 5 successor</remarks>
        /// </summary>
        /// <param name="tokens">String array tokenized from line of file</param>

        public void Parse(string[] tokens) {
            //use to parse right/left context
            LSystemLetterSequence ls = new LSystemLetterSequence();

            if (tokens.Length != 6) {
                throw new Exception("Invalid number of tokens in parse production.");
            }


            string leftC = tokens[1].Trim();
            string pred = tokens[2].Trim();
            string rightC = tokens[3].Trim();
            string cond = tokens[4].Trim();
            string succ = tokens[5].Trim();
            
            //check for empty  predecessor
            if (String.IsNullOrEmpty(pred)) {
                throw new Exception("Invalid empty predecessor in production.");
            }

            //resolve predecessor - parse the text
            ls.Parse(pred);
            if (ls.Count == 1) {
                _predecessor = ls.LetterAt(0);
            } else {
                throw new Exception("Malformed predecessor in production:" + pred);
            }


            //check left hand context
            if (leftC != "*") {
                //init and parse letter sequence
                _leftContext = new LSystemLetterSequence();
                _leftContext.Parse(leftC);
                if (_leftContext.Count == 0) {
                    throw new Exception("Malformed left context in production:" + leftC);
                } 
            }

            //check right context
            if (rightC != "*") {
                //init and parse letter sequence
                _rightContext = new LSystemLetterSequence();
                _rightContext.Parse(rightC);
                if (_rightContext.Count == 0) {
                    throw new Exception("Malformed right context in production:" + rightC);
                } 
            }

            //condition
            if (cond != "*") {
                _condition = cond;
            }

            //check for empty successor
            if (String.IsNullOrEmpty(succ)) {
                throw new Exception("Invalid empty successor for production: " + pred);
            }

            //parse letter sequence for successor
            _successor = new LSystemLetterSequence();
            _successor.Parse(tokens[5].Trim());

        }//end Parse 

       /// <summary>
       /// Sets the internal representation of the Ignore statement defined in the LSystem.
       /// This can be a string with any number of characters, but should not have any whitespace (ends are trimmed).
       /// </summary>
       /// <param name="ignore"></param>
        public void SetIgnoreSymbols(string ignore) {

            _ignoreSymbols = ignore;

        }

       /// <summary>
       /// Sets the internal representation of the Branch Symbols defined in the LSystem.
       /// This will be checked on parsing of the LSystem, but must be two character string,
        /// i.e: "[]" =  branch open / branch close
       /// </summary>
       /// <param name="branch"></param>
        public void SetBranchSymbols(string branch) {

            _branchSymbols = branch;

        }


        #endregion


        #region "ToString"

        /// <summary>
        /// Generates a string of the production.
        /// </summary>
        /// <returns>String representation of production</returns>
        public override string ToString() {
            string pString;
            string lc = "*";
            string rc = "*";
            string cd = "*";

            if (_leftContext != null) {
                lc = _leftContext.ToString();
            }
            if (_rightContext != null) {
                rc = _rightContext.ToString();
            }

            if (_condition != String.Empty) {
                cd = _condition;
            }

            pString = "production";
            pString += " : " + lc;
            pString += " : " + _predecessor;
            pString += " : " + rc;
            pString += " : " + cd;
            pString += " : " + _successor;

            return pString;
        }

        #endregion


   }//end class
}//end namespace
