using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MirAI.Forma
{
    public partial class AddUnitUIForm : Form
    {
        public AI.NodeType selectedNodeType = AI.NodeType.None;
        public AddUnitUIForm()
        {
            InitializeComponent();
        }

        private void AddUnitUIForm_Load(object sender, EventArgs e)
        {
            selunit1.refNode = new AI.Node { Type = AI.NodeType.Action };
            selunit2.refNode = new AI.Node { Type = AI.NodeType.Condition };
            selunit3.refNode = new AI.Node { Type = AI.NodeType.Connector };
            selunit4.refNode = new AI.Node { Type = AI.NodeType.SubAI };
            int lineY1 = 10;
            int lineY2 = Math.Max(selunit1.Height, selunit2.Height) + lineY1 * 2;
            int lineX1 = Math.Max(selunit1.Width / 2, selunit3.Width / 2) + lineY1;
            int lineX2 = Math.Max(selunit2.Width / 2, selunit4.Width / 2) + lineX1 * 2;
            selunit1.refNode.X = lineX1; selunit1.refNode.Y = lineY1;
            selunit2.refNode.X = lineX2; selunit2.refNode.Y = lineY1;
            selunit3.refNode.X = lineX1; selunit3.refNode.Y = lineY2;
            selunit4.refNode.X = lineX2; selunit4.refNode.Y = lineY2;
            selunit1.ReLocation();
            selunit2.ReLocation();
            selunit3.ReLocation();
            selunit4.ReLocation();
            ClientSize = new Size(Math.Max(selunit1.Width, selunit3.Width) + Math.Max(selunit2.Width, selunit4.Width) + lineY1 * 3,
                Math.Max(selunit1.Height, selunit2.Height) + Math.Max(selunit3.Height, selunit4.Height) + lineY1 * 5 + cancelBtn.Height);
            cancelBtn.Top = ClientSize.Height - cancelBtn.Height - lineY1;
            cancelBtn.Left = ClientSize.Width / 2 - cancelBtn.Width / 2;
        }

        public void AddUnitUIForm_MouseUp(object sender, MouseEventArgs e)
        {
            if( sender is UnitUI )
            {
                selectedNodeType = ((UnitUI)sender).refNode.Type;
                this.Close();
            }
        }
    }
}
