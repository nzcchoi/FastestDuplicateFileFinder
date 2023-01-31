using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace PhotoOrganizer.Models
{
    internal class PhotoFileInfo
    {
        private static Regex _r = new Regex(":");
        private FileInfo _fileInfo;

        public PhotoFileInfo(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public DateTime DateTaken
        {
            get
            {
                return GetDateTaken();
            }
        }

        public string FullName
        {
            get
            {
                return _fileInfo.FullName;
            }
        }

        public string Name
        {
            get
            {
                return _fileInfo.Name;
            }
        }

        public string Extension
        {
            get
            {
                return _fileInfo.Extension;
            }
        }

        public Boolean IsPhoto { get; set; }


        private DateTime GetDateTaken()
        {
            try
            {
                using (FileStream fs = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = _r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    IsPhoto = true;
                    return DateTime.Parse(dateTaken);
                }
            }
            catch
            {
                return new[] { _fileInfo.CreationTime, _fileInfo.LastWriteTime, _fileInfo.LastAccessTime }.Min();
            }
        }

    }
}
