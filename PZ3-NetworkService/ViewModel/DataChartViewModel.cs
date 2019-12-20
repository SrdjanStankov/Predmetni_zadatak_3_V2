using PZ3_NetworkService.Model;

using System.Collections.ObjectModel;
using System.Windows;

namespace PZ3_NetworkService.ViewModel
{
    public class DataChartViewModel : BindableBase
    {
        public MyICommand ShowButtonCommand { get; set; }
        public ObservableCollection<MyRect> Rectangles { get; set; }
        public Visibility Vis { get; set; } = Visibility.Visible;
        public string ShowButtonText { get; set; } = "PRIKAZI";

        public DataChartViewModel()
        {
            ShowButtonCommand = new MyICommand(OnShow);
            Rectangles = StaticClass.Rectangles;
        }

        public void OnShow()
        {
            ShowButtonText = (ShowButtonText == "PRIKAZI") ? "SAKRIJ" : "PRIKAZI";
            OnPropertyChanged("ShowButtonText");
            Vis = (Vis == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged("Vis");
        }
    }
}
