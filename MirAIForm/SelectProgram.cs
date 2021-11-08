using MirAI.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MirAI.Forma
{
    public partial class SelectProgram : Form
    {
        public string selectedProgram = "";
        public SelectProgram(object dataSource, string programName)
        {
            InitializeComponent();
            List<Program> programs = (List<Program>)dataSource;
            foreach (var p in programs)
            {
                if (p.Name != programName)
                    listBox1.Items.Add(p.Name);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                selectedProgram = listBox1.SelectedItem.ToString();
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
