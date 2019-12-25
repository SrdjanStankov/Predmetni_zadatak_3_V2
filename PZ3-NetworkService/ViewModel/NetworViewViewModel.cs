using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PZ3_NetworkService.Model;

namespace PZ3_NetworkService.ViewModel
{
    public class NetworViewViewModel : BindableBase
    {
        public static Dictionary<int, Server> ServersD { get; set; } = new Dictionary<int, Server>();
        public static Dictionary<int, Server> Dropped { get; set; } = new Dictionary<int, Server>();
        public static Dictionary<int, Canvas> CanvasesData { get; set; } = new Dictionary<int, Canvas>();
        public static Dictionary<string, Server> Canvases { get; set; } = new Dictionary<string, Server>();
        public ObservableCollection<Server> Servers { get; set; } = new ObservableCollection<Server>();

        public MyICommand LeftMBUp { get; set; }
        public MyICommand<ListView> SELCHANGE { get; set; }
        public MyICommand<Canvas> DCommand { get; set; }
        public MyICommand<Canvas> LCommand { get; set; }
        public MyICommand<Canvas> FreeCanvas { get; set; }
        public MyICommand<Canvas> CanSel { get; set; }

        private Server selectedObject;
        public Server SelectedObject { get => selectedObject; set => selectedObject = value; }

        public static Server draggedItem = null;
        private bool dragging = false;
        private bool fromList = false;
        private Canvas CanvasToEmpty;

        public void OnLoad(Canvas c)
        {
            if (Canvases.ContainsKey(c.Name))
            {
                var logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(Canvases[c.Name].ImgSrc);
                logo.EndInit();
                c.Background = new ImageBrush(logo);
                ((TextBlock)c.Children[0]).Text = Canvases[c.Name].Id.ToString() + "." + Canvases[c.Name].Name;
                ((TextBlock)c.Children[0]).Foreground = Brushes.Black;

                ChangeStateBar(c);

                if (!c.Resources.Contains("taken"))
                {
                    c.Resources.Add("taken", true);
                }
            }
        }

        private void RefreshList()
        {
            Servers.Clear();
            foreach (var d in ServersD.Values)
            {
                Servers.Add(d);
            }
            OnPropertyChanged("Servers");
        }

        private void StateChanged(State state, int id)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in StaticClass.Servers)
                {
                    if (CanvasesData.ContainsKey(item.Id))
                    {
                        ChangeStateBar(CanvasesData[item.Id]);
                    }
                }

            });
        }

        private void ChangeStateBar(Canvas c)
        {
            foreach (var item in StaticClass.Servers)
            {
                if (Canvases.ContainsKey(c.Name) && Canvases[c.Name].Id == item.Id)
                {
                    Switch(c, item.State);
                }
            }
        }

        private void Switch(Canvas c, State s)
        {
            switch (s)
            {
                case State.NORMAL:
                    ((TextBlock)c.Children[0]).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7fff00"));
                    break;
                case State.HIGH:
                    ((TextBlock)c.Children[0]).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff4500"));
                    break;
                case State.LOW:
                    ((TextBlock)c.Children[0]).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffa500"));
                    break;
                default:
                    break;
            }
            ((TextBlock)c.Children[0]).Foreground = Brushes.Black;
        }

        public NetworViewViewModel()
        {
            StaticClass.StateChange += StateChanged;
            StaticClass.ObjectAdded += UpdateNonDroppedItems;
            StaticClass.ObjectDeleted += UpdateDroppedItems;

            LeftMBUp = new MyICommand(OnMouseLeftButtonUp);
            SELCHANGE = new MyICommand<ListView>(SelectionChange);
            DCommand = new MyICommand<Canvas>(Drop);
            LCommand = new MyICommand<Canvas>(OnLoad);
            FreeCanvas = new MyICommand<Canvas>(OnRunFreeCanvas);
            CanSel = new MyICommand<Canvas>(CanvasSelect);

            UpdateNonDroppedItems(0);
        }

        private void UpdateNonDroppedItems(int id)
        {
            ServersD.Clear();
            foreach (var item in StaticClass.Servers)
            {
                if (!Dropped.ContainsKey(item.Id))
                {
                    ServersD.Add(item.Id, item);
                }
            }
            RefreshList();
        }

        private void UpdateDroppedItems(int id)
        {
            if (Dropped.ContainsKey(id))
            {
                int temp = Dropped[id].Id;

                foreach (var item in Canvases)
                {
                    if (item.Value.Id == temp)
                    {
                        Console.WriteLine("Canvas removed");
                        Canvases.Remove(item.Key);
                        CanvasesData.Remove(item.Value.Id);
                        break;
                    }
                }
                Dropped.Remove(id);
            }
            UpdateNonDroppedItems(id);
        }

        private void OnRunFreeCanvas(Canvas canvas)
        {
            if (canvas.Resources["taken"] != null)
            {
                char[] delimiters = new char[] { '.' };
                string[] words = ((TextBlock)canvas.Children[0]).Text.Split(delimiters);
                int id = int.Parse(words[0]);

                ServersD[id] = Dropped[id];
                Dropped.Remove(id);
                Canvases.Remove(canvas.Name);
                CanvasesData.Remove(id);
                OnPropertyChanged("Servers");

                RefreshList();

                canvas.Background = Brushes.White;
                ((TextBlock)canvas.Children[0]).Text = "Dostupno";
                ((TextBlock)canvas.Children[0]).Foreground = Brushes.Black;
                ((TextBlock)canvas.Children[0]).Background = Brushes.White;
                canvas.Resources.Remove("taken");
            }
        }

        public void Drop(Canvas c)
        {
            if (draggedItem != null)
            {
                if (c.Resources["taken"] == null)
                {
                    var logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri(draggedItem.ImgSrc, UriKind.Absolute);
                    logo.EndInit();

                    if (fromList == false)
                    {
                        if (Canvases.ContainsKey(CanvasToEmpty.Name))
                        {
                            Canvases.Remove(CanvasToEmpty.Name);
                            CanvasesData.Remove(draggedItem.Id);
                        }
                    }

                    c.Background = new ImageBrush(logo);
                    ((TextBlock)c.Children[0]).Text = draggedItem.Id + "." + draggedItem.Name;
                    ((TextBlock)c.Children[0]).Foreground = Brushes.Black;
                    ((TextBlock)c.Children[0]).Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff");

                    c.Resources.Add("taken", true);

                    if (fromList)
                    {
                        Dropped[draggedItem.Id] = ServersD[draggedItem.Id];
                        ServersD.Remove(draggedItem.Id);
                        OnPropertyChanged("Servers");
                        fromList = false;
                    }
                    else
                    {
                        EmptyCanvas();
                    }
                }
                RefreshList();

                CanvasesData.Add(draggedItem.Id, c);
                Canvases[c.Name] = draggedItem;
                StateChanged(State.LOW, 0);

                dragging = false;
                draggedItem = null;
            }
        }

        private void EmptyCanvas()
        {
            CanvasToEmpty.Background = Brushes.White;
            Canvases.Remove(CanvasToEmpty.Name);
            CanvasesData.Remove(draggedItem.Id);
            ((TextBlock)CanvasToEmpty.Children[0]).Text = "Dostupno";
            ((TextBlock)CanvasToEmpty.Children[0]).Foreground = Brushes.Black;
            ((TextBlock)CanvasToEmpty.Children[0]).Background = Brushes.White;
            CanvasToEmpty.Resources.Remove("taken");
            CanvasToEmpty = null;
        }

        private void OnMouseLeftButtonUp()
        {
            draggedItem = null;
            selectedObject = null;
            dragging = false;
            CanvasToEmpty = null;
        }

        public void SelectionChange(ListView o)
        {
            if (!dragging)
            {
                fromList = true;
                dragging = true;

                draggedItem = new Server(SelectedObject);

                DragDrop.DoDragDrop(o, draggedItem, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void CanvasSelect(Canvas canvas)
        {
            if (canvas.Resources["taken"] != null)
            {
                char[] delimiters = new char[] { '.' };
                string[] words = ((TextBlock)canvas.Children[0]).Text.Split(delimiters);
                int id = int.Parse(words[0]);

                draggedItem = Dropped[id];

                if (!dragging)
                {
                    fromList = false;
                    dragging = true;
                    fromList = false;
                    CanvasToEmpty = canvas;
                    DragDrop.DoDragDrop(canvas, draggedItem, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }
    }

}
