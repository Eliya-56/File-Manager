using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MyFileManager.OpenBox;
using System.Diagnostics;

namespace MyFileManager
{
    class DirectoryView
    {
        private Window view;
        public DirectoryInfo CurrentDirectory { get; set; }
        public DirectoryInfo ChosenDirectory { get; private set; }
        public int NumChosenDirectory { get; private set; }
        private int StartItem;
        public bool IsActive { get; set; }

        public DirectoryView(DirectoryInfo Dir,int startVertical, int startHorizontal, bool IsActive)
        {
            view = new Window(Console.BufferWidth / 2 - 2, Console.BufferHeight - 4, startVertical,startHorizontal);
            CurrentDirectory = Dir;
            ChosenDirectory = new DirectoryInfo(CurrentDirectory.GetFileSystemInfos().FirstOrDefault()?.ToString());
            StartItem = 0;
            this.IsActive = IsActive;
        }      
        
        public void DrawDirectoryView(WindowTheme MainTheme, WindowTheme HLTheme)
        {
            List<string> items = new List<string>();
            var counter = 0;
            foreach (var item in CurrentDirectory.EnumerateFileSystemInfos().Skip(StartItem).OrderBy(x => { if (x is DirectoryInfo) return 0; else return 1; }).ThenBy(y => y.Name))
            {
                if (counter == NumChosenDirectory)
                {
                    ChosenDirectory = new DirectoryInfo(item.FullName);
                }
                if (item is DirectoryInfo)
                    items.Add(item.Name + GetIndent(item.Name) + "<dir>");
                else
                    items.Add(item.Name);
                counter++;
            }
            view.DrawWindow(CurrentDirectory.FullName, items, MainTheme, HLTheme, NumChosenDirectory);
        }

        private string GetIndent(string item)
        {
            string output = "";
            if (!(item.Length >= view.MaxStringLength  - 7))
            {
                for (int i = 0; i < view.MaxStringLength - 9 - item.Length; i++)
                {
                    output += " ";
                }
            }
            return output;
        }

        public void Up()
        {
            if (NumChosenDirectory <= 0)
            {
                if (StartItem >= 0)
                    StartItem--;
            }
            else
                NumChosenDirectory--;
        }
        public void Down()
        {
            if (NumChosenDirectory >= view.MaxItems - 1 || NumChosenDirectory == CurrentDirectory.GetFileSystemInfos().Count() - 1)
            {
                if (CurrentDirectory.GetFileSystemInfos().Count() - StartItem > view.MaxItems)
                    StartItem++;
            }
            else
                NumChosenDirectory++;
        }
        public void Enter()
        {
            if (Directory.Exists(ChosenDirectory.FullName))
                EnterDirectory();
            else if (File.Exists(ChosenDirectory.FullName))
                RunFile();
        }
        public void OutDirectory()
        {
            if (CurrentDirectory.Parent != null)
                SelectDirectory(CurrentDirectory.Parent.FullName);
        }
        public void ResetView(WindowTheme MainTheme, WindowTheme HLTheme)
        {
            NumChosenDirectory = 0;
            StartItem = 0;
            DrawDirectoryView(MainTheme, HLTheme);
        }
        public void SelectDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                CurrentDirectory = new DirectoryInfo(path);
                StartItem = 0;
                NumChosenDirectory = 0;
            }
            else
                throw new Exception("Directory doesn't exist");
        }

        private void EnterDirectory()
        {
            SelectDirectory(ChosenDirectory.FullName);
        }
        private void SelectRunFile(string path)
        {
            Process.Start(path);
        }
        private void RunFile()
        {
            SelectRunFile(ChosenDirectory.FullName);
        }
    }
}
