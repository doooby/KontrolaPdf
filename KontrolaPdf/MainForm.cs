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

namespace KontrolaPdf
{
    public partial class MainForm : Form
    {
        private DirectoryInfo directory_info;

        public MainForm()
        {
            InitializeComponent();
            //selectDirectory(@"C:\Users\user\Source\Repos\test_files"); // for testing
        }

        public void actualizeKontrolaPdf()
        {
            if (directory_info == null) return;

            long total_size = 0L;
            dataGridView1.Rows.Clear();
            FileInfo[] files = directory_info.GetFiles("*.pdf");

            foreach (FileInfo fi in files)
            {
                PdfSharp.Pdf.PdfDocument doc = PdfSharp.Pdf.IO.PdfReader.Open(fi.OpenRead());
                long size = doc.FileSize;
                total_size += size;
                dataGridView1.Rows.Add(new object[]{ fi.Name, doc.PageCount.ToString(), longToMb(size).ToString("n2") });
            }

            label3.Text = files.Length.ToString();
            label4.Text = string.Format("{0} MB", longToMb(total_size).ToString("n2"));
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

        private void selectDirectory(string pre_selected_path=null)
        {
            if (pre_selected_path != null) {
                directory_info = new DirectoryInfo(pre_selected_path);
            }
            else
            {
                FolderBrowserDialog d = new FolderBrowserDialog();
                d.ShowNewFolderButton = false;
                if (d.ShowDialog() == DialogResult.OK)
                {
                    directory_info = new DirectoryInfo(d.SelectedPath);
                }
            }

            textBox1.Text = directory_info.FullName;
            actualizeKontrolaPdf();
        }

        private float longToMb(long bytes_count)
        {
            return (float)(Convert.ToDouble(bytes_count) / 1024d / 1024d);
        }

        // select directory button
        private void button1_Click(object sender, EventArgs e)
        {
            selectDirectory();
        }

        // directory textbox double-click
        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectDirectory();
        }

        // reload button
        private void button3_Click(object sender, EventArgs e)
        {
            if (directory_info != null)
            {
                directory_info.Refresh();
                actualizeKontrolaPdf();
            }
        }

        // export to CSV button
        private void button2_Click(object sender, EventArgs e)
        {
            exportToCsv();
        }
    }
}
