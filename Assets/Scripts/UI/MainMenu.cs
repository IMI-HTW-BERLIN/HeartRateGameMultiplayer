using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Button btnPlay;
        [SerializeField] private Button btnExit;
        [SerializeField] private Toggle tglSizeChange;
        [SerializeField] private Toggle tglSpeedChange;
        [SerializeField] private Button btnStart;
        
        private static readonly int ShowOptions = Animator.StringToHash("ShowOptions");

        private void Awake()
        {
            btnPlay.onClick.AddListener(OnPlay);
            btnExit.onClick.AddListener(OnExit);
            btnStart.onClick.AddListener(OnStart);
        }

        private void OnPlay() => animator.SetTrigger(ShowOptions);

        private void OnExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnStart()
        {
            GameManager.Instance.SizeChange = tglSizeChange.isOn;
            GameManager.Instance.SpeedChange = tglSpeedChange.isOn;
            GameManager.Instance.LoadPongGame();
        }
    }
}