namespace ScottPlotSample
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnJump = new Button();
            btnFull = new Button();
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            btnSlide = new Button();
            SuspendLayout();
            // 
            // btnJump
            // 
            btnJump.Location = new Point(151, 20);
            btnJump.Margin = new Padding(4, 5, 4, 5);
            btnJump.Name = "btnJump";
            btnJump.Size = new Size(126, 57);
            btnJump.TabIndex = 9;
            btnJump.Text = "Jump";
            btnJump.UseVisualStyleBackColor = true;
            // 
            // btnFull
            // 
            btnFull.Location = new Point(17, 20);
            btnFull.Margin = new Padding(4, 5, 4, 5);
            btnFull.Name = "btnFull";
            btnFull.Size = new Size(126, 57);
            btnFull.TabIndex = 8;
            btnFull.Text = "Full";
            btnFull.UseVisualStyleBackColor = true;
            btnFull.Click += btnFull_Click;
            // 
            // formsPlot1
            // 
            formsPlot1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlot1.DisplayScale = 1F;
            formsPlot1.Location = new Point(17, 87);
            formsPlot1.Margin = new Padding(4, 5, 4, 5);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(979, 575);
            formsPlot1.TabIndex = 6;
            // 
            // btnSlide
            // 
            btnSlide.Location = new Point(286, 20);
            btnSlide.Margin = new Padding(4, 5, 4, 5);
            btnSlide.Name = "btnSlide";
            btnSlide.Size = new Size(126, 57);
            btnSlide.TabIndex = 10;
            btnSlide.Text = "Slide";
            btnSlide.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1013, 682);
            Controls.Add(btnSlide);
            Controls.Add(btnJump);
            Controls.Add(btnFull);
            Controls.Add(formsPlot1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "DataLogger";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        private Button btnJump;
        private Button btnFull;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private Button btnSlide;

        #endregion
    }
}
