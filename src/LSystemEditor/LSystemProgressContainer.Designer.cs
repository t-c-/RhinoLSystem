namespace LSystemEditor {
    partial class LSystemProgressContainer {
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.progressLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(10, 357);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(380, 30);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // progressLayoutPanel
            // 
            this.progressLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLayoutPanel.AutoScroll = true;
            this.progressLayoutPanel.AutoScrollMinSize = new System.Drawing.Size(380, 320);
            this.progressLayoutPanel.ColumnCount = 1;
            this.progressLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.progressLayoutPanel.Location = new System.Drawing.Point(10, 10);
            this.progressLayoutPanel.Name = "progressLayoutPanel";
            this.progressLayoutPanel.RowCount = 1;
            this.progressLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.progressLayoutPanel.Size = new System.Drawing.Size(380, 340);
            this.progressLayoutPanel.TabIndex = 1;
            // 
            // LSystemProgressContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressLayoutPanel);
            this.Controls.Add(this.cancelButton);
            this.Name = "LSystemProgressContainer";
            this.Size = new System.Drawing.Size(400, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel progressLayoutPanel;
    }
}
