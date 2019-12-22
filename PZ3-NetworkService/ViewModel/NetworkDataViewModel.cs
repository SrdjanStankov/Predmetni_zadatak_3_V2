using PZ3_NetworkService.Model;

using System;
using System.Collections.ObjectModel;

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

            AddStartingValues();
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
            var finded = new ObservableCollection<Server>();

            if (filterServer.IpAddress == "NaN" && NaN)
            {
                ServersToShow = finded;
                return;
            }

            if (NaN)
            {
                foreach (var item in StaticClass.Servers)
                {
                    if (item.IpAddress == filterServer.IpAddress)
                    {
                        finded.Add(item);
                    }
                }
                ServersToShow = finded;
                return;
            }

            if (filterServer.IpAddress == "NaN")
            {
                foreach (var item in StaticClass.Servers)
                {
                    if (Lt)
                    {
                        if (item.Id < FilterServer.Id)
                        {
                            finded.Add(item);
                        }
                    }
                    else if (Gt)
                    {
                        if (item.Id > FilterServer.Id)
                        {
                            finded.Add(item);
                        }
                    }
                }
                ServersToShow = finded;
                return;
            }

            foreach (var item in StaticClass.Servers)
            {
                if (item.IpAddress == FilterServer.IpAddress)
                {
                    if (Lt)
                    {
                        if (item.Id < FilterServer.Id)
                        {
                            finded.Add(item);
                        }
                    }
                    else if (Gt)
                    {
                        if (item.Id > FilterServer.Id)
                        {
                            finded.Add(item);
                        }
                    }
                }
            }

            ServersToShow = finded;
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
