
namespace MirAI.Forma
{
    partial class UnitUI
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UnitUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Name = "UnitUI";
            this.Size = new System.Drawing.Size(200, 80);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UnitUI_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UnitUI_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UnitUI_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UnitUI_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UnitUI_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
