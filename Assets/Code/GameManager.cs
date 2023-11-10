using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace HocaInk.InteractiveWall
{


    public class GameManager : MonoBehaviour
    {
        public static string _curDir;

        private void Start()
        {
            AudioManager.instance.PlayMusic("BG1");
        }

        private void OnApplicationQuit()
        {
            System.Environment.CurrentDirectory = _curDir;
            UnityEngine.Debug.Log("TryToKill");
            try
            {
                Process[] proc = Process.GetProcessesByName("DavinciLaunch");
                proc[0].Kill();
                UnityEngine.Debug.Log("Succsess!");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e.Message + " Cant Kill the procces");
            }
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
