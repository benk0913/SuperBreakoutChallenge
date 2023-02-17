using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    [CreateAssetMenu(fileName = "Extra Ball", menuName = "Game Actions/Extra Ball", order = 2)]
    public class GAExtraBall : GameAction
    {
        [SerializeField]
        string _ballPrefabKey;

        public override void Execute()
        {
            int count = 1;
            if (int.TryParse(_value, out count))
            {
                Vector2 spawn;
                if (BallEntity.BallsInSession.Count == 0)
                {
                    spawn = Vector2.zero;
                }
                else
                {
                    spawn = BallEntity.BallsInSession[0].transform.position;
                }

                for (int i = 0; i < count; i++)
                {
                    ResourcesManager.Instance.LoadObject(_ballPrefabKey, (GameObject loadedBall)=>{loadedBall.transform.position = spawn;});
                }
            }
            else
            {
                Debug.LogError(this.name + "'s Value parameter is not a number!");
            }
        }
    }
}