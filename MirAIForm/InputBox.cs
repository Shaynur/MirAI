using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MirAI.Forma
{
    public partial class InputBox : Form
    {
        public int MaxTextLenght = 40;
        private List<string> ProgNames;

        public InputBox(List<string> progNames)
        {
            ProgNames = progNames;
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string t = textBox1.Text;
            if (t.Contains(" "))
            {
                t = t.Replace(" ", "");
                textBox1.Text = t;
                textBox1.Focus();
                textBox1.SelectionStart = textBox1.Text.Length;
            }
            if (t.Length == 0 || t.Length > MaxTextLenght || ProgNames.Contains(t))
            {
                btnOk.Enabled = false;
                return;
            }
            btnOk.Enabled = true;
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if( e.KeyCode == Keys.Return)
            {
                textBox1_TextChanged(sender, e);
                if (btnOk.Enabled)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else if( e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
