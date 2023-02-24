using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    public void SetScene(string _scene)
    {
        SceneManager.LoadScene(_scene);
    }
}
