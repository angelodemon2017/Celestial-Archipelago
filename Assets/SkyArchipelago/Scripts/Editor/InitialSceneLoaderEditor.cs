using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class InitialSceneLoaderEditor
{
    private const string PREV_SCENE_KEY = "AutoLoader_PreviousScenePath";

    static InitialSceneLoaderEditor()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            var bootstrapName = Dicts.Scenes.BootStrapScene;
            if (activeScene.name != bootstrapName && activeScene.isLoaded)
            {
                EditorPrefs.SetString(PREV_SCENE_KEY, activeScene.path);
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string bootstrapPath = $"Assets/SkyArchipelago/Scenes/{bootstrapName}.unity";
                EditorSceneManager.OpenScene(bootstrapPath, OpenSceneMode.Single);
            }
        }

        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            string savedPath = EditorPrefs.GetString(PREV_SCENE_KEY, "");
            if (!string.IsNullOrEmpty(savedPath))
            {
                EditorApplication.delayCall += () =>
                {
                    EditorSceneManager.OpenScene(savedPath, OpenSceneMode.Single);
                    EditorPrefs.DeleteKey(PREV_SCENE_KEY);
                };
            }
        }
    }
}