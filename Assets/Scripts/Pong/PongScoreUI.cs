using TMPro;
using UnityEngine;

namespace Pong
{
    public class PongScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtScorePlayerOne;
        [SerializeField] private TextMeshProUGUI txtScorePlayerTwo;

        /// <summary>
        /// Number of goals player one has.
        /// </summary>
        private int _playerOneScore;
        /// <summary>
        /// Number of goals player two has.
        /// </summary>
        private int _playerTwoScore;

        /// <summary>
        /// Updates the score.
        /// </summary>
        /// <param name="playerIndex">The player index of the player in which goal the other player scored.</param>
        public void ScoredInPlayerGoal(int playerIndex)
        {
            if (playerIndex == 1)
            {
                _playerOneScore++;
                txtScorePlayerOne.SetText(_playerOneScore.ToString());
            }
            else
            {
                _playerTwoScore++;
                txtScorePlayerTwo.SetText(_playerTwoScore.ToString());
            }
            
        }
    }
}