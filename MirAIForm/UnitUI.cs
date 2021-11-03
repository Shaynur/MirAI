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
                                break;
                            }
                        case NodeType.Action:
                            {
                                Width = 160;
                                Height = 100;
                                BackColor = Color.LemonChiffon;
                                break;
                            }
                        case NodeType.Condition:
                            {
                                Width = 160;
                                Height = 100;
                                gPath.AddEllipse(0, 0, this.Height, this.Height);
                                gPath.AddEllipse(Width - Height - 1, 0, this.Height, this.Height);
                                gPath.AddRectangle(new Rectangle(Height / 2, 0, Width - Height - 1, Height));
                                BackColor = Color.LightSkyBlue;
                                break;
                            }
                        case NodeType.Connector:
                            {
                                Width = 80;
                                Height = 80;
                                Point[] myArray =  {
                                     new Point(Width/2, 0),
                                     new Point(0, Height/2),
                                     new Point(Width/2, Height),
                                     new Point(Width, Height/2)};
                                gPath.AddPolygon(myArray);
                                BackColor = Color.DeepSkyBlue;
                                break;
                            }
                        case NodeType.SubAI:
                            {
                                BackColor = Color.LightGray;
                                break;
                            }
                        default:
                            break;
                    }
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
                int newLeft = Left + e.X - pointUnderUnit.X;
                if (newLeft > 0)
                {
                    Left = newLeft;
                    refNode.X = Left;
                }

                int newTop = Top + e.Y - pointUnderUnit.Y;
                if (newTop > 0)
                {
                    Top = newTop;
                    refNode.Y = Top;
                }
                this.Parent.Refresh();
            }
        }
        //private readonly Pen borderPen = new Pen(Color.Red, 3);
        private void UnitUI_Paint(object sender, PaintEventArgs e)
        {
            if (refNode != null)
            {
                Font drawFont = new Font("Arial", 10);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                e.Graphics.DrawString(refNode.Type.ToString(), drawFont, drawBrush, 10, Height / 3);
            }
        }

        private void UnitUI_MouseUp(object sender, MouseEventArgs e)
        {
            this.Parent.Refresh();
        }
    }
}
