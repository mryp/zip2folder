using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zip2Folder.Models
{
    public class UnzipLogItem
    {
        public string FilePath
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public bool IsSuccess
        {
            get;
            set;
        }

        public UnzipLogItem()
        {
        }

        public override string ToString()
        {
            string isSuccessText = IsSuccess ? "OK" : "NG";
            string fileName = Path.GetFileName(FilePath);
            string message = IsSuccess ? "" : " " + Message;
            return $"[{isSuccessText}]{message} {fileName}";
        }
    }
}
