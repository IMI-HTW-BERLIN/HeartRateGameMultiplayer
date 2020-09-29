using Managers.Abstract;
using Pong;
using UnityEngine;

namespace Managers
{
    public class PongManager : SceneSingleton<PongManager>
    {
        [SerializeField] private PongScoreUI pongScoreUI;
        public void ScoredInPlayerGoal(int playerIndex) => pongScoreUI.ScoredInPlayerGoal(playerIndex);
    }
}