using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    [CreateAssetMenu(fileName = "Set Board Scale", menuName = "Game Actions/Set Board Scale", order = 2)]
    public class GASetBoardScale : GameAction
    {
        public override void Execute()
        {
            if (BoardEntity.CurrentBoard == null) return;

            float width = 0f;
            if (float.TryParse(_value, out width))
            {
                BoardEntity.CurrentBoard.SetWidth(width);
            }
            else
            {
                Debug.LogError(this.name + "'s Value parameter is not a number!");
            }
        }
    }
}