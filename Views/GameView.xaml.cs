using Assignment5.ViewModels;
using ReactiveUI;
using System;
using System.Windows;
using System.Reactive.Linq;

namespace Assignment5.Views
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class GameView : IViewFor<GameViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(GameViewModel), typeof(GameView), null);

        public GameView()
        {
            InitializeComponent();

            this.WhenActivated(z =>
            {
                z(this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext));
                z(this.BindCommand(ViewModel, vm => vm.Submit, view => view.SubmitButton));
                z(this.BindCommand(ViewModel, vm => vm.ExitGame, view => view.ExitGameButton));
                z(this.BindCommand(ViewModel,vm=> vm.StartGame, view => view.DialogStartGameButton));

            });
            
        }

        public GameViewModel ViewModel
        {
            get { return (GameViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (GameViewModel)value; }
        }
    }
}
