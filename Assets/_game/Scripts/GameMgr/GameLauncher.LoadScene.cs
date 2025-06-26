using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public partial class GameLauncher
{
    private bool isSceneLoaded = false;
    private string sceneLoadedName;
    private Scene scene;

    public static class SceneName
    {
        public const string Main = "SceneMain";
        public const string Battle = "SceneBattle";
    }
    
    public async UniTask<Scene> LoadScene(string sceneName)
    {
        isSceneLoaded = false;
        sceneLoadedName = sceneName;
        
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        await UniTask.WaitUntil(() => isSceneLoaded);
        return scene;
    }

    //Init once
    private void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded += (arg0, mode) =>
        {
            if (arg0.name == sceneLoadedName)
            {
                isSceneLoaded = true;
                scene = arg0;
            }
        };
    }
    
}
