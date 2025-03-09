using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public void changeSceneTo(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
  public void unload(){

      SceneManager.UnloadSceneAsync(gameObject.scene);
      MapNode.Exited.Invoke();
  }
}
