using Managers.Abstract;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private float sizeChangeFactor = 1;
        [SerializeField] private AnimationCurve sizeChangeCurve;

        [SerializeField] private float speedChangeFactor = 1;
        [SerializeField] private AnimationCurve speedChangeCurve;

        /// <summary>
        /// Whether the size change option is enabled
        /// </summary>
        public bool SizeChange { get; set; }

        /// <summary>
        /// Whether the speed change is enabled.
        /// </summary>
        public bool SpeedChange { get; set; }

        /// <summary>
        /// Loads the pong game scene.
        /// </summary>
        public void LoadPongGame() => SceneManager.LoadScene(1);

        /// <summary>
        /// Gets the size change of the given player depending on the current heart rate delta.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <returns>The size change for this player's pong-bat.</returns>
        public float GetSizeChange(int playerIndex)
        {
            float delta = HeartRateManager.Instance.GetPlayerDelta(playerIndex);
            return sizeChangeCurve.Evaluate(delta) * sizeChangeFactor;
        }

        /// <summary>
        /// Gets the speed change for the pong ball after the player hits it depending on the current heart rate delta.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <returns>The speed change for this player's hits.</returns>
        public float GetSpeedChange(int playerIndex)
        {
            float delta = HeartRateManager.Instance.GetPlayerDelta(playerIndex);
            return speedChangeCurve.Evaluate(delta) * speedChangeFactor;
        }
    }
}