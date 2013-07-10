using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace LSystemEngine {


    public struct LSystemLetter {

        //character used as key
        public char key;

        //number of parameters present - imposed 256 argument limit by using byte
        public byte parameters;

        //array of template values - evaulated against language
        //not a value based variable - needs to be handled for safe copy
        public string[] template;

        //array of template values - evaluated against language
        //not a value based varaibale - needs to be handled for safe copy
        public float[] values;


        /// <summary>
        /// Constructor for new letter that has no parameters.
        /// </summary>
        /// <param name="k">character representing letter key</param>
        public LSystemLetter(char k) {

            key = k;
            template = null; // new string[] { };
            values = null; // new float[] { };
            parameters = 0;

        }


        /// <summary>
        /// Constructor for new letter with parameters.  Note, the array sizes must match, and p must represent this size.
        /// </summary>
        /// <param name="k">array of template expressions</param>
        /// <param name="t">array for holding template values</param>
        /// <param name="v">character representing letter key</param>
        /// <param name="p">number of parameters, this must match k & t array counts</param>
        public LSystemLetter(char k, string[] t, float[] v, byte p) {

            key = k;
            template = t;
            values = v;
            parameters = p;

        }

        /// <summary>
        /// Copy Constructor - creates safe copy of letter.
        /// </summary>
        /// <param name="letter">letter to copy</param>
        public LSystemLetter(LSystemLetter letter) {

            key = letter.key;
            parameters = letter.parameters;

            template = new string[letter.parameters];
            values = new float[letter.parameters];

            //if there are parameters, copy to new
            if (parameters > 0) {

                System.Array.Copy(letter.template, template, parameters);
                System.Array.Copy(letter.values, values, parameters);

            }//end if copy parameters


        }//lsysLetter

        /// <summary>
        /// print pretty...
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            //always print the key
            string lstr = key.ToString();

            //check for parameters
            if (parameters > 0) {
                //print the template
                lstr += "(";
                for (int t = 0; t < template.Length; t++) {
                    lstr += template[t];
                    if (t < template.Length - 1) { lstr += ","; }
                }
                lstr += ")";
               //print the template value
                lstr += "{";
                for (int v = 0; v < values.Length; v++) {
                    lstr += values[v];
                    if (v < values.Length - 1) { lstr += ","; }
                }
                lstr += "} ";

            }//end if

            return lstr;
        }

    }//end LsysLEtter struct



}//end namespace
