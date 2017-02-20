using SpecCreator.FileHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpecCreator
{
    public enum FileType { Sql, Word, }

    public static class CommonFunc
    {
        public static int GetWidth(this string s)
        {
            return Encoding.GetEncoding("big5").GetByteCount(s);
        }

        public static string Remove(this string input, string pattern, RegexOptions options = RegexOptions.None)
        {
            return Regex.Replace(input, pattern, "", options);
        }

        /// <summary>
        /// 移除特殊控制字元
        /// http://stackoverflow.com/questions/1497885/remove-control-characters-from-php-string
        /// </summary>
        public static string RemoveCtrlChar(this string s)
        {
            return s.Remove(@"[\x00-\x1F\x7F]");
        }

        #region 轉檔流程封裝

        private static IFileHandler SqlHandler = new SqlHandler();
        private static IFileHandler WordHandler = new WordHandler();

        public static void ConvertFiles(FileType sourceType, FileType targetType, bool isByFolder, ProgressBar progress = null)
        {
            var reader = GetFileHandler(sourceType);
            var writer = GetFileHandler(targetType);

            if (reader == null || writer == null)
                return;

            IEnumerable<string> sourceFiles;
            string targetFolder = string.Empty;

            if (isByFolder)
                sourceFiles = ShowFolderDialog(reader, ref targetFolder);
            else
                sourceFiles = ShowFileDialog(reader, ref targetFolder);

            if (sourceFiles == null || !Directory.Exists(targetFolder))
                return;

            int totalFileCount = sourceFiles.Count();

            // 定義是否要讓使用者一一決定存檔位置的條件
            bool showSaveDialog = !isByFolder && totalFileCount == 1;

            var failedFiles = new List<string>();

            if (progress != null)
            {
                progress.Maximum = totalFileCount;
                progress.Value = 0;
            }

            foreach (string sourceFile in sourceFiles)
            {
                try
                {
                    var table = reader.Load(sourceFile);

                    // Should normalize talbe here.

                    string targetFileName = string.Format(@"{0}\{1}.{2}",
                        targetFolder, table.TableName, writer.Extension.Split('|').First());

                    if (showSaveDialog)
                        targetFileName = ShowSaveFileDialog(writer, targetFileName);

                    writer.Save(table, targetFileName);

                    if (progress != null)
                        ++progress.Value;
                }
                catch (Exception)
                {
                    failedFiles.Add(sourceFile);
                }
            }
            MessageBox.Show(string.Format("轉換完成！\r\n資料筆數：{0}\r\n成功筆數：{1}\r\n失敗筆數：{2}",
                totalFileCount, totalFileCount - failedFiles.Count, failedFiles.Count));
        }

        private static IEnumerable<string> FindFiles(string dirPath, IFileHandler fileHandler)
        {
            string pattern = GetExtensionPattern(fileHandler);

            return Directory.EnumerateFiles(dirPath, "*.*", SearchOption.AllDirectories).
                Where(fileName => Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase));
        }

        private static string GetExtensionPattern(IFileHandler fileHandler)
        {
            return string.Format(@"\.(?:{0})$", fileHandler.Extension);
        }

        private static IFileHandler GetFileHandler(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Sql:
                    return SqlHandler;

                case FileType.Word:
                    return WordHandler;

                default:
                    return null;
            }
        }

        private static string GetFilterText(IFileHandler fileHandler)
        {
            string filter = string.Join(";", fileHandler.Extension.
                Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).
                Select(ext => string.Format("*.{0}", ext)));

            return string.Format("{0}({1})|{1}", fileHandler.Description, filter);
        }

        private static IEnumerable<string> ShowFileDialog(IFileHandler fileHandler, ref string folderPath)
        {
            using (var openFile = new OpenFileDialog())
            {
                openFile.Filter = GetFilterText(fileHandler);
                openFile.Title = "匯入檔案";
                openFile.Multiselect = true;

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    folderPath = Directory.GetParent(openFile.FileName).FullName;
                    return openFile.FileNames;
                }
                else
                    return null;
            }
        }

        private static IEnumerable<string> ShowFolderDialog(IFileHandler fileHandler, ref string folderPath)
        {
            using (var openFolder = new FolderBrowserDialog())
            {
                if (openFolder.ShowDialog() == DialogResult.OK)
                {
                    folderPath = openFolder.SelectedPath;
                    return FindFiles(openFolder.SelectedPath, fileHandler);
                }
                else
                    return null;
            }
        }

        private static string ShowSaveFileDialog(IFileHandler fileHandler, string defaultFileFullName)
        {
            using (var saveFile = new SaveFileDialog())
            {
                saveFile.Filter = GetFilterText(fileHandler);
                saveFile.Title = "匯出檔案";
                saveFile.InitialDirectory = Path.GetDirectoryName(defaultFileFullName);
                saveFile.FileName = Path.GetFileName(defaultFileFullName);

                if (saveFile.ShowDialog() == DialogResult.OK)
                    return saveFile.FileName;
                else
                    return null;
            }
        }

        #endregion 轉檔流程封裝
    }
}