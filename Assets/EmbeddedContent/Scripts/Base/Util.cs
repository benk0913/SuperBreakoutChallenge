using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SuperBreakout
{
    public class Util
    {
        #region Dynamic Event Handling

        public class CommonEvents
        {
            public const string NEW_GAME = "New Game";
            public const string GAME_LEVEL_LOADED = "Game Level Loaded";
            public const string MAIN_MENU_LOADED = "Main Menu Loaded";
            public const string GAME_OVER = "Game Over";
            public const string BRICK_DESTROYED = "Brick Destroyed";
        }
        static Dictionary<string, UnityEvent> DynamicEvents = new Dictionary<string, UnityEvent>();

        public static void SubscribeToEvent(string eventKey, UnityAction action)
        {
            if (!DynamicEvents.ContainsKey(eventKey))
            {
                DynamicEvents.Add(eventKey, new UnityEvent());
            }

            DynamicEvents[eventKey].AddListener(action);
        }

        public static void UnsubscribeFromEvent(string eventKey, UnityAction action)
        {
            if (!DynamicEvents.ContainsKey(eventKey))
            {
                //EVENT DOESNT EXIST
                return;
            }

            DynamicEvents[eventKey].RemoveListener(action);
        }

        public static void InvokeEvent(string eventKey)
        {
            if (!DynamicEvents.ContainsKey(eventKey))
            {
                DynamicEvents.Add(eventKey, new UnityEvent());
            }

            DynamicEvents[eventKey].Invoke();
        }
        #endregion
    }
}