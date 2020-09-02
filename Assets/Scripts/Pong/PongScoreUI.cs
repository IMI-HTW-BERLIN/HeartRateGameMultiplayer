using TMPro;
using UnityEngine;

namespace Pong
{
    public class PongScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtScorePlayerOne;
        [SerializeField] private TextMeshProUGUI txtScorePlayerTwo;

        private int _playerOneScore;
        private int _playerTwoScore;

        public void AddScoreForPlayer(int playerIndex)
        {
            if (playerIndex == 0)
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