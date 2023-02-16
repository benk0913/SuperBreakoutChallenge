using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuperBreakout
{
    public class GameManager : MonoBehaviour
    {
        public const string MAIN_MENU_SCENE = "MainMenu";

        void Awake()
        {
            IntializeApplication();
        }

        void IntializeApplication()
        {
            DontDestroyOnLoad(this.gameObject);

            Application.targetFrameRate = 60;

            SceneManager.LoadScene(MAIN_MENU_SCENE);
        }
    }
}