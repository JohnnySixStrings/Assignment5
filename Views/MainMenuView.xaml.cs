using Assignment5.ViewModels;
using ReactiveUI;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace Assignment5.Views
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenuView : IViewFor<MainMenuViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(MainMenuViewModel), typeof(MainMenuView), null);

        public MainMenuView()
        {
            InitializeComponent();

            this.WhenActivated(vm =>
            {
                vm(this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext));
                vm(this.BindCommand(ViewModel, vm => vm.StartGame, view => view.StartGameButton));
            });
        }

        public MainMenuViewModel ViewModel
        {
            get { return (MainMenuViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }

        }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainMenuViewModel)value; }
        }
    }
}
