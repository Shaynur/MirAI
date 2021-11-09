
namespace MirAI.Forma
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.itemBoxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemAddProg = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemDelProg = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddProg = new System.Windows.Forms.ToolStripButton();
            this.btnDelProg = new System.Windows.Forms.ToolStripButton();
            this.MenuItemRename = new System.Windows.Forms.ToolStripMenuItem();
            this.itemBoxMenu.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(163, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(787, 551);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.listBox1.ContextMenuStrip = this.itemBoxMenu;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listBox1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 21;
            this.listBox1.Location = new System.Drawing.Point(0, 25);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(163, 551);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // itemBoxMenu
            // 
            this.itemBoxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemAddProg,
            this.MenuItemRename,
            this.MenuItemDelProg});
            this.itemBoxMenu.Name = "itemBoxMenu";
            this.itemBoxMenu.ShowImageMargin = false;
            this.itemBoxMenu.Size = new System.Drawing.Size(156, 92);
            // 
            // MenuItemAddProg
            // 
            this.MenuItemAddProg.Name = "MenuItemAddProg";
            this.MenuItemAddProg.Size = new System.Drawing.Size(155, 22);
            this.MenuItemAddProg.Text = "Новая...";
            this.MenuItemAddProg.Click += new System.EventHandler(this.MenuItemAddProg_Click);
            // 
            // MenuItemDelProg
            // 
            this.MenuItemDelProg.Name = "MenuItemDelProg";
            this.MenuItemDelProg.Size = new System.Drawing.Size(155, 22);
            this.MenuItemDelProg.Text = "Удалить";
            this.MenuItemDelProg.Click += new System.EventHandler(this.MenuItemDelProg_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddProg,
            this.btnDelProg});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(950, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddProg
            // 
            this.btnAddProg.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnAddProg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddProg.Image = ((System.Drawing.Image)(resources.GetObject("btnAddProg.Image")));
            this.btnAddProg.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddProg.Name = "btnAddProg";
            this.btnAddProg.Size = new System.Drawing.Size(129, 22);
            this.btnAddProg.Text = "Добавить программу";
            this.btnAddProg.Click += new System.EventHandler(this.MenuItemAddProg_Click);
            // 
            // btnDelProg
            // 
            this.btnDelProg.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnDelProg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDelProg.Image = ((System.Drawing.Image)(resources.GetObject("btnDelProg.Image")));
            this.btnDelProg.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelProg.Name = "btnDelProg";
            this.btnDelProg.Size = new System.Drawing.Size(121, 22);
            this.btnDelProg.Text = "Удалить программу";
            this.btnDelProg.Click += new System.EventHandler(this.MenuItemDelProg_Click);
            // 
            // MenuItemRename
            // 
            this.MenuItemRename.Name = "MenuItemRename";
            this.MenuItemRename.Size = new System.Drawing.Size(155, 22);
            this.MenuItemRename.Text = "Переименовать";
            this.MenuItemRename.Click += new System.EventHandler(this.MenuItemRename_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 576);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.toolStrip1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "MirAI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.itemBoxMenu.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ContextMenuStrip itemBoxMenu;
        private System.Windows.Forms.ToolStripMenuItem MenuItemAddProg;
        private System.Windows.Forms.ToolStripMenuItem MenuItemDelProg;
        private System.Windows.Forms.ToolStripButton btnAddProg;
        private System.Windows.Forms.ToolStripButton btnDelProg;
        private System.Windows.Forms.ToolStripMenuItem MenuItemRename;
    }
}

