using UnityEngine.SceneManagement;

public struct LoadSceneSignal
{
    public string SceneName { get; }
    public bool ShowLoadingScreen { get; }
    public LoadSceneMode Mode { get; }

    public LoadSceneSignal(string sceneName, bool showLoading = true, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneName = sceneName;
        ShowLoadingScreen = showLoading;
        Mode = mode;
    }
}

public struct SceneLoadedSignal
{
    public string SceneName { get; }

    public SceneLoadedSignal(string sceneName)
    {
        SceneName = sceneName;
    }
}

public struct SceneInstalledSignal
{

}

public struct SceneLoadingProgressSignal
{
    public float Progress { get; }

    public SceneLoadingProgressSignal(float progress)
    {
        Progress = progress;
    }
}