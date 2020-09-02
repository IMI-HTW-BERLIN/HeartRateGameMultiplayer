using Managers;
using UnityEngine;

namespace Pong
{
    [RequireComponent(typeof(Collider2D))]
    public class PongGoal : MonoBehaviour
    {
        [SerializeField] private int playerIndex;

        public void Goal() => PongManager.Instance.ScoredInPlayerGoal(playerIndex);
    }
}