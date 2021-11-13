using Assignment5.Enums;
using Assignment5.Models;
using Microsoft.Extensions.FileProviders;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Media;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Assignment5.ViewModels
{
    public class GameViewModel : ReactiveObject, IRoutableViewModel
    {
        //bound state
        /// <summary>
        /// Time that has passed in mm:ss from game start
        /// </summary>
        private string _elapsedTime;
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set { this.RaiseAndSetIfChanged(ref _elapsedTime, value); }
        }
        /// <summary>
        /// guess value from the guess textbox
        /// </summary>
        private string _textGuess;
        public string TextGuess
        {
            get { return _textGuess; }
            set { this.RaiseAndSetIfChanged(ref _textGuess, value); }
        }
        /// <summary>
        /// Value that shows the correct answer when incorrectly answered
        /// </summary>
        private string _correctAnswer;
        public string CorrectAnswer
        {
            get { return _correctAnswer; }
            set { this.RaiseAndSetIfChanged(ref _correctAnswer, value); }
        }
        /// <summary>
        /// Text showing the problem to be answered
        /// </summary>
        private string _problemText = "";
        public string ProblemText
        {
            get { return _problemText; }
            set { this.RaiseAndSetIfChanged(ref _problemText, value); }
        }
        /// <summary>
        /// State of whether the start dialog is open or closed 
        /// </summary>
        private bool _isStartGameDialogOpen;
        public bool IsStartGameDialogOpen
        {
            get { return _isStartGameDialogOpen; }
            set { this.RaiseAndSetIfChanged(ref _isStartGameDialogOpen, value); }
        }
        /// <summary>
        /// State of whether the correct answer dialog is open or closed with congraluatory message
        /// </summary>
        private bool _isCorrectAnswerDialogOpen;
        public bool IsCorrectAnswerDialogOpen
        {
            get { return _isCorrectAnswerDialogOpen; }
            set { this.RaiseAndSetIfChanged(ref _isCorrectAnswerDialogOpen, value); }
        }
        /// <summary>
        /// State of whether the wrong answer dialog is open or closed showing the correct answer
        /// </summary>
        private bool _isWrongAnswerDialogOpen;
        public bool IsWrongAnswerDialogOpen
        {
            get { return _isWrongAnswerDialogOpen; }
            set { this.RaiseAndSetIfChanged(ref _isWrongAnswerDialogOpen, value); }
        }
        /// <summary>
        /// Uri for the background image
        /// </summary>
        private Uri _backgroundImageUri = new("Images/GameImage.jpg", UriKind.Relative);
        public Uri BackgroundImageUri
        {
            get { return _backgroundImageUri; }
            set { this.RaiseAndSetIfChanged(ref _backgroundImageUri, value); }
        }

        //private object state
        /// <summary>
        /// The numerical value to compare the answer against
        /// </summary>
        private int ActualAnswer { get; set; }
        /// <summary>
        /// Stopwatch to keep track of the time from start to finish
        /// </summary>
        private Stopwatch _stopwatch = new();
        /// <summary>
        /// DispatchTimer it dispatchs a call to update the viewed time every second
        /// </summary>
        private DispatcherTimer _dispatchTimer;
        /// <summary>
        /// Random used to generate the values for the questions
        /// </summary>
        private Random _random = new();
        /// <summary>
        /// What turn game is on to know when to end the game
        /// </summary>
        private int _turnCount;
        /// <summary>
        /// Number of correctly answered questions
        /// </summary>
        private int _countOfCorrect;
        /// <summary>
        /// Number of incorrectly answered questions
        /// </summary>
        private int _countOfIncorrect;
        /// <summary>
        /// User (Name,Age)
        /// </summary>
        private UserModel User { get; }
        /// <summary>
        /// Allows to access the embedded files using reflection
        /// </summary>
        private EmbeddedFileProvider _embeddedFileProvider;
        /// <summary>
        /// GameType Addition, Subtraction, Division, Multiplication
        /// </summary>
        private GameType GameType { get; }

        //reactive ui
        /// <summary>
        /// Reactive url path segement 
        /// </summary>
        public string UrlPathSegment { get; } = "Game";
        /// <summary>
        /// Hostscreen allow for routing between different view/viewmodels
        /// </summary>
        public IScreen HostScreen { get; }

        //bound commands
        /// <summary>
        /// Handles submitting of guess
        /// </summary>
        public ReactiveCommand<Unit, Unit> Submit { get; protected set; }
        /// <summary>
        /// Handle the quiting back to the main menu
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExitGame { get; protected set; }
        /// <summary>
        /// Handles the starting the stopwatch and dispatch timer to update the displayed time
        /// </summary>
        public ReactiveCommand<Unit, Unit> StartGame { get; protected set; }


        public GameViewModel(IScreen hostScreen, GameType gameType, UserModel userModel)
        {
            //Store passed state
            HostScreen = hostScreen;
            GameType = gameType;
            User = userModel;
            _embeddedFileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            //show initial start game option
            IsStartGameDialogOpen = true;
            // setup the commands behind the view 
            Submit = ReactiveCommand.CreateFromTask(
                //the async nature of this function could lead to issues if submit or enter is spammed
                async () =>
                {
                    if (!int.TryParse(TextGuess, out int guess))
                    {
                        //add snackbar
                        return;
                    }
                    //check if correct
                    var isCorrect = guess == ActualAnswer;
                    if (isCorrect)
                    {
                        _countOfCorrect++;
                        IsCorrectAnswerDialogOpen = true;
                        var player = new SoundPlayer(_embeddedFileProvider.GetFileInfo("Sounds/Blaster.wav").CreateReadStream());
                        player.Play();
                        await Task.Delay(1400);
                        IsCorrectAnswerDialogOpen = false;
                    }
                    else
                    {

                        _countOfIncorrect++;
                        CorrectAnswer = $"{ProblemText} {ActualAnswer}";
                        IsWrongAnswerDialogOpen = true;
                        var player = new SoundPlayer(_embeddedFileProvider.GetFileInfo("Sounds/Hit.wav").CreateReadStream());
                        player.Play();
                        await Task.Delay(1400);
                        IsWrongAnswerDialogOpen = false;

                    }
                    //if game over route to final score page
                    if (_turnCount > 8)
                    {
                        await HostScreen.Router.Navigate.Execute(new FinalScoreViewModel(HostScreen, new ScoreModel
                        {
                            User = User,
                            CountOfCorrect = _countOfCorrect,
                            CountOfIncorrect = _countOfIncorrect,
                            ElapsedTime = ElapsedTime
                        }));
                    }
                    //else setup next problem
                    _turnCount++;
                    SetupProblem();
                });

            StartGame = ReactiveCommand.Create(
                () =>
                {
                    _dispatchTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
                    _dispatchTimer.Tick += (obj, e) =>
                    {
                        ElapsedTime = _stopwatch.Elapsed.ToString(@"mm\:ss");
                    };
                    var player = new SoundPlayer(_embeddedFileProvider.GetFileInfo("Sounds/StartGame.wav").CreateReadStream());
                    player.Play();
                    _stopwatch = Stopwatch.StartNew();
                    _dispatchTimer.Start();
                    IsStartGameDialogOpen = false;
                });

            ExitGame = ReactiveCommand.CreateFromTask(async () =>
                await HostScreen.Router.Navigate
                .Execute(new MainMenuViewModel(HostScreen, User))
                .Select(_ => Unit.Default)
            );

            SetupProblem();
        }

        /// <summary>
        /// Setups problems based on gametype chosen
        /// </summary>
        private void SetupProblem()
        {
            TextGuess = String.Empty;
            int firstNumber = _random.Next(1, 10);
            int secondNumber = _random.Next(1, 10);

            switch (GameType)
            {
                case GameType.Addition:
                    ProblemText = $"{firstNumber} + {secondNumber} =";
                    ActualAnswer = firstNumber + secondNumber;
                    break;
                case GameType.Subtraction:
                    (firstNumber, secondNumber) = GenerateSubractionPair();
                    ProblemText = $"{firstNumber} - {secondNumber} =";
                    ActualAnswer = firstNumber - secondNumber;

                    break;
                case GameType.Multiplication:
                    ProblemText = $"{firstNumber} x {secondNumber} =";
                    ActualAnswer = firstNumber * secondNumber;
                    break;
                case GameType.Division:
                    (firstNumber, secondNumber) = GenerateDivisiblePair();
                    ProblemText = $"{firstNumber} / {secondNumber} =";
                    ActualAnswer = firstNumber / secondNumber;
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Finds a pair of numbers divide equally 
        /// </summary>
        /// <returns></returns>
        private (int, int) GenerateDivisiblePair()
        {
            int firstNumber = _random.Next(1, 10);
            int secondNumber = _random.Next(1, 10);
            while (firstNumber % secondNumber != 0)
            {
                firstNumber = _random.Next(1, 10);
                secondNumber = _random.Next(1, 10);
            }
            return (firstNumber, secondNumber);
        }

        /// <summary>
        /// Finds a pair of numbers that subtract and are not negativ
        /// </summary>
        /// <returns></returns>
        private (int, int) GenerateSubractionPair()
        {
            //terrible for performance but don't have time to come up with another method.
            int firstNumber = _random.Next(1, 10);
            int secondNumber = _random.Next(1, 10);
            while (firstNumber <= secondNumber)
            {
                firstNumber = _random.Next(1, 10);
                secondNumber = _random.Next(1, 10);
            }
            return (firstNumber, secondNumber);
        }
    }
}
