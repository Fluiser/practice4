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
    public partial class Form1 : Form
    {
        private CSVContainer _container = new CSVContainer();
        public Form1()
        {
            InitializeComponent();

            _container.parse(File.ReadAllText("f.csv"));
        }
    }
}
