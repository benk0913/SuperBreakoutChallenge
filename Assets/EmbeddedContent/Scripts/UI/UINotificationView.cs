using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperBreakout
{
    public class UINotificationView : MonoBehaviour
    {
        #region Parameters
        public static UINotificationView Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    Debug.LogError("UINotificationView is not yet initialized!");

                    return null;
                }

                return _instance;
            }
        }
        static UINotificationView _instance;
        static bool _isInitialized;


        List<NotificationInstance> _notificationQue = new List<NotificationInstance>();

        [SerializeField]
        CanvasGroup _canvasGroup;

        [SerializeField]
        TextMeshProUGUI _notificationLabel;

        [SerializeField]
        float _notificationDuration = 4f;

        [SerializeField]
        AudioClip _notificationSound;

        #endregion

        #region Frame Lifecycle

        private void Awake()
        {
            _instance = this;
            _isInitialized = true;
        }

        #endregion

        #region Public Interface
        public static void ShowNotification(string content)
        {
            ShowNotification(new NotificationInstance(content));
        }

        public static void ShowNotification(NotificationInstance instance)
        {
            if (!_isInitialized)
            {
                Debug.LogError("UINotificationView not initialized ");
                return;
            }

            if (_instance._notificationQue.Find(x => x.Content == instance.Content) != null) return;

            if (_instance._notificationRoutineInstance != null)
            {
                _instance._notificationQue.Add(instance);
                return;
            }

            _instance._notificationRoutineInstance = _instance.StartCoroutine(_instance.NotificationRoutine(instance));
        }

        public static void ShowNextNotification()
        {
            if (!_isInitialized)
            {
                Debug.LogError("UINotificationView not initialized ");
                return;
            }

            if (_instance._notificationQue.Count <= 0)
            {
                return;
            }

            _instance._notificationRoutineInstance = _instance.StartCoroutine(_instance.NotificationRoutine(_instance._notificationQue[0]));
            _instance._notificationQue.RemoveAt(0);
        }

        #endregion

        #region Internal

        Coroutine _notificationRoutineInstance;
        IEnumerator NotificationRoutine(NotificationInstance notification)
        {
            _notificationLabel.text = notification.Content;

            SoundManager.Instance.PlaySound(_notificationSound);

            _canvasGroup.alpha = 0f;
            while (_canvasGroup.alpha < 1f)
            {
                _canvasGroup.alpha += 2f * Time.deltaTime;


                yield return 0;
            }

            yield return new WaitForSeconds(_notificationDuration);


            _canvasGroup.alpha = 1f;
            while (_canvasGroup.alpha > 0f)
            {
                _canvasGroup.alpha -= 2f * Time.deltaTime;

                yield return 0;
            }

            _notificationRoutineInstance = null;
            ShowNextNotification();
        }

        #endregion

    }

    #region Dataobjects

    [System.Serializable] //Open to adding "Icons / other data in the future"
    public class NotificationInstance
    {
        public string Content;
        public NotificationInstance(string content)
        {
            this.Content = content;
        }
        #endregion
    }
}