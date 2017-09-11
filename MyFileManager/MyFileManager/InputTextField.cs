using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileManager
{
    
    class InputTextField : OpenBox
    {
        
        public WindowTheme MainTheme { get; set; }
        public WindowTheme TextFieldTheme { get; set; }

        public InputTextField(int width, int height) : base(width, height, Console.WindowHeight / 2 - height / 2, Console.WindowWidth / 2 - width / 2)
        {
            MainTheme = new WindowTheme(ConsoleColor.DarkCyan, ConsoleColor.Green);
            TextFieldTheme = new WindowTheme(ConsoleColor.DarkRed, ConsoleColor.White);
        }

        public string GetText(string info, string startText = "")
        {
            DrawField(MainTheme);
            SetCursorStart();
            NewLine();
            WriteText(info);
            NewLine();

            var output = ScanString(startText);
           
            CloseMessage();
            return output;
        }

        private void DrawInputField(string startText = "")
        {
            WindowTheme.SetTheme(TextFieldTheme);
            foreach (var item in Enumerable.Repeat(' ', width - 4))
            {
                Console.Write(item);
            }
            StartString();
            Console.Write(startText);
        }

        private string ScanString(string startText = "")
        {
            DrawInputField(startText);
            string output = startText;
            var counter = startText.Length;
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                    break;
                else if (key.Key == ConsoleKey.Backspace)
                {
                    var position = Console.CursorLeft;
                    if (position+1 <= startHorizontal + 2)
                    {
                        Console.Write(' ');
                        continue;
                    }
                    Console.Write(' ');
                    Console.CursorLeft = position;
                    output = output.Remove(output.Length - 1);
                    counter--;
                    continue;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                if (counter > width - 2 * horizontalBorder - 1)
                {
                    Console.CursorLeft = Console.CursorLeft - 1;
                    Console.Write(' ');
                    Console.CursorLeft = Console.CursorLeft - 1;
                    continue;
                }
                output += key.KeyChar;
                counter++;
            }
            return output;
        }
    }
}
