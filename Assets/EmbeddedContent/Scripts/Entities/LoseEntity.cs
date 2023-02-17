using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    public class LoseEntity : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag != BallEntity.BALL_TAG) return;

            GameManager.LoseLife();

            collision.collider.GetComponent<BallEntity>().Respawn();
        }

    }
}
