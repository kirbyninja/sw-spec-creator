using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpecCreator
{
    public partial class FormSpecCreator : Form
    {
        private DataTable dtSource;

        public FormSpecCreator()
        {
            InitializeComponent();
            ShowVersionOnTitle();

            btnS2D.Click += (s, e) =>
            {
                if (IsFileOpened(true)) SaveFile(false);
            };
            btnD2S.Click += (s, e) =>
            {
                if (IsFileOpened(false)) SaveFile(true);
            };

            btnExportBatch.Click += (s, e) =>
            {
                OpenFolder();
            };
        }

        private bool IsFileOpened(bool isSql)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Filter = isSql ? "SQL|*.sql" : "Docx|*.docx|Doc|*.doc";
                openFile.Title = "匯入檔案";

                if (openFile.ShowDialog() != DialogResult.OK
                    || openFile.FileName == string.Empty)
                    return false;
                if (dtSource != null) dtSource.Clear();
                try
                {
                    if (isSql)
                    {
                        if (LoadSql.LoadSpec(openFile.FileName))
                            dtSource = LoadSql.dt;
                        else
                            return false;
                    }
                    else
                    {
                        if (LoadWord.LoadSpec(openFile.FileName))
                            dtSource = LoadWord.dt;
                        else
                            return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }
            }
        }

        private void SaveFile(bool isSql)
        {
            using (SaveFileDialog saveFile = new SaveFileDialog())
            {
                saveFile.Title = "匯出檔案";
                saveFile.Filter = isSql ? "SQL|*.sql" : "Docx|*.docx|Doc|*.doc";
                saveFile.FileName = isSql ? ExportSql.TableName : ExportWord.TableCName;

                if (saveFile.ShowDialog() == DialogResult.OK && saveFile.FileName.Length > 0)
                {
                    try
                    {
                        if (isSql)
                        {
                            using (StreamWriter writer = new StreamWriter(saveFile.OpenFile()))
                            {
                                ExportSql.WordTable = dtSource;
                                ExportSql.Template = Properties.Resources.SqlTemplate;
                                writer.WriteLine(ExportSql.GetExportSql());
                            }
                        }
                        else
                        {
                            ExportWord.dt = dtSource;
                            ExportWord.ExportSpec(saveFile.FileName);
                        }
                        MessageBox.Show("完成");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void OpenFolder()
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    progressBar.Value = 0;
                    SearchFile(folderBrowser.SelectedPath);
                }
                MessageBox.Show("完成");
            }
        }

        private void SearchFile(string dir)
        {
            var extensions = new List<string> { ".doc", ".docx" };
            var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                .Where(s => extensions.Any(ext => s.ToLower().EndsWith(ext)));

            progressBar.Maximum = files.Count();
            foreach (string file in files)
            {
                if (dtSource != null) dtSource.Clear();
                try
                {
                    if (LoadWord.LoadSpec(file))
                        dtSource = LoadWord.dt;
                    string filePath = string.Format(@"{0}\{1}.sql", dir, ExportSql.TableName);
                    using (StreamWriter writer = new StreamWriter(filePath, false))
                    {
                        ExportSql.WordTable = dtSource;
                        ExportSql.Template = Properties.Resources.SqlTemplate;
                        writer.WriteLine(ExportSql.GetExportSql());
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                progressBar.Value++;
            }
        }

        private void ShowVersionOnTitle()
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            this.Text = string.Format("{0} {1}", fileVersionInfo.ProductName, fileVersionInfo.FileVersion);
        }
    }
}