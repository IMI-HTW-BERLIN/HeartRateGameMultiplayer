using Managers.Abstract;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public bool SizeChange { get; set; }
        public bool SpeedChange { get; set; }
        
        
        public void LoadPongGame() => SceneManager.LoadScene(1);
        
        
    }
}