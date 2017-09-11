using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MyFileManager.OpenBox;

namespace MyFileManager
{
    class FileManagerView
    {
        private List<DirectoryView> windows;
        private WindowTheme ActiveTheme;
        private WindowTheme ActiveHLTheme;
        private WindowTheme PassiveTheme;
        private WindowTheme PassiveHLTheme;
        private DirectoryInfo buffer;
        private bool copyFlag = false;

        public FileManagerView(DirectoryInfo Dir1, DirectoryInfo Dir2)
        {
            windows = new List<DirectoryView>();
            windows.Add(new DirectoryView(Dir1, 0, 1, true));
            windows.Add(new DirectoryView(Dir2, 0, Console.BufferWidth / 2, false));
            ActiveTheme = new WindowTheme(ConsoleColor.White, ConsoleColor.Black);
            ActiveHLTheme = new WindowTheme(ConsoleColor.Black, ConsoleColor.White);
            PassiveTheme = new WindowTheme(ConsoleColor.Gray, ConsoleColor.DarkGray);
            PassiveHLTheme = new WindowTheme(ConsoleColor.DarkGray, ConsoleColor.Gray);
        }

        public void Work()
        {
            DrawWindow();
            DrawHelpString();
            while (true)
            {
                ConsoleKeyInfo button = new ConsoleKeyInfo();
                while (Console.KeyAvailable)
                    button = Console.ReadKey(true);
                switch (button.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                        ChangeActive();
                        break;
                    case ConsoleKey.UpArrow:
                        Up();
                        break;
                    case ConsoleKey.DownArrow:
                        Down();
                        break;
                    case ConsoleKey.Enter:
                        Enter();
                        break;
                    case ConsoleKey.Backspace:
                        Out();
                        break;
                    case ConsoleKey.F1:
                        Cut();
                        break;
                    case ConsoleKey.F2:
                        Copy();
                        break;
                    case ConsoleKey.F3:
                        Insert();
                        break;
                    case ConsoleKey.F4:
                        DeleteItem();
                        break;
                    case ConsoleKey.F5:
                        PrintDriversInfo();
                        break;
                    case ConsoleKey.F6:
                        ShowProperties();
                        break;
                    case ConsoleKey.F7:
                        CreateDirectory();
                        break;
                    case ConsoleKey.F8:
                        CreateFile();
                        break;
                    case ConsoleKey.F9:
                        ChangeName();
                        break;
                    case ConsoleKey.F10:
                        SelectPath();
                        break;
                    case ConsoleKey.F:
                        Search();
                        break;
                }
                DrawHelpString();
            }
        }

        private void DrawHelpString()
        {
            Console.CursorTop = Console.BufferHeight - 3;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("F1 - cut | F2  - copy | F3 - insert | F4 - delete | F5 - logical disks | F6 - propeties | F7 - create directory             F8 - create file | F9 - change name | F10 - select path | F - search");
        }

        private void ShowProperties()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            foreach (var window in windows)
                if(window.IsActive)
                {
                    properties.Add("Name", window.ChosenDirectory.Name);
                    properties.Add("Parent", window.ChosenDirectory.Parent.FullName);
                    properties.Add("Root", window.ChosenDirectory.Root.FullName);
                    properties.Add("Last Write Time", window.ChosenDirectory.LastWriteTime.ToString());
                    properties.Add("Last Access Time", window.ChosenDirectory.LastAccessTime.ToString());
                    if(Directory.Exists(window.ChosenDirectory.FullName))
                    { 
                        properties.Add("Folders", window.ChosenDirectory.EnumerateDirectories().Count().ToString());
                        properties.Add("Files", window.ChosenDirectory.EnumerateFiles().Count().ToString());
                    }
                    else if(File.Exists(window.ChosenDirectory.FullName))
                    {
                        properties.Add("IsReadOnly", new FileInfo(window.ChosenDirectory.FullName).IsReadOnly.ToString());
                    }
                    properties.Add("Size (Bytes)", "");
                    var message = new MessageBox(90, 15);
                    message.ShowMessage(properties, "Properties");
                    Thread getSize = new Thread(() => window.ChosenDirectory.GetSizeInBytes(Console.CursorLeft, 0));
                    getSize.Start();
                    message.CheckKeyPress(ConsoleKey.Enter);
                    getSize.Abort();
                    message.CloseMessage();
                }
            DrawWindow();
        }

        private void Search()
        {
            try
            {
                foreach (var window in windows)
                    if (window.IsActive)
                    {
                        string path = (new InputTextField(100, 8).GetText("Try something to find:"));
                        if (path == null)
                        {
                            DrawWindow();
                            return;
                        }
                        DrawWindow();
                        List<string> results = SearchOrBreak(ConsoleKey.Escape, window.CurrentDirectory, path);
                        ShowSearchResults(results, window, path);
                    }
            }
            catch (Exception e)
            {
                DrawWindow();
                var message = new MessageBox(50, 15);
                message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                message.ShowMessageLock(e.Message);
            }
            DrawWindow();
        }

        private void ShowSearchResults(List<string> results, DirectoryView activeWindow, string searchQuery)
        {
            var win = new Window(100, 35, 3, Console.BufferWidth / 10);
            if (results is null)
                return;
            else if (results.Count() == 0)
            {
                new MessageBox(50, 10).ShowMessageLock(Environment.NewLine + Environment.NewLine + Environment.NewLine + "  Nothig \"" + searchQuery + "\" in here");
            }
            else
            {
                var numStr = 0;
                var mainTheme = new WindowTheme(ConsoleColor.DarkGreen, ConsoleColor.Yellow);
                var hlTheme = new WindowTheme(ConsoleColor.Yellow, ConsoleColor.DarkGreen);
                win.DrawWindow("Results of search queue(F1 - cut, F2 - copy, Enter - run): " + searchQuery, results, mainTheme, hlTheme, numStr);
                while (true)
                {
                    ConsoleKeyInfo button = new ConsoleKeyInfo();
                    bool IsOut = false;
                    while (Console.KeyAvailable)
                        button = Console.ReadKey(true);

                    switch (button.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (!(numStr <= 0))
                            {
                                numStr--;
                                win.DrawWindow("Results of search queue(F1 - cut, F2 - copy, Enter - run): " + searchQuery, results, mainTheme, hlTheme, numStr);
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (!(numStr >= win.MaxItems - 1 || numStr >= results.Count() - 1))
                            {
                                numStr++;
                                win.DrawWindow("Results of search queue(F1 - cut, F2 - copy, Enter - run): " + searchQuery, results, mainTheme, hlTheme, numStr);
                            }
                            break;
                        case ConsoleKey.Enter:
                            if (File.Exists(results[numStr]))
                                Process.Start(results[numStr]);
                            else if (Directory.Exists(results[numStr]))
                            {
                                activeWindow.CurrentDirectory = new DirectoryInfo(results[numStr]);
                                IsOut = true;
                            }
                            break;
                        case ConsoleKey.F1:
                            buffer = new DirectoryInfo(results[numStr]);
                            copyFlag = false;
                            break;
                        case ConsoleKey.F2:
                            buffer = new DirectoryInfo(results[numStr]);
                            copyFlag = true;
                            break;
                        case ConsoleKey.Escape:
                            IsOut = true;
                            break;
                    }
                    if (IsOut)
                        break;
                }
                win.CloseMessage();
            }
        }

        private List<string> SearchOrBreak(ConsoleKey BreakKey, DirectoryInfo targetDirectory, string searchQuery)
        {
            var searchMessage = new MessageBox(50, 6);
            searchMessage.ShowMessage(Environment.NewLine + "Searching.." + Environment.NewLine + "Press " + BreakKey + " to break");
            List<string> results = new List<string>();
            bool IsSearching = true;
            object syncObject = new object();
            Thread searching = new Thread(() => { results = targetDirectory.Search(searchQuery).Select(x => x.FullName).ToList(); lock (syncObject) { IsSearching = false; } });
            searching.Start();
            while (IsSearching)
            {
                ConsoleKeyInfo key = new ConsoleKeyInfo();
                while (Console.KeyAvailable)
                    key = Console.ReadKey(true);
                if(key.Key == BreakKey)
                { 
                   searching.Abort();
                   searchMessage.CloseMessage();
                   DrawWindow();
                   return null;
                }
            }
            DrawWindow();
            return results;
        }
        
        private void SelectPath()
        {
            try
            {
                foreach (var window in windows)
                    if (window.IsActive)
                    {
                        string path = (new InputTextField(100, 8).GetText("Select directory:", startText: window.CurrentDirectory.FullName));
                        if (path == null)
                        {
                            DrawWindow();
                            return;
                        }
                        window.SelectDirectory(path);
                    }
            }
            catch (Exception e)
            {
                DrawWindow();
                var message = new MessageBox(50, 15);
                message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                message.ShowMessageLock(e.Message);
            }
            DrawWindow();

        }

        private void ChangeName()
        {
            try
            {
                foreach (var window in windows)
                    if(window.IsActive)
                    {
                        string path = (new InputTextField(50, 8).GetText("Type new name:", startText:window.ChosenDirectory.Name));
                        if (path == null)
                        {
                            DrawWindow();
                            return;
                        }
                        if (File.Exists(window.ChosenDirectory.FullName))
                            File.Move((window.ChosenDirectory.FullName), window.CurrentDirectory.FullName + Path.AltDirectorySeparatorChar + path);
                        else if (Directory.Exists(window.ChosenDirectory.FullName))
                            Directory.Move((window.ChosenDirectory.FullName), window.CurrentDirectory.FullName + Path.AltDirectorySeparatorChar + path);
                    }
            }
            catch (Exception e)
            {
                DrawWindow();
                var message = new MessageBox(50, 15);
                message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                message.ShowMessageLock(e.Message);
            }
            DrawWindow();
        }

        private void Cut()
        {
            foreach (var window in windows)
                if (window.IsActive)
                {
                    buffer = window.ChosenDirectory;
                    copyFlag = false;
                }
        }

        private void Copy()
        {
            foreach (var window in windows)
                if (window.IsActive)
                {
                    buffer = window.ChosenDirectory;
                    copyFlag = true;
                }

        }

        private void Insert()
        {
            try
            {
                foreach (var window in windows)
                    if (window.IsActive)
                    {
                        if (!copyFlag)
                            buffer.MoveTo(window.CurrentDirectory.FullName + Path.AltDirectorySeparatorChar + buffer.Name);
                        else
                            buffer.CopyDirectory(window.CurrentDirectory.FullName + Path.AltDirectorySeparatorChar + buffer.Name);
                    }
            }
            catch (Exception e)
            {
                DrawWindow();
                var message = new MessageBox(50, 15);
                message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                message.ShowMessageLock(e.Message);
            }
            DrawWindow();
        }

        private void DeleteItem()
        {
            try
            {
                foreach (var window in windows)
                {
                    if (File.Exists(window.ChosenDirectory.FullName))
                        File.Delete((window.ChosenDirectory.FullName));
                    else
                        window.ChosenDirectory.Delete(true);
                    window.ResetView(ActiveTheme, ActiveHLTheme);
                }
            }
            catch(Exception e)
            {
                DrawWindow();
                var message = new MessageBox(50, 15);
                message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                message.ShowMessageLock("Can't delete:" + Environment.NewLine + e.Message);
            }
            DrawWindow();
        }

        private void CreateFile()
        {
            var name = new InputTextField(50, 8).GetText("Enter name of new file");
            if (name == null)
            {
                DrawWindow();
                return;
            }
            foreach (var window in windows)
                if (window.IsActive)
                {
                    try
                    {
                        using (File.Create(window.CurrentDirectory + Path.AltDirectorySeparatorChar.ToString() + name));
                    }
                    catch (Exception e)
                    {
                        DrawWindow();
                        var message = new MessageBox(50, 15);
                        message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                        message.ShowMessageLock("Can't create file:" + Environment.NewLine + e.Message);
                    }
                }
            DrawWindow();
        }

        private void CreateDirectory()
        {
            var name = new InputTextField(50, 8).GetText("Enter name of new directory");
            if (name == null)
            {
                DrawWindow();
                return;
            }
            foreach (var window in windows)
                if (window.IsActive)
                {
                    try
                    {
                        Directory.CreateDirectory(window.CurrentDirectory + Path.AltDirectorySeparatorChar.ToString() + name);
                    }
                    catch (Exception e)
                    {
                        DrawWindow();
                        var message = new MessageBox(50, 15);
                        message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                        message.ShowMessageLock("Can't create directory:" + Environment.NewLine + e.Message);
                    }
                }
            DrawWindow();

        }

        private void PrintDriversInfo()
        {
            List<string> drivers = new List<string>();
            drivers.Add("List of logical drivers:");
            foreach (var driver in Directory.GetLogicalDrives())
            {
                drivers.Add("   " + driver);
            }
            var message = new MessageBox(50, 15);
            message.MainTheme = new WindowTheme(ConsoleColor.DarkRed, ConsoleColor.White);
            message.ShowMessageLock(drivers.ToArray());
            DrawWindow();
        }

        private void DrawWindow()
        {
            try
            {
                foreach (var win in windows)
                {
                    if (win.IsActive)
                        win.DrawDirectoryView(ActiveTheme, ActiveHLTheme);
                    else
                        win.DrawDirectoryView(PassiveTheme, PassiveHLTheme);
                }
            }
            catch (Exception e)
            {
                var message = new MessageBox(50, 15);
                message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                message.ShowMessageLock(e.Message);
                foreach (var win in windows)
                {
                    if (win.IsActive)
                        win.OutDirectory();
                }
                DrawWindow();
            }

        }

        private void ChangeActive()
        {
            foreach (var window in windows)
            {
                window.IsActive = !window.IsActive;
            }
            DrawWindow();
        }

        private void Up()
        {
            foreach (var window in windows)
                if (window.IsActive)
                    window.Up();
            DrawWindow();
        }

        private void Down()
        {
            foreach (var window in windows)
                if (window.IsActive)
                    window.Down();
            DrawWindow();
        }

        private void Enter()
        {
            try
            {
                foreach (var window in windows)
                    if (window.IsActive)
                        window.Enter();
            }
            catch (Exception e)
            {
                DrawWindow();
                var message = new MessageBox(50, 15);
                message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                message.ShowMessageLock(e.Message);
            }
            DrawWindow();
        }

        private void Out()
        {
            try
            {
                foreach (var window in windows)
                    if (window.IsActive)
                        window.OutDirectory();
            }
            catch (Exception e)
            {
                DrawWindow();
                var message = new MessageBox(50, 15);
                message.MainTheme = new WindowTheme(ConsoleColor.Red, ConsoleColor.Yellow);
                message.ShowMessageLock(e.Message);
            }
            DrawWindow();
        }
        


    }
}
