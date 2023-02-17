using UnityEngine;

namespace SuperBreakout
{
    public class MainMenuHandler : MonoBehaviour
    {
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