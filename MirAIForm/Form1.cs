using MirAI.AI;
using MirAI.DB;
using System;
using System.Collections;
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
        private static Pen linkPen = new Pen(UnitUI.linkColor, 3);
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
            UnitUI.Mover = UnitMover;
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
                    units.Add(new UnitUI { Location = new Point(p.Nodes[i].X, p.Nodes[i].Y), refNode = p.Nodes[i] });
                    panel1.Controls.Add(units[i]);
                }
                panel1.ResumeLayout(false);
                Refresh();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var line in GetLinks())
            {
                e.Graphics.DrawLine(linkPen, line.x1, line.y1, line.x2, line.y2);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (var line in GetLinks())
            {
                float delta = 0.04F;
                float z;
                if (line.x1 == line.x2)
                    z = line.x1 - e.X;
                else if (line.y1 == line.y2)
                    z = line.y1 - e.Y;
                else
                    z = (e.X - line.x1) / (line.x2 - line.x1) - (e.Y - line.y1) / (line.y2 - line.y1);

                if ((Math.Abs(z) < delta) &&
                   ((e.X >= Math.Min(line.x1, line.x2)) && (e.X <= Math.Max(line.x1, line.x2))) &&
                         ((e.Y >= Math.Min(line.y1, line.y2)) && (e.Y <= Math.Max(line.y1, line.y2))))
                {
                    MessageBox.Show("Упс!");  //TODO RemoveLink
                }
            }
        }

        private IEnumerable<(float x1, float y1, float x2, float y2)> GetLinks()
        {
            if (units.Count > 1)
            {
                foreach (var fromUnit in units)
                {
                    Node fromNode = fromUnit.refNode;
                    if (fromNode != null && fromNode.Next != null && fromNode.Next.Count > 0)
                    {
                        int fromX = fromUnit.Left + fromUnit.Width / 2;
                        int fromY = fromUnit.Top + fromUnit.Height - UnitUI.connectorSize;
                        foreach (var toNode in fromNode.Next)
                        {
                            UnitUI toUnit = units.Find(u => u.refNode == toNode);
                            if (toNode.Type != NodeType.Root && toUnit != null)
                            {
                                yield return (fromX, fromY, toUnit.Left + toUnit.Width / 2, toUnit.Top + UnitUI.connectorSize);
                            }
                        }
                    }
                }
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void UnitMover(UnitUI unit, Size offset)
        {
            curentProgram.UnDiscover();
            DFCmover(unit, offset);
        }

        private bool DFCmover(UnitUI unit, Size offset)
        {
            Node node = unit.refNode;
            if (!node.discovered)
            {
                node.discovered = true;
                int newLeft = unit.Left + offset.Width;
                int newTop = unit.Top + offset.Height;
                if (newLeft < 0 || newTop < 0)
                    return false;
                if (node.Type != NodeType.SubAI)
                {
                    foreach (var nextNode in node.Next)
                    {
                        UnitUI nextUnit = units.Find(u => u.refNode == nextNode);
                        if (!DFCmover(nextUnit, offset))
                            return false;
                    }
                }
                unit.refNode.X = unit.Left = newLeft;
                unit.refNode.Y = unit.Top = newTop;
            }
            return true;
        }
    }
}
