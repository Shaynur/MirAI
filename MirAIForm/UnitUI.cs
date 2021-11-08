using MirAI.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace MirAI.Forma
{
    public partial class UnitUI : UserControl
    {
        public delegate void MoverHandler(UnitUI unit, Point offset);
        public static MoverHandler Mover;
        public static MoverHandler SetLink;
        public static MoverHandler SelectUnit;
        public static Color linkColor = Color.LightGray;
        private static Pen linkPen = new Pen(linkColor, 4);
        private static SolidBrush linkBrush = new SolidBrush(linkColor);
        public static int connectorR = 8;
        public Program program;
        private Point pointUnderUnit;
        public Point mouseMovePos;
        private Node _refNode;
        public bool moveLink = false;
        public bool moveUnit = false;
        public bool isSelected = false;
        public Node refNode
        {
            get { return _refNode; }
            set
            {
                _refNode = value;
                if (_refNode != null)
                {
                    SetRegion();
                    ReLocation();
                }
            }
        }

        public void ReLocation()
        {
            if (_refNode != null)
            {
                Location = new Point(_refNode.X - Width / 2, _refNode.Y);
            }
        }

        public UnitUI()
        {
            InitializeComponent();
        }

        private void SetRegion()
        {
            GraphicsPath gPath = new GraphicsPath { FillMode = FillMode.Winding };
            switch (_refNode.Type)
            {
                case NodeType.Root:
                    {
                        Width = 300; 
                        Height = 60;
                        BackColor = Color.Gray;
                        AddRoundedRectangle(ref gPath, new Rectangle(0, 0, Width, Height - connectorR), 6);
                        break;
                    }
                case NodeType.Action:
                    {
                        Width = 160;
                        Height = 100;
                        AddRoundedRectangle(ref gPath, new Rectangle(0, connectorR, Width, Height - connectorR), 4);
                        BackColor = Color.LemonChiffon;
                        break;
                    }
                case NodeType.Condition:
                    {
                        Width = 160;
                        Height = 100;
                        gPath.AddEllipse(0, 0 + connectorR, Height, Height - connectorR * 2);
                        gPath.AddEllipse(Width - Height - 1, 0 + connectorR, Height, Height - connectorR * 2);
                        gPath.AddRectangle(new Rectangle(Height / 2, 0 + connectorR, Width - Height - 1, Height - connectorR * 2));
                        BackColor = Color.LightSkyBlue;
                        break;
                    }
                case NodeType.Connector:
                    {
                        Width = 60;
                        Height = 80;
                        gPath.AddEllipse(0, 0 + connectorR, Width, Height - connectorR * 2);
                        BackColor = Color.DeepSkyBlue;
                        break;
                    }
                case NodeType.SubAI:
                    {
                        AddRoundedRectangle(ref gPath, new Rectangle(0, connectorR, Width, Height - connectorR), 4);
                        BackColor = Color.LightGray;
                        break;
                    }
                default:
                    break;
            }
            if (refNode.Type != NodeType.Root)
                gPath.AddEllipse(Width / 2 - connectorR, 0, connectorR * 2, connectorR * 2);
            if (refNode.Type != NodeType.Action && refNode.Type != NodeType.SubAI)
                gPath.AddEllipse(Width / 2 - connectorR, Height - connectorR * 2, connectorR * 2, connectorR * 2);
            Region = new Region(gPath);
        }

        private void UnitUI_MouseDown(object sender, MouseEventArgs e)
        {
            if (refNode != null && refNode.Type != NodeType.Action && refNode.Type != NodeType.SubAI) // нельзя тянуть линк от Action и SubAI 
            {
                Rectangle rect = new Rectangle(Width / 2 - connectorR, Height - connectorR * 2, connectorR * 2, connectorR * 2); // область нижнего коннектора
                if (rect.Contains(e.Location))  // мышка "ухватила" нижний коннектор
                {
                    moveLink = true;
                }
            }
            pointUnderUnit = e.Location;
        }

        private void UnitUI_MouseUp(object sender, MouseEventArgs e)
        {
            bool inForm1 = ParentForm.GetType().Name == "Form1";
            if (moveLink && inForm1)
            {
                SetLink?.Invoke(this, e.Location);
                moveLink = false;
            }
            else
            {
                if (!moveUnit)
                    SelectUnit?.Invoke(this, e.Location);
            }

            if (!inForm1) return;
            if (moveUnit)
            {
                program.UnDiscover();
                foreach (var node in Program.DFC(refNode))
                {
                    node.Save();
                    if (node.Type == NodeType.SubAI)
                        node.discovered = true;
                }
                moveUnit = false;
            }
            if (Parent != null)
                Parent.Refresh();
        }

        private void UnitUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (ParentForm.GetType().Name != "Form1")
                return;
            mouseMovePos = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                if (!moveLink)
                {
                    moveUnit = true;
                    Point offset = new Point(e.X - pointUnderUnit.X, e.Y - pointUnderUnit.Y);
                    Mover?.Invoke(this, offset);
                }
                else
                    this.Parent.Refresh();
            }
        }

        private void UnitUI_Paint(object sender, PaintEventArgs e)
        {
            if (refNode != null)
            {
                Font drawFont = new Font("Arial", 12);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                Rectangle drawRect = this.ClientRectangle;
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Center;
                drawFormat.LineAlignment = StringAlignment.Center;
                string drawText;

                switch (refNode.Type)
                {
                    case NodeType.Root:
                        drawText = Program.GetName(refNode.ProgramId);
                        break;
                    case NodeType.Action:
                    case NodeType.Condition:
                        drawText = refNode.Command.ToString();
                        break;
                    case NodeType.SubAI:
                        if (refNode.LinkTo.Count > 0) { drawText = Program.GetName(refNode.LinkTo[0].To.ProgramId); }
                        else { drawText = "?"; }
                        break;
                    case NodeType.Connector:
                        drawText = "";
                        break;
                    default:
                        drawText = "Неизвестный тип";
                        break;
                }

                e.Graphics.DrawString(drawText, drawFont, drawBrush, drawRect, drawFormat);

                if (refNode.Type != NodeType.Root)
                {
                    e.Graphics.DrawEllipse(linkPen, Width / 2 - connectorR + 1, 0 + 1, connectorR * 2 - 4, connectorR * 2 - 5);
                }
                if (refNode.Type != NodeType.Action && refNode.Type != NodeType.SubAI)
                {
                    e.Graphics.FillEllipse(linkBrush, Width / 2 - connectorR - 1, Height - connectorR * 2 - 1, connectorR * 2, connectorR * 2);
                }
            }
        }
        private static void AddRoundedRectangle(ref GraphicsPath path, Rectangle r, int radius)
        {
            int d = radius * 2;
            path.AddEllipse(r.Right - d - 1, r.Top, d, d);
            path.AddEllipse(r.Right - d - 1, r.Bottom - d - 1, d, d);
            path.AddEllipse(r.Left, r.Bottom - d - 1, d, d);
            path.AddEllipse(r.Left, r.Top, d, d);
            path.AddRectangle(new Rectangle(r.Left + radius, r.Top, r.Width - d, r.Height));
            path.AddRectangle(new Rectangle(r.Left, r.Top + radius, r.Width, r.Height - d));
            path.CloseFigure();
        }
    }
}
