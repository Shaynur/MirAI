using MirAI.AI;
using MirAI.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirAI.Forma
{
    public partial class Form1 : Form
    {
        private List<UnitUI> units = new List<UnitUI>();
        public Program curentProgram;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.DataSource = MirDBRoutines.GetPrograms(true);
            listBox1.DisplayMember = "Name";
            if (listBox1.Items.Count > 0)
            {
                listBox1.SetSelected(0, true);
                curentProgram = listBox1.SelectedItem as Program;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program p = listBox1.SelectedItem as Program;
            if (curentProgram != p)
            {
                if (curentProgram != null)
                    curentProgram.Save();
                curentProgram = p;
                panel1.SuspendLayout();
                panel1.Controls.Clear();
                units.Clear();
                int ucount = p.Nodes.Count;
                for (int i = 0; i < ucount; i++)
                {
                    units.Add(new UnitUI
                    {
                        Location = new Point(p.Nodes[i].X, p.Nodes[i].Y),
                        refNode = p.Nodes[i]
                    });
                    panel1.Controls.Add(units[i]);
                }
                panel1.ResumeLayout(false);
                Refresh();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (units.Count > 1)
            {
                Pen linkPen = new Pen(Color.White, 3);
                foreach (var fromUnit in units)
                {
                    Node fromNode = fromUnit.refNode;
                    if (fromNode != null && fromNode.Next != null && fromNode.Next.Count > 0)
                    {
                        int ux = fromUnit.Left + fromUnit.Width / 2;
                        int uy = fromUnit.Top + fromUnit.Height;
                        foreach (var toNode in fromNode.Next)
                        {
                            UnitUI toUnit = units.Find(u => u.refNode == toNode);
                            if (toNode.Type != NodeType.Root && toUnit != null)
                                e.Graphics.DrawLine(linkPen, ux, uy, toUnit.Left + toUnit.Width / 2, toUnit.Top);
                        }
                    }
                }
            }
        }
    }
}
