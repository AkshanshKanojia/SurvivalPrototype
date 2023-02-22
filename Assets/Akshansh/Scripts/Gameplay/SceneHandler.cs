using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SceneHandler : NetworkBehaviour
{
    public void SetScene(string _scene)
    {
        NetworkManager.SceneManager.LoadScene(_scene, LoadSceneMode.Single);
    }
}
