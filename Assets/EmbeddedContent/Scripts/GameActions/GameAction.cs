using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    [CreateAssetMenu(fileName = "GameAction", menuName = "Game Actions/Game Action", order = 2)]
    public class GameAction : ScriptableObject
    {
        [SerializeField]
        protected string _value;

        public virtual void Execute()
        {
            //Does nothing
        }
    }
}