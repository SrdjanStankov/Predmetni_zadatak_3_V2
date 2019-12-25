using System;
using System.Collections.ObjectModel;
using PZ3_NetworkService.Model;

namespace PZ3_NetworkService.ViewModel
{
    public class NetworkDataViewModel : BindableBase
    {
        public ObservableCollection<Server> ServersToShow
        {
            get => _serversToShow; set
            {
                _serversToShow = value;
                OnPropertyChanged("ServersToShow");
            }
        }
        public MyICommand AddServerCommand { get; set; }
        public MyICommand DeleteServerCommand { get; set; }
        public MyICommand FindServerCommand { get; set; }
        public string ImgSrc { get; set; }

        private Server currentServer;
        private string selectedIp;
        private Server selectedServer;
        private Server filterServer;

        public bool Lt { get; set; }
        public bool Gt { get; set; }
        public bool NaN { get; set; }

        private ObservableCollection<Server> _serversToShow;

        public NetworkDataViewModel()
        {
            ServersToShow = StaticClass.Servers;
            ImgSrc = Environment.CurrentDirectory + "\\server_PNG20.png";
            currentServer = new Server
            {
                ImgSrc = ImgSrc
            };
            AddServerCommand = new MyICommand(OnAdd);
            DeleteServerCommand = new MyICommand(OnDelete);

            Lt = true;
            FilterServer = new Server()
            {
                ImgSrc = ImgSrc
            };
            FindServerCommand = new MyICommand(OnFilter);
            StaticClass.FilteredServers.CollectionChanged += FilteredServersChanged;

            AddStartingValues();
        }

        private void FilteredServersChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ServersToShow = StaticClass.FilteredServers;
        }

        private void AddStartingValues()
        {
            StaticClass.Servers.Add(new Server() { Id = 1, ImgSrc = ImgSrc, IpAddress = StaticClass.IpAddresses[0], Name = "Server1" });
            StaticClass.Servers.Add(new Server() { Id = 2, ImgSrc = ImgSrc, IpAddress = StaticClass.IpAddresses[0], Name = "Server2" });
            StaticClass.Servers.Add(new Server() { Id = 3, ImgSrc = ImgSrc, IpAddress = StaticClass.IpAddresses[0], Name = "Server3" });
            StaticClass.Servers.Add(new Server() { Id = 4, ImgSrc = ImgSrc, IpAddress = StaticClass.IpAddresses[0], Name = "Server4" });
            StaticClass.Servers.Add(new Server() { Id = 5, ImgSrc = ImgSrc, IpAddress = StaticClass.IpAddresses[0], Name = "Server5" });

            StaticClass.Rectangles.Add(new MyRect("Server1"));
            StaticClass.Rectangles.Add(new MyRect("Server2"));
            StaticClass.Rectangles.Add(new MyRect("Server3"));
            StaticClass.Rectangles.Add(new MyRect("Server4"));
            StaticClass.Rectangles.Add(new MyRect("Server5"));
        }

        private void OnFilter()
        {
            string ipAddress = FilterServer.IpAddress.ToLower();
            string idMode = NaN ? "nan" : Lt ? "lt" : Gt ? "gt" : "";
            int id = FilterServer.Id;

            StaticClass.FilterServers(ipAddress, idMode, id);
        }

        public ObservableCollection<string> IpAddresses => new ObservableCollection<string>(StaticClass.IpAddresses)
        {
            "NaN"
        };

        public Server FilterServer
        {
            get => filterServer;
            set
            {
                filterServer = value;
                OnPropertyChanged("FilterServer");
            }
        }

        private void OnDelete()
        {
            if (SelectedServer is null)
            {
                return;
            }
            StaticClass.DeleteServerIfExist(SelectedServer);
        }

        private void OnAdd()
        {
            CurrentServer.Validate();
            if (CurrentServer.IsValid)
            {
                StaticClass.AddServerIfNotExist(new Server(CurrentServer));
            }
        }

        public Server CurrentServer
        {
            get => currentServer;
            set
            {
                currentServer = value;
                OnPropertyChanged("CurrentServer");
            }
        }

        public string SelectedIp
        {
            get => selectedIp;

            set
            {
                selectedIp = value;
                CurrentServer.IpAddress = value;
                OnPropertyChanged("CurrentServer");
                OnPropertyChanged("SelectedIp");
            }
        }

        public Server SelectedServer
        {
            get => selectedServer;

            set
            {
                selectedServer = value;
                OnPropertyChanged("selectedServer");
            }
        }
    }
}
