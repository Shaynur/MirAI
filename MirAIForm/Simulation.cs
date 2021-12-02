using MirAI.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MirAI.Forma {
    public partial class Simulation : Form {
        private List<Unit> units;
        private List<Program> programs;
        private Random rand;
        private int Tick = 0;
        private Font fnt = new Font("Arial",10);
        public Simulation( object dataSource ) {
            InitializeComponent();
            programs = (List<Program>)dataSource;
            LoadUnits();
        }

        private void toolStripButton1_Click( object sender, EventArgs e ) {
            listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
        }

        void LoadUnits() {
            units = Unit.GetListUnits();
            listView1.Items.Clear();
            foreach (var u in units) {
                ListViewItem i = new ListViewItem(u.Name);
                i.SubItems.Add( u.Type.ToString() );
                //i.SubItems.Add( programs.Find( p => p.Id == u.ProgramId ).Name );
                i.SubItems.Add( u.Program.Name );
                i.Tag = u;
                listView1.Items.Add( i );
            }
            listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
        }

        private void addUnitMenu_Click( object sender, EventArgs e ) {
            EditUnitForm eu = new EditUnitForm( programs );
            if (eu.ShowDialog() == DialogResult.OK) {
                LoadUnits();
            }
            eu.Dispose();
        }

        private void editUnitMenu_Click( object sender, EventArgs e ) {
            if (listView1.SelectedItems.Count < 1) {
                return;
            }
            Unit u = (Unit)listView1.SelectedItems[0].Tag;
            EditUnitForm eu = new EditUnitForm( programs, u );
            if (eu.ShowDialog() == DialogResult.OK) {
                LoadUnits();
            }
            eu.Dispose();
        }

        private void deleteUnitMenu_Click( object sender, EventArgs e ) {
            if (listView1.SelectedItems.Count < 1) {
                return;
            }
            Unit u = (Unit)listView1.SelectedItems[0].Tag;
            if (MessageBox.Show( "Удалить юнита " + u.Name + "? ", "Внимание",
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question ) == DialogResult.Cancel)
                return;
            Unit.RemoveUnit( u );
            LoadUnits();
        }

        private void startStripButton_Click( object sender, EventArgs e ) {
            if (Tick == 0) {
                timer1.Enabled = gameSceneLoad();
                curUnit = 0;
            } else {
                timer1.Enabled = true;
            }
        }

        private void stopStripButton_Click( object sender, EventArgs e ) {
            timer1.Enabled = false;
            Tick = 0;
        }

        static int dd = 2;
        static int dd2 = dd*2+1;
        int curUnit = 0;
        int NextDD() => rand.Next( dd2 ) - dd;

        private void timer1_Tick( object sender, EventArgs e ) {
            Tick++;
            int command = units[curUnit].Program.Run(ref programs)?.Command ?? 0;
            if (command != 0) {
                //TODO расчет действий юнитов
            }
            if( ++curUnit >= units.Count ) {
                curUnit = 0;
            }
            foreach (var u in units) {
                int newx = u.X + NextDD();
                if (newx > 0 && newx < gameScene.Width)
                    u.X = newx;
                int newy = u.Y + NextDD();
                if (newy > 0 && newy < gameScene.Height)
                    u.Y = newy;
            }
            gameScene.Refresh();
        }

        static int radius = 8;
        static int diametr = radius * 2;
        private void gameScene_Paint( object sender, PaintEventArgs e ) {
            Graphics g = e.Graphics;
            if (timer1.Enabled) {
                g.DrawString( "Tick = " + Tick.ToString(), fnt, Brushes.Blue, new Point( 10, 10 ) );
                foreach (var u in units) {
                    Brush br = u.Type switch  {
                        UnitType.Soldier => Brushes.Red,
                        UnitType.Worker => Brushes.Blue,
                        UnitType.Medic => Brushes.Green,
                        _ => Brushes.White
                    };
                    g.FillEllipse( br, u.X - radius, u.Y - radius, diametr, diametr );
                }
            }
        }

        private bool gameSceneLoad() {
            //TODO инициализация сцены перед стартом симуляции
            if( units.Count == 0 ) {
                return false;
            }
            foreach (var u in units) {
                Program p = u.Program;
                if (p is null) {
                    MessageBox.Show( "Program for unit (" + u.Id + ", " + u.Name + ") not found", "Error gameSceneLoad()", MessageBoxButtons.OK );
                    return false;
                }
                if (p.CheckLenght( out int len, ref programs ) == false) {
                    MessageBox.Show( "Program " + p.Name + " for unit " + u.Name + " has too much lenght", "Error gameSceneLoad()", MessageBoxButtons.OK );
                    return false;
                }
                rand = new Random();
                u.X = rand.Next( gameScene.Width );
                u.Y = rand.Next( gameScene.Height );
            }
            return true;
        }
    }
}
