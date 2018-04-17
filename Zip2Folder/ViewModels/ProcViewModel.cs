using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zip2Folder.Models;

namespace Zip2Folder.ViewModels
{
    public class ProcViewModel : Screen
    {
        private const string BUTTON_NAME_CANCEL = "キャンセル";
        private const string BUTTON_NAME_BACK = "戻る";

        private readonly INavigationService _navigationService;
        private string _cancelButtonName;
        private int _progressMax;
        private int _progressValue;
        private object _selectedLogItem;
        private UnzipManager _unzip;

        public string FolderPath
        {
            get;
            set;
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

        public int ProgressMax
        {
            get { return _progressMax; }
            set
            {
                _progressMax = value;
                NotifyOfPropertyChange(() => ProgressMax);
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

        public object SelectedLogItem
        {
            get { return _selectedLogItem; }
            set
            {
                _selectedLogItem = value;
                NotifyOfPropertyChange(() => SelectedLogItem);
            }
        }

        public BindableCollection<UnzipLogItem> ProgressItemList
        {
            get;
            set;
        }

        public ProcViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.CancelButtonName = BUTTON_NAME_CANCEL;
            this.ProgressItemList = new BindableCollection<UnzipLogItem>();
        }

        public void Loaded()
        {
            _unzip = new UnzipManager(this.FolderPath);
            _unzip.Loaded += unzip_Loaded;
            _unzip.ProgressChanged += unzip_ProgressChanged;
            _unzip.Completed += unzip_Completed;
            _unzip.ExtractAsync();
        }

        public void Cancel()
        {
            if (this.CancelButtonName == BUTTON_NAME_CANCEL)
            {
                _unzip.Cancel();
            }
            else
            {
                _navigationService.For<StartViewModel>().Navigate();
            }
        }

        public void CopyLogItem()
        {
            if (SelectedLogItem is UnzipLogItem)
            {
                UnzipLogItem item = (UnzipLogItem)SelectedLogItem;
                Clipboard.SetData(DataFormats.Text, item.FilePath);
                MessageBox.Show(Path.GetFileName(item.FilePath) + "のパスをクリップボードにコピーしました。");
            }
        }

        private void unzip_Loaded(object sender, int fileCount)
        {
            this.ProgressValue = 0;
            this.ProgressMax = fileCount;
        }

        private void unzip_ProgressChanged(object sender, UnzipLogItem logItem)
        {
            Debug.WriteLine(logItem);
            this.ProgressValue = this.ProgressValue + 1;
            ProgressItemList.Add(logItem);
        }

        private void unzip_Completed(object sender, UnzipCompletedStatus result)
        {
            switch (result)
            {
                case UnzipCompletedStatus.Completed:
                    MessageBox.Show("すべてのファイル処理が完了しました");
                    break;
                case UnzipCompletedStatus.Untreated:
                    MessageBox.Show("処理を行うファイルがありません");
                    break;
                case UnzipCompletedStatus.Cancel:
                    MessageBox.Show("キャンセルされました");
                    break;
            }
            this.CancelButtonName = BUTTON_NAME_BACK;
        }
    }
}
