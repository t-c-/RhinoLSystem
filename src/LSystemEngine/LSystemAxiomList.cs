using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystemEngine {


    /// <summary>
    /// Custom Linked List for LSystem letters
    /// </summary>
    public class LSystemAxiomList {


        private LSystemAxiomListNode _first;
        private LSystemAxiomListNode _last;
        private int _count;


        public LSystemAxiomList() {

            _first = null;
            _last = null;
            _count = 0;

        }//end constructor



        /// <summary>
        /// Constructor that initializes the linked list with a Letter Sequence.  
        /// Designed to be used with the axiom list parsed from file.
        /// </summary>
        /// <param name="seq">Letter Sequence to initialize list with.</param>
        public LSystemAxiomList(LSystemLetterSequence seq) {

            //set the count
            _count = seq.Count;

            //make sure list is empty already
            if (_first == null && _last == null) {

                //see if there is at least one item to process
                if (_count > 0) {

                    //get first letter in list
                    LSystemLetter fl = seq.LetterAtCopy(0);
                    //set the intial node (this intializes with Previous = null & Next = null)
                    LSystemAxiomListNode leftNode = new LSystemAxiomListNode(fl);
                    _first = leftNode;
                    _last = leftNode;

                    if (_count > 1) {

                        //add additional items to right hand side
                        // start index at 1
                        for (int l = 1; l < _count; l++) {

                            //get copy of letter
                            //LsysLetter cur = seq.LetterAtCopy(l);
                            LSystemAxiomListNode node = new LSystemAxiomListNode(seq.LetterAtCopy(l));

                            //set the previous node's next to current (left to right)
                            leftNode.Next = node;
                            //set node's previous (left) to first 
                            node.Previous = leftNode;
                            //nothing to the right
                            node.Next = null;
                            //it is the last node
                            _last = node;

                            //prepare for next iteration
                            leftNode = node;
                        }//end or

                        //if only item to process, set it as  last node
                    } else {
                        _last = leftNode;
                    }

                } else {

                    throw new Exception("Initialization error: Cannot initialize Axiom with empty list");

                }//end if


            } else {

                throw new Exception("Initialization error: Axiom list is not empty.");
            }//end if/else

        }//end InitializeLetterSequence


        /// <summary>
        /// The First Node in the list
        /// </summary>
        /// <returns></returns>
        public LSystemAxiomListNode First {
            get { return _first; }
        }//end first


        /// <summary>
        /// The Last Node in the list
        /// </summary>
        /// <returns></returns>
        public LSystemAxiomListNode Last {
            get { return _last; }
        }//end last

        /// <summary>
        /// Number of items in the list.
        /// </summary>
        /// <returns></returns>
        public int Count {
            get { return _count; }
        }//end count



        /// <summary>
        /// Clears List (First and Last)
        /// </summary>
        public void Clear() {

            _first = null;
            _last = null;
            _count = 0;

        }//end clear


        /// <summary>
        /// !! Testing - do not use !!
        /// Retrieves the list's letter node at the specified index.  List is iterated from begining or end depending on index.  
        /// This should be used in try/catch block - throws error if index not found.
        /// </summary>
        /// <param name="index">index of letter to retrieve </param>
        /// <returns></returns>
        public LSystemAxiomListNode NodeAt(int index) {


            bool found = false;
            LSystemAxiomListNode foundNode = null;

            int cur_i = 0;
            int dir = 1;
            LSystemAxiomListNode node = null;

            if (index < 0 || index >= _count) {
                throw new Exception("Axiom List invalid index:" + index + ", for " + _count + " items");
            }

            //check which end of list to start from
            //low end 
            if (index < _count / 2) {
                dir =1;
                cur_i = 0;
                node = _first;
            } else {
                dir = -1;
                cur_i = _count -1;
                node = _last;
            }

            

            while (!found) {

                //if we are on specified index, escape
                if (cur_i == index) {
                    found = true;
                    foundNode = node;
                } else {

                    //dir = 1 left to right, dir = -1: right to left
                    if (dir == 1) {
                        node = node.Next;
                        cur_i += 1;
                        if (cur_i >= _count) {
                            throw new Exception("Axiom List index out of bounds:" + index + ", of " + _count + " items");
                        }//end if upper limit check
                    } else if (dir == -1) {
                        node = node.Previous;
                        cur_i -= 1;
                        if (cur_i < 0) {
                            throw new Exception("Axiom List index out of bounds:" + index + ", of " + _count + " items");
                        }//end if lowe limit check
                    }

                }//end if/else found

            }//end while

            if (foundNode != null) {
                return foundNode;
            } else {
                throw new Exception("Axiom List index out of bounds:" + index + ", of " +  _count + " items");
            }


        }//end LetterAt


        /// <summary>
        /// Adds a node to the begining of the list 
        /// TODO: double check this
        /// </summary>
        /// <param name="node"></param>
        public void AddFirst(LSystemAxiomListNode node) {

            //if  list is empty
            if (_count == 0) {

                _first = node;
                _last = node;
                node.Previous = null;
                node.Next = null;
                _count = 1;
            } else {

                //link current first to node
                _first.Previous = node;
                //link node to current first
                node.Next = _first;
                //it's first
                node.Previous = null;
                //set first to node
                _first = node;

                //increment count
                _count += 1;
            }//end if


        }//end AddFirst


        /// <summary>
        /// Adds a node to the end of the list
        /// </summary>
        /// <param name="node"></param>
        public void AddLast(LSystemAxiomListNode node) {

            //if  list is empty
            if (_count == 0) {

                _first = node;
                _last = node;
                node.Previous = null;
                node.Next = null;
                _count = 1;

            } else {

                //link current first to node
                _last.Next = node;
                //link node to current last
                node.Previous = _last;
                //it's last
                node.Next = null;
                //set last to node 
                _last = node;

                //increment count
                _count += 1;
            }//end if

        }

        /// <summary>
        /// Adds a List Node before the specified node (to the left).
        /// </summary>
        /// <param name="before">Node at which to add in front of</param>
        /// <param name="node">Node to add to List</param>
        public void AddBefore(LSystemAxiomListNode before, LSystemAxiomListNode node) {

            //grab the before node's previous (left hand node)
            LSystemAxiomListNode previous = before.Previous;

            if (previous == null) {

                node.Previous = null;
                _first = node;

            } else {

                //associate right hand of previous
                previous.Next = node;
                //associate node's Previous with before's previous
                node.Previous = previous;

            }
            
            //set the current node previous and next
            node.Next = before;
            //link before node's previous
            before.Previous = node;

            //increment count
            _count += 1;

        }//end AddBefore


        /// <summary>
        /// Adds a List Node after the specified node (to the right).
        /// </summary>
        /// <param name="after">Node at which to add behind </param>
        /// <param name="node">Node to add to List</param>
        public void AddAfter(LSystemAxiomListNode after, LSystemAxiomListNode node) {

            //grab the after node's current Next (right hand node)
            LSystemAxiomListNode next = after.Next;

            if (next == null) {
                //after was last node
                _last = node;
                node.Next = null;

            } else {

                //set the next node's Previous (left hand node) to node
                next.Previous = node;
                //set the current node's next (right hand node)
                node.Next = next;
            }

            //set the curent node's previous (left hand node)
            node.Previous = after;
            //set the after node's next
            after.Next = node;

            //increment count
            _count += 1;

        }//end AddAfter




        /// <summary>
        /// Removes the Node from the list
        /// </summary>
        /// <param name="node"></param>
        public void Remove(LSystemAxiomListNode node) {

            LSystemAxiomListNode previous = node.Previous;  //left hand
            LSystemAxiomListNode next = node.Next;  //right hand

            //if this was the only one in the list
            if (previous == null && next == null) {

                _first  = null;
                _last = null;

                //first item in list
            } else if (previous == null) {

                next.Previous = null;
                _first = next;

                //last item in list
            } else if (next == null) {

                previous.Next = null;
                _last = previous;

            } else {

                //link the right & left nodes with each other
                previous.Next = next;
                next.Previous = previous;

            }//end if/else


            //decrement count
            _count -= 1;

        }//end remove

        /// <summary>
        /// Adds a Letter Sequence in front of specified node
        /// </summary>
        /// <param name="before"></param>
        /// <param name="seq"></param>
        public void AddSequenceBefore(LSystemAxiomListNode before,  LSystemLetterSequence seq) {

             
            LSystemAxiomListNode leftNode = before.Previous;
            int firstIndex = 0;
            int seqCnt = seq.Count;

            //if 'before' was the first node
            if (leftNode == null) {
                firstIndex = 1;
                leftNode = new LSystemAxiomListNode(seq.LetterAtCopy(0));
                //it's the first now
                _first = leftNode;
                //update counter for added node
                _count += 1;
            }


            //add additional items to right hand side
            // start index at 1
            for (int l = firstIndex; l < seqCnt; l++) {

                //get copy of letter
                //LsysLetter cur = seq.LetterAtCopy(l);
                LSystemAxiomListNode node = new LSystemAxiomListNode(seq.LetterAtCopy(l));

                //set the previous node's next to current (left to right)
                leftNode.Next = node;
                //set node's previous (left) to first 
                node.Previous = leftNode;
                //before is to the right
                node.Next = before;
                //update before
                before.Previous = node;

                //update counter
                _count += 1;

                //prepare for next iteration
                leftNode = node;
            }//end or

            //after all nodes are addded
            //connect right hand link
            leftNode.Next = before;
            before.Previous = leftNode;
                

        

        }//end AddSequenceBefore


 
        /// <summary>
        /// Adds a sequence to the end of the list
        /// Performs a safe copy (looks ok, but verify)
        /// </summary>
        /// <param name="seq"></param>
        public void AddSequenceToEnd( LSystemLetterSequence seq) {


            LSystemAxiomListNode leftNode = _last;
            int firstIndex = 0;
            int seqCnt = seq.Count;


            //first node in list?
            if (leftNode == null) {
                //intialize first sequence letter as first list letter...
                firstIndex = 1;
                leftNode = new LSystemAxiomListNode(seq.LetterAtCopy(0));
                //it's the first now
                _first = leftNode;
                //and the last...
                _last = leftNode;
                //update counter for first node
                _count = 1;
            }


            //add additional letters to right hand side
            for (int l = firstIndex; l < seqCnt; l++) {

                //get copy of letter
                //LsysLetter cur = seq.LetterAtCopy(l);
                LSystemAxiomListNode node = new LSystemAxiomListNode(seq.LetterAtCopy(l));

                //set the previous node's next to current (left to right)
                leftNode.Next = node;
                //set node's previous (left) to first 
                node.Previous = leftNode;
                //make this tail node the last node
                node.Next = null;
                _last = node;

                //update counter
                _count += 1;

                //prepare for next iteration
                leftNode = node;
            }//end or



        }//end AddSequenceToEnd


        /// <summary>
        /// Replaces a Letter with a LetterSequence.
        /// This is causing the context troubles - by using this context is changing, duh!
        /// Discontinue use...
        /// </summary>
        /// <param name="replace"></param>
        /// <param name="seq"></param>
        public void ReplaceNode(LSystemAxiomListNode replace, LSystemLetterSequence seq) {


            LSystemAxiomListNode leftNode = replace.Previous;
            int firstIndex = 0;
            int seqCnt = seq.Count;

            // --- routine based on add before

            //if 'before' was the first node
            if (leftNode == null) {
                firstIndex = 1;
                leftNode = new LSystemAxiomListNode(seq.LetterAtCopy(0));
                //it's the first now
                _first = leftNode;
                //update counter for added node
                _count += 1;
            }


            //add additional items to right hand side
            // start index at 1
            for (int l = firstIndex; l < seqCnt; l++) {

                //get copy of letter
                //LsysLetter cur = seq.LetterAtCopy(l);
                LSystemAxiomListNode node = new LSystemAxiomListNode(seq.LetterAtCopy(l));

                //set the previous node's next to current (left to right)
                leftNode.Next = node;
                //set node's previous (left) to first 
                node.Previous = leftNode;
                //before is to the right
                node.Next = replace;
                //update before
                replace.Previous = node;

                //update counter
                _count += 1;

                //prepare for next iteration
                leftNode = node;
            }//end or

            //resolve right side
            LSystemAxiomListNode rightNode = replace.Next;

            //after all nodes are addded
            //connect right hand link if present, otherwise node is last
            if (rightNode != null) {

                leftNode.Next = rightNode;
                rightNode.Previous = leftNode;

                //right is null, this node is last
            } else {

                leftNode.Next = null;
                _last = leftNode;

            }//end if/else right node

            //remeber to take one away for removed member
            _count -= 1;

        }//end ReplaceNode


        /// <summary>
        /// Prints the  axiom list
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            string listStr = "";
            LSystemAxiomListNode node = _first;

            //make sure there are nodes to print
            if (_count > 0 && node != null) {

                while (node != null) {
                    //capture letter in string
                    listStr += node.ToString();
                    //move next
                    node = node.Next;

                }//end while
               

            }//end if nodes to print

            return listStr + " (" + _count + ")";
        }

    }//end class


    public class LSystemAxiomListNode {

         LSystemAxiomListNode _previous;
         LSystemAxiomListNode _next;

         LSystemLetter _letter;


        /// <summary>
        /// Not currently used...
        /// </summary>
        public LSystemAxiomListNode() {

 
        }//end constructor

        /// <summary>
        /// Constructor that takes Letter as input
        /// </summary>
        /// <param name="letter"></param>
        public LSystemAxiomListNode(LSystemLetter letter) {

            _letter = letter;
            _previous = null;
            _next = null;


            //for memory management, template could be cleared here
            //_letter.ClearTemplate();

        }//end constructor

        /// <summary>
        /// Internal Constructor usedby Clone();
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="letter"></param>
        /// <param name="next"></param>
        private LSystemAxiomListNode(LSystemAxiomListNode prev, LSystemLetter letter, LSystemAxiomListNode next) {

            _letter = new LSystemLetter(letter);
            //maintain links
            _previous = prev;
            _next = next;

        }

        /// <summary>
        /// Previous Letter in list (left hand neighbor)
        /// </summary>
        public LSystemAxiomListNode Previous {
            set {
                _previous = value;
            }
            get {
                return _previous;
            }
        }//end predecessor


        /// <summary>
        /// Next Letter in the List (right hand neighbor)
        /// </summary>
        public LSystemAxiomListNode Next {
            set {
                _next = value;
            }
            get {
                return _next;
            }

        }//end ssuccessor

        /// <summary>
        /// Letter contained in list node
        /// </summary>
        public LSystemLetter Letter {
            set {
                _letter = value;
            }
            get {
                return  _letter;
            }
        }//end letter


        public LSystemAxiomListNode Clone() {

            return new LSystemAxiomListNode(_previous, _letter, _next);

        }


        /// <summary>
        /// Print the letter in the node
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            
            return _letter.ToString();
            
        }//end to string

    }//end class LsysAxiomListNode



}//end namespace
