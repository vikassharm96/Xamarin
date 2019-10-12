namespace demo
{
    public class MainActivityViewModel : Android.Arch.Lifecycle.ViewModel
    {
        public string Text { get; set; }

        public string TextMethod()
        {
            return "";
        }
    }
}