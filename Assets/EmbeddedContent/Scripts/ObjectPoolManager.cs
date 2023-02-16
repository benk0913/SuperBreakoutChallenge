using System.Collections.Generic;
using UnityEngine;


namespace SuperBreakout
{
    public class ObjectPoolManager : MonoBehaviour
    {
        #region  Parameters

        [SerializeField]
        List<GameObject> _instantiatableObjects = new List<GameObject>();

        public static Dictionary<string, List<GameObject>> ObjectPool = new Dictionary<string, List<GameObject>>();

        static List<GameObject> _preloadedObjects;
        #endregion

        #region  FrameLifecycle

        private void Awake()
        {
            _preloadedObjects = _instantiatableObjects;
        }
        #endregion

        #region  Functionality

        public static GameObject GetObjectPool(GameObject objectKey)
        {
            CreateEntry(objectKey.name);

            GameObject foundObject = ObjectPool[objectKey.name].Find(X => !X.activeInHierarchy);

            if (foundObject == null)
            {
                foundObject = Instantiate(objectKey);
                foundObject.name = objectKey.name;
            }

            return foundObject;
        }

        public static GameObject GetObjectPool(string objectKey)
        {
            CreateEntry(objectKey);

            GameObject foundObject = ObjectPool[objectKey].Find(X => !X.activeInHierarchy);

            if (foundObject == null)
            {
                GameObject prefab = _preloadedObjects.Find(x => x.name == objectKey);

                if (prefab == null)
                {
                    Debug.LogError("NO AVAILABLE WITH KEY " + objectKey);
                    return null;
                }

                foundObject = Instantiate(prefab);
                foundObject.name = objectKey;
            }

            return foundObject;
        }

        #endregion

        #region  Internal
        static void CreateEntry(string objectKey)
        {
            if (!ObjectPool.ContainsKey(objectKey))
            {
                ObjectPool.Add(objectKey, new List<GameObject>());
            }
        }

        #endregion
    }
}