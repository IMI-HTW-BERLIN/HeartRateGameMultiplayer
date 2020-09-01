using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MiBandManagerUI : MonoBehaviour
    {
        [SerializeField] private Button btnAddMiBand;
        [SerializeField] private Transform hlgMiBands;
        [SerializeField] private MiBandUI miBandUIPrefab;

        private int _numberOfMiBands;
        private void Awake()
        {
            btnAddMiBand.onClick.AddListener(OnAddMiBand);
        }

        private void OnAddMiBand()
        {
            StartCoroutine(MiBandManager.Instance.AddMiBand());
            Instantiate(miBandUIPrefab, hlgMiBands).SetDeviceIndex(_numberOfMiBands);
            _numberOfMiBands++;
        }
    }
}