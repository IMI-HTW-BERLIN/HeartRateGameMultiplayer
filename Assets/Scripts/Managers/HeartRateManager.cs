using System;
using System.Collections.Generic;
using Data.ResponseTypes;
using Managers.Abstract;
using Util;

namespace Managers
{
    public class HeartRateManager : Singleton<HeartRateManager>
    {
        /// <summary>
        /// A list of player data related to their heart rate.
        /// ListIndex equals DeviceIndex and PlayerIndex.
        /// </summary>
        private readonly List<PlayerHeartRateData> _playerData = new List<PlayerHeartRateData>();

        /// <summary>
        /// Subscribes to the HeartRateChange event. Event will be ignored if player was not added.
        /// </summary>
        private void OnEnable() => MiBandManager.OnHeartRateChange += OnHeartRateChange;

        private void OnDisable() => MiBandManager.OnHeartRateChange -= OnHeartRateChange;

        /// <summary>
        /// Call this when a player is added.
        /// Automatically stores and updates the heart rate data.
        /// </summary>
        public void AddPlayer() => _playerData.Add(new PlayerHeartRateData());

        /// <summary>
        /// Returns the heart rate delta of the given player(-index).
        /// </summary>
        /// <param name="playerIndex">The player index of the player</param>
        /// <returns>The heart rate delta of the player. Between 0 and 1.</returns>
        public float GetPlayerDelta(int playerIndex) => _playerData[playerIndex].HeartRateDelta;

        /// <summary>
        /// Returns the base heart rate of the given player(-index).
        /// </summary>
        /// <param name="playerIndex">The player index of the player</param>
        /// <returns>The base heart rate of the player (Here: highest measured heart rate).</returns>
        public int GetPlayerBaseHeartRate(int playerIndex) => _playerData[playerIndex].BaseHeartRate;

        /// <summary>
        /// Updates the heart rate data for the player.
        /// </summary>
        /// <param name="heartRateResponse">Heart Rate Response data</param>
        private void OnHeartRateChange(HeartRateResponse heartRateResponse)
        {
            if (_playerData.Count <= heartRateResponse.DeviceIndex) 
                return;
            
            PlayerHeartRateData playerData = _playerData[heartRateResponse.DeviceIndex];
            playerData.AddHeartRate(heartRateResponse.HeartRate);
        }

        /// <summary>
        /// Small class to store and update heart rate related data.
        /// </summary>
        public class PlayerHeartRateData
        {
            /// <summary>
            /// The base heart rate is the players "normal" heart rate.
            /// To decrease difficulty, this will be the highest measured average.
            /// </summary>
            public int BaseHeartRate { get; private set; }

            /// <summary>
            /// The dynamic (depending on the <see cref="BaseHeartRate"/>) difference between the base heart rate and
            /// the average heart rate.
            /// A value between 0 and 1.
            /// </summary>
            public float HeartRateDelta { get; private set; } = 1;
            private readonly LimitedList<int> _heartRates = new LimitedList<int>(10);

            public void AddHeartRate(int heartRate)
            {
                _heartRates.Add(heartRate);
                int avg = _heartRates.Average();
                // If average is higher than base, update base.
                BaseHeartRate = Math.Max(avg, BaseHeartRate);
                if(BaseHeartRate != 0)
                    HeartRateDelta = Math.Min(1, (float)avg / BaseHeartRate);
            }
        }
    }
}