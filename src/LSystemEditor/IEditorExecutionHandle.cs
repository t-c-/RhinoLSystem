using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystemEditor {

    /// <summary>
    /// This is designed to maintain portability betqween potential host enviornments, 
    /// while providing a means to control the context of execution.
    /// </summary>
   public interface IEditorExecutionHandle {

        /// <summary>
        /// This is called by the Editor when the execute is triggered (button pressed).
        /// 
        /// </summary>
        /// <param name="system"></param>
        void ExecuteSystem(string system);


    }
}
