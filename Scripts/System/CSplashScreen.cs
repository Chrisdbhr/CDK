using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK
{
    
    public class CSplashScreen : MonoBehaviour
    {
        [Header("System persistent aditive scene. Loaded first with important references.")]
        [SerializeField] CSceneField systemAditiveScene;
	
        [Header("Active scene will be the first of the list")]
        [SerializeField] CSceneField[] firstScenes;
	
        public CUnityEventFloat onLoadingProgressChanged;
	

        private IEnumerator Start()
        {
            Scene currentScene = this.gameObject.scene;

            SceneManager.LoadScene(this.systemAditiveScene, LoadSceneMode.Additive);

            CSceneField isFirstScene = null;

            foreach (var scene in this.firstScenes)
            {
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);

                if(isFirstScene == null){				
                    isFirstScene = scene;

                }
            }

            yield return null;

            var firstSceneName = SceneManager.GetSceneByName(isFirstScene);
            if(isFirstScene != null) SceneManager.SetActiveScene(firstSceneName);

            SceneManager.UnloadSceneAsync(currentScene);


        }
    }

}
