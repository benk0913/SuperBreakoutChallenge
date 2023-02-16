using UnityEngine;

namespace SuperBreakout
{
    public class BallEntity : MonoBehaviour
    {
        #region  Parameters 

        [SerializeField]
        Rigidbody2D _rigidbody;

        [Tooltip("Base speed paramter onto all modifiers apply")]
        [SerializeField]
        float _baseSpeed = 1f;

        [SerializeField]
        GameObject ImpactEffect;

        Vector2 _direction;

        #endregion

        #region  FrameLifecycle

        void Start()
        {
            RandomizeDirection();
        }

        void FixedUpdate()
        {
            _rigidbody.velocity = _direction * _baseSpeed;
        }

        #endregion

        #region  Behaviour

        void OnCollisionEnter2D(Collision2D collision)
        {
            ContactPoint2D contactPoint = collision.GetContact(0);

            _direction = Vector2.Reflect(_direction, contactPoint.normal).normalized;

            ObjectPoolManager.GetObjectPool(ImpactEffect).transform.position = contactPoint.point;
        }




        void RandomizeDirection()
        {
            float startX = Random.Range(0, 2) == 0 ? 1f : -1f;
            float startY = Random.Range(0, 2) == 0 ? 1f : -1f;
            _direction = new Vector2(startX, startY);
        }
     
        #endregion

    }
}