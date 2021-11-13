using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osumania
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkedListBox1.GetItemCheckState(checkedListBox1.SelectedIndex) == CheckState.Checked)
                {
                    Form1 newForm = new Form1(checkedListBox1.SelectedIndex);
                    newForm.Location = new Point(Location.X, Location.Y);
                    newForm.ShowDialog();
                    //this.Close();
                }
            }
            catch(Exception)
            {
                MessageBox.Show("난이도를 선택하지 않았습니다.");

            }
            
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for(int i = 0; i < 3; i++)
                checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
            checkedListBox1.SetItemCheckState(checkedListBox1.SelectedIndex, CheckState.Checked);
        }

        private void checkedListBox1_MouseClick(object sender, MouseEventArgs e)
        {
            checkedListBox1.SetItemCheckState(checkedListBox1.SelectedIndex, CheckState.Checked);
        }

        private void checkedListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            checkedListBox1.SetItemCheckState(checkedListBox1.SelectedIndex, CheckState.Checked);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
