using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace PZ3_NetworkService.ViewModel
{
    public class ConsoleViewModel : BindableBase
    {
        private const string DefaultTerminalString = ">>>";
        private string consoleText = DefaultTerminalString;
        private Dictionary<string, Action<List<string>>> commands = new Dictionary<string, Action<List<string>>>();
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

        private void AddCmd(List<string> parameters)
        {
            if (parameters.Count != 3)
            {
                WriteNewCommandLine();
                return;
            }
        }

        private void OnEnter(TextBox tb)
        {
            if (this.tb is null)
            {
                this.tb = tb;
            }
            string commandWithParameters = ConsoleText.Split(new string[] { DefaultTerminalString }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            string command;
            List<string> parameters;
            if (commandWithParameters != null)
            {
                string[] commandAndParameters = commandWithParameters.Split(' ');
                parameters = commandAndParameters.Skip(1).ToList();
                command = commandAndParameters.FirstOrDefault();
                if (command != null)
                {
                    command = command.ToLower();
                }
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

            var cmd = commands.Select(item => item.Value).FirstOrDefault();
            cmd.Invoke(parameters);
        }

        private void WriteNewCommandLine()
        {
            ConsoleText += $"{Environment.NewLine}{DefaultTerminalString}";
            tb.CaretIndex = ConsoleText.Length;
        }
    }
}
