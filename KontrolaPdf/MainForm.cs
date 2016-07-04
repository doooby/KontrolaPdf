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

            directory_info = new DirectoryInfo(@"C:\Projects\kontrola_pdf\KontrolaPdf\some_pdfs");
            actualizeKontrolaPdf();
        }

        public void actualizeKontrolaPdf()
        {
            if (directory_info == null) return;

            dataGridView1.Rows.Clear();
            FileInfo[] files = directory_info.GetFiles("*.pdf");
            foreach (FileInfo fi in files)
            {
                object[] row = { fi.Name };
                dataGridView1.Rows.Add(row);
            }
        }

        private void selectDirectory()
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.ShowNewFolderButton = false;
            if (d.ShowDialog() == DialogResult.OK)
            {
                directory_info = new DirectoryInfo(d.SelectedPath);
                actualizeKontrolaPdf();
            }
        }

        // Vybrat složku
        private void button1_Click(object sender, EventArgs e)
        {
            selectDirectory();
        }

        // directory textfield double-click
        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectDirectory();
        }

        // Obnovit button
        private void button3_Click(object sender, EventArgs e)
        {
            if (directory_info != null)
            {
                directory_info.Refresh();
                actualizeKontrolaPdf();
            }
        }
    }
}
