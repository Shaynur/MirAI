﻿using MirAI.AI;
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
        private static Pen linkPen = new Pen(UnitUI.linkColor, 5);
        private static Pen selectPen = new Pen(Color.Red, 3);
        private Point mousePressedPos;
        //private Point mouseMovePos;
        private Point oldPanelPos;
        private int selectedUnit;

        public Form1()
        {
            InitializeComponent();
            UnitUI.Mover = UnitMover;
            UnitUI.SetLink = SetLinkTo;
            UnitUI.SelectUnit = SelectUnit;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            curentProgram = null;
            listBox1.DataSource = null;
            listBox1.Items.Clear();
            listBox1.DataSource = Program.GetListPrograms();
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
                if (curentProgram != null) curentProgram.Save();
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
            selectedUnit = -1;
            Refresh();
        }
        private UnitUI AddUnit(Node node)
        {
            int ucount = units.Count + 1;
            UnitUI unit = new UnitUI
            {
                Location = new Point(node.X, node.Y),
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
            if (selectedUnit != -1)
            {
                int border = 5;
                e.Graphics.DrawRectangle(selectPen,
                                         units[selectedUnit].Left - border,
                                         units[selectedUnit].Top - border,
                                         units[selectedUnit].Width + border * 2,
                                         units[selectedUnit].Height + border * 2);
            }
        }


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePressedPos = e.Location;
            oldPanelPos = new Point(-panel1.AutoScrollPosition.X, -panel1.AutoScrollPosition.Y);
            foreach (var line in GetLinks())
            {
                float linkSize = LineLenght(line.from, line.to);
                float mouseSize = LineLenght(line.from, e.Location) + LineLenght(line.to, e.Location);
                if (mouseSize - linkSize < 0.2)
                {
                    line.fromNode.RemoveChildNode(line.toNode);
                    curentProgram.Save();
                    Refresh();
                    break;
                }
            }
        }

        private float LineLenght(PointF pt1, PointF pt2)
        {
            return (float)(Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2)));
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
                    if (fromNode != null /*&& fromNode.LinkTo != null*/ && fromNode.LinkTo.Count > 0)
                    {
                        PointF from = new PointF(fromUnit.Left + fromUnit.Width / 2, fromUnit.Top + fromUnit.Height - UnitUI.connectorR);
                        foreach (var nodeLink in fromNode.LinkTo)
                        {
                            UnitUI toUnit = units.Find(u => u.refNode == nodeLink.To);
                            if (nodeLink.To.Type != NodeType.Root && toUnit != null)
                            {
                                PointF to = new PointF(toUnit.Left + toUnit.Width / 2, toUnit.Top + UnitUI.connectorR);
                                yield return (from, to, fromNode, nodeLink.To);
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
        //---------------------------------------------------------
        // Обработчики различных событий вызываемые из UnitUI
        //---------------------------------------------------------
        private void UnitMover(UnitUI unit, Point offset)
        {
            Node startNode = unit.refNode;
            curentProgram.UnDiscover();
            foreach (var node in curentProgram.DFC(startNode))
            {
                UnitUI nextUnit = units.Find(u => u.refNode == node);
                int newLeft = nextUnit.Left + offset.X;
                int newTop = nextUnit.Top + offset.Y;
                if (newLeft < 0 || newTop < 0)
                    return;
                if (node.Type == NodeType.SubAI)
                    node.discovered = true;
            }
            curentProgram.UnDiscover();
            foreach (var node in curentProgram.DFC(startNode))
            {
                UnitUI nextUnit = units.Find(u => u.refNode == node);
                int newLeft = nextUnit.Left + offset.X;
                int newTop = nextUnit.Top + offset.Y;
                nextUnit.refNode.X = nextUnit.Left = newLeft;
                nextUnit.refNode.Y = nextUnit.Top = newTop;
                if (node.Type == NodeType.SubAI)
                    node.discovered = true;
            }
            Refresh();
        }

        private void SetLinkTo(UnitUI unit, Point offset)
        {
            Point coord = new Point(unit.Left + offset.X, unit.Top + offset.Y);
            foreach (var u in units)
            {
                Region region = u.Region;
                Point innerCoord = new Point(coord.X - u.Left, coord.Y - u.Top);
                bool contained = region.IsVisible(innerCoord);
                if (contained)
                {
                    if (u == unit || u.refNode.Type == NodeType.Root)
                        return;
                    if (curentProgram.AddLink(unit.refNode, u.refNode))
                    {
                        curentProgram.Reload();
                        RedrawProgram();
                    }
                    return;
                }
            }
            Node node = curentProgram.AddNode(unit.refNode, NodeType.Action); //TODO Диалог создания нового нода
            panel1.SuspendLayout();
            node.X = coord.X;
            node.Y = coord.Y - UnitUI.connectorR;
            UnitUI newUnit = AddUnit(node);
            newUnit.Left = coord.X - newUnit.Width / 2;
            panel1.ResumeLayout(false);
            node.X = newUnit.Left;
            node.Save();
            //Form1_Load(this, null);
            curentProgram.Reload();
            RedrawProgram();
        }

        private void SelectUnit(UnitUI unit, Point offset)
        {
            int newSel = units.IndexOf(unit);
            if (selectedUnit == -1 || selectedUnit != newSel)
                selectedUnit = newSel;
            else
                selectedUnit = -1;
        }
        //---------------------------------------------------------

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //MessageBox.Show($"KeyUp code: {e.KeyCode}, value: {e.KeyValue}");
            if (e.KeyCode == Keys.Delete && selectedUnit != -1)
            {
                if (!curentProgram.RemoveNode(units[selectedUnit].refNode))
                    return;
                panel1.Controls.Remove(units[selectedUnit]);
                units.RemoveAt(selectedUnit);
                //Form1_Load(this, e);
                curentProgram.Reload();
                RedrawProgram();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            MirDBRoutines.CreateSomeDB();
            Form1_Load(sender, e);
        }
    }
}
