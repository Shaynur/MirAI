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
                i.SubItems.Add( programs.Find( p => p.Id == u.ProgramId ).Name );
                i.Tag = u;
                listView1.Items.Add( i );
            }
            listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
        }

        private void addUnitMenu_Click( object sender, EventArgs e ) {
            EditUnitForm eu = new EditUnitForm( programs );
            if (eu.ShowDialog() == DialogResult.OK) {
                eu.CreateUnit();
                LoadUnits();
            }
            eu.Dispose();
        }

        private void startStripButton_Click( object sender, EventArgs e ) {
            if( Tick == 0 ) {
                gameSceneLoad();
            }
            timer1.Enabled = true;
        }

        private void stopStripButton_Click( object sender, EventArgs e ) {
            timer1.Enabled = false;
            Tick = 0;
        }

        private void timer1_Tick( object sender, EventArgs e ) {
            Tick++;
            //TODO расчет действий юнитов
            gameScene.Refresh();
        }

        private void gameScene_Paint( object sender, PaintEventArgs e ) {
            Graphics g = e.Graphics;
            if (timer1.Enabled) {
                g.DrawString( "Tick = " + Tick.ToString(), fnt, Brushes.Blue, new Point( 10, 10 ) );
            }
        }

        private void gameSceneLoad() {
                //TODO инициализация сцены перед стартом симуляции
        }
            
    }
}
