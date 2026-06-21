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
    public partial class GraphicsView : Form
    {
        protected struct memberStatistic
        {
            public float percent;
            public decimal value;
        };

        Color[] colors = new Color[] { Color.Red, Color.Orange, Color.Green, Color.Yellow, Color.BlueViolet };

        CSVContainer _container;

        decimal totalPopulation;
        Dictionary<string, memberStatistic> statistics = new Dictionary<string, memberStatistic>();

        public GraphicsView(CSVContainer container)
        {
            InitializeComponent();
            _container = container;

            foreach(var m in _container.rows)
            {
                totalPopulation += m.population;

                memberStatistic value = new memberStatistic();
                if (statistics.TryGetValue(m.region, out value))
                {
                    value.value += m.population;
                } else
                {
                    value.value = m.population;
                }
                statistics[m.region] = value;
            }

            string[] keys = new string[statistics.Count];
            statistics.Keys.CopyTo(keys, 0);
            foreach (var key in keys)
            {
                memberStatistic v = statistics[key];
                if(totalPopulation > 0)
                    v.percent = (float)(v.value / totalPopulation);
                statistics[key] = v; 
            }

            int idxColor = 0;
            foreach (var v in statistics)
            {
                dataGridView1.Rows.Add(v.Key, v.Value.value.ToString("N2"), (v.Value.percent*100).ToString("N2").PadLeft(5) + " %");
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = colors[idxColor++ % colors.Length]; 
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }
    }
}
