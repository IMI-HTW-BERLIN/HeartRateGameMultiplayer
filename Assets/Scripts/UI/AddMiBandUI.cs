using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Adds a MiBand and shows the progress.
    /// </summary>
    public class AddMiBandUI : MonoBehaviour
    {
        [SerializeField] private Button btnAddMiBand;
        [SerializeField] private GameObject addMiBandContent;
        [SerializeField] private GameObject connectingMiBandContent;
        [SerializeField] private GameObject connectedMiBandContent;
        private void Awake() => btnAddMiBand.onClick.AddListener(() => StartCoroutine(OnAddMiBand()));

        /// <summary>
        /// Connects to the mi band with the deviceIndex provided as playerIndex. Switches contents to show progress.
        /// </summary>
        private IEnumerator OnAddMiBand()
        {
            addMiBandContent.SetActive(false);
            connectingMiBandContent.SetActive(true);
            yield return MiBandManager.Instance.ConnectToBand(MiBandManager.Instance.ConnectedBands);
            connectingMiBandContent.SetActive(false);
            connectedMiBandContent.SetActive(true);
            yield return MiBandManager.Instance.StartMeasurement(MiBandManager.Instance.ConnectedBands - 1);
        }
    }
}