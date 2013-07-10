using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystemExpressions {

    /// <summary>
    /// This Interface is for the use of an embedded expression editor.
    /// This frees the LSystemEngine to specifiy language object in one place,
    /// so different versions can be tested and swapped out
    /// </summary>
    public interface ILSystemExpressionEvaluator {

        /// <summary>
        /// Evaluates the text based expression.  THe expression can be the string form of a number, a variable name, or an expression.
        /// Any input will either throw an error, or return a float.  In the case of logical expressions, 1 or 0 will be retunrned.
        /// </summary>
        /// <param name="text">text to evaluate</param>
        /// <returns></returns>
         float Evaluate(string text);

        /// <summary>
        /// Sets a variable to the specified value.
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <param name="value">value of variable</param>
         void SetVariable(string name, float value);

 
        /// <summary>
        /// Sets a variable to the result of the expression.
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <param name="expression">expression to evaluate</param>
         void SetVariable(string name, string expression);

        /// <summary>
        /// Retrieves a variable value.  Throws error if variable is undefined.
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <returns>value of variable</returns>
         float GetVariable(string name);

        /// <summary>
        /// See if a string is a variable.
        /// </summary>
        /// <param name="name">string to lookup</param>
        /// <returns>true if variable name, false otherwise</returns>
         bool IsVariable(string name);


        /// <summary>
        /// Saves the variable state.  This is used to capture defined variables.  During the course of execution, temporary variables
        /// are created and need to be discarded after evaluation.
        /// </summary>
         void SaveVariableState();

        /// <summary>
        /// Restores the variables state fromteh saved copy
        /// </summary>
         void RestoreVariableState();


        // ***********  New Temporary Variable Section ***************

        /// <summary>
        /// Sets a temporary varaible to the result of the expression
        /// - for use in productions
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <param name="expression"></param>
         void SetTemporaryVariable(string name, string expression);

        /// <summary>
        /// Sets a Temporary variable to the value
        /// </summary>
        /// <param name="name">name of the variable</param>
        /// <param name="value">value of the variable</param>
         void SetTemporaryVariable(string name, float value);

        /// <summary>
        /// Clears the Temporary Variables - to be called after production is done.
        /// </summary>
         void ClearTemporaryVariables();

    }
}
