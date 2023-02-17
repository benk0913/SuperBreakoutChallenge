using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    [CreateAssetMenu(fileName = "Slowmo", menuName = "Game Actions/Slowmo", order = 2)]
    public class GASlowmo : GameAction
    {
        public override void Execute()
        {
            if(GameManager.Instance == null) return;

            float time = 0f;
            if (float.TryParse(_value, out time))
            {
                if(_currentSlowmoSession != null) GameManager.Instance.StopCoroutine(_currentSlowmoSession);

                _currentSlowmoSession = GameManager.Instance.StartCoroutine(SlowmoSession(time));
            }
            else
            {
                Debug.LogError(this.name + "'s Value parameter is not a number!");
            }
        }

        static Coroutine _currentSlowmoSession;

        IEnumerator SlowmoSession(float duration)
        {
            Time.timeScale = 0.5f;
            yield return new WaitForSeconds(duration);
            Time.timeScale = 1f;

            _currentSlowmoSession = null;
        }
    }
}