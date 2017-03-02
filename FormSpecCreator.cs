using SpecCreator.Converting;
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
        private readonly Dictionary<Button, ConvertInfo> buttons = new Dictionary<Button, ConvertInfo>();

        public FormSpecCreator()
        {
            InitializeComponent();
            ShowVersionOnTitle();

            buttons.Add(btnS2D, new ConvertInfo(FileType.Sql, FileType.Word, false));
            buttons.Add(btnD2S, new ConvertInfo(FileType.Word, FileType.Sql, false));
            buttons.Add(btnBatchS2D, new ConvertInfo(FileType.Sql, FileType.Word, true));
            buttons.Add(btnBatchD2S, new ConvertInfo(FileType.Word, FileType.Sql, true));

            foreach (var b in buttons)
                b.Key.Click += btnConvert_Click;
        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            ConvertInfo convertInfo;
            if (sender is Button && buttons.TryGetValue(sender as Button, out convertInfo))
            {
                foreach (var button in buttons.Keys)
                    button.Enabled = false;

                await Converter.ConvertFilesAsync(convertInfo, new Progress<int>(p => progressBar.Value = p));

                foreach (var button in buttons.Keys)
                    button.Enabled = true;

                (sender as Button).Focus();
            }
        }

        private void ShowVersionOnTitle()
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            this.Text = string.Format("{0} {1}", fileVersionInfo.ProductName, fileVersionInfo.FileVersion);
        }
    }
}