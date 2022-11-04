using UnityEngine;
using UnityEngine.SceneManagement;

namespace GulchGuardians
{
    public class TitleScene : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene(Constants.Scene.MainScene);
        }
    }
}
