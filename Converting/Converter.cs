﻿using SpecCreator.FileHandlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpecCreator.Converting
{
    public class Converter
    {
        private static IFileHandler SqlHandler = new SqlHandler();
        private static IFileHandler WordHandler = new WordHandler();

        public static async Task ConvertFilesAsync(FileType sourceType, FileType targetType, bool isByFolder, IProgress<int> progress)
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
            int fileCount = 0;

            if (totalFileCount == 0)
            {
                MessageBox.Show("找無符合條件的檔案");
                return;
            }

            // 定義是否要讓使用者一一決定存檔位置的條件
            bool showSaveDialog = !isByFolder && totalFileCount == 1;

            var results = new List<ConvertResult>();

            progress.Report(0);

            var timer = new Stopwatch();
            timer.Start();

            if (showSaveDialog)
            {
                // 使用單一執行緒一個一個檔依序讓使用者決定轉檔後的檔名
                foreach (var sourceFile in sourceFiles)
                {
                    results.Add(ConvertFile(reader, writer, sourceFile, true));
                    progress.Report(Interlocked.Increment(ref fileCount) * 100 / totalFileCount);
                }
            }
            else
            {
                await Task.Factory.StartNew(() =>
                {
                    // 因不需指定檔名，就使用多執行緒以加速批次轉檔
                    sourceFiles.AsParallel().ForAll(sourceFile =>
                    {
                        results.Add(ConvertFile(reader, writer, sourceFile, false));
                        progress.Report(Interlocked.Increment(ref fileCount) * 100 / totalFileCount);
                    });
                });
            }

            timer.Stop();
            MessageBox.Show(GetSummary(results, timer.Elapsed));
        }

        private static ConvertResult ConvertFile(IFileHandler reader, IFileHandler writer, string sourceFile, bool showSaveDialog)
        {
            try
            {
                var table = reader.Load(sourceFile);

                string targetFile = string.Format(@"{0}\{1}.{2}",
                        Path.GetDirectoryName(sourceFile), table.TableName, writer.Extension.Split('|').First());

                if (showSaveDialog)
                {
                    ShowSaveFileDialog(writer, ref targetFile);

                    if (targetFile == null)
                        return new ConvertResult(sourceFile, false, "使用者取消");
                }

                writer.Save(table, targetFile);
                return new ConvertResult(sourceFile, true, string.Empty);
            }
            catch (Exception ex)
            {
                return new ConvertResult(sourceFile, false, ex.Message);
            }
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

        private static string GetSummary(IEnumerable<ConvertResult> results, TimeSpan elapsedTime)
        {
            int total = results.Count();
            int successCount = results.Count(r => r.IsSuccessful);

            return string.Format("轉換完成！\r\n資料筆數：{0}\r\n成功筆數：{1}\r\n失敗筆數：{2}{3}\r\n共歷時{4:n3}秒",
                total,
                successCount,
                total - successCount,
                total > successCount
                    ? string.Format("\r\n失敗檔案如下：\r\n{0}", string.Join("\r\n", results.Where(r => !r.IsSuccessful).Select(r => r.FileName)))
                    : "",
                elapsedTime.TotalSeconds);
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

        private static void ShowSaveFileDialog(IFileHandler fileHandler, ref string fileName)
        {
            using (var saveFile = new SaveFileDialog())
            {
                saveFile.Filter = GetFilterText(fileHandler);
                saveFile.Title = "匯出檔案";
                saveFile.InitialDirectory = Path.GetDirectoryName(fileName);
                saveFile.FileName = Path.GetFileName(fileName);

                if (saveFile.ShowDialog() == DialogResult.OK)
                    fileName = saveFile.FileName;
                else
                    fileName = null;
            }
        }
    }
}