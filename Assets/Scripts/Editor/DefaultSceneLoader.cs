#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class DefaultSceneLoader {
    static DefaultSceneLoader() {
        EditorApplication.playModeStateChanged += LoadDefaultScene;
    }

    static void LoadDefaultScene(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingEditMode) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode && SceneManager.GetActiveScene().buildIndex != 0) {
            if (EditorUtility.DisplayDialog("Load the first scene?", "Would you like to load the first scene in the build order instead of the current one?", "Load first scene", "Stay on current scene")) {
                foreach (var DontDestroyOnLoadObject in GetDontDestroyOnLoadObjects()) {
                    Object.DestroyImmediate(DontDestroyOnLoadObject);
                }

                SceneManager.LoadScene(0);
            }
        }
    }

    public static GameObject[] GetDontDestroyOnLoadObjects() {
        GameObject temp = null;
        try {
            temp = new GameObject();
            Object.DontDestroyOnLoad(temp);
            Scene dontDestroyOnLoad = temp.scene;
            Object.DestroyImmediate(temp);
            temp = null;

            return dontDestroyOnLoad.GetRootGameObjects();
        } finally {
            if (temp != null)
                Object.DestroyImmediate(temp);
        }
    }

}
#endif