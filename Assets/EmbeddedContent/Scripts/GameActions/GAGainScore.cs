using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    [CreateAssetMenu(fileName = "Gain Score", menuName = "Game Actions/Gain Score", order = 2)]
    public class GAGainScore : GameAction
    {
        public override void Execute()
        {
            int scoreAdded = 0;
            if (int.TryParse(_value, out scoreAdded))
            {
                GameManager.SetScore(GameManager.Instance.CurrentScore+scoreAdded);
            }
            else
            {
                Debug.LogError(this.name + "'s Value parameter is not a number!");
            }
        }
    }
}