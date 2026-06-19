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
    public partial class SelectMethodSort : Form
    {
        bool sort = false;
        public SelectMethodSort()
        {
            InitializeComponent();
        }

        public bool ShowDialog(ref CSVContainer container)
        {
            comboBox1.SelectedIndex = 0;
            base.ShowDialog();

            if (sort) {

                Action<List<CSVContainer.CSVRow>>[] baseSort = new Action<List<CSVContainer.CSVRow>>[] {
                    (List<CSVContainer.CSVRow> c) => c.Sort((a,b) => a.id - b.id),
                    (List<CSVContainer.CSVRow> c) => c.Sort((a,b) => string.Compare(a.region, b.region)),
                    (List<CSVContainer.CSVRow> c) => c.Sort((a,b) => string.Compare(a.state, b.state)),
                    (List<CSVContainer.CSVRow> c) => c.Sort((a,b) => (int)((float)(a.area - b.area) * 100.0f)),
                    (List<CSVContainer.CSVRow> c) => c.Sort((a,b) => (int)((float)(a.population - b.population) * 100.0f))
                };
                Action<List<CSVContainer.CSVRow>>[] reverseSort = new Action<List<CSVContainer.CSVRow>>[] {
                    (List<CSVContainer.CSVRow> c) => c.Sort((b,a) => a.id - b.id),
                    (List<CSVContainer.CSVRow> c) => c.Sort((b,a) => string.Compare(a.region, b.region)),
                    (List<CSVContainer.CSVRow> c) => c.Sort((b,a) => string.Compare(a.state, b.state)),
                    (List<CSVContainer.CSVRow> c) => c.Sort((b,a) => (int)((float)(a.area - b.area) * 100.0f)),
                    (List<CSVContainer.CSVRow> c) => c.Sort((b,a) => (int)((float)(a.population - b.population) * 100.0f))
                };

                var rows = container.rows;
                if(checkBox1.Checked)
                    reverseSort[comboBox1.SelectedIndex](rows);
                else
                    baseSort[comboBox1.SelectedIndex](rows);
                container.replaceRows(rows);

                return true;
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sort = true;
            base.Close();
        }
    }
}
