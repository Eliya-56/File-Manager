using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileManager
{
    class MessageBox : OpenBox
    {

        public WindowTheme MainTheme { get; set; }

        public MessageBox(int width, int height) : base(width, height, Console.WindowHeight / 2 - height / 2, Console.WindowWidth / 2 - width / 2)
        {
            MainTheme = new WindowTheme(ConsoleColor.DarkGray, ConsoleColor.Yellow);
        }
        public virtual void ShowMessage(string message)
        {
            DrawField(MainTheme);
            SetCursorStart();
            WriteText(message);
        }

        public virtual void ShowMessageLock(string message)
        {
            ShowMessage(message);
            CheckKeyPress(ConsoleKey.Enter);
            CloseMessage();
        }

        
        public void ShowMessageLock(Dictionary<string, string> dictionary, string header = null)
        {
            ShowMessage(dictionary, header);
            CheckKeyPress(ConsoleKey.Enter);
            CloseMessage();
        }

        public void ShowMessage(Dictionary<string, string> dictionary, string header = null)
        {
            DrawField(MainTheme);
            SetCursorStart();
            if (header != null)
            {
                Console.Write(header);
                NewLine();
            }
            foreach (var item in dictionary)
            {
                NewLine();
                DrawDictionaryLine(item);
            }
        }

        public virtual void ShowMessageLock(string[] messages, string header = null)
        {
            ShowMessage(messages, header);
            CheckKeyPress(ConsoleKey.Enter);
            CloseMessage();
        }

        public virtual void ShowMessage(string[] messages, string header = null)
        {
            DrawField(MainTheme);
            SetCursorStart();
            if (header != null)
            {
                Console.Write(header);
                NewLine();
            }
            var counter = 0;
            foreach (var message in messages)
            {
                var str = message;
                if (str.Length > width - 3)
                    str = message.Substring(0, width - 3);
                Console.Write(str);
                NewLine();
                counter++;
                if (counter >= height)
                    break;
            }
        }

        private void DrawDictionaryLine(KeyValuePair<string, string> pair)
        {
            var keyStr = pair.Key;
            var valueStr = pair.Value;
            if (pair.Key.Length > width / 2 - horizontalBorder)
                keyStr = keyStr.Remove(width / 2 - horizontalBorder - 3) + "..";
            if (pair.Value.Length > width / 2 - horizontalBorder)
                valueStr = valueStr.Remove(width / 2 - horizontalBorder - 3) + "..";

            StartString();
            Console.Write(keyStr);
            Console.CursorLeft = startHorizontal + width / 2 - horizontalBorder;
            Console.Write(valueStr);
        }
  
    }
}
