using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSystemEditor {
    public partial class AboutBox : Form {
        private static string NO_IMPLEMENTATION_NOTES = "<no implementation notes available>";
   

        public AboutBox() {
            InitializeComponent();


            implementationNotes.Text = NO_IMPLEMENTATION_NOTES;

        }

        private void closeBttn_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e) {


            this.Close();

        }

        /// <summary>
        /// Sets the Implementation Notes section of the About Box with the provided notes.
        /// </summary>
        public string ImplementationNotes {

            set {
                string notes = value;

                if (String.IsNullOrEmpty(notes)) {

                    notes = NO_IMPLEMENTATION_NOTES;

                } 
                if (implementationNotes != null) {

                    implementationNotes.Text = notes;

                }

            }//end set

        }//end ImplementationNotes



  
    }
}
