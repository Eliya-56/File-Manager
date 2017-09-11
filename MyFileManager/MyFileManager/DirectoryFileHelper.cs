using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MyFileManager.OpenBox;
using System.Threading;

namespace MyFileManager
{
    static class DirectoryFileHelper
    {

        static public long GetSizeInBytes(this FileSystemInfo Path, int curosLeft , long startSize)
        {
            long size = 0;
            try
            {
                if (Directory.Exists(Path.FullName))
                {
                    foreach (var item in ((DirectoryInfo)Path).EnumerateFileSystemInfos())
                    {
                        size += item.GetSizeInBytes(curosLeft, size);
                        startSize = size;
                    }
                }
                else
                {
                    FileInfo file = new FileInfo(Path.FullName);
                    size += file.Length;
                    startSize = size;

                }

                if (size >= startSize)
                {
                    startSize = size;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.CursorLeft = curosLeft;
                    Console.Write(size);
                }
                return size;
            }
            catch
            {
                return size;
            }
        }

        static public void CopyDirectory(this FileSystemInfo directory, string DestinationPath)
        {
            if(Directory.Exists(directory.FullName))
            {
                Directory.CreateDirectory(DestinationPath);
                var newDirectory = new DirectoryInfo(DestinationPath);
                foreach (var entrie in ((DirectoryInfo)directory).EnumerateFileSystemInfos())
                    entrie.CopyDirectory(Path.Combine(newDirectory.FullName, entrie.Name));
            }
            else
            {
                File.Copy(directory.FullName, DestinationPath);
            }
        }

        static public List<FileSystemInfo> Search(this DirectoryInfo directory, string text)
        {
            List<FileSystemInfo> results = new List<FileSystemInfo>();
            try
            {
                foreach (var item in directory.EnumerateFileSystemInfos())
                { 
                    if ((item.FullName == text) || (item.Name == text))
                        results.Add(item);
                    if (item is DirectoryInfo)
                        results.AddRange(((DirectoryInfo)item).Search(text));
                }
            }
            catch
            {
            }
            return results;
        }
        
    }
}
