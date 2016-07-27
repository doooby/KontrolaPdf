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

namespace CheckPdf
{
    public partial class MainForm : Form
    {
        private DirectoryInfo directory_info;

        public MainForm()
        {
            InitializeComponent();
            this.ClientSize = Properties.Settings.Default.WindowSize;
            System.Console.WriteLine(this.ClientSize.ToString());
        }

        public void actualizeCheckPdf()
        {
            string directory = textBox1.Text;
            if (directory.Length == 0 || !Directory.Exists(directory)) return;

            directory_info = new DirectoryInfo(directory);
            if (directory_info == null) return;

            int total_pages = 0;
            dataGridView1.Rows.Clear();
            FileInfo[] files = directory_info.GetFiles("*.pdf");

            foreach (FileInfo fi in files)
            {
                PdfSharp.Pdf.PdfDocument doc = PdfSharp.Pdf.IO.PdfReader.Open(fi.OpenRead());
                long size = doc.FileSize;
                total_pages += doc.PageCount;
                dataGridView1.Rows.Add(new object[]{ fi.Name, doc.PageCount.ToString(), longToMb(size).ToString("n2") });
            }

            label3.Text = files.Length.ToString();
            label4.Text = total_pages.ToString();
        }

        public void exportToCsv()
        {
            if (directory_info == null) return;
            if (dataGridView1.Rows.Count < 1) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                StringBuilder sb = new StringBuilder();

                // columns
                string[] columnNames = dataGridView1.Columns.Cast<DataGridViewTextBoxColumn>().
                    Select(column => String.Format("\"{0}\"", column.HeaderText)).ToArray();
                sb.AppendLine(string.Join(",", columnNames));

                // each row
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string[] fields = row.Cells.Cast<DataGridViewTextBoxCell>().
                        Select(cell => String.Format("\"{0}\"", (string)cell.Value)).ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(saveFileDialog1.FileName, sb.ToString());

            }

            
        }

        private float longToMb(long bytes_count)
        {
            return (float)(Convert.ToDouble(bytes_count) / 1024d / 1024d);
        }

        // select directory button
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (Properties.Settings.Default.LastSelected.Length != 0)
            {
                d.SelectedPath = Properties.Settings.Default.LastSelected;
            }
            d.ShowNewFolderButton = false;
            if (d.ShowDialog() != DialogResult.OK) return;

            textBox1.Text = d.SelectedPath;
            Properties.Settings.Default.LastSelected = d.SelectedPath;
            actualizeCheckPdf();
        }

        // reload button
        private void button3_Click(object sender, EventArgs e)
        {
            actualizeCheckPdf();
        }

        // export to CSV button
        private void button2_Click(object sender, EventArgs e)
        {
            exportToCsv();
        }

        // save window size on close
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WindowSize = this.ClientSize;
            Properties.Settings.Default.Save();
        }
    }
}
