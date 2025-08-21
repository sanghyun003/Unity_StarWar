using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]
    private ChangeScene changeScene;
    public void RedSceneChange()
    {
        SceneManager.LoadScene("RedScene");
    }
    public void BlueSceneChange()
    {
        SceneManager.LoadScene("BlueScene");
    }
    public void YellowSceneChange()
    {
        SceneManager.LoadScene("YellowScene");
    }
}
