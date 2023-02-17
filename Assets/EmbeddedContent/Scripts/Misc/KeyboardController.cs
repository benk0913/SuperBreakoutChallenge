using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    public class KeyboardController : MonoBehaviour
    {
        [SerializeField]
        BoardEntity _boardToControl;

        void Update()
        {
            if(Input.GetKey(KeyCode.RightArrow)) _boardToControl.MoveRight();
            else if(Input.GetKey(KeyCode.LeftArrow)) _boardToControl.MoveLeft();
        }

    }
}
