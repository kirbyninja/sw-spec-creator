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
        }

        private void ShowVersionOnTitle()
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            this.Text = string.Format("{0} {1}", fileVersionInfo.ProductName, fileVersionInfo.FileVersion);
        }
    }
}