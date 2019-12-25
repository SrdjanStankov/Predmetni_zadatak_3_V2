using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using PZ3_NetworkService.Model;

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
            commands.Add("remove", RemoveCmd);
            commands.Add("delete", RemoveCmd);
            commands.Add("del", RemoveCmd);
            commands.Add("rem", RemoveCmd);
            commands.Add("filter", FilterCmd);
            commands.Add("report", ReportCmd);
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
                WriteNewCommandLineWithMessage("Unknown command");
                return;
            }

            var cmd = commands.Where(item => item.Key == command).Select(item => item.Value).FirstOrDefault();
            cmd.Invoke(parameters);
        }

        private void AddCmd(List<string> parameters)
        {
            if (parameters.Count != 3)
            {
                WriteNewCommandLineWithMessage("Usage: add id name ip address");
                return;
            }

            #region Id Check

            if (!int.TryParse(parameters.First(), out int srvId))
            {
                WriteNewCommandLineWithMessage("index must be number greater than zero");
                return;
            }

            if (srvId < 1)
            {
                WriteNewCommandLineWithMessage("index must be number greater than zero");
                return;
            }

            #endregion

            #region Name Check

            if (string.IsNullOrEmpty(parameters[1]) || string.IsNullOrWhiteSpace(parameters[1]))
            {
                WriteNewCommandLineWithMessage("Name must be valid string");
                return;
            }

            #endregion

            #region Ip address check

            string[] ipSplitted = parameters[2].Split('.');
            if (ipSplitted.Length != 4)
            {
                WriteNewCommandLineWithMessage("Ip address format: 192.168.0.1");
                return;
            }

            foreach (string item in ipSplitted)
            {
                if (!int.TryParse(item, out int result))
                {
                    WriteNewCommandLineWithMessage("Ip address format: 192.168.0.1");
                    return;
                }
                if (result < 0 || result > 255)
                {
                    WriteNewCommandLineWithMessage("Ip address must be between 0 and 255, inclusive");
                    return;
                }
            }

            #endregion

            var srv = new Server() { Id = srvId, Name = parameters[1], IpAddress = parameters[2], ImgSrc = $"{Environment.CurrentDirectory}\\server_PNG20.png" };
            srv.Validate();
            if (srv.IsValid)
            {
                StaticClass.AddServerIfNotExist(srv);
                WriteNewCommandLineWithMessage("Server added");
                return;
            }

            #region Write validation errors

            string a = srv.ValidationErrors[nameof(srv.Id)];
            if (a != "")
            {
                WriteNewCommandLineWithMessage(a);
            }
            string b = srv.ValidationErrors[nameof(srv.Name)];
            if (b != "")
            {
                WriteNewCommandLineWithMessage(b);
            }
            string c = srv.ValidationErrors[nameof(srv.IpAddress)];
            if (c != "")
            {
                WriteNewCommandLineWithMessage(c);
            }

            #endregion
        }

        private void RemoveCmd(List<string> parameters)
        {
            if (parameters.Count != 1)
            {
                WriteNewCommandLineWithMessage("Usage: remove id");
                return;
            }

            #region Id Check

            if (!int.TryParse(parameters.First(), out int srvId))
            {
                WriteNewCommandLineWithMessage("index must be number greater than zero");
                return;
            }

            if (srvId < 1)
            {
                WriteNewCommandLineWithMessage("index must be number greater than zero");
                return;
            }

            #endregion

            if (StaticClass.DeleteServerIfExist(srvId))
            {
                WriteNewCommandLineWithMessage("Server removed");
                return;
            }
            WriteNewCommandLineWithMessage("Server does not exist");
        }

        private void FilterCmd(List<string> parameters)
        {
            if (parameters.Count != 3)
            {
                WriteNewCommandLineWithMessage("Usage: filter ipAddress/nan lt/gt/nan id");
                return;
            }

            string ipAddress;
            string idMode;
            int id;

            #region Ip address / nan validation

            if (parameters[0] != "nan")
            {
                string[] ipSplitted = parameters[0].Split('.');
                if (ipSplitted.Length != 4)
                {
                    WriteNewCommandLineWithMessage("Ip address format: 192.168.0.1");
                    return;
                }

                foreach (string item in ipSplitted)
                {
                    if (!int.TryParse(item, out int result))
                    {
                        WriteNewCommandLineWithMessage("Ip address format: 192.168.0.1");
                        return;
                    }
                    if (result < 0 || result > 255)
                    {
                        WriteNewCommandLineWithMessage("Ip address must be between 0 and 255, inclusive");
                        return;
                    }
                }

                ipAddress = parameters[0];
            }
            else
            {
                ipAddress = "nan";
            }

            #endregion

            #region Id mode validation

            switch (parameters[1].ToLower())
            {
                case "lt":
                case "gt":
                case "nan":
                    idMode = parameters[1].ToLower();
                    break;
                default:
                    WriteNewCommandLineWithMessage("Invalid second parameter. Available vaues are: lt, gt or nan");
                    return;
            }

            #endregion

            #region Id validation

            if (!int.TryParse(parameters[2], out int srvId))
            {
                WriteNewCommandLineWithMessage("index must be number greater than zero");
                return;
            }

            if (srvId < 1)
            {
                WriteNewCommandLineWithMessage("index must be number greater than zero");
                return;
            }

            id = srvId;

            #endregion

            StaticClass.FilterServers(ipAddress, idMode, id);
            WriteNewCommandLine();
        }

        private void ReportCmd(List<string> parameters)
        {
            var reports = new SortedDictionary<string, string>();
            using (var sr = new StreamReader("Log.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string temp = sr.ReadLine();
                    string[] vals = temp.Split(',');
                    string message = "\t" + vals[0] + " " + vals[1] + ", CHANGED STATE: " + vals[3] + Environment.NewLine;
                    if (!reports.ContainsKey(vals[2]))
                    {
                        reports.Add(vals[2], message);
                    }
                    else
                    {
                        reports[vals[2]] += message;
                    }
                }
            }

            string reportShow = "REPORT:" + Environment.NewLine;
            foreach (var item in reports)
            {
                reportShow += item.Key + ":" + Environment.NewLine + item.Value;
            }

            ReportViewModel.OnReport(reportShow);
            WriteNewCommandLineWithMessage("Report created");
        }

        private void WriteNewCommandLine()
        {
            ConsoleText += $"{Environment.NewLine}{DefaultTerminalString}";
            tb.CaretIndex = ConsoleText.Length;
        }

        private void WriteNewCommandLineWithMessage(string message)
        {
            ConsoleText += $"{Environment.NewLine}{message}";
            WriteNewCommandLine();
            tb.CaretIndex = ConsoleText.Length;
        }
    }
}
