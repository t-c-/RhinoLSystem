using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystemEngine {

    public interface ILSystemListener {

        /// <summary>
        /// Called when string (file) parsing begins.
        /// </summary>
         void ParseBegin();


        /// <summary>
        /// Called during string (file) parsing when an LSystem is loaded.
        /// </summary>
        /// <param name="name"></param>
         void ParseLoadedSystem(string name);


        /// <summary>
        /// Called when string (file) parsing ends.  Reports the number of loaded LSystems.
        /// </summary>
         void ParseEnd();


         /// <summary>
         /// Called when the LSystem parser encounters an exception.
         /// </summary>
         /// <param name="msg">The exception message.</param>
         /// <param name="ex">The exception encountered.</param>
         void LSystemParseError(string msg, Exception ex);


        /// <summary>
        /// Called when the execution of an LSystem begins.  This will also be called when a nested LSystem executes.
        /// </summary>
        /// <param name="name">The name of the LSystem to be evaluated.</param>
        void LSystemExecutionBegin(string name);

        /// <summary>
        /// Called when the execution of a sub-system (nested LSystem) begins.
        /// </summary>
        /// <param name="name"></param>
        void SubSystemExecutionBegin(string name);

        /// <summary>
        /// Called when the evaluation of an LSystem begins (start of production cycle).
        /// </summary>
        /// <param name="nam">The name of the LSytem being evaluated.</param>
        void EvaluationBegin(string name);

       
        /// <summary>
        /// Called during the evaluation of an LSystem when a letter is being test against the productions.
        /// </summary>
        /// <param name="name">The name of the LSytem beign evaluated.</param>
        /// <param name="iteration">The current iteration beign processed.</param>
        /// <param name="ofIterations">The total number of iterations to process.</param>
        /// <param name="letter">The index number of the current letter being processed</param>
        /// <param name="ofLetters">The total number of letters to process.</param>
        void EvaluationProgress(string name, int iteration, int ofIterations, int letter, int ofLetters);


        /// <summary>
        /// Called when the evaluation of an LSystem ends (end of production cycle).
        /// </summary>
        /// <param name="name">The name of the LSytem evaluated.</param>
        void EvaluationEnd(string name);


        /// <summary>
        /// Called when the resolving of an LSystem begins (all final axim letters are resolved from groups, aliases, and nested LSystems.
        /// </summary>
        /// <param name="name">The name of the LSystem to be resolved.</param>
        void ResolverBegin(string name);


        /// <summary>
        /// Called during the resolver process.
        /// </summary>
        /// <param name="name">The name of the LSystem being resolved</param>
        /// <param name="letter">The index of the Letter currently being resolved.</param>
        /// <param name="ofLetters">The number of Letters to process.</param>
        void ResolverProgress(string name, int letter, int ofLetters);


        /// <summary>
        /// Called when the resolving of an LSystem ends.
        /// </summary>
        /// <param name="name">The name of the LSystem resolved.</param>
        void ResolverEnd(string name);

        /// <summary>
        /// Called when the execution of a sub-system (nested LSystem) ends.
        /// </summary>
        /// <param name="name"></param>
        void SubSystemExecutionEnd(string name);

        /// <summary>
        /// Called when the execution of an LSystem ends.
        /// </summary>
        /// <param name="name">The name of the Lsystem executed.</param>
        void LSystemExecutionEnd(string name);
        

        /// <summary>
        /// Called when the LSystem encounters an exception.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        /// <param name="ex">The exception encountered.</param>
        void LSystemExecutionError(string msg, Exception ex);


    }//end interface

}//end namespace
