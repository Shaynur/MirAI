using MirAI.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MirAI.Forma {
    public partial class EditUnitForm : Form {

        private List<Program> programs;
        public EditUnitForm( List<Program> programs ) {
            InitializeComponent();
            this.programs = programs;
        }

        private void EditUnitForm_Load( object sender, EventArgs e ) {
            var values = Enum.GetValues(typeof(UnitType));
            foreach (var t in values) {
                unitTypeBox.Items.Add( t.ToString() );
            }
            if (unitTypeBox.Items.Count > 0) {
                unitTypeBox.SelectedIndex = 0;
            }

            unitProgramBox.Items.Clear();
            for (int i = 0; i < programs.Count; i++) {
                unitProgramBox.Items.Add( programs[i].Name );
            }
            if (unitProgramBox.Items.Count > 0) {
                unitProgramBox.SelectedIndex = 0;
            }
        }

        public Unit CreateUnit() {
            Unit u = new Unit();
            u.Name = unitNameBox.Text;
            u.Type = (UnitType)Enum.Parse( typeof(UnitType), unitTypeBox.SelectedItem.ToString() );
            u.ProgramId = programs[unitProgramBox.SelectedIndex].Id;
            u.Save();
            return u;
        }
    }
}

