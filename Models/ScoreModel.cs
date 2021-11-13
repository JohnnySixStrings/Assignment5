namespace Assignment5.Models
{
    public class ScoreModel
    {
        /// <summary>
        /// User (name,ager)
        /// </summary>
        public UserModel User { get; set; }
        /// <summary>
        /// Number of correctly answered questions
        /// </summary>
        public int CountOfCorrect { get; set; }
        /// <summary>
        /// Number of incorrectly answered questions
        /// </summary>
        public int CountOfIncorrect { get; set; }
        /// <summary>
        /// The time that had elapsed from the beginning of the game till the end
        /// </summary>
        public string ElapsedTime { get; set; }
    }
}
