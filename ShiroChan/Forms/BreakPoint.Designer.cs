namespace ShiroChan.Forms
{
	partial class BreakPoint
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BreakPoint));
            this.label1 = new System.Windows.Forms.Label();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.Continue = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Shiro is at a break point ";
            // 
            // ValueLabel
            // 
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Location = new System.Drawing.Point(23, 36);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(37, 13);
            this.ValueLabel.TabIndex = 1;
            this.ValueLabel.Text = "Value:";
            // 
            // Continue
            // 
            this.Continue.Location = new System.Drawing.Point(26, 62);
            this.Continue.Name = "Continue";
            this.Continue.Size = new System.Drawing.Size(192, 23);
            this.Continue.TabIndex = 2;
            this.Continue.Text = "Continue";
            this.Continue.UseVisualStyleBackColor = true;
            this.Continue.Click += new System.EventHandler(this.Continue_Click);
            // 
            // BreakPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 88);
            this.Controls.Add(this.Continue);
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(261, 126);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(261, 126);
            this.Name = "BreakPoint";
            this.Text = "Shiro Break Point";
            this.Load += new System.EventHandler(this.BreakPoint_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label ValueLabel;
		private System.Windows.Forms.Button Continue;
	}
}