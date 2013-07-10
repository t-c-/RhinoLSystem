namespace LSystemEditor {
    partial class LSystemProgressCtrl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.productionLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.productionLtrProg = new System.Windows.Forms.ProgressBar();
            this.derivationLabel = new System.Windows.Forms.Label();
            this.derivationProg = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.resolverLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.resolverLtrProg = new System.Windows.Forms.ProgressBar();
            this.executionLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // productionLabel
            // 
            this.productionLabel.AutoSize = true;
            this.productionLabel.Location = new System.Drawing.Point(85, 67);
            this.productionLabel.Name = "productionLabel";
            this.productionLabel.Size = new System.Drawing.Size(101, 13);
            this.productionLabel.TabIndex = 11;
            this.productionLabel.Text = "< productionLabel >";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Testing:";
            // 
            // productionLtrProg
            // 
            this.productionLtrProg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.productionLtrProg.Location = new System.Drawing.Point(11, 84);
            this.productionLtrProg.Name = "productionLtrProg";
            this.productionLtrProg.Size = new System.Drawing.Size(338, 18);
            this.productionLtrProg.TabIndex = 9;
            // 
            // derivationLabel
            // 
            this.derivationLabel.AutoSize = true;
            this.derivationLabel.Location = new System.Drawing.Point(85, 28);
            this.derivationLabel.Name = "derivationLabel";
            this.derivationLabel.Size = new System.Drawing.Size(97, 13);
            this.derivationLabel.TabIndex = 8;
            this.derivationLabel.Text = "< derivationLabel >";
            // 
            // derivationProg
            // 
            this.derivationProg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.derivationProg.Location = new System.Drawing.Point(11, 45);
            this.derivationProg.Name = "derivationProg";
            this.derivationProg.Size = new System.Drawing.Size(338, 18);
            this.derivationProg.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Derivations:";
            // 
            // resolverLabel
            // 
            this.resolverLabel.AutoSize = true;
            this.resolverLabel.Location = new System.Drawing.Point(85, 107);
            this.resolverLabel.Name = "resolverLabel";
            this.resolverLabel.Size = new System.Drawing.Size(88, 13);
            this.resolverLabel.TabIndex = 14;
            this.resolverLabel.Text = "< resolverLabel >";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 107);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Resolving:";
            // 
            // resolverLtrProg
            // 
            this.resolverLtrProg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resolverLtrProg.Location = new System.Drawing.Point(11, 124);
            this.resolverLtrProg.Name = "resolverLtrProg";
            this.resolverLtrProg.Size = new System.Drawing.Size(338, 18);
            this.resolverLtrProg.TabIndex = 12;
            // 
            // executionLabel
            // 
            this.executionLabel.AutoSize = true;
            this.executionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.executionLabel.Location = new System.Drawing.Point(85, 7);
            this.executionLabel.Name = "executionLabel";
            this.executionLabel.Size = new System.Drawing.Size(118, 16);
            this.executionLabel.TabIndex = 18;
            this.executionLabel.Text = "< executing label >";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 19;
            this.label1.Text = "Executing :";
            // 
            // LSystemProgressCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.executionLabel);
            this.Controls.Add(this.resolverLabel);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.resolverLtrProg);
            this.Controls.Add(this.productionLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.productionLtrProg);
            this.Controls.Add(this.derivationLabel);
            this.Controls.Add(this.derivationProg);
            this.Controls.Add(this.label6);
            this.Name = "LSystemProgressCtrl";
            this.Size = new System.Drawing.Size(360, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label productionLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar productionLtrProg;
        private System.Windows.Forms.Label derivationLabel;
        private System.Windows.Forms.ProgressBar derivationProg;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label resolverLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ProgressBar resolverLtrProg;
        private System.Windows.Forms.Label executionLabel;
        private System.Windows.Forms.Label label1;
    }
}
