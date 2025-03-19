namespace waerp_toolpilot.ViewModels
{
    internal class MainViewModel
    {
        public ViewModelBase CurrentViewModel { get; }

        public MainViewModel()
        {
            CurrentViewModel = new HomeViewModel();
        }
    }
}
