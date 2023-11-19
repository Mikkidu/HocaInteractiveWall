using UnityEngine;
using TMPro;

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {/*
#if !UNITY_EDITOR
        
        static string myLog = "";
        private string output;
        private string stack;
        [SerializeField] private TextMeshProUGUI _console;
        [SerializeField] private bool needConsole = false;

        private void Start()
        {
            if (!needConsole) return;
            _console.transform.parent.gameObject.SetActive(true);
        }

        void OnEnable()
        {
            if (!needConsole) return;
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            if (!needConsole) return;
            output = logString;
            stack = stackTrace;
            myLog = output + stack + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }
        void OnGUI()
        {
            if (!needConsole) return;
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            {
                _console.text = myLog;

            }
        }
#endif*/
    }
}
