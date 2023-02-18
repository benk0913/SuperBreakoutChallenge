using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    public class BonusEntity : MonoBehaviour
    {
        [SerializeField][Tooltip("The possible things that may happen when the bonus is obtained")]
        List<GameAction> _possibleBonuses = new List<GameAction>();

        [SerializeField]
        float _existanceDuration = 10f;

        void Start()
        {
            StartCoroutine(DeathCooldown()); //No need to handle multiple invokations because they can never happen.
        }

        IEnumerator DeathCooldown()
        {
            yield return new WaitForSeconds(_existanceDuration);
            this.gameObject.SetActive(false);    
        }

        void OnTriggerEnter2D(Collider2D otherCollider)
        {
            if(otherCollider.tag != BoardEntity.BOARD_TAG) return;

            GameAction bonus = _possibleBonuses[Random.Range(0,_possibleBonuses.Count)];
            bonus.Execute();

            UINotificationView.ShowNotification(bonus.name);

            this.gameObject.SetActive(false);
        }
    }
}
