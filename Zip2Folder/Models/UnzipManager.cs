using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zip2Folder.Models
{
    delegate void UnzipLoadedEventHandler(object sender, int fileCount);
    delegate void UnzipProgressChangedEventHandler(object sender, UnzipLogItem logItem);
    delegate void UnzipCompletedEventHandler(object sender, UnzipCompletedStatus result);

    enum UnzipCompletedStatus
    {
        Completed,
        Untreated,
        Cancel,
    }

    /// <summary>
    /// 指定したフォルダ内にZIPファイルを展開する
    /// </summary>
    class UnzipManager
    {
        private const string SEARCH_PATTERN_ZIP = "*.zip";
        private string _dirPath;
        private bool _isCancel = false;
        private Task _extractTask;

        public event UnzipLoadedEventHandler Loaded;
        public event UnzipProgressChangedEventHandler ProgressChanged;
        public event UnzipCompletedEventHandler Completed;

        public UnzipManager(string dirPath)
        {
            _dirPath = dirPath;
        }

        public void ExtractAsync()
        {
            if (_extractTask != null)
            {
                if (!_extractTask.IsCompleted)
                {
                    return;
                }
            }

            _isCancel = false;
            _extractTask = Task.Run(() =>
            {
                if (!Directory.Exists(_dirPath))
                {
                    Completed?.Invoke(this, UnzipCompletedStatus.Untreated);
                    return;
                }
                string[] files = Directory.GetFiles(_dirPath, SEARCH_PATTERN_ZIP);
                if (files.Length == 0)
                {
                    Completed?.Invoke(this, UnzipCompletedStatus.Untreated);
                    return;
                }

                Loaded?.Invoke(this, files.Length);
                foreach (string file in files)
                {
                    if (_isCancel)
                    {
                        Completed?.Invoke(this, UnzipCompletedStatus.Cancel);
                        return;
                    }
                    UnzipLogItem logItem = unzipFile(file);
                    ProgressChanged?.Invoke(this, logItem);
                }

                Completed?.Invoke(this, UnzipCompletedStatus.Completed);
            });
        }

        public void Cancel()
        {
            _isCancel = true;
        }

        private UnzipLogItem unzipFile(string zipPath)
        {
            string outputPath = Path.Combine(Path.GetDirectoryName(zipPath),
                Path.GetFileNameWithoutExtension(zipPath));
            if (Directory.Exists(outputPath))
            {
                return new UnzipLogItem()
                {
                    FilePath = zipPath,
                    Message = "展開先に既にフォルダが存在しています",
                    IsSuccess = false,
                };
            }

            try
            {
                Directory.CreateDirectory(outputPath);
                ZipFile.ExtractToDirectory(zipPath, outputPath);
            }
            catch (Exception e)
            {
                return new UnzipLogItem()
                {
                    FilePath = zipPath,
                    Message = "例外発生" + e.Message,
                    IsSuccess = false,
                };
            }

            return new UnzipLogItem()
            {
                FilePath = zipPath,
                Message = "成功",
                IsSuccess = true,
            };
        }
    }
}
