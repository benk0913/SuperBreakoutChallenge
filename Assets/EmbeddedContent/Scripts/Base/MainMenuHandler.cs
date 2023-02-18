using UnityEngine;

namespace SuperBreakout
{
    public class MainMenuHandler : MonoBehaviour
    {
        [SerializeField]
        AudioClip _mainMenuMusic;
        
        void Start()
        {
            SoundManager.Instance.SetMusic(_mainMenuMusic);
        }

        public void NewGame()
        {
            Util.InvokeEvent(Util.CommonEvents.NEW_GAME);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}