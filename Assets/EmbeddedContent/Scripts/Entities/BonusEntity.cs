using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    public class BonusEntity : MonoBehaviour
    {
        [SerializeField][Tooltip("The possible things that may happen when the bonus is obtained")]
        List<GameAction> _possibleBonuses = new List<GameAction>();

        void OnTriggerEnter2D(Collider2D otherCollider)
        {
            if(otherCollider.tag != BoardEntity.BOARD_TAG) return;

            _possibleBonuses[Random.Range(0,_possibleBonuses.Count)].Execute();

            this.gameObject.SetActive(false);
        }
    }
}
