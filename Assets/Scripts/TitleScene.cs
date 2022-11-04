using UnityEngine;
using UnityEngine.SceneManagement;
using GulchGuardians.Constants;

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
