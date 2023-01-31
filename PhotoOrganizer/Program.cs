using PhotoOrganizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var files = new List<FileInfo>();
            if (args.Length < 2)
            {
                throw new ArgumentException("No path specified.");
            }
            var sourcePath = args[0];
            var targetPath = args[1];

            if (!Directory.Exists(sourcePath))
            {
                Console.Write("Source path does not exist. Skipping: " + sourcePath);
                return;
            }
            if (!Directory.Exists(targetPath))
            {
                Console.Write("Target path does not exist. Skipping: " + targetPath);
                return;
            }

            var directoryInfo = new DirectoryInfo(sourcePath);

            Console.WriteLine("Checking folder: " + directoryInfo.Name);

            // "*.*" will only get files with extensions. But "*" will get every file.
            // And if SearchOption.AllDirectories wasn't used, it wouldn't be recursive scan and it'd only get the files in the target path, with not including files in the child directories.
            files.AddRange(directoryInfo.GetFiles("*", SearchOption.AllDirectories));

            files.Select(f => new PhotoFileInfo(f)).ToList().ForEach(f =>
            {
                var targetFilePath = Path.Combine(targetPath, f.DateTaken.Year.ToString(), f.DateTaken.ToString("yyyyMMdd"), f.Name);
                if (f.Extension.Equals(".CR2", StringComparison.InvariantCulture))
                {
                    targetFilePath = Path.Combine(targetPath, "Raw", f.DateTaken.Year.ToString(), f.DateTaken.ToString("yyyy_MM_dd"), f.Name);
                }
                else if (!f.Extension.Equals(".MOV", StringComparison.InvariantCulture) && !f.IsPhoto)
                {
                    targetFilePath = Path.Combine(targetPath, "NoPhotos", f.DateTaken.Year.ToString(), f.DateTaken.ToString("yyyyMMdd"), f.Name);
                }

                var targetDir = Path.GetDirectoryName(targetFilePath);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                var uniqueNo = 1;
                while (File.Exists(targetFilePath))
                {
                    var newFileName = (uniqueNo > 1)
                        ? Path.GetFileNameWithoutExtension(targetFilePath).Replace($"_{uniqueNo - 1}", $"_{uniqueNo}") + Path.GetExtension(targetFilePath)
                        : Path.GetFileNameWithoutExtension(targetFilePath) + $"_{uniqueNo}" + Path.GetExtension(targetFilePath);
                    targetFilePath = Path.Combine(targetDir, newFileName);
                    uniqueNo++;
                }

                File.Move(f.FullName, targetFilePath);
            });

        }
    }
}
