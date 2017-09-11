using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileManager
{
    class OpenBox
    {
        public class WindowTheme
        {
            public ConsoleColor BackgroundColor { get; set; }
            public ConsoleColor Foreground { get; set; }

            public WindowTheme(ConsoleColor BackgroundColor, ConsoleColor Foreground)
            {
                this.BackgroundColor = BackgroundColor;
                this.Foreground = Foreground;
            }

            static public void SetTheme(WindowTheme Theme)
            {
                Console.BackgroundColor = Theme.BackgroundColor;
                Console.ForegroundColor = Theme.Foreground;
            }
        }
        
        protected int height;
        protected int width;
        protected readonly int startVertical;
        protected readonly int startHorizontal;
        protected readonly int verticalBorder = 1;
        protected readonly int horizontalBorder = 2;

        public OpenBox(int width, int height, int startVertical, int startHorizontal)
        {
            this.width = width;
            this.height = height;
            this.startVertical = startVertical;
            this.startHorizontal = startHorizontal;
        }

        protected void DrawField(WindowTheme Theme)
        {
            Console.CursorLeft = startHorizontal;
            Console.CursorTop = startVertical;
            WindowTheme.SetTheme(Theme);
            for (int i = 0; i < height; i++)
            {
                if (!(i == 0 || i == height - 1))
                    Console.Write("||");
                else
                    Console.Write("==");
                foreach (var element in Enumerable.Repeat(' ', width - 2 * horizontalBorder))
                {
                    if (i == 0 || i == height - 1)
                        Console.Write('=');
                    else
                        Console.Write(element);
                }
                if (!(i == 0 || i == height - 1))
                {
                    Console.Write("||");
                }
                else
                    Console.Write("==");
                Console.WriteLine();
                Console.CursorLeft = startHorizontal;
            }

        }

        public void CloseMessage()
        {
            Console.ResetColor();
            Console.CursorLeft = startHorizontal;
            Console.CursorTop = startVertical;
            for (int i = 0; i < height; i++)
            {
                foreach (var element in Enumerable.Repeat(' ', width))
                {
                    Console.Write(element);
                }
                Console.WriteLine();
                Console.CursorLeft = startHorizontal;
            }
        }

        protected void SetCursorStart()
        {
            Console.CursorTop = startVertical + verticalBorder;
            StartString();
        }

        protected void NewLine()
        {
            Console.WriteLine();
            StartString();
        }

        protected void StartString()
        {
            Console.CursorLeft = startHorizontal + horizontalBorder;
        }

        public void CheckKeyPress(ConsoleKey Key)
        {
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == Key)
                    break;
            }
        }

        protected void WriteText(string text)
        {
            foreach (var symbol in text)
            {
                if (symbol == '\r')
                {
                    StartString();
                    continue;
                }
                if (symbol == '\n')
                {
                    NewLine();
                    continue;
                }
                Console.Write(symbol);
                if (Console.CursorLeft == startHorizontal + (width - 2))
                    NewLine();
            }
        }

    }
}
