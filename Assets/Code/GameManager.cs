using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HocaInk.InteractiveWall
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            AudioManager.instance.PlayMusic("BG1");
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
