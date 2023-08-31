using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HocaInk.InteractiveWall
{
    public class GameManager : MonoBehaviour
    {
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
