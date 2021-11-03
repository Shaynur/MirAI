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
        public delegate void MoverHandler(UnitUI unit, Size offset);
        public static MoverHandler Mover;
        public static Color linkColor = Color.LightGray;
        private static Pen linkPen = new Pen(linkColor, 4);
        private static SolidBrush linkBrush = new SolidBrush(linkColor);
        public static int connectorSize = 8;
        private Point pointUnderUnit;
        private Node _refNode;
        public Node refNode
        {
            get { return _refNode; }
            set
            {
                _refNode = value;
                if (_refNode != null)
                {
                    GraphicsPath gPath = new GraphicsPath { FillMode = FillMode.Winding };
                    switch (_refNode.Type)
                    {
                        case NodeType.Root:
                            {
                                Height = 60;
                                BackColor = Color.Gray;
                                gPath.AddRectangle(new Rectangle(0, 0 + connectorSize, Width, Height - connectorSize * 2));
                                break;
                            }
                        case NodeType.Action:
                            {
                                Width = 160;
                                Height = 100;
                                gPath.AddRectangle(new Rectangle(0, 0 + connectorSize, Width, Height - connectorSize * 2));
                                BackColor = Color.LemonChiffon;
                                break;
                            }
                        case NodeType.Condition:
                            {
                                Width = 160;
                                Height = 100;
                                gPath.AddEllipse(0, 0 + connectorSize, Height, Height - connectorSize * 2);
                                gPath.AddEllipse(Width - Height - 1, 0 + connectorSize, Height, Height - connectorSize * 2);
                                gPath.AddRectangle(new Rectangle(Height / 2, 0 + connectorSize, Width - Height - 1, Height - connectorSize * 2));
                                BackColor = Color.LightSkyBlue;
                                break;
                            }
                        case NodeType.Connector:
                            {
                                Width = 60;
                                Height = 80;
                                gPath.AddEllipse(0, 0 + connectorSize, Width, Height - connectorSize * 2);
                                BackColor = Color.DeepSkyBlue;
                                break;
                            }
                        case NodeType.SubAI:
                            {
                                gPath.AddRectangle(new Rectangle(0, 0 + connectorSize, Width, Height - connectorSize * 2));
                                BackColor = Color.LightGray;
                                break;
                            }
                        default:
                            break;
                    }
                    if (refNode.Type != NodeType.Root)
                        gPath.AddEllipse(Width / 2 - connectorSize, 2, connectorSize * 2, connectorSize * 2);
                    if (refNode.Type != NodeType.Action && refNode.Type != NodeType.SubAI)
                        gPath.AddEllipse(Width / 2 - connectorSize, Height - 2 - connectorSize * 2, connectorSize * 2, connectorSize * 2);
                    if (gPath.PointCount > 0)
                        Region = new Region(gPath);
                }
            }
        }

        public UnitUI()
        {
            InitializeComponent();
        }

        private void UnitUI_MouseDown(object sender, MouseEventArgs e)
        {
            pointUnderUnit = e.Location;
        }

        private void UnitUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Size offset = new Size(e.X - pointUnderUnit.X, e.Y - pointUnderUnit.Y);
                Mover(this, offset);
                this.Parent.Refresh();
            }
        }

        private void UnitUI_Paint(object sender, PaintEventArgs e)
        {
            if (refNode != null)
            {
                Font drawFont = new Font("Arial", 10);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                e.Graphics.DrawString(refNode.Type.ToString(), drawFont, drawBrush, 10, Height / 3);
                if (refNode.Type != NodeType.Root)
                {
                    e.Graphics.DrawEllipse(linkPen, Width / 2 - connectorSize + 1, 0 + 2, connectorSize * 2 - 4, connectorSize * 2 - 5);
                }
                if (refNode.Type != NodeType.Action && refNode.Type != NodeType.SubAI)
                {
                    e.Graphics.FillEllipse(linkBrush, Width / 2 - connectorSize, Height - 2 - connectorSize * 2, connectorSize * 2, connectorSize * 2);
                }
            }
        }

        private void UnitUI_MouseUp(object sender, MouseEventArgs e)
        {
            this.Parent.Refresh();
        }
    }
}
