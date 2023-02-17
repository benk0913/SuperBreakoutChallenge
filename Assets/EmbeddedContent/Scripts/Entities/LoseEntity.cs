using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    public class LoseEntity : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag != BallEntity.BALL_TAG) return;

            GameManager.LoseLife();

            collider.GetComponent<BallEntity>().Respawn();
        }

    }
}
