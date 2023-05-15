using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace CDK.Editor {
    public class CFindScenesToOpenEditor : EditorWindow {
               
        #region <<---------- Properties and Fields ---------->>

        private static EditorWindow window;
        private const float windowWidth = 400f;
        private const float windowHeight = 200f;

        private static string[] assetsScenesPaths;

        private List<string> _filteredScenes = new List<string>();
        private static bool isSearchingScenes;

        private static string searchFilter = "";
        private static string searchDirectory;
        private Vector2 scrollViewSize;

        public int CurrentIndex {
            get { return this._currentIndex; }
            set {
                this._currentIndex = value;
                this.ClampSelectedIndexValue();
            }
        }

        private int _currentIndex;

        private static GUIStyle redText = new GUIStyle();

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Mono Behaviour ---------->>

        [MenuItem("Tools/Find scene to open... %q", false, 180)]
        public static void OpenWindow() {
            if (window != null) {
                window.Close();
                return;
            }

            redText.normal.textColor = Color.red;
            window = GetWindow(typeof(CFindScenesToOpenEditor), true, "Find scene to open", true);
            window.minSize = new Vector2(windowWidth, windowHeight);
            // window.position = new Rect(
            //     720 - windowWidth * .5f,
            //     540 - windowHeight * .5f,
            //     windowWidth,
            //     windowHeight
            // );

            //window.CenterOnMainWin();
            window.titleContent = new GUIContent("Find scene to open");
            searchDirectory = Application.dataPath;
            isSearchingScenes = true;

            assetsScenesPaths = EditorBuildSettings.scenes.Where(bs => bs.enabled).Select(b => b.path.Replace('\\', '/')).ToArray();
            isSearchingScenes = false;
            window.Show();
            window.Repaint();
        }

        private void OnEnable() {
            window = this;
        }

        private void OnLostFocus() {
            this.Close();
        }

        private void OnGUI() {

            this.ClampSelectedIndexValue();

            var e = Event.current;
            if (e.type == EventType.KeyDown) {

                // close window
                if (e.keyCode == KeyCode.Escape) {
                    this.Close();
                    return;
                }

                // open selected scene.
                if (e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Return) {
                    if (this.HasSceneOnAssetsFolder()) {
                        if (e.shift) {
                            EditorSceneManager.OpenScene(this._filteredScenes[this.CurrentIndex], OpenSceneMode.Additive);
                        }
                        else if (e.shift && e.alt) {
                            EditorSceneManager.OpenScene(this._filteredScenes[this.CurrentIndex], OpenSceneMode.AdditiveWithoutLoading);
                        }
                        else {
                            EditorSceneManager.OpenScene(this._filteredScenes[this.CurrentIndex], OpenSceneMode.Single);
                            this.Close();
                            return;
                        }
                    }
                }

                // iterate over scenes list
                if (e.keyCode == KeyCode.UpArrow) {
                    this.CurrentIndex -= 1;
                }
                else if (e.keyCode == KeyCode.DownArrow) {
                    this.CurrentIndex += 1;
                }
            }


            EditorGUILayout.BeginVertical();


            GUI.SetNextControlName("searchFilter");
            searchFilter = EditorGUILayout.TextField(searchFilter);

            if (this.HasSceneOnAssetsFolder()) {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{assetsScenesPaths.Length} scenes included in Build Settings", EditorStyles.boldLabel);
                if (isSearchingScenes) EditorGUILayout.LabelField("Updating scenes search...");
                EditorGUILayout.EndHorizontal();

                this._filteredScenes.Clear();

                // filter scenes to show
                for (int i = 0; i < assetsScenesPaths.Length; i++) {
                    if (!IsEquivalentStrings(TrimScenePath(assetsScenesPaths[i]), searchFilter)) continue;
                    this._filteredScenes.Add(assetsScenesPaths[i]);
                }

                // show list result
                if (this._filteredScenes.Count <= 0) {
                    EditorGUILayout.LabelField("Can't find any scene with this filter");
                }
                else {
                    if (this._filteredScenes.Count != assetsScenesPaths.Length) EditorGUILayout.LabelField($"Found {this._filteredScenes.Count} scenes");

                    this.scrollViewSize = EditorGUILayout.BeginScrollView(this.scrollViewSize, false, false);
                    for (int i = 0; i < this._filteredScenes.Count; i++) {

                        EditorGUILayout.BeginHorizontal();

                        bool isSelected = (this.CurrentIndex == i);
                        if (GUILayout.Button("Additive", EditorStyles.miniButton, GUILayout.Width(60))) {
                            EditorSceneManager.OpenScene(this._filteredScenes[i], OpenSceneMode.Additive);
                        }

                        string buttonLabel = TrimScenePath(this._filteredScenes[i]);

                        if (GUILayout.Button((isSelected ? ("> " + buttonLabel + " <") : buttonLabel), EditorStyles.miniButton)) {
                            EditorSceneManager.OpenScene(this._filteredScenes[i], OpenSceneMode.Single);
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                }

            }
            else {
                if (!isSearchingScenes) {
                    EditorGUILayout.LabelField("Can't find any scene! Check if there is any included in BuildSettings", redText);
                }
                else {
                    EditorGUILayout.LabelField("Searching scenes...");
                }
            }

            this.ClampSelectedIndexValue();

            EditorGUILayout.EndVertical();

            EditorGUI.FocusTextInControl("searchFilter");

        }

        #endregion <<---------- Mono Behaviour ---------->>

        
        

        #region <<---------- General ---------->>

        private bool HasSceneOnAssetsFolder() {
            return assetsScenesPaths != null && assetsScenesPaths.Length > 0;
        }

        private bool HasFilteredScenes() {
            return this._filteredScenes.Count > 0;
        }

        private void ClampSelectedIndexValue() {
            this._currentIndex = Mathf.Clamp(this._currentIndex, 0, this._filteredScenes.Count - 1);
        }

        private static string TrimScenePath(string fullPath) {
            return fullPath.Split('/').Last().Replace(".unity", string.Empty).Trim();
        }

        private static bool IsEquivalentStrings(string stringOne, string stringTwo) {
            return stringOne.ToLower().Contains(stringTwo.ToLower().Trim());
        }

        private static string[] GetAllScenesInProject() {
            var info = new DirectoryInfo(searchDirectory);
            var filesInfos = info.GetFiles("*.unity", SearchOption.AllDirectories).OrderBy(p => p.LastAccessTimeUtc).ToArray();
            return filesInfos.Select(fileInfo => fileInfo.FullName).ToArray();
        }

        #endregion <<---------- General ---------->>
        
    }
}