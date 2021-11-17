using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MirAI.Forma {
    public partial class Simulation : Form {
        public Simulation() {
            InitializeComponent();
            listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
        }

        private void Simulation_FormClosed( object sender, FormClosedEventArgs e ) {
        }

        private void toolStripButton1_Click( object sender, EventArgs e ) {
            listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
        }
    }
}
