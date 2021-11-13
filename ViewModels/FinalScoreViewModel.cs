using Assignment5.Models;
using Microsoft.Extensions.FileProviders;
using ReactiveUI;
using System;
using System.Media;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;

namespace Assignment5.ViewModels
{
    public class FinalScoreViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <summary>
        /// Binding for Name of the user
        /// </summary>
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { this.RaiseAndSetIfChanged(ref _userName, value); }
        }
        /// <summary>
        /// Binding for the Age of the user
        /// </summary>
        private string _userAge;
        public string UserAge
        {
            get { return _userAge; }
            set { this.RaiseAndSetIfChanged(ref _userAge, value); }
        }
        /// <summary>
        /// Binding for the Time it took to finish
        /// </summary>
        private string _timeTaken;
        public string TimeTaken
        {
            get { return _timeTaken; }
            set { this.RaiseAndSetIfChanged(ref _timeTaken, value); }
        }
        /// <summary>
        ///  Binding for the number of questions answered correctly
        /// </summary>
        private string _countOfCorrect;
        public string CountOfCorrect
        {
            get { return _countOfCorrect; }
            set { this.RaiseAndSetIfChanged(ref _countOfCorrect, value); }
        }
        /// <summary>
        /// Binding for the number of questions answered incorrectly
        /// </summary>
        private string _countOfIncorrect;
        public string CountOfIncorrect
        {
            get { return _countOfIncorrect; }
            set { this.RaiseAndSetIfChanged(ref _countOfIncorrect, value); }
        }
        /// <summary>
        /// Binding for the Uri for the background image
        /// </summary>
        private Uri _backgroundImageUri;
        public Uri BackgroundImageUri
        {
            get { return _backgroundImageUri; }
            set { this.RaiseAndSetIfChanged(ref _backgroundImageUri, value); }
        }
        /// <summary>
        /// Allows to access the embedded files using reflection
        /// </summary>
        private EmbeddedFileProvider _embeddedFileProvider;
        /// <summary>
        /// Binding for the command of the to go back to the menu to choose a new game
        /// </summary>
        public ReactiveCommand<Unit, Unit> NewGame { get; protected set; }
        /// <summary>
        /// Reactive url path string
        /// </summary>
        public string UrlPathSegment { get; } = "FinalScore";
        /// <summary>
        /// HostScreen allow to route between views/viewmodels
        /// </summary>
        public IScreen HostScreen { get; }
        /// <summary>
        /// Holds score information 
        /// </summary>
        public ScoreModel ScoreModel { get; protected set; }
        public FinalScoreViewModel(IScreen hostScreen, ScoreModel scoreModel)
        {
            //set initial values
            HostScreen = hostScreen;
            ScoreModel = scoreModel;
            _embeddedFileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            //setup commands
            NewGame = ReactiveCommand.CreateFromTask(async () =>
           {
               await HostScreen.Router.Navigate.Execute(new MainMenuViewModel(HostScreen, ScoreModel.User));

           });

            //set State 
            SetScore(ScoreModel);
            BackgroundImageUri = scoreModel.CountOfCorrect switch
            {
                < 5 => new Uri("Images/LowScore.jpg", UriKind.Relative),
                < 8 => new Uri("Images/MidScore.jpg", UriKind.Relative),
                < 11 => new Uri("Images/HighScore.jpg", UriKind.Relative),
                _ => new Uri("Images/LowScore.jpg", UriKind.Relative)
            };
            var player = new SoundPlayer(_embeddedFileProvider.GetFileInfo("Sounds/Sword.wav").CreateReadStream());
            player.Play();
        }

        /// <summary>
        /// Set the scores for the viewmodel
        /// </summary>
        /// <param name="score"></param>
        private void SetScore(ScoreModel score)
        {
            UserName = $"User: {score.User.Name}";
            UserAge = $"Age: {score.User.Age}";
            TimeTaken = $"Time: {score.ElapsedTime}";
            CountOfCorrect = $"Answered Correct: {score.CountOfCorrect}";
            CountOfIncorrect = $"Answered Incorrect: {score.CountOfIncorrect}";
        }
    }
}
