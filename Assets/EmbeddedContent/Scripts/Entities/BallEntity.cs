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

        [SerializeField]
        [Tooltip("Should the ball go to a random direction when it spawns?")]
        bool _isRandomDirectionAtSpawn;

        [SerializeField]
        [Tooltip("Should the respawn when lost?")]
        bool _isRespawning = true;



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

            BallsInSession.Add(this);

            RefreshBallScale();

            _spawnPoint = transform.position;

            ResetDirection();
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

        public void Respawn()
        {
            if (!_isRespawning)
            {
                this.gameObject.SetActive(false);
                return;
            }

            SoundManager.Instance.PlaySound(_respawnSound);
            transform.position = _spawnPoint;
            ResetDirection();
        }

        #endregion

        #region  Behaviour

        void OnCollisionStay2D(Collision2D collision)
        {
            if (_impactCooldown > 0f) return;

            _impactCooldown = IMPACT_COOLDOWN;

            ContactPoint2D contactPoint = collision.GetContact(0);

            _direction = Vector2.Reflect(_direction, contactPoint.normal).normalized + new Vector2(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));

            ObjectPoolManager.GetObjectPool(ImpactEffect).transform.position = contactPoint.point;

            SoundManager.Instance.PlaySound(_impactSound);
        }


        void ResetDirection()
        {
            if (_isRandomDirectionAtSpawn)
            {
                RandomizeDirection();
            }
            else
            {
                _direction = new Vector2(1f, 1f);
            }
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



        #endregion

    }
}