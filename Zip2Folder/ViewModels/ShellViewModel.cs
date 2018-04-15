using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Zip2Folder.ViewModels
{
    public class ShellViewModel : Screen
    {
        private string _folderPath;
        private int _progressValue;

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                NotifyOfPropertyChange(() => FolderPath);
            }
        }

        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                NotifyOfPropertyChange(() => ProgressValue);
            }
        }

        public ShellViewModel()
        {
            this.FolderPath = "";
            this.ProgressValue = 0;
        }

        public void SetSelectFolder()
        {
            //https://github.com/aybe/Windows-API-Code-Pack-1.1 を使用
            var dialog = new CommonOpenFileDialog("フォルダ選択")
            {
                IsFolderPicker = true,
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.FolderPath = dialog.FileName;
            }
        }

        public void Start()
        {
            Task.Run(() => {
                string message = unzipFilesFromFolder(this.FolderPath);
                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("完了");
                }
                else
                {
                    MessageBox.Show(message, "エラー"
                        , MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private string unzipFilesFromFolder(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                return "指定したフォルダが見つかりません";
            }

            string[] files = Directory.GetFiles(dirPath, "*.zip");
            if (files.Length == 0)
            {
                return "ZIPファイルが見つかりません";
            }

            foreach (string file in files)
            {
                string result = unzipFile(file);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }

            return "";
        }

        private string unzipFile(string zipPath)
        {
            string outputPath = Path.Combine(Path.GetDirectoryName(zipPath),
                Path.GetFileNameWithoutExtension(zipPath));
            if (Directory.Exists(outputPath))
            {
                return "フォルダは既に存在しています";
            }

            try
            {
                Directory.CreateDirectory(outputPath);
                ZipFile.ExtractToDirectory(zipPath, outputPath);
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return "";
        }
    }
}
