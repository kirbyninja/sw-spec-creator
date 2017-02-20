using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpecCreator
{
    public partial class FormSpecCreator : Form
    {
        public FormSpecCreator()
        {
            InitializeComponent();
            ShowVersionOnTitle();

            btnS2D.Click += (s, e) => CommonFunc.ConvertFiles(FileType.Sql, FileType.Word, false, progressBar);

            btnD2S.Click += (s, e) => CommonFunc.ConvertFiles(FileType.Word, FileType.Sql, false, progressBar);

            btnBatchS2D.Click += (s, e) => CommonFunc.ConvertFiles(FileType.Sql, FileType.Word, true, progressBar);

            btnBatchD2S.Click += (s, e) => CommonFunc.ConvertFiles(FileType.Word, FileType.Sql, true, progressBar);
        }

        private void ShowVersionOnTitle()
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            this.Text = string.Format("{0} {1}", fileVersionInfo.ProductName, fileVersionInfo.FileVersion);
        }
    }
}