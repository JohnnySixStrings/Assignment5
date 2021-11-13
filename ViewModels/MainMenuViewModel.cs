using Assignment5.Enums;
using Assignment5.Models;
using Microsoft.Extensions.FileProviders;
using ReactiveUI;
using System;
using System.Linq;
using System.Media;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace Assignment5.ViewModels
{


    public class MainMenuViewModel : ReactiveObject, IRoutableViewModel
    {

        /// <summary>
        /// Name of User
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        /// <summary>
        /// Age of User
        /// </summary>
        private string _age;
        public string Age
        {
            get { return _age; }
            set { this.RaiseAndSetIfChanged(ref _age, value); }
        }
        /// <summary>
        /// Error message that is shown for invalid input
        /// </summary>
        private string _errorText;
        public string ErrorText
        {
            get { return _errorText; }
            set { this.RaiseAndSetIfChanged(ref _errorText, value); }
        }
        /// <summary>
        /// state of whether error message is visible
        /// </summary>
        private Visibility _showError = Visibility.Collapsed;
        public Visibility ShowError
        {
            get { return _showError; }
            set { this.RaiseAndSetIfChanged(ref _showError, value); }
        }
        /// <summary>
        /// Type of Game Chosen
        /// </summary>
        private GameType _gameType;
        public GameType GameType
        {
            get { return _gameType; }
            set { this.RaiseAndSetIfChanged(ref _gameType, value); }
        }
        /// <summary>
        /// Uri for the background image
        /// </summary>
        private Uri _backgroundImageUri = new("Images/MainMenuImage.jpg", UriKind.Relative);
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
        /// Reactive route url segement
        /// </summary>
        public string UrlPathSegment { get; } = "MainMenu";
        /// <summary>
        /// Reactive allows for routing and reacting with the HostScreen
        /// </summary>
        public IScreen HostScreen { get; set; }
        /// <summary>
        /// Handles the logic to start the game
        /// </summary>
        public ReactiveCommand<Unit, Unit> StartGame { get; protected set; }

        public MainMenuViewModel(IScreen screen, UserModel? user = null)
        {
            HostScreen = screen;
            _embeddedFileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

            if (user is not null)
            {
                Name = user.Name;
                Age = user.Age.ToString();
            }
            StartGame = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!ValidInput())
                {
                    return;
                }

                var player = new SoundPlayer(_embeddedFileProvider.GetFileInfo("Sounds/StartGameMainMenu.wav").CreateReadStream());

                player.Play();
                await HostScreen.Router.Navigate
                .Execute(new GameViewModel(HostScreen, GameType, new UserModel { Name = Name, Age = int.Parse(Age) }))
                .Select(_ => Unit.Default);
            }
            );
        }
        /// <summary>
        /// Handles the logic for validating and displaying the information
        /// </summary>
        /// <returns></returns>
        private bool ValidInput()
        {
            ErrorText = string.Empty;
            ShowError = Visibility.Collapsed;

            Regex regex = new Regex("[a-z ,.'-]+$", RegexOptions.IgnoreCase);

            bool valid = true;
            string text = string.Empty;

            //name validation
            if (string.IsNullOrWhiteSpace(Name))
            {
                text += "Name is required\n";
                valid = false;
            }
            else if (!regex.IsMatch(Name))
            {
                text += "Invalid characters in name\n";
            }

            //age validation
            if (string.IsNullOrWhiteSpace(Age))
            {
                text += "Age is required\n";
                valid = false;
            }
            else if (!int.TryParse(Age, out int age))
            {
                text += "Age needs to be a valid number\n";
                valid = false;
            }
            else if (age < 3)
            {
                text += "Too young for this Game\n";
                valid = false;
            }
            else if (age > 10)
            {
                text += "Too old for this game\n";
                valid = false;
            }

            if (!valid)
            {
                ErrorText = text;
                ShowError = Visibility.Visible;
            }
            return valid;
        }
    }
}
