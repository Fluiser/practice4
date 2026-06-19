using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace practice4
{
    public partial class EditRecord : Form
    {
        private CSVContainer.CSVRow row;
        private CSVContainer _container = null;
        bool save = false;

        public EditRecord()
        {
            InitializeComponent();
        }

        public bool ShowDialog(ref CSVContainer.CSVRow r_arg, CSVContainer _c)
        {
            _container = _c;

            int id = 0;
            if (r_arg.id == 0)
            {
                foreach (var item in _container.rows)
                    if (id <= item.id) id = item.id + 1;
            }
            else id = r_arg.id;

            numericID.Value = id;
            textBoxREGION.Text = r_arg.region;
            textBoxSTATE.Text = r_arg.state;
            numericAREA.Value = r_arg.area;
            numericPOPULATION.Value = r_arg.population;

            base.ShowDialog();

            r_arg = row;
            return save;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            row.id = ((int)numericID.Value);
            row.region = textBoxREGION.Text;
            row.state = textBoxSTATE.Text;
            row.area = ((int)numericAREA.Value);
            row.population = ((int)numericPOPULATION.Value);

            if(_container.rows.FindIndex(r => r.id == row.id) > -1)
            {
                MessageBox.Show(
                    "В файле уже есть запись с таким id", 
                    "Ошибка",                             
                    MessageBoxButtons.OK,                    
                    MessageBoxIcon.Error                    
                );
                return;
            }

            save = true;
            base.Close();
        }
    }
}
