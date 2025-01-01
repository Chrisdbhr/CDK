using System;
using System.Collections;
using UnityEngine;

namespace CDK
{
    public class StaticRoutinesRunner : MonoBehaviour
    {
        static StaticRoutinesRunner _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            _instance = GameObjectCreate.WithComponent<StaticRoutinesRunner>();
            DontDestroyOnLoad(_instance);
        }

        public static Coroutine RunRoutine(IEnumerator routine)
        {
            return _instance.StartCoroutine(routine);
        }

    }
}