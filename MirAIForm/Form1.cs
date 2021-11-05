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
        private Point mousePressedPos;
        //private Point mouseMovePos;
        private Point oldPanelPos;

        public Form1()
        {
            InitializeComponent();
            UnitUI.Mover = UnitMover;
            UnitUI.SetLink = SetLinkTo;
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
                RedrawProgram();
            }
        }
        private void RedrawProgram()
        {
            panel1.SuspendLayout();
            panel1.Controls.Clear();
            units.Clear();
            for (int i = 0; i < curentProgram.Nodes.Count; i++)
            {
                AddUnit(curentProgram.Nodes[i]);
            }
            panel1.ResumeLayout(false);
            Refresh();
        }
        private UnitUI AddUnit(Node node)
        {
            int ucount = units.Count + 1;
            UnitUI unit = new UnitUI
            {
                Location = new Point(node.X,
                                     node.Y),
                refNode = node,
                Name = "UnitUI" + ucount.ToString()
            };
            units.Add(unit);
            panel1.Controls.Add(unit);
            return unit;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var line in GetLinks())
            {
                e.Graphics.DrawLine(linkPen, line.from, line.to);
            }
            foreach (var unit in units)
            {
                if (unit.moveLink)
                {
                    int fromX = unit.Left + unit.Width / 2;
                    int fromY = unit.Top + unit.Height - UnitUI.connectorR;
                    Point mouseMovePos = unit.mouseMovePos;
                    e.Graphics.DrawLine(linkPen, fromX, fromY, unit.Left + mouseMovePos.X, unit.Top + mouseMovePos.Y);
                }
            }
        }


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePressedPos = e.Location;
            oldPanelPos = new Point(-panel1.AutoScrollPosition.X, -panel1.AutoScrollPosition.Y);
            foreach (var line in GetLinks())
            {
                float delta = 0.04F;
                float z;
                if (line.from.X == line.to.X)
                    z = line.from.X - e.X;
                else if (line.from.Y == line.to.Y)
                    z = line.from.Y - e.Y;
                else
                    z = (e.X - line.from.X) / (line.to.X - line.from.X) - (e.Y - line.from.Y) / (line.to.Y - line.from.Y);

                if ((Math.Abs(z) < delta) &&
                   ((e.X >= Math.Min(line.from.X, line.to.X)) && (e.X <= Math.Max(line.from.X, line.to.X))) &&
                         ((e.Y >= Math.Min(line.from.Y, line.to.Y)) && (e.Y <= Math.Max(line.from.Y, line.to.Y))))
                {
                    //MessageBox.Show("Упс!");  //TODO RemoveLink
                    line.fromNode.Next.Remove(line.toNode);
                    curentProgram.Save();
                    Refresh();
                    break;
                }
            }
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("panel1_MouseUp");
        }

        private IEnumerable<(PointF from, PointF to, Node fromNode, Node toNode)> GetLinks()
        {
            if (units.Count > 1)
            {
                foreach (var fromUnit in units)
                {
                    Node fromNode = fromUnit.refNode;
                    if (fromNode != null && fromNode.Next != null && fromNode.Next.Count > 0)
                    {
                        PointF from = new PointF(fromUnit.Left + fromUnit.Width / 2, fromUnit.Top + fromUnit.Height - UnitUI.connectorR);
                        foreach (var toNode in fromNode.Next)
                        {
                            UnitUI toUnit = units.Find(u => u.refNode == toNode);
                            if (toNode.Type != NodeType.Root && toUnit != null)
                            {
                                PointF to = new PointF(toUnit.Left + toUnit.Width / 2, toUnit.Top + UnitUI.connectorR);
                                yield return (from, to, fromNode, toNode);
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
                if (oldPanelPos.X != -1)
                {
                    Point newSP = new Point(oldPanelPos.X - (e.X - mousePressedPos.X), oldPanelPos.Y - (e.Y - mousePressedPos.Y));
                    panel1.AutoScrollPosition = newSP;
                }
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
            Refresh();
        }

        public void SetLinkTo(UnitUI unit, Size offset)
        {
            Point coord = new Point(unit.Left + offset.Width, unit.Top + offset.Height);
            foreach (var u in units)
            {
                Region region = u.Region;
                Point innerCoord = new Point(coord.X - u.Left, coord.Y - u.Top);
                bool contained = region.IsVisible(innerCoord);
                if (contained)
                {
                    if (u == unit || u.refNode.Type == NodeType.Root)
                        return;
                    //MessageBox.Show($"{u.Name}: {coord.X}, {coord.Y} / {offset.Width}, {offset.Height}");
                    if (curentProgram.AddLink(unit.refNode, u.refNode))
                    {
                        curentProgram.Save();
                        Refresh();
                    }
                    return;
                }
            }
            Node node = curentProgram.AddNode(NodeType.Action, unit.refNode); //TODO Диалог создания нового нода
            node.X = coord.X;
            node.Y = coord.Y - UnitUI.connectorR;
            panel1.SuspendLayout();
            UnitUI newUnit = AddUnit(node);
            newUnit.Left = coord.X - newUnit.Width / 2;
            panel1.ResumeLayout(false);
            curentProgram.Save();
        }
    }
}
