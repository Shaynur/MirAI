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
        private Unit _unit;
        public EditUnitForm( List<Program> programs ) {
            InitializeComponent();
            this.programs = programs;
            _unit = null;
        }
        public EditUnitForm( List<Program> programs, Unit unit ) : this( programs ) {
            _unit = unit;
        }

        private void EditUnitForm_Load( object sender, EventArgs e ) {
            CreateListBoxesItems();
            GetDataFromUnit();
        }

        private void SaveUnit() {
            if (_unit is null) {
                _unit = new Unit();
            }
            _unit.Name = unitNameBox.Text;
            _unit.Type = (UnitType)Enum.Parse( typeof( UnitType ), unitTypeBox.SelectedItem.ToString() );
            //_unit.ProgramId = programs[unitProgramBox.SelectedIndex].Id;
            _unit.Program = programs[unitProgramBox.SelectedIndex];
            _unit.Save();
        }

        private void CreateListBoxesItems() {
            unitNameBox.Text = string.Empty;
            var values = Enum.GetValues(typeof(UnitType));
            foreach (var t in values) {
                unitTypeBox.Items.Add( t.ToString() );
            }
            if (unitTypeBox.Items.Count > 0) {
                unitTypeBox.SelectedIndex = 0;
                unitProgramBox.Items.Clear();
                for (int i = 0; i < programs.Count; i++) {
                    unitProgramBox.Items.Add( programs[i].Name );
                }
            }
            if (unitProgramBox.Items.Count > 0) {
                unitProgramBox.SelectedIndex = 0;
            }
        }

        private void GetDataFromUnit() {
            if (_unit is null) {
                return;
            }
            unitNameBox.Text = _unit.Name;
            unitNameBox.SelectionStart = unitNameBox.Text.Length;
            unitTypeBox.SelectedIndex = unitTypeBox.Items.IndexOf( _unit.Type.ToString() );
            //unitProgramBox.SelectedIndex = unitProgramBox.Items.IndexOf( programs.Find( p => p.Id == _unit.ProgramId ).Name );
            unitProgramBox.SelectedIndex = unitProgramBox.Items.IndexOf( _unit.Program.Name );
        }

        private void button1_Click( object sender, EventArgs e ) {
            SaveUnit();
        }
    }
}

