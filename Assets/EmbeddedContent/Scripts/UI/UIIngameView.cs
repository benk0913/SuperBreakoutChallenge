using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SuperBreakout
{
    public class UIIngameView : MonoBehaviour
    {

        #region  Parameters

        public const float SCORE_INTERPOLATION_MULTIPLIER = 1f;

        [SerializeField]
        TextMeshProUGUI _scoreLabel;

        [SerializeField]
        TextMeshProUGUI _livesLabel;

        [SerializeField]
        TextMeshProUGUI _levelLabel;

        [SerializeField]
        UIJoystickController _joystick;

        static UIIngameView _instance;
        static bool _initialized;

        int _lastScore;

        #endregion

        #region Lifecycle

        void Awake()
        {
            _instance = this;
            _initialized = true;
            Util.SubscribeToEvent(Util.CommonEvents.GAME_LEVEL_LOADED, ShowUI);
            Util.SubscribeToEvent(Util.CommonEvents.MAIN_MENU_LOADED, HideUI);
            HideUI();
        }

        #endregion

        #region Public Interface

        public static void SetLives(int lives)
        {
            if (!_initialized)
            {
                Debug.LogError("Ingame UI View is not initialized!");
                return;
            }

            _instance._livesLabel.text = "Lives: " + lives.ToString("N0");
        }

        public static void SetLevel(int level)
        {
            if (!_initialized)
            {
                Debug.LogError("Ingame UI View is not initialized!");
                return;
            }

            _instance._levelLabel.text = "Level: " + (level + 1).ToString("N0");
        }

        public static void SetScore(int score)
        {
             if(!_initialized) 
            {
                Debug.LogError("Ingame UI View is not initialized!");   
                return;
            }

            if (!_instance.gameObject.activeInHierarchy)
            {
                _instance._scoreLabel.text = "Score: " + score.ToString("N0");
                _instance._lastScore = score;
                return;
            }

            if (_instance._interpolateToNumberInstance != null) _instance.StopCoroutine(_instance._interpolateToNumberInstance);

            _instance._interpolateToNumberInstance = _instance.StartCoroutine(_instance.InterpolateToNumber(_instance._lastScore, score));

            _instance._lastScore = score;
        }

        #endregion

        #region  Internal

        Coroutine _interpolateToNumberInstance;
        IEnumerator InterpolateToNumber(int fromScore, int toScore)
        {
            int currentScore = fromScore;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * SCORE_INTERPOLATION_MULTIPLIER;

                currentScore = Mathf.RoundToInt(Mathf.Lerp(currentScore, toScore, t));

                _scoreLabel.text = "Score: " + currentScore.ToString("N0");

                yield return 0;
            }

            _scoreLabel.text = "Score: " + currentScore.ToString("N0");
            _interpolateToNumberInstance = null;
        }

        void ShowUI()
        {
            this.gameObject.SetActive(true);
            _joystick.SetBoardToControl(BoardEntity.CurrentBoard);
        }

        void HideUI()
        {
            this.gameObject.SetActive(false);
        }

        #endregion
    }
}