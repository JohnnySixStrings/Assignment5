using Assignment5.ViewModels;
using ReactiveUI;
using System.Windows;

namespace Assignment5.Views
{
    /// <summary>
    /// Interaction logic for FinalScore.xaml
    /// </summary>
    public partial class FinalScoreView : IViewFor<FinalScoreViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
                nameof(ViewModel),
                typeof(FinalScoreViewModel),
                typeof(FinalScoreView),
                null);

        public FinalScoreView()
        {
            InitializeComponent();
            this.WhenActivated(z =>
            {
                z(this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext));
            });
        }

        public FinalScoreViewModel ViewModel
        {
            get { return (FinalScoreViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }

        }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (FinalScoreViewModel)value; }
        }
    }
}
