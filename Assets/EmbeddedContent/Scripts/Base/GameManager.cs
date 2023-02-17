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

        public int CurrentLevel {private set; get;}
        public int CurrentScore {private set; get;}
        public int CurrentLives {private set; get;}

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

            SceneManager.sceneLoaded += OnSceneLoaded;

            LoadScene(MAIN_MENU_SCENE);

            _isInitialized = true;
        }

        void NewGame()
        {
            SetLevel(0);
            SetScore(0);
            SetLives(1);

            LoadScene(_campaign.Levels[CurrentLevel].LevelSceneKey);
        }

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

                SetLives(CurrentLives = _campaign.Levels[CurrentLevel].Lives);
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

        #endregion

        public void SetLives(int lives)
        {
            CurrentLives = lives;
            UIIngameView.SetLives(CurrentLives);
        }

        public void SetScore(int score)
        {
            CurrentScore = score;
            UIIngameView.SetScore(CurrentScore);
        }

        public void SetLevel(int level)
        {
            CurrentLevel = level;
            UIIngameView.SetLevel(CurrentScore);
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
            public int Lives;
            public List<TimeLineEvent> TimeLineEvents = new List<TimeLineEvent>();
        }

        [System.Serializable]
        public class TimeLineEvent
        {
            public float TimeSinceLevelStart;
            public List<GameAction> ActionsOnEvent = new List<GameAction>();
        }

        #endregion
    }
}