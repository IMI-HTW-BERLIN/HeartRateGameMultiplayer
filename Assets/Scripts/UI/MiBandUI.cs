using Data.ResponseTypes;
using LocalServer;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MiBandUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtHeartRate;

        private int _deviceIndex = -1;

        private void OnEnable() => MiBandManager.OnHeartRateChange += OnHeartRateChange;

        private void OnDisable() => MiBandManager.OnHeartRateChange -= OnHeartRateChange;

        public void SetDeviceIndex(int deviceIndex) => _deviceIndex = deviceIndex;

        private void OnHeartRateChange(HeartRateResponse heartRateResponse)
        {
            if(heartRateResponse.DeviceIndex != _deviceIndex)
                return;
            
            txtHeartRate.SetText(heartRateResponse.HeartRate.ToString());
        }
    }
}