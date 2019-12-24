using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace PZ3_NetworkService.ViewModel
{
    public class ReportViewModel : BindableBase
    {
        private string _errorMessage = "";

        public MyICommand ShowReportCommand { get; set; }
        public string ReportShow { get; set; }
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }
        public ReportViewModel()
        {
            ShowReportCommand = new MyICommand(ShowReport);
        }

        private void ShowReport()
        {
            try
            {
                DateTime startDate;
                DateTime endDate;
                var reports = new SortedDictionary<string, string>();
                ReportShow = "";

                string[] startDat = StartDate.Split('.');
                string[] endDat = EndDate.Split('.');

                #region checking combobox
                if (startDat.Length != 3)
                {
                    ErrorMessage = "Nepravilan pocetni datum. Format je DD.MM.GGGG";
                    return;
                }
                if (endDat.Length != 3)
                {
                    ErrorMessage = "Nepravilan krajnji datum. Format je DD.MM.GGGG";
                    return;
                }

                if (!int.TryParse(startDat[0], out int startD))
                {
                    ErrorMessage = "Dan mora da bude broj";
                    return;
                }
                if (!int.TryParse(startDat[1], out int startM))
                {
                    ErrorMessage = "Mesec mora da bude broj";
                    return;
                }
                if (!int.TryParse(startDat[2], out int startY))
                {
                    ErrorMessage = "Godina mora da bude broj";
                    return;
                }

                if (!int.TryParse(endDat[0], out int endD))
                {
                    ErrorMessage = "Dan mora da bude broj";
                    return;
                }
                if (!int.TryParse(endDat[1], out int endM))
                {
                    ErrorMessage = "Mesec mora da bude broj";
                    return;
                }
                if (!int.TryParse(endDat[2], out int endY))
                {
                    ErrorMessage = "Godina mora da bude broj";
                    return;
                }

                try
                {
                    startDate = new DateTime(startY, startM, startD);
                }
                catch (Exception)
                {
                    ErrorMessage = "Nepostojeci pocetni datum";
                    return;
                }

                try
                {
                    endDate = new DateTime(endY, endM, endD);
                }
                catch (Exception)
                {
                    ErrorMessage = "Nepostojeci krajnji datum";
                    return;
                }

                if (startDate > endDate)
                {
                    ErrorMessage = "Krajnji datum je manji od pocetnog datuma";
                    return;
                }
                ErrorMessage = "";
                #endregion


                using (var sr = new StreamReader("Log.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        string temp = sr.ReadLine();
                        string[] vals = temp.Split(',');
                        var date = Convert.ToDateTime(vals[0]);
                        if (date >= startDate && date <= endDate)
                        {
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
                }

                ReportShow = "REPORT:" + Environment.NewLine;
                foreach (var item in reports)
                {
                    ReportShow += item.Key + ":" + Environment.NewLine + item.Value;
                    OnPropertyChanged("ReportShow");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }

}
