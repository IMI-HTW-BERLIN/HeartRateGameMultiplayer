using Data.ResponseTypes;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Shows the current HeartRate of a player.
    /// </summary>
    public class HeartRateUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtHeartRate;
        [SerializeField] private int playerIndex;

        private void OnEnable() => MiBandManager.OnHeartRateChange += OnHeartRateChange;

        private void OnDisable() => MiBandManager.OnHeartRateChange -= OnHeartRateChange;

        /// <summary>
        /// Will be called on both player's heart rates.
        /// Only uses the heartrates corresponding to the playerIndex of this UI
        /// </summary>
        private void OnHeartRateChange(HeartRateResponse heartRateResponse)
        {
            if(heartRateResponse.DeviceIndex != playerIndex)
                return;
            
            txtHeartRate.SetText(heartRateResponse.HeartRate.ToString());
        }
    }
}