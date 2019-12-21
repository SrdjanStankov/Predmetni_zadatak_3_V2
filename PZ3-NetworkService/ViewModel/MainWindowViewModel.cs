namespace PZ3_NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand ReportCommand { get; set; }
        public MyICommand NetworkDataCommand { get; set; }
        public MyICommand DataChartCommand { get; set; }
        public MyICommand NetworkViewCommand { get; set; }

        private BindableBase currentViewModel;
        private BindableBase consoleViewModelProp;

        public BindableBase ConsoleViewModelPoop
        {
            get => consoleViewModelProp;
            set => SetProperty(ref consoleViewModelProp, value);
        }


        private ReportViewModel reportViewModel = new ReportViewModel();
        private NetworkDataViewModel networkDataViewModel = new NetworkDataViewModel();
        private DataChartViewModel dataChartViewModel = new DataChartViewModel();
        private NetworViewViewModel networViewViewModel = new NetworViewViewModel();
        private ConsoleViewModel consoleViewModel = new ConsoleViewModel();

        public BindableBase CurrentViewModel
        {
            get => currentViewModel;

            set => SetProperty(ref currentViewModel, value);
        }


        public MainWindowViewModel()
        {
            CurrentViewModel = networkDataViewModel;
            ConsoleViewModelPoop = consoleViewModel;
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
