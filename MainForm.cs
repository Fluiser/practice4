using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static practice4.CSVContainer;


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

        //Для удобства
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

        //Задание 1. Создание файла - сохранение пустого файла тоже создание.
        //И не пустого - сохранение.
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

        // Задание 3. Добавление записи.
        private void добавитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditRecord form = new EditRecord();
            CSVRow row = new CSVRow();
            if(form.ShowDialog(ref row, _container))
            {
                _container.addRecord(row);
            }
            refreshViewData();
        }
        // Задание 2. Печать файла
        private void печататьToolStripMenuItem_Click(object _, EventArgs __)
        {
            _currentRowIndex = 0;

            PrintDocument printDoc = new PrintDocument();

            printDoc.PrintPage += (object sender, PrintPageEventArgs e) => PrintDoc_PrintPage(sender, e, false);

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        private int _currentRowIndex = 0;
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e, bool calcFunc)
        {
            const int fontSize = 8;
            Font font = new Font("Arial", fontSize, FontStyle.Regular);
            Font headerFont = new Font("Arial", fontSize, FontStyle.Bold);
            Brush brush = Brushes.Black;
            Pen linePen = new Pen(Color.Gray, 0.5f);

            float startX = e.MarginBounds.Left;
            float currentY = e.MarginBounds.Top;
            float pageBottom = e.MarginBounds.Bottom;

            const float colWidthId = 50;
            const float colWidthRegion = 270;
            const float colWidthState = 100;
            const float colWidthArea = 120;
            const float colWidthPopulation = 70;
            const float colWidthDensity = 70;

            float rowHeight = font.GetHeight(e.Graphics) + 10;

            DrawRow(e.Graphics, 
                headerFont, 
                brush, 
                startX, 
                currentY, 
                "ID", "Регион", "Штат", "Площадь", "Население", (calcFunc ? "Кол-во тыс. людей\n на тыс. кв. км" : ""), 
                colWidthId, (calcFunc ? colWidthRegion - colWidthDensity : colWidthRegion), colWidthState, colWidthArea, colWidthPopulation, colWidthDensity);

            currentY += rowHeight*2;
            e.Graphics.DrawLine(linePen, startX, currentY, e.MarginBounds.Right, currentY);
            currentY += 5; 


            while (_currentRowIndex < _container.rows.Count)
            {
                if (currentY + rowHeight > pageBottom)
                {
                    e.HasMorePages = true;
                    return; 
                }

                CSVRow row = _container.rows[_currentRowIndex];

                DrawRow(
                    e.Graphics, font, brush, startX, currentY,
                    row.id.ToString(),
                    row.region,
                    row.state,
                    row.area.ToString("N2"),
                    row.population.ToString("N0"),
                    (calcFunc ? (row.population/row.area).ToString("N2") : ""),

                    colWidthId, (calcFunc ? colWidthRegion-colWidthDensity : colWidthRegion), colWidthState, colWidthArea, colWidthPopulation, colWidthDensity
                );

                currentY += rowHeight;

                e.Graphics.DrawLine(linePen, startX, currentY, e.MarginBounds.Right, currentY);
                currentY += 5;

                _currentRowIndex++; 
            }

            e.HasMorePages = false;
            _currentRowIndex = 0;
        }

        private void DrawRow(Graphics g, Font font, Brush brush, float x, float y,
                             string id, string region, string state, string area, string pop, string densityPopArea,
                             float wId, float wReg, float wSt, float wAr, float wPop, float wDensityPA)
        {
            float currentX = x;
            float rowHeight = font.GetHeight(g) + 10; 

            StringFormat stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.EllipsisCharacter; 
            stringFormat.FormatFlags = StringFormatFlags.NoWrap;

            g.DrawString(id, font, brush, currentX, y);
            currentX += wId;

            RectangleF regionRect = new RectangleF(currentX, y, wReg, rowHeight);
            g.DrawString(region, font, brush, regionRect, stringFormat);
            currentX += wReg;

            RectangleF stateRect = new RectangleF(currentX, y, wSt, rowHeight);
            g.DrawString(state, font, brush, stateRect, stringFormat);
            currentX += wSt;

            g.DrawString(area, font, brush, currentX, y);
            currentX += wAr;

            g.DrawString(pop, font, brush, currentX, y);
            currentX += wPop;

            if(densityPopArea.Length > 0)
                g.DrawString(densityPopArea, font, brush, currentX, y);
        }

        //Задание 4. Корректировка записей.
        private void изменитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int idx = gridViewCSV.SelectedCells[0].RowIndex;

            if (_container.rows.Count < 1 || gridViewCSV.SelectedCells.Count < 1 || idx < 0 || idx >= _container.rows.Count)
            {
                MessageBox.Show("Отсутствуют записи или не выбрана запись для корректировки");
                return;
            }

            CSVRow row = _container.rows[idx];

            EditRecord form = new EditRecord();
            if (form.ShowDialog(ref row, _container))
            {
                _container.replaceRecord(idx, row);
            }
            refreshViewData();
        }

        //Задание 5. Удаление одной и более записей в файле.
        private void удалитьВыбранныеЗаписиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridViewCSV.SelectedCells.Count < 1) return;

            if(MessageBox.Show(
                    "Вы уверены?",
                    "Точно?",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                ) == DialogResult.OK)
            {
                int removed = 0;
                var cells = gridViewCSV.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.RowIndex).Distinct().OrderBy(i => i);
                foreach(int idx in cells)
                {
                    _container.deleteRecord(idx - removed++);
                }
                gridViewCSV.ClearSelection();


                refreshViewData();
            }
        }

        //Задание 6. Упорядочение записей
        private void упорядочитьПоПолюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectMethodSort form = new SelectMethodSort();

            if(form.ShowDialog(ref _container))
                refreshViewData();
        }

        //Задание 7.  Итог. показатели
        private void расчётПоказателейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int cellIndex = 5;
            for(int r = 0; r < gridViewCSV.RowCount-1; ++r)
            {
                var v = _container.rows[r];
                gridViewCSV.Rows[r].Cells[cellIndex].Value = (v.population / v.area).ToString("N2");
            }
        }

        //Задание 8. печать результирующего файла
        private void печатьИтогРасчётовToolStripMenuItem_Click(object _, EventArgs __)
        {
            _currentRowIndex = 0;

            PrintDocument printDoc = new PrintDocument();

            printDoc.PrintPage += (object sender, PrintPageEventArgs e) => PrintDoc_PrintPage(sender, e, true);

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        //Задание 9. Графический вывод
        private void свобдныеГрафикиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphicsView graphicsView = new GraphicsView(_container);
            graphicsView.ShowDialog();
        }
    }
}
