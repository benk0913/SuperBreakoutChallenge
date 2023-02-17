using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    public class BoardEntity : MonoBehaviour
    {
        const float BASE_BOARD_WIDTH = 1.365394f;
        const float BASE_BOARD_HEIGHT = 0.2928521f;

        public const string BOARD_TAG = "Board";

        public static BoardEntity? CurrentBoard;

        #region  Parameters 

        [SerializeField]
        Rigidbody2D _rigidbody;

        [SerializeField]
        BoxCollider2D _collider;

        [SerializeField]
        SpriteRenderer _renderer;

        [SerializeField][Tooltip("Base speed paramter onto all modifiers apply")]
        float _baseSpeed = 1f;

        float CurrentWidth = 1f;

        void Reset() // Not executed in playmode*
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        #endregion

        #region  Frame Lifecycle

        void Awake()
        {
            CurrentBoard = this;
        }

        #endregion

        #region Public Interface
        public void MoveLeft(float joystickValue = 1f)
        {
            _rigidbody.position += -Vector2.right * _baseSpeed * joystickValue * Time.deltaTime;
        }

        public void MoveRight(float joystickValue = 1f)
        {
            _rigidbody.position += Vector2.right * _baseSpeed * joystickValue * Time.deltaTime;
        }

        public void SetWidth(float scale = 1f)
        {
            _renderer.size = new Vector2(BASE_BOARD_WIDTH * scale, BASE_BOARD_HEIGHT);
            _collider.size = _renderer.size;
        }



        #endregion
    }

}