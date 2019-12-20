using System;
using System.Collections.ObjectModel;

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
        public static ObservableCollection<MyRect> Rectangles { get; set; }
        public static ObservableCollection<string> IpAddresses { get; set; }

        static StaticClass()
        {
            Servers = new ObservableCollection<Server>();
            Rectangles = new ObservableCollection<MyRect>();
            NetworkManagment.CreateListener();
            LoadIps();
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
