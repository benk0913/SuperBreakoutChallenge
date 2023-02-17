using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    public class BallEntity : MonoBehaviour
    {
        #region  Parameters

        const float IMPACT_COOLDOWN = 0.1f;

        public const string BALL_TAG = "Ball";

        public static List<BallEntity> BallsInSession = new List<BallEntity>();

        [SerializeField]
        Rigidbody2D _rigidbody;

        [Tooltip("Base speed paramter onto all modifiers apply")]
        [SerializeField]
        float _baseSpeed = 1f;

        [Tooltip("Base scale paramter onto all modifiers apply")]
        [SerializeField]
        float _baseScale = 1f;


        [SerializeField]
        [Tooltip("Spawn this effect when I hit the wall")]
        GameObject ImpactEffect;

        [SerializeField]
        [Tooltip("Sound on ball hit")]
        AudioClip _impactSound;

        [SerializeField]
        [Tooltip("Respawn Sound")]
        AudioClip _respawnSound;


        float _modifiedScale = 1f;

        Vector2 _direction;

        float _impactCooldown;

        Vector2 _spawnPoint;

        float BallScale
        {
            get
            {
                return _baseScale * _modifiedScale;
            }
        }

        void Reset() // Not executed in playmode*
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }


        #endregion

        #region  FrameLifecycle

        void OnEnable()
        {
            RandomizeDirection();

            BallsInSession.Add(this);

            RefreshBallScale();

            _spawnPoint = transform.position;
        }

        void OnDisable()
        {
            BallsInSession.Remove(this);
        }

        void FixedUpdate()
        {
            _rigidbody.velocity = _direction * _baseSpeed;

            if (_impactCooldown > 0f) _impactCooldown -= Time.fixedDeltaTime;
        }

        #endregion

        #region Public Interface

        public void SetBallScale(float scale)
        {
            _modifiedScale = scale;
            RefreshBallScale();
        }

        #endregion

        #region  Behaviour

        void OnCollisionStay2D(Collision2D collision)
        {
            if (_impactCooldown > 0f) return;

            _impactCooldown = IMPACT_COOLDOWN;

            ContactPoint2D contactPoint = collision.GetContact(0);

            _direction = Vector2.Reflect(_direction, contactPoint.normal).normalized;

            ObjectPoolManager.GetObjectPool(ImpactEffect).transform.position = contactPoint.point;

            SoundManager.Instance.PlaySound(_impactSound);
        }

        void RandomizeDirection()
        {
            float startX = Random.Range(0, 2) == 0 ? 1f : -1f;
            float startY = Random.Range(0, 2) == 0 ? 1f : -1f;
            _direction = new Vector2(startX, startY);
        }

        private void RefreshBallScale()
        {
            transform.localScale = Vector3.one * BallScale;
        }

        internal void Respawn()
        {
            SoundManager.Instance.PlaySound(_respawnSound);
            transform.position = _spawnPoint;
            RandomizeDirection();
        }

        #endregion

    }
}