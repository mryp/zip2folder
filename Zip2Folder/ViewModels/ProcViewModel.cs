using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Zip2Folder.ViewModels
{
    public class ProcViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private string _folderPath;
        private string _cancelButtonName;
        private int _progressValue;

        public string FolderPath
        {
            get { return _folderPath; }
            set { Set(ref _folderPath, value); }
        }

        public string CancelButtonName
        {
            get { return _cancelButtonName; }
            set
            {
                _cancelButtonName = value;
                NotifyOfPropertyChange(() => CancelButtonName);
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

        public ProcViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.CancelButtonName = "キャンセル";
            this.ProgressValue = 50;
        }

        public void Cancel()
        {
            _navigationService.For<StartViewModel>().Navigate();
        }

        public void Loaded()
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
