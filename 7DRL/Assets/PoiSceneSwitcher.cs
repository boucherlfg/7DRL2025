using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoiSceneSwitcher : MonoSingleton<PoiSceneSwitcher>
{
    [SerializeField] private string farmScene;
    [SerializeField] private string cityScene;


    public void SwitchScenes(NodeType obj)
    {
        switch (obj)
        {
            case NodeType.Ruins:
                break;
            case NodeType.City:
                MapNode.Entered?.Invoke(NodeType.City);
                SceneManager.LoadScene(cityScene, LoadSceneMode.Additive);
                break;
            case NodeType.Farm:
                MapNode.Entered?.Invoke(NodeType.Farm);
                SceneManager.LoadScene(farmScene, LoadSceneMode.Additive);
                
                break;
            // case NodeType.Dungeon:
            //     break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
        }
    }
}
