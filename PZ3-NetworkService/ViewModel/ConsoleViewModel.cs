using PZ3_NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PZ3_NetworkService.ViewModel
{
    public class ConsoleViewModel : BindableBase
    {
        private const string DefaultTerminalString = ">>>";
        private string consoleText = DefaultTerminalString;
        private Dictionary<string, Action> commands = new Dictionary<string, Action>();
        private TextBox tb;

        public string ConsoleText
        {
            get => consoleText;
            set => SetProperty(ref consoleText, value);
        }

        public MyICommand<TextBox> EnterPressedCommand { get; set; }

        public ConsoleViewModel()
        {
            EnterPressedCommand = new MyICommand<TextBox>(OnEnter);
            commands.Add("add", AddCmd);
        }

        private void AddCmd()
        {
            WriteNewCommandLine();

        }

        private void OnEnter(TextBox tb)
        {
            if (this.tb is null)
            {
                this.tb = tb;
            }
            string command = ConsoleText.Split(new string[] { DefaultTerminalString }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (command != null)
            {
                command = command.ToLower();
            }
            else
            {
                WriteNewCommandLine();
                return;
            }
            if (!commands.ContainsKey(command))
            {
                WriteNewCommandLine();
                return;
            }

            var cmd = commands.Select(item=>item.Value).FirstOrDefault();
            cmd.Invoke();
        }

        private void WriteNewCommandLine()
        {
            ConsoleText += $"{Environment.NewLine}{DefaultTerminalString}";
            tb.CaretIndex = ConsoleText.Length;
        }
    }
}
