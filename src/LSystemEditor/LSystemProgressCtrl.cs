using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LSystemEditor {
    public partial class LSystemProgressCtrl : UserControl {


        string _exe_name;



        public LSystemProgressCtrl() {
            InitializeComponent();

            //init the name
            _exe_name = "";
            

            ResetProgressState();

        }//end constructor

        /// <summary>
        /// Resets the display labels
        /// </summary>
        public void ResetProgressState() {


            executionLabel.Text = "< no system >"; 
            derivationLabel.Text = "< not started >";
            productionLabel.Text = "< not started >";
            resolverLabel.Text = "< not started >";

            derivationProg.Value = 0;
            productionLtrProg.Value = 0;
            resolverLtrProg.Value = 0;


        }


        /// <summary>
        /// Update for production status
        /// </summary>
        /// <param name="it"></param>
        /// <param name="of_it"></param>
        /// <param name="ltr"></param>
        /// <param name="of_ltr"></param>
        public void UpdateProductionStatus(int it, int of_it, int ltr, int of_ltr) {

           
            derivationLabel.Text = it + " of " + of_it;
            derivationProg.Minimum = 0;
            derivationProg.Maximum = of_it;
            derivationProg.Value = it;

            productionLabel.Text = ltr + " of " + of_ltr;
            productionLtrProg.Minimum = 0;
            productionLtrProg.Maximum = of_ltr;
            productionLtrProg.Value = ltr;


            //update the controls
            derivationLabel.Invalidate();
            derivationProg.Invalidate();
            productionLabel.Invalidate();
            productionLtrProg.Invalidate();
            this.Update();

 

        }



        /// <summary>
        /// Update for resolver process
        /// </summary>
        /// <param name="ltr"></param>
        /// <param name="of_ltr"></param>
        public void UpdateResolverStatus(int ltr, int of_ltr) {

           resolverLabel.Text = ltr + " of " + of_ltr;
           resolverLtrProg.Minimum = 0;
           resolverLtrProg.Maximum = of_ltr;
           resolverLtrProg.Value = ltr;


            //use this to update controls - seems most efficient
            resolverLabel.Invalidate();
            resolverLtrProg.Invalidate();
            this.Update();


        }


        /// <summary>
        /// Set the Executing LSystems name
        /// </summary>
        public string ExecutingName {

            set {
                _exe_name = value;
                executionLabel.Text = _exe_name;
            }

            get {
                return executionLabel.Text;

            }

        }//end property




    }//end class
}//end namepsace
