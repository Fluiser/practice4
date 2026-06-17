using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace practice4
{
    public partial class MainForm : Form
    {
        private CSVContainer _container = new CSVContainer();
        public MainForm()
        {
            InitializeComponent();
        }

        private void refreshViewData()
        {
            gridViewCSV.Rows.Clear();

            foreach(var data in _container.rows)
            {
                gridViewCSV.Rows.Add(data.id, data.region, data.state, data.area, data.population);
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            ofd.Filter = "CSV Files(*.csv)|*.csv|All files(*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _container.clear();
                if (!_container.parse(File.ReadAllText(ofd.FileName)))
                    MessageBox.Show("Ошибка чтения файла");
                refreshViewData();
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Directory.GetCurrentDirectory();
            sfd.FileName = "file.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, _container.getString());
            }
        }
    }
}
