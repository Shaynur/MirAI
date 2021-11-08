
namespace MirAI.Forma
{
    partial class AddUnitUIForm
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
            this.selunit1 = new MirAI.Forma.UnitUI();
            this.selunit2 = new MirAI.Forma.UnitUI();
            this.selunit3 = new MirAI.Forma.UnitUI();
            this.selunit4 = new MirAI.Forma.UnitUI();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selunit1
            // 
            this.selunit1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.selunit1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selunit1.Location = new System.Drawing.Point(12, 12);
            this.selunit1.Name = "selunit1";
            this.selunit1.refNode = null;
            this.selunit1.Size = new System.Drawing.Size(200, 80);
            this.selunit1.TabIndex = 0;
            // 
            // selunit2
            // 
            this.selunit2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.selunit2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selunit2.Location = new System.Drawing.Point(218, 12);
            this.selunit2.Name = "selunit2";
            this.selunit2.refNode = null;
            this.selunit2.Size = new System.Drawing.Size(200, 80);
            this.selunit2.TabIndex = 1;
            // 
            // selunit3
            // 
            this.selunit3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.selunit3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selunit3.Location = new System.Drawing.Point(12, 98);
            this.selunit3.Name = "selunit3";
            this.selunit3.refNode = null;
            this.selunit3.Size = new System.Drawing.Size(200, 80);
            this.selunit3.TabIndex = 2;
            // 
            // selunit4
            // 
            this.selunit4.BackColor = System.Drawing.SystemColors.ControlDark;
            this.selunit4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selunit4.Location = new System.Drawing.Point(218, 98);
            this.selunit4.Name = "selunit4";
            this.selunit4.refNode = null;
            this.selunit4.Size = new System.Drawing.Size(200, 80);
            this.selunit4.TabIndex = 3;
            // 
            // cancelBtn
            // 
            this.cancelBtn.BackColor = System.Drawing.Color.Red;
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(167, 184);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(99, 44);
            this.cancelBtn.TabIndex = 4;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = false;
            // 
            // AddUnitUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 358);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.selunit4);
            this.Controls.Add(this.selunit3);
            this.Controls.Add(this.selunit2);
            this.Controls.Add(this.selunit1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AddUnitUIForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.AddUnitUIForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MirAI.Forma.UnitUI selunit1;
        private MirAI.Forma.UnitUI selunit2;
        private MirAI.Forma.UnitUI selunit3;
        private MirAI.Forma.UnitUI selunit4;
        private System.Windows.Forms.Button cancelBtn;
    }
}