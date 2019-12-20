namespace PZ3_NetworkService.Model
{
    public class MyRect : BindableBase
    {
        private int height;
        private string nameRect;

        public MyRect()
        {
        }

        public MyRect(string name)
        {
            nameRect = name;
            height = 0;
        }

        public string NameRect
        {
            get => nameRect;
            set
            {
                nameRect = value;
                OnPropertyChanged("Name");
            }
        }


        public int Height
        {
            get => height;

            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }
    }
}
