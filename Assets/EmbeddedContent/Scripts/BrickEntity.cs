using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SuperBreakout
{
    public class BrickEntity : MonoBehaviour
    {
        public const string BALL_TAG = "Ball";

        #region  Parameters
        [SerializeField]
        SpriteRenderer _renderer;

        [SerializeField]
        [Tooltip("Spawn this effect when I break")]
        GameObject BreakEffect;

        [Tooltip("The brick will set the sprite that is corresponding with its % of HP ")]
        [SerializeField]
        List<Sprite> _damageProgressSprites = new List<Sprite>();

        [SerializeField]
        int _MaxHP;

        int _currentHP;

        #endregion

        #region Frame Lifecycle

        void Start()
        {
            Initialize();
        }



        #endregion

        #region  Behaviour


        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag != BALL_TAG) return;

            _currentHP--;

            if (_currentHP == 0)
            {
                Break();
            }
            else
            {
                RefrshHPState();
            }
        }

        private void Break()
        {
            ObjectPoolManager.GetObjectPool(BreakEffect).transform.position = transform.position;
            this.gameObject.SetActive(false);
        }

        private void Initialize()
        {
            _currentHP = _MaxHP;
            RefrshHPState();
        }

        private void RefrshHPState()
        {
            _renderer.sprite = _damageProgressSprites[Mathf.RoundToInt(Mathf.Lerp(0, _damageProgressSprites.Count - 1, (float)_currentHP / _MaxHP))];
        }

        #endregion
    }
}
