using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class CFindScenesToOpenEditor : EditorWindow {
    private static EditorWindow window;
    private const float windowWidth = 640f;
    private const float windowHeight = 280f;
        
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

    


    [MenuItem( "Tools/Find scene to open... %q", false, 180 )]
    public static void OpenWindow() {
        if (window != null) {
            CloseWindow();
            return;
        }
        redText.normal.textColor = Color.red;
        window = EditorWindow.GetWindow( typeof( CFindScenesToOpenEditor ) );
        window.position = new Rect(
            720 - windowWidth * .5f,
            540 - windowHeight * .5f,
            windowWidth,
            windowHeight
        );
        //window.CenterOnMainWin();
        window.titleContent = new GUIContent( "Find scene to open" );
        searchDirectory = Application.dataPath;
        isSearchingScenes = true;
        
        var info = new DirectoryInfo(searchDirectory);
        var filesInfos = info.GetFiles("*.unity", SearchOption.AllDirectories).OrderBy(p => p.LastAccessTimeUtc).ToArray();
        assetsScenesPaths = filesInfos.Select(fileInfo => fileInfo.FullName).ToArray();
        isSearchingScenes = false;
        window.Repaint();
    }
    
    private void OnLostFocus() {
        CloseWindow();
    }

    private void OnGUI() {

        this.ClampSelectedIndexValue();
        
        var e = Event.current;
        if (e.type == EventType.KeyDown) {

            // close window
            if (e.keyCode == KeyCode.Escape) {
                CloseWindow();
                return;
            }
            
            // open selected scene.
            if (e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Return) {
                if (this.HasSceneOnAssetsFolder()) {
                    if (e.shift) {
                        EditorSceneManager.OpenScene(this._filteredScenes[this.CurrentIndex], OpenSceneMode.Additive);
                    }else if (e.shift && e.alt) {
                        EditorSceneManager.OpenScene(this._filteredScenes[this.CurrentIndex], OpenSceneMode.AdditiveWithoutLoading);
                    }
                    else {
                        EditorSceneManager.OpenScene(this._filteredScenes[this.CurrentIndex], OpenSceneMode.Single);
                        CloseWindow();
                        return;
                    }
                }
            }
            
            // iterate over scenes list
            if (e.keyCode == KeyCode.UpArrow) {
                this.CurrentIndex -= 1;
            }else if (e.keyCode == KeyCode.DownArrow) {
                this.CurrentIndex += 1;
            }
        }
        

        EditorGUILayout.BeginVertical();

        
        GUI.SetNextControlName("searchFilter");
        searchFilter = EditorGUILayout.TextField(searchFilter);
        
        if (this.HasSceneOnAssetsFolder()) {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{assetsScenesPaths.Length} scenes found in project", EditorStyles.boldLabel);
            if(isSearchingScenes) EditorGUILayout.LabelField("Updating scenes search...");
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
                if(this._filteredScenes.Count != assetsScenesPaths.Length) EditorGUILayout.LabelField($"Found {this._filteredScenes.Count} scenes");

                this.scrollViewSize = EditorGUILayout.BeginScrollView(this.scrollViewSize, false, false );
                for (int i = 0; i < this._filteredScenes.Count; i++) {
                
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Open", EditorStyles.miniButton, GUILayout.Width(44))) {
                        EditorSceneManager.OpenScene(this._filteredScenes[i], OpenSceneMode.Single);
                    }
                    if (GUILayout.Button("Additive", EditorStyles.miniButton, GUILayout.Width(60))) {
                        EditorSceneManager.OpenScene(this._filteredScenes[i], OpenSceneMode.Additive);
                    }

                    if (this.CurrentIndex == i) {
                        EditorGUILayout.LabelField(TrimScenePath(this._filteredScenes[i]), EditorStyles.boldLabel);
                    }
                    else {
                        EditorGUILayout.LabelField(TrimScenePath(this._filteredScenes[i]));
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }

        }
        else {
            if (!isSearchingScenes) {
                EditorGUILayout.LabelField("Can't find any scene on project!", redText);
            }
            else {
                EditorGUILayout.LabelField("Searching scenes...");
            }
        }
        
        this.ClampSelectedIndexValue();

        EditorGUILayout.EndVertical();

        EditorGUI.FocusTextInControl("searchFilter");
        
    }

    private bool HasSceneOnAssetsFolder() {
        return assetsScenesPaths != null && assetsScenesPaths.Length > 0;
    }

    private bool HasFilteredScenes() {
        return this._filteredScenes.Count > 0;
    }

    private void ClampSelectedIndexValue() {
        this._currentIndex = Mathf.Clamp(this._currentIndex, 0, this._filteredScenes.Count);
    }

    private static string TrimScenePath(string fullPath) {
        return fullPath.Split('\\').Last().Replace(".unity", string.Empty).Trim();
    }

    private static bool IsEquivalentStrings(string stringOne, string stringTwo ) {
        return stringOne.ToLower().Contains(stringTwo.ToLower().Trim());
    }

    private static void CloseWindow() {
        if (window == null) return;
        window.Close();
    }
}
