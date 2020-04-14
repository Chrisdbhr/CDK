using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CDK
{
    
    public class CSplashScreen : MonoBehaviour
    {
        [Header("System persistent aditive scene. Loaded first with important references.")]
        [SerializeField] CSceneField systemAditiveScene;
	
        [Header("Active scene will be the first of the list")]
        [SerializeField] CSceneField[] firstScenes;
	
        public UnityEventFloat onLoadingProgressChanged;
	
        [System.Serializable]
        public class UnityEventFloat : UnityEvent<float>{}


        private IEnumerator Start()
        {
            Scene currentScene = gameObject.scene;

            SceneManager.LoadScene(systemAditiveScene, LoadSceneMode.Additive);

            CSceneField isFirstScene = null;

            foreach (var scene in firstScenes)
            {
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);

                if(isFirstScene == null){				
                    isFirstScene = scene;

                }
            }

            yield return null;

            var firstSceneName = SceneManager.GetSceneByName(isFirstScene);
            if(isFirstScene != null && firstSceneName != null) SceneManager.SetActiveScene(firstSceneName);

            SceneManager.UnloadSceneAsync(currentScene);


        }
    }

}
