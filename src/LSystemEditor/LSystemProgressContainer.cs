using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;

namespace LSystemEditor {
    public partial class LSystemProgressContainer : UserControl {

        private static int PROG_INTIAL_CAPCITY = 1; //intial progress display capacity

        //reference to engine
        private LSystemEngine.LSystemEvaluationEngine _engine;

        
        private LSystemProgressCtrl _cur_prog;

        private int _cur_stack_index;

        public LSystemProgressContainer() {
            InitializeComponent();


            _cur_prog = null;

            _cur_stack_index = -1;

            intProgressBars();

        }

        #region "Listener calls"


        private void intProgressBars() {

            progressLayoutPanel.SuspendLayout();

            for (int i = 0; i < PROG_INTIAL_CAPCITY; i++) {

                LSystemProgressCtrl sys_prg = new LSystemProgressCtrl();

                //anchor top/left/right...
                sys_prg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

                sys_prg.Visible = false;

                //add this control
                progressLayoutPanel.Controls.Add(sys_prg, 0, i);
  

            }

            progressLayoutPanel.ResumeLayout();
            progressLayoutPanel.Invalidate();


        }//end intProgressBars



        /// <summary>
        /// Call on ExectuionBegin
        /// </summary>
        /// <param name="name">Name of LSystem that is begining Execution</param>
        public void ExecutionBegin(string name) {

            //set index to first control
            _cur_stack_index = 0;
            //set first control
            //_cur_prog = new LSystemProgressCtrl();
            _cur_prog = (LSystemProgressCtrl)progressLayoutPanel.Controls[_cur_stack_index];

            _cur_prog.ExecutingName = name;

            _cur_prog.Visible = true;

           // this.Invalidate();


        }


        public void SubSystemExecutionBegin(string name) {



            //set index to next control
            _cur_stack_index += 1;

            //check for space for the control?
            if (_cur_stack_index > progressLayoutPanel.Controls.Count - 1) {

                progressLayoutPanel.SuspendLayout();

                LSystemProgressCtrl sys_prg = new LSystemProgressCtrl();

                //anchor top/left/right...
                sys_prg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
                //add this control
                progressLayoutPanel.Controls.Add(sys_prg, 0, _cur_stack_index);

                progressLayoutPanel.ResumeLayout();
                progressLayoutPanel.Invalidate();

            }

            //set first control
            //_cur_prog = new LSystemProgressCtrl();
            _cur_prog = (LSystemProgressCtrl)progressLayoutPanel.Controls[_cur_stack_index];

            _cur_prog.ExecutingName = name;

            _cur_prog.Visible = true;


            //this.Invalidate();

        }



        /// <summary>
        /// Removes Last LSytem from List
        /// </summary>
        /// <param name="name"></param>
        public void ExecutionEnd(string name) {

            //reset labels
            _cur_prog.ResetProgressState();

            //turn current off
            _cur_prog.Visible = false;

            //set to -1
            _cur_stack_index = -1;

            _cur_prog = null;


        }//end execution end


        /// <summary>
        /// Remove progress from the stack and grabs current
        /// </summary>
        /// <param name="name"></param>
        public void SubSystemExecutionEnd(string name) {

            //reset progress state
            _cur_prog.ResetProgressState();

            //turn current off
            _cur_prog.Visible = false;

            //decrement index
            _cur_stack_index -= 1;

            //set to next lower control
           _cur_prog = (LSystemProgressCtrl)progressLayoutPanel.Controls[_cur_stack_index];



            this.Invalidate();

        }


        /// <summary>
        /// Wrappers for sub-controls
        /// </summary>
        /// <param name="it"></param>
        /// <param name="of_it"></param>
        /// <param name="ltr"></param>
        /// <param name="of_ltr"></param>
        public void ProductionUpdate(int it, int of_it, int ltr, int of_ltr) {

            if (_cur_prog != null) {

                _cur_prog.UpdateProductionStatus(it, of_it, ltr, of_ltr);
            }


        }

        public void ResolverUpdate(int ltr, int of_ltr) {

            if (_cur_prog != null) {

                _cur_prog.UpdateResolverStatus( ltr, of_ltr);
            }

        }

        /// <summary>
        /// Resets the Progress System
        /// </summary>
        public void ResetProgressDisplay() {


            _cur_stack_index = -1;

            _cur_prog = null;

            //clear the progress panels since they are being held onto
            foreach (LSystemProgressCtrl lpc in progressLayoutPanel.Controls) {

                lpc.ResetProgressState();
                lpc.Visible = false;

            }


        }


        #endregion



        /// <summary>
        /// Links the reference to the Engine (to implement cancel)
        /// </summary>
        /// <param name="engine"></param>
        public void setEngine(LSystemEngine.LSystemEvaluationEngine engine) {


            _engine = engine;

        }

        /// <summary>
        /// Issues Cancel to the LSystem Engine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e) {

            if (_engine != null) {

                _engine.CancelExecution();

            }

        }//end class


    }//end class
}
