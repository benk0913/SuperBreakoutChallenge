using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    [CreateAssetMenu(fileName = "Set Ball Scale", menuName = "Game Actions/Set Ball Scale", order = 2)]
    public class GASetBallScale : GameAction
    {
        public override void Execute()
        {
            float scale = 0f;
            if (float.TryParse(_value, out scale))
            {
                BallEntity.BallsInSession.ForEach(x=>x.SetBallScale(scale));
            }
            else
            {
                Debug.LogError(this.name + "'s Value parameter is not a number!");
            }
        }
    }
}