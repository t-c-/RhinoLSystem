


namespace LSystemEngine {

   public interface ILSystemModeler {
        //pass command as string
        //args are evaluated in embedded lang and passed as array

       /// <summary>
       /// Calls a command in the modeller.  Commands are called with any arguments defined in the alias. 
       /// Arguments defined in the alias must comply with required modeller arguemtents or the modeler will throw any exception.
       /// </summary>
       /// <param name="command">Command forthe modeler to excecute.</param>
       /// <param name="args">Arguments for the command</param>
        void Command(string command, float[] args);

        /// <summary>
        /// Initialize is called by the engine before an LSystem is evaluated.  Method is provided to apply any pre-process tasks needed.
        /// </summary>
        void Initialize();



       /// <summary>
        /// CloseDown is called after all letters in the final axiom are resolved.  Method is provided to apply any post-process tasks needed.
       /// </summary>
        void CloseDown();


       /// <summary>
       /// ExecutionError is called during the resolver phase if an error is encountered.
       /// This is a chance to do any cleanup on error.
       /// </summary>
        void ExecutionError();


       /// <summary>
       /// Verify if a command is available.  Used for defintion validation during the parsing process.
        /// If the command is available, and return the number of arguments.  Currently, a mis-defined command will default to Int32.MinValue.
        /// If the command does not exist, -1 is returned.
       /// </summary>
       /// <param name="command"></param>
       /// <returns></returns>
        int VerifyCommand(string command);


       /// <summary>
       /// This is used for the execution of subsystems, so the Engine can skip them to honor pruning.
       /// </summary>
       /// <returns></returns>
        bool IsPruningOn();

    }
}
