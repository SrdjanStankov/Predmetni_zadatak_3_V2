namespace PZ3_NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand ReportCommand { get; set; }
        public MyICommand NetworkDataCommand { get; set; }
        public MyICommand DataChartCommand { get; set; }
        public MyICommand NetworkViewCommand { get; set; }
        public MyICommand TutorialNetworkDataCommand { get; set; }
        public MyICommand TutorialReportCommand { get; set; }
        public MyICommand TutorialGraphCommand { get; set; }
        public MyICommand TutorialNetworkViewCommand { get; set; }

        private BindableBase currentViewModel;

        private ReportViewModel reportViewModel = new ReportViewModel();
        private NetworkDataViewModel networkDataViewModel = new NetworkDataViewModel();
        private DataChartViewModel dataChartViewModel = new DataChartViewModel();
        private NetworViewViewModel networViewViewModel = new NetworViewViewModel();


        public BindableBase CurrentViewModel
        {
            get => currentViewModel;

            set => SetProperty(ref currentViewModel, value);
        }


        public MainWindowViewModel()
        {
            CurrentViewModel = networkDataViewModel;
            ReportCommand = new MyICommand(OnReport);
            NetworkDataCommand = new MyICommand(OnNetworkData);
            DataChartCommand = new MyICommand(OnDataChart);
            NetworkViewCommand = new MyICommand(OnNetworkView);
        }

        private void OnNetworkView()
        {
            CurrentViewModel = networViewViewModel;
        }

        private void OnDataChart()
        {
            CurrentViewModel = dataChartViewModel;
        }

        private void OnNetworkData()
        {
            CurrentViewModel = networkDataViewModel;
        }

        private void OnReport()
        {
            CurrentViewModel = reportViewModel;
        }
    }
}
