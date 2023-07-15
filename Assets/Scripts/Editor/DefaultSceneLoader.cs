#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoadAttribute]
public static class DefaultSceneLoader {
    static DefaultSceneLoader() {
        EditorApplication.playModeStateChanged += LoadDefaultScene;
    }

    static void LoadDefaultScene(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingEditMode) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode && EditorSceneManager.GetActiveScene().buildIndex != 0) {
            if (EditorUtility.DisplayDialog("Load the default scene?", "Would you like to load the first scene in the project instead of the current one?", "Load first scene", "Stay on current scene")) {
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
#endif