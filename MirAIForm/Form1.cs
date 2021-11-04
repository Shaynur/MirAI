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
        private Point pointUnderPanel;
        private Point oldPanelPos;

        public Form1()
        {
            InitializeComponent();
            UnitUI.Mover = UnitMover;
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
            pointUnderPanel = e.Location;// + (Size)panel1.AutoScrollPosition;
            oldPanelPos.X = -panel1.AutoScrollPosition.X;
            oldPanelPos.Y = -panel1.AutoScrollPosition.Y;
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
                        int fromY = fromUnit.Top + fromUnit.Height - UnitUI.connectorR;
                        foreach (var toNode in fromNode.Next)
                        {
                            UnitUI toUnit = units.Find(u => u.refNode == toNode);
                            if (toNode.Type != NodeType.Root && toUnit != null)
                            {
                                yield return (fromX, fromY, toUnit.Left + toUnit.Width / 2, toUnit.Top + UnitUI.connectorR);
                            }
                        }
                    }
                }
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //int dx = (e.X > pointUnderPanel.X) ? 1 : -1;
                //int dy = (e.Y > pointUnderPanel.Y) ? 1 : -1;
                //Point newSP = new Point(-panel1.AutoScrollPosition.X + dx, -panel1.AutoScrollPosition.Y + dy);
                Point newSP = new Point(oldPanelPos.X - (e.X - pointUnderPanel.X), oldPanelPos.Y - (e.Y - pointUnderPanel.Y));
                panel1.AutoScrollPosition = newSP;
            }
        }

        private void UnitMover(UnitUI unit, Size offset)
        {
            Node startNode = unit.refNode;

            curentProgram.UnDiscover();
            foreach (var node in curentProgram.DFC(startNode))
            {
                UnitUI nextUnit = units.Find(u => u.refNode == node);
                int newLeft = nextUnit.Left + offset.Width;
                int newTop = nextUnit.Top + offset.Height;
                if (newLeft < 0 || newTop < 0)
                    return;
                if (node.Type == NodeType.SubAI)
                    node.discovered = true;
            }

            curentProgram.UnDiscover();
            foreach (var node in curentProgram.DFC(startNode))
            {
                UnitUI nextUnit = units.Find(u => u.refNode == node);
                int newLeft = nextUnit.Left + offset.Width;
                int newTop = nextUnit.Top + offset.Height;
                nextUnit.refNode.X = nextUnit.Left = newLeft;
                nextUnit.refNode.Y = nextUnit.Top = newTop;
                if (node.Type == NodeType.SubAI)
                    node.discovered = true;
            }
        }
    }
}
