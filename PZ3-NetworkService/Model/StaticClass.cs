using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PZ3_NetworkService.Model
{
    public delegate void StaticClassChangeHandler(int id);
    public delegate void ServersStateChangeHanler(State state, int id);

    public static class StaticClass
    {
        public static event StaticClassChangeHandler ObjectAdded;
        public static event StaticClassChangeHandler ObjectDeleted;
        public static event StaticClassChangeHandler ValueChanged;
        public static event ServersStateChangeHanler StateChange;

        private const int IpAddressNum = 9;

        public static ObservableCollection<Server> Servers { get; set; }
        public static ObservableCollection<Server> FilteredServers { get; set; }
        public static ObservableCollection<MyRect> Rectangles { get; set; }
        public static ObservableCollection<string> IpAddresses { get; set; }

        public static Func<Server, bool> undoAction;
        private static Server undoServer;

        static StaticClass()
        {
            Servers = new ObservableCollection<Server>();
            Rectangles = new ObservableCollection<MyRect>();
            FilteredServers = new ObservableCollection<Server>(Servers);
            NetworkManagment.CreateListener();
            LoadIps();
        }

        public static void Undo()
        {
            undoAction?.Invoke(undoServer);
        }

        public static bool AddServerIfNotExist(Server server)
        {
            if (!ServerExist(server))
            {
                Servers.Add(server);
                Rectangles.Add(new MyRect(server.Name));
                ChangeMade(server.Id, Operation.ADD);
                undoAction = DeleteServerIfExist;
                undoServer = server;
                return true;
            }
            return false;
        }

        public static bool DeleteServerIfExist(Server server)
        {
            if (ServerExist(server))
            {
                int idx = Servers.IndexOf(server);
                Servers.RemoveAt(idx);
                Rectangles.RemoveAt(idx);
                ChangeMade(server.Id, Operation.REMOVE);
                undoAction = AddServerIfNotExist;
                undoServer = server;
                return true;
            }
            return false;
        }

        public static bool DeleteServerIfExist(int id)
        {
            var serv = Servers.Where(srv => srv.Id == id);
            if (serv.Any())
            {
                return DeleteServerIfExist(serv.FirstOrDefault());
            }
            return false;
        }

        public static void FilterServers(string IpAddress = "nan", string idMode = "nan", int id = 0)
        {
            FilteredServers.Clear();
            if (IpAddress == "nan" && idMode == "nan")
            {
                Servers.ToList().ForEach(item => FilteredServers.Add(item));
                return;
            }

            if (idMode == "nan")
            {
                Servers.Where(item => item.IpAddress == IpAddress).ToList().ForEach(item => FilteredServers.Add(item));
                return;
            }

            if (IpAddress == "nan")
            {
                if (idMode == "lt")
                {
                    Servers.Where(item => item.Id < id).ToList().ForEach(item => FilteredServers.Add(item));
                }
                else if (idMode == "gt")
                {
                    Servers.Where(item => item.Id > id).ToList().ForEach(item => FilteredServers.Add(item));
                }
                return;
            }

            if (idMode == "lt")
            {
                Servers.Where(item => item.Id < id && item.IpAddress == IpAddress).ToList().ForEach(item => FilteredServers.Add(item));
            }
            else if (idMode == "gt")
            {
                Servers.Where(item => item.Id > id && item.IpAddress == IpAddress).ToList().ForEach(item => FilteredServers.Add(item));
            }
        }

        public static bool ServerExist(Server server)
        {
            return Servers.Where(item => item.Id == server.Id).Any();
        }

        public static void LoadIps()
        {
            var temp = new ObservableCollection<string>();
            var r = new Random();
            string ip = "";

            temp.Add("127.0.0.1");

            for (int i = 0; i < IpAddressNum; i++)
            {
                ip += r.Next(127, 192);
                ip += ".";
                ip += r.Next(255).ToString();
                ip += ".";
                ip += r.Next(255).ToString();
                ip += ".";
                ip += r.Next(255).ToString();

                temp.Add(ip);
                ip = "";
            }
            IpAddresses = temp;
        }

        public static void ChangeMade(int id, Operation operation)
        {
            switch (operation)
            {
                case Operation.ADD:
                    ObjectAdded?.Invoke(id);
                    break;
                case Operation.REMOVE:
                    ObjectDeleted?.Invoke(id);
                    break;
                case Operation.VALUE_CHANGE:
                    ValueChanged?.Invoke(id);
                    break;
                default:
                    break;
            }
        }

        public static void StateChanged(State state, int id)
        {
            StateChange?.Invoke(state, id);
        }
    }
}
