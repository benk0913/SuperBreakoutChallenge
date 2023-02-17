using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SuperBreakout
{
    public class UIJoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region  Parameters 

        [SerializeField]
        Slider _joystickSlider;

        [SerializeField]
        float _returnSpeed = 1f;

        [SerializeField][Tooltip("Usually set automatically by the game system, but serialized for optional testing.")]
        BoardEntity _boardToControl;


        bool _isTouched;

        void Reset() // Not executed in playmode*
        {
            _joystickSlider = GetComponent<Slider>();
        }


        #endregion

        #region Frame Lifecycle

        void Update()
        {
            if (_isTouched)
            {
                if (_joystickSlider.value > 0.5f) // Move right
                {
                    _boardToControl?.MoveRight(Mathf.InverseLerp(0.5f, 1f, _joystickSlider.value));
                }
                else                            //Move left
                {
                    _boardToControl?.MoveLeft(Mathf.InverseLerp(0.5f, 0f, _joystickSlider.value));
                }
            }
            else
            {
                _joystickSlider.value = Mathf.Lerp(_joystickSlider.value, 0.5f, Time.deltaTime * _returnSpeed);
            }
        }

        #endregion

        #region  Public Interface


        public void OnPointerDown(PointerEventData eventData)
        {
            _isTouched = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isTouched = false;
        }

        public void SetBoardToControl(BoardEntity board)
        {
            _boardToControl = board;
        }


        #endregion
    }
}
