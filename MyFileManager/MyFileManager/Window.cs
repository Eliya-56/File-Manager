using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileManager
{
    class Window : OpenBox
    {
        public int MaxItems { get; }
        public int MaxStringLength { get; }

        public Window(int width, int height, int startVertical, int startHorizontal) : base(width, height, startVertical, startHorizontal)
        {
            MaxItems = height - 2 * verticalBorder - 2;
            MaxStringLength = width - 2 * horizontalBorder;
        }

        public void DrawWindow<T>(string header, IEnumerable<T> items, WindowTheme MainTheme, WindowTheme HLTheme, int NumHighlightString = -1)
        {
            WindowTheme.SetTheme(MainTheme);
            DrawField(MainTheme);
            SetCursorStart();
            PrintHeader(header);
            var count = 0;
            foreach (var item in items)
            {
                if (count == NumHighlightString)
                {
                    WindowTheme.SetTheme(HLTheme);
                    HighLigth(item.ToString());
                    WindowTheme.SetTheme(MainTheme);
                }
                else
                    Console.Write(FormatString(item.ToString()));
                NewLine();
                count++;
                if (count == MaxItems)
                    break;
            }
        }

        private void PrintHeader(string header)
        {
            SetCursorStart();
            Console.Write(FormatHeader(header));
            NewLine();
            for (int i = 0; i < width - 2 * horizontalBorder; i++)
            {
                Console.Write("-");
            }
            NewLine();
        }

        protected virtual string FormatString(string item)
        {
            if (item.Length >= width - 2 * horizontalBorder)
                item = item.Substring(0, width - 2 * horizontalBorder - 2) + "..";
            return item;
        }

        protected virtual string FormatHeader(string header)
        {
            return FormatString(header);
        }

        private void HighLigth(string item)
        {
            StartString();
            Console.Write(FormatString(item));
            for (int i = 0; i < MaxStringLength - item.Length; i++)
            {
                Console.Write(" ");
            }
        }
    }
}
