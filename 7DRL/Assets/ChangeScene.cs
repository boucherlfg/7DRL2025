using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
  public void changeScene(){

      SceneManager.UnloadSceneAsync(gameObject.scene);
      MapNode.Exited.Invoke();
  }
}
