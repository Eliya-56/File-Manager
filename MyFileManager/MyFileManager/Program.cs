using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MyFileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(7 * (Console.LargestWindowWidth / 9) - 2, 8 * (Console.LargestWindowHeight / 9));
            Console.SetBufferSize(7 * (Console.LargestWindowWidth / 9) - 2, 8 * (Console.LargestWindowHeight / 9));
            Console.Title = "MyFileManager";
            Console.InputEncoding = Encoding.UTF8;
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;




            FileManagerView manager = new FileManagerView(new DirectoryInfo(Environment.GetLogicalDrives()[0]), new DirectoryInfo(Environment.GetLogicalDrives()[1]));
            manager.Work();


        }
    }
}
