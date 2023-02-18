using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuperBreakout
{
    public class GameManager : MonoBehaviour
    {
        #region  Parameters
        public const string MAIN_MENU_SCENE = "MainMenu";

        public static GameManager Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    Debug.LogError("Game Manager is not yet initialized!");

                    return null;
                }

                return _instance;
            }
        }
        static GameManager _instance;
        static bool _isInitialized;

        [SerializeField]
        public LevelCampaign _campaign;

        [SerializeField]
        AudioClip WinJingle;

        [SerializeField]
        AudioClip LoseJingle;

        [SerializeField]
        AudioClip _loseLifeSound;

        public int CurrentLevel { private set; get; }
        public int CurrentScore { private set; get; }
        public int CurrentLives { private set; get; }

        #endregion

        #region  Frame Lifecycle
        void Awake()
        {
            IntializeApplication();
        }
        #endregion

        #region  Internal

        void IntializeApplication()
        {
            _instance = this;

            DontDestroyOnLoad(this.gameObject);

            Application.targetFrameRate = 60;

            Util.SubscribeToEvent(Util.CommonEvents.NEW_GAME, NewGame);
            Util.SubscribeToEvent(Util.CommonEvents.BRICK_DESTROYED, OnBrickDestroyed);

            SceneManager.sceneLoaded += OnSceneLoaded;

            GoToMainMenu();

            _isInitialized = true;
        }

        #region Level Navigation

        void GoToMainMenu()
        {
            LoadScene(MAIN_MENU_SCENE);
        }

        void NewGame()
        {
            SetLevel(0);
            SetScore(0);
            SetLives(1);

            SetGameLevel(_campaign.Levels[CurrentLevel]);

        }

        void GoToNextLevel()
        {
            CurrentLevel++;

            if (_campaign.Levels.Count <= CurrentLevel)
            {
                GoToMainMenu();
                return;
            }

            SetGameLevel(_campaign.Levels[CurrentLevel]);
        }

        void SetGameLevel(LevelInstance levelInstance)
        {
            SetLives(levelInstance.Lives);
            
            BrickEntity.BricksInSession?.Clear();

            LoadScene(levelInstance.LevelSceneKey);

            if (!string.IsNullOrEmpty(levelInstance.BackgroundMusicKey))
            {
                UILoadingWindow.Instance.AddLoader(levelInstance.BackgroundMusicKey);
                ResourcesManager.Instance.LoadSound(levelInstance.BackgroundMusicKey, (AudioClip musicClip) =>
                {
                    UILoadingWindow.Instance.RemoveLoader(levelInstance.BackgroundMusicKey);
                    SoundManager.Instance.SetMusic(musicClip);
                });
            }
        }

        #endregion

        #region  Gamewide Scene Management
        void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
        {
            UILoadingWindow.Instance?.RemoveLoader(UILoadingWindow.CommonLoadOperations.LOAD_SCENE);

            if (loadedScene.name == MAIN_MENU_SCENE)
            {
                Util.InvokeEvent(Util.CommonEvents.MAIN_MENU_LOADED);
                StopTimelineHandler();
            }
            else
            {
                Util.InvokeEvent(Util.CommonEvents.GAME_LEVEL_LOADED);
                ResetTimelineHandler();
            }
        }

        void LoadScene(string targetScene)
        {
            UILoadingWindow.Instance?.AddLoader(UILoadingWindow.CommonLoadOperations.LOAD_SCENE);

            SceneManager.LoadScene(targetScene);
        }

        #endregion

        #region  Level Event Timeline
        void ResetTimelineHandler()
        {
            StopTimelineHandler();

            _timelineHandlerRoutineInstance = StartCoroutine(TimelineHandlerRoutine());
        }

        void StopTimelineHandler()
        {
            if (_timelineHandlerRoutineInstance != null)
            {
                StopCoroutine(_timelineHandlerRoutineInstance);
            }

            _timelineHandlerRoutineInstance = null;
        }

        Coroutine _timelineHandlerRoutineInstance;
        IEnumerator TimelineHandlerRoutine()
        {
            if (_campaign.Levels[CurrentLevel].TimeLineEvents.Count == 0)
            {
                _timelineHandlerRoutineInstance = null;
                yield break;
            }

            float sessionTime = 0f;
            int eventIndex = 0;
            float timeTillNextEvent = _campaign.Levels[CurrentLevel].TimeLineEvents[eventIndex].TimeSinceLevelStart;
            while (true)
            {
                sessionTime += Time.deltaTime;

                if (sessionTime >= timeTillNextEvent)
                {
                    TimeLineEvent timelineEvent = _campaign.Levels[CurrentLevel].TimeLineEvents[eventIndex];
                    timelineEvent.ActionsOnEvent.ForEach(x => x.Execute());

                    if (!string.IsNullOrEmpty(timelineEvent.NotificationText))
                    {
                        UINotificationView.ShowNotification(timelineEvent.NotificationText);
                    }

                    eventIndex++;
                    if (eventIndex >= _campaign.Levels[CurrentLevel].TimeLineEvents.Count)
                    {
                        break;
                    }
                    else
                    {
                        timeTillNextEvent = _campaign.Levels[CurrentLevel].TimeLineEvents[eventIndex].TimeSinceLevelStart;
                    }
                }

                yield return 0;
            }

            _timelineHandlerRoutineInstance = null;
        }

        static void ShowNotInitializedError()
        {
            Debug.LogError("Game Manager not initialized!");
        }

        #endregion

        public static void SetLives(int lives)
        {
            if (!_isInitialized)
            {
                ShowNotInitializedError();
                return;
            }

            Instance.CurrentLives = lives;
            UIIngameView.SetLives(Instance.CurrentLives);
        }

        public static void SetScore(int score)
        {
            if (!_isInitialized)
            {
                ShowNotInitializedError();
                return;
            }

            Instance.CurrentScore = score;
            UIIngameView.SetScore(Instance.CurrentScore);
        }

        public static void SetLevel(int level)
        {
            if (!_isInitialized)
            {
                ShowNotInitializedError();
                return;
            }

            Instance.CurrentLevel = level;
            UIIngameView.SetLevel(Instance.CurrentScore);
        }

        public static void LoseLife()
        {
            if (!_isInitialized)
            {
                ShowNotInitializedError();
                return;
            }


            SoundManager.Instance.PlaySound(Instance._loseLifeSound);

            SetLives(Instance.CurrentLives-1);

            if (Instance.CurrentLives <= 0)
            {
                Instance.LoseLevel();
            }
        }

        void OnBrickDestroyed()
        {
            
            Debug.LogError("DESTROYED "+BrickEntity.BricksInSession.Count+" bricks left");
            if (BrickEntity.BricksInSession == null) return;

            if (BrickEntity.BricksInSession.Count > 0) return;

            WinLevel();
        }

        void WinLevel()
        {
            if (_winLevelRoutineInstance != null) StopCoroutine(_winLevelRoutineInstance);

            _winLevelRoutineInstance = StartCoroutine(WinLevelRoutine());
        }

        Coroutine _winLevelRoutineInstance;
        IEnumerator WinLevelRoutine()
        {
            UINotificationView.ShowNotification("Level Complete!");

            SoundManager.Instance.PlaySound(WinJingle);
            Time.timeScale = 0.5f;

            yield return new WaitForSecondsRealtime(3f);

            Time.timeScale = 1f;

            GoToNextLevel();

            _loseLevelRoutineInstance = null;
        }

        void LoseLevel()
        {
            if (_loseLevelRoutineInstance != null) StopCoroutine(_loseLevelRoutineInstance);

            _loseLevelRoutineInstance = StartCoroutine(LoseLevelRoutine());
        }

        Coroutine _loseLevelRoutineInstance;
        IEnumerator LoseLevelRoutine()
        {
            UINotificationView.ShowNotification("Game Over");

            SoundManager.Instance.SetMusic(null);
            SoundManager.Instance.PlaySound(LoseJingle);
            Time.timeScale = 0.5f;

            yield return new WaitForSecondsRealtime(3f);

            Time.timeScale = 1f;

            GoToMainMenu();

            _loseLevelRoutineInstance = null;
        }



        #endregion



        #region Game Classes

        [System.Serializable]
        public class LevelCampaign
        {
            public List<LevelInstance> Levels = new List<LevelInstance>();
        }

        [System.Serializable]
        public class LevelInstance
        {
            public string LevelSceneKey;
            public string BackgroundMusicKey;
            public int Lives;
            public List<TimeLineEvent> TimeLineEvents = new List<TimeLineEvent>();
        }

        [System.Serializable]
        public class TimeLineEvent
        {
            public float TimeSinceLevelStart;
            public string NotificationText;
            public List<GameAction> ActionsOnEvent = new List<GameAction>();
        }

        #endregion
    }
}