using Assignment5.ViewModels;
using Assignment5.Views;
using ReactiveUI;
using Splat;

namespace Assignment5
{
    public class AppBootstrapper : ReactiveObject, IScreen, IRoutableViewModel
    {
        public RoutingState Router { get; private set; }

        public string? UrlPathSegment { get; } = "BootStrapper";

        public IScreen HostScreen => throw new System.NotImplementedException();

        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
        {
            Router = testRouter ?? new RoutingState();
            dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

            //Bind Routes
            RegisterParts(dependencyResolver);

            // Navigate to the opening page of the application
            _ = Router.Navigate.Execute(new MainMenuViewModel(this));
        }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));
            dependencyResolver.Register(() => new MainMenuView(), typeof(IViewFor<MainMenuViewModel>));
            dependencyResolver.Register(() => new GameView(), typeof(IViewFor<GameViewModel>));
            dependencyResolver.Register(() => new FinalScoreView(), typeof(IViewFor<FinalScoreViewModel>));
        }
    }
}