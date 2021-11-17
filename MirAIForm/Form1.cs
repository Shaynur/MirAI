using MirAI.AI;
using MirAI.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MirAI.Forma {
    public partial class Form1 : Form {
        private List<UnitUI> units = new List<UnitUI>();
        public Program curentProgram;
        private static Pen linkPen = new Pen( UnitUI.linkColor, 5 );
        private static Pen selectPen = new Pen(Color.Red, 3);
        private Point mousePressedPos;
        private Point oldPanelPos;
        private int selectedUnit = -1;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load( object sender, EventArgs e ) {
            curentProgram = null;
            listBox1.DataSource = null;
            listBox1.Items.Clear();
            statusLabel1.Text = "";
            selectedUnit = -1;
            try {
                listBox1.DataSource = Program.GetListPrograms();
                statusLabel1.Text = Path.GetFileName( MirDBContext.DBFileName );
            }
            catch (Exception ex) {
                MessageBox.Show( ex.Message, "Ошибка чтения БД", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            listBox1.DisplayMember = "Name";
            if (listBox1.Items.Count > 0) {
                listBox1.SelectedIndex = listBox1.TopIndex;
                curentProgram = listBox1.SelectedItem as Program;
            } else {
                RedrawProgram();
            }
        }

        private void listBox1_SelectedIndexChanged( object sender, EventArgs e ) {
            Program p = listBox1.SelectedItem as Program;
            if (curentProgram != p) {
                selectedUnit = -1;
                if (curentProgram != null) curentProgram.Save();
                curentProgram = p;
                RedrawProgram();
            }
        }
        private void RedrawProgram() {
            panel1.SuspendLayout();
            panel1.Controls.Clear();
            units.Clear();
            for (int i = 0; i < (curentProgram?.Nodes.Count ?? 0); i++) {
                AddUnit( curentProgram.Nodes[i] );
            }
            panel1.ResumeLayout( false );
            if (curentProgram != null) {
                List<Program> programs = listBox1.DataSource as List<Program>;
                if (curentProgram.CheckLenght( out int len, ref programs )) {
                    statusLabel2.BackColor = SystemColors.Control;
                    statusLabel2.Text = "Размер " + curentProgram.Name + " =  " + len.ToString() + " / " + Program.MaxLenght.ToString();
                } else {
                    statusLabel2.BackColor = Color.Red;
                    statusLabel2.Text = "Размер " + curentProgram.Name + "  > " + Program.MaxLenght.ToString();
                }
            }
            else {
                statusLabel2.BackColor = SystemColors.Control;
                statusLabel2.Text = string.Empty;
            }
            //selectedUnit = -1;
            Refresh();
        }
        private UnitUI AddUnit( Node node ) {
            int ucount = units.Count + 1;
            UnitUI unit = new UnitUI
            {
                refNode = node,
                Name = "UnitUI" + ucount.ToString(),
                program = curentProgram
            };
            units.Add( unit );
            panel1.Controls.Add( unit );
            return unit;
        }

        private void panel1_Paint( object sender, PaintEventArgs e ) {
            foreach (var line in GetLinks()) {
                e.Graphics.DrawLine( linkPen, line.from, line.to );
            }
            foreach (var unit in units) {
                if (unit.moveLink) {
                    int fromX = unit.Left + unit.Width / 2;
                    int fromY = unit.Top + unit.Height - UnitUI.connectorR;
                    Point mouseMovePos = unit.mouseMovePos;
                    e.Graphics.DrawLine( linkPen, fromX, fromY, unit.Left + mouseMovePos.X, unit.Top + mouseMovePos.Y );
                }
            }
            if (selectedUnit != -1) {
                int border = 5;
                e.Graphics.DrawRectangle( selectPen,
                                         units[selectedUnit].Left - border,
                                         units[selectedUnit].Top - border,
                                         units[selectedUnit].Width + border * 2,
                                         units[selectedUnit].Height + border * 2 );
            }
        }

        private void panel1_MouseDown( object sender, MouseEventArgs e ) {
            mousePressedPos = e.Location;
            oldPanelPos = new Point( -panel1.AutoScrollPosition.X, -panel1.AutoScrollPosition.Y );
            foreach (var line in GetLinks())        // ишем не нажали ли мы на линк, чтобы удалить его
            {
                float linkSize = LineLenght(line.from, line.to);  // методом сравнения длинны линка с суммой длинн растояний
                float mouseSize = LineLenght(line.from, e.Location) + LineLenght(line.to, e.Location);  // от концов до мышки
                if (mouseSize - linkSize < 0.2) {
                    line.fromNode.RemoveChildNode( line.toNode );
                    RedrawProgram();
                    //Refresh();
                    return;
                }
            }
            selectedUnit = -1;
            Refresh();
        }

        private float LineLenght( PointF pt1, PointF pt2 ) {
            return (float)(Math.Sqrt( Math.Pow( pt2.X - pt1.X, 2 ) + Math.Pow( pt2.Y - pt1.Y, 2 ) ));
        }

        private void panel1_MouseUp( object sender, MouseEventArgs e ) {
            //MessageBox.Show("panel1_MouseUp");
        }

        private IEnumerable<(PointF from, PointF to, Node fromNode, Node toNode)> GetLinks() {
            if (units.Count > 1) {
                foreach (var fromUnit in units) {
                    Node fromNode = fromUnit.refNode;
                    if (fromNode != null && fromNode.LinkTo.Count > 0) {
                        PointF from = new PointF(fromUnit.Left + fromUnit.Width / 2, fromUnit.Top + fromUnit.Height - UnitUI.connectorR);
                        foreach (var nodeLink in fromNode.LinkTo) {
                            UnitUI toUnit = units.Find(u => u.refNode == nodeLink.To);
                            if (nodeLink.To.Type != NodeType.Root && toUnit != null) {
                                PointF to = new PointF(toUnit.Left + toUnit.Width / 2, toUnit.Top + UnitUI.connectorR);
                                yield return (from, to, fromNode, nodeLink.To);
                            }
                        }
                    }
                }
            }
        }

        private void panel1_MouseMove( object sender, MouseEventArgs e ) {
            if (e.Button == MouseButtons.Left) {
                if (oldPanelPos.X != -1) {
                    Point newSP = new Point(oldPanelPos.X - (e.X - mousePressedPos.X), oldPanelPos.Y - (e.Y - mousePressedPos.Y));
                    panel1.AutoScrollPosition = newSP;
                }
            }
        }
        //---------------------------------------------------------
        // Обработчики различных событий вызываемые из UnitUI
        //---------------------------------------------------------
        public void UnitMover( UnitUI unit, Point offset ) {
            if (unit.Parent != this.panel1)
                return;
            Node startNode = unit.refNode;
            curentProgram.UnDiscover();
            foreach (var node in Program.DFC( startNode )) {
                UnitUI nextUnit = units.Find(u => u.refNode == node);
                int newLeft = nextUnit.Left + offset.X;
                int newTop = nextUnit.Top + offset.Y;
                if (newLeft < 0 || newTop < 0)
                    return;
                if (node.Type == NodeType.SubAI)
                    node.discovered = true;
            }
            curentProgram.UnDiscover();
            foreach (var node in Program.DFC( startNode )) {
                UnitUI nextUnit = units.Find(u => u.refNode == node);
                int newLeft = nextUnit.Left + offset.X;
                int newTop = nextUnit.Top + offset.Y;
                nextUnit.Left = newLeft;
                nextUnit.Top = newTop;
                nextUnit.refNode.X = nextUnit.Left + nextUnit.Width / 2;
                nextUnit.refNode.Y = nextUnit.Top;
                if (node.Type == NodeType.SubAI)
                    node.discovered = true;
            }
            Refresh();
        }

        public void SetLinkTo( UnitUI unit, Point offset ) {
            Point coord = new Point(unit.Left + offset.X, unit.Top + offset.Y);
            foreach (var u in units) {
                Region region = u.Region;
                Point innerCoord = new Point(coord.X - u.Left, coord.Y - u.Top);
                bool contained = region.IsVisible(innerCoord);
                if (contained) {
                    if (u == unit || u.refNode.Type == NodeType.Root)
                        return;
                    if (curentProgram.AddLink( unit.refNode, u.refNode )) {
                        curentProgram.Reload();
                        RedrawProgram();
                    }
                    return;
                }
            }
            var addform = new AddUnitUIForm();
            addform.ShowDialog( this );
            NodeType newNodeType = addform.selectedNodeType;
            addform.Dispose();
            if (newNodeType != NodeType.None) {
                Program subprog = null;
                int command = 0;
                switch (newNodeType) {
                    case NodeType.SubAI:
                    if (!GetProgramFromList( ref subprog ))
                        return;
                    break;
                    case NodeType.Action:
                    //TODO Диалог выбора команды для ноды действия
                    // (возможность отмены и выхода из ф-ии создания ноды)
                    // command = ??
                    break;
                    case NodeType.Condition:
                    //TODO Диалог выбора команды для ноды действия
                    // (возможность отмены и выхода из ф-ии создания ноды)
                    // command = ??
                    break;
                    default:
                    break;
                }
                Node node = curentProgram.AddNode(unit.refNode, newNodeType);
                switch (newNodeType) {
                    case NodeType.SubAI:
                    node.AddChildNode( subprog.GetRootNode() );
                    break;
                    case NodeType.Action:
                    case NodeType.Condition:
                    node.Command = command;
                    break;
                    default:
                    break;
                }
                panel1.SuspendLayout();
                node.X = coord.X;
                node.Y = coord.Y - UnitUI.connectorR;
                UnitUI newUnit = AddUnit(node);
                panel1.ResumeLayout( false );
                node.Save();
                curentProgram.Reload();
                RedrawProgram();
            }
        }

        public bool GetProgramFromList( ref Program program ) {
            if (listBox1.Items.Count < 2) {
                MessageBox.Show( "Невозможно создать ссылку на подпрограмму т.к. в списке программ всего одна." +
                    "\n\n(Сначала создайте еще программу, что-бы можно было добавить ее в качестве подпрограммы)",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                return false;
            }
            var selpform = new SelectProgram(listBox1.DataSource, curentProgram.Name);
            DialogResult dr = selpform.ShowDialog(this);
            string subprogname = selpform.selectedProgram;
            selpform.Dispose();
            if (dr == DialogResult.Cancel)
                return false;
            foreach (Program p in listBox1.Items) {
                if (p.Name == subprogname) {
                    program = p;
                    return true;
                }
            }
            return false;
        }
        public void SelectUnit( UnitUI unit, Point offset ) {
            int newSel = units.IndexOf(unit);
            if (selectedUnit == -1 || selectedUnit != newSel)
                selectedUnit = newSel;
            else
                selectedUnit = -1;
        }

        private void Form1_KeyUp( object sender, KeyEventArgs e ) {
            //MessageBox.Show($"KeyUp code: {e.KeyCode}, value: {e.KeyValue}");
            if (e.KeyCode == Keys.Delete && selectedUnit != -1) {
                if (!curentProgram.RemoveNode( units[selectedUnit].refNode ))
                    return;
                panel1.Controls.Remove( units[selectedUnit] );
                units.RemoveAt( selectedUnit );
                curentProgram.Reload();
                selectedUnit = -1;
                RedrawProgram();
            }
        }

        private void toolStripButton1_Click( object sender, EventArgs e ) {
            MirDBRoutines.CreateSomeDB();
            Form1_Load( sender, e );
        }

        private void MenuItemDelProg_Click( object sender, EventArgs e ) {
            if (listBox1.SelectedItem != null) {
                Program p = listBox1.SelectedItem as Program;
                if (MessageBox.Show( "Удалить программу " + p.Name + "? ", "Внимание",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question ) == DialogResult.Cancel)
                    return;
                Program.RemoveProgramm( p );
                Form1_Load( sender, e );
            }
        }

        private void MenuItemAddProg_Click( object sender, EventArgs e ) {
            List<string> progNames = new List<string>();
            List<Program> programs = (List<Program>)listBox1.DataSource;
            if (programs is null)
                return;
            foreach (var p in programs) {
                progNames.Add( p.Name );
            }

            var ibox = new InputBox(progNames);
            if (ibox.ShowDialog( this ) == DialogResult.OK) {
                Program newProg = new Program(ibox.textBox1.Text);
                Form1_Load( sender, e );
                listBox1.SelectedIndex = listBox1.FindString( newProg.Name );
            }
            ibox.Dispose();
        }

        private void MenuItemRename_Click( object sender, EventArgs e ) {
            if (listBox1.SelectedItem != null) {
                Program selprog = listBox1.SelectedItem as Program;

                List<string> progNames = new List<string>();
                List<Program> programs = (List<Program>)listBox1.DataSource;
                foreach (var p in programs) {
                    if (p.Name != selprog.Name)
                        progNames.Add( p.Name );
                }

                var ibox = new InputBox(progNames);
                ibox.textBox1.Text = selprog.Name;
                if (ibox.ShowDialog( this ) == DialogResult.OK && ibox.textBox1.Text != selprog.Name) {
                    selprog.Name = ibox.textBox1.Text;
                    selprog.Save();
                    listBox1.DisplayMember = "";
                    listBox1.DisplayMember = "Name";
                    listBox1.SelectedIndex = listBox1.FindString( selprog.Name );
                }
                ibox.Dispose();
            }
        }

        private void openToolStripButton_Click( object sender, EventArgs e ) {
            openFileDialog1.InitialDirectory = Path.GetDirectoryName( MirDBContext.DBFileName );
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                MirDBContext.DBFileName = openFileDialog1.FileName;
                Form1_Load( sender, e );
            }
        }

        private void newToolStripButton_Click( object sender, EventArgs e ) {
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName( MirDBContext.DBFileName );
            saveFileDialog1.FileName = string.Empty;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                MirDBContext.DBFileName = saveFileDialog1.FileName;
                Form1_Load( sender, e );
            }
        }

        private void unitsToolStripButton_Click( object sender, EventArgs e ) {
            Simulation sf = new Simulation();
            sf.ShowDialog();
        }
    }
}
