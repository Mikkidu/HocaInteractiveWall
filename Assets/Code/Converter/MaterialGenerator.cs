using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace HocaInk.InteractiveWall
{


    public class MaterialGenerator : MonoBehaviour
    {
        /*[DllImport("__Internal")]
        private static extern void ImageUploaderCaptureClick();*/

        [DllImport("__Internal")]
        private static extern void UploadFile(string gameObjectName, string methodName);


        [SerializeField] private Shader _shader;
        [SerializeField] private Material _material;
        [SerializeField] private UnitSpawner _spawnManager;
        [SerializeField] private Material _webGLMaterial;

        public Texture2D _textureTest;

        private FileSystemWatcher _watcher;
        private List<string> _filePaths = new List<string>();
        private ImageConverter _converter;
        private string _path = "c:/InteractiveSoftware/Scans";
        private bool _isREadyForImport = true;


        void Start()
        {
            Debug.Log("Start Debug");
            _converter = new ImageConverter(this);
            _converter.StartConversion();
            if (!Directory.Exists(_path))
            {
                try
                {
                    Directory.CreateDirectory(_path);
                    Debug.Log("Directory Created");
                }
                catch (System.Exception e)
                {
                    Debug.Log("Filed oto create directory " + e.Message);
                }
            }
            _watcher = new FileSystemWatcher(_path);
            _watcher.NotifyFilter = NotifyFilters.DirectoryName |
                                    NotifyFilters.FileName;
            _watcher.Created += OnCreated;
            _watcher.Error += OnError;
            _watcher.Filter = "*.jpg";
            _watcher.EnableRaisingEvents = true;
        }

        void Update()
        {
            
            if(_filePaths.Count > 0 & _isREadyForImport)
            {
                string picturePath = _filePaths[0];
                _isREadyForImport = false;
                //_converter.GenerateTexture(picturePath);
                //_converter.LoadImageFromUrl(picturePath);
                //StartCoroutine(_converter.LoadImageFromUrl(picturePath));
                _converter.LoadImageFromUrlTest(picturePath).Forget();
                _filePaths.Remove(picturePath);
            }
        }


        private void OnCreated(object sender, FileSystemEventArgs ea)
        {
            try
            {
                _filePaths.Add(ea.FullPath);
            }
            catch(System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void OnButtonPointerDown()
        {
#if UNITY_EDITOR
            string path = UnityEditor.EditorUtility.OpenFilePanel("Open image", "", "jpg,png,bmp");
            if (!System.String.IsNullOrEmpty(path))
                _filePaths.Add("file:///" + path);
            //FileSelected("file:///" + path);
#else
        UploadFile(gameObject.name, "FileSelected");
        //ImageUploaderCaptureClick ();
#endif
        }

        public void FileSelected(string url)
        {
            Debug.Log("File selected:" + url);
            _filePaths.Add(url);
        }

        public void CreateMaterial(Texture2D texture)
        {
            var t1 = System.Environment.TickCount;
            if (texture == null) return;
            var newMaterial = new Material(_material);
            newMaterial.mainTexture = texture;
            newMaterial.name = texture.name;
            _spawnManager.AddMaterial(newMaterial, GetVehicleType(texture.name));
            _isREadyForImport = true;
            Debug.Log($"{texture.name}.material. Delta counts {System.Environment.TickCount - t1}");
        }

        public void CreateMaterial(Texture2D texture, Vector2 center, int alightAngle, float correctAngle, float tilling)
        {
            Debug.Log($"Spawn: {texture.name}, center {center}, alight {alightAngle}, angle {correctAngle}, talling {tilling}");
            var t1 = System.Environment.TickCount;
            if (texture == null) return;
            var newMaterial = new Material(_webGLMaterial);
            newMaterial.SetTexture("_Texture", texture);
            newMaterial.SetVector("_OffsetCenter", center);
            newMaterial.SetFloat("_RotateOrientation", alightAngle);
            newMaterial.SetFloat("_ScaleFactor", tilling);
            newMaterial.SetFloat("_OffsetAngle", correctAngle);
            newMaterial.name = texture.name;
            _spawnManager.AddMaterial(newMaterial, GetVehicleType(texture.name));
            _isREadyForImport = true;
            Debug.Log($"{texture.name}.material. Delta counts {System.Environment.TickCount - t1}");
        }

        private void OnError(object sender, ErrorEventArgs ea)
        {
            Debug.Log(ea.GetException().Message);
        }

        private VehicleType GetVehicleType(string pictureName)
        {
            var retVehicleType = VehicleType.Plane;

            switch (pictureName.Substring(0, 3))
            {
                case "tnk":
                    retVehicleType = VehicleType.Tank;
                    break;
                case "bot":
                    retVehicleType = VehicleType.Boat;
                    break;
                case "pln":
                    retVehicleType = VehicleType.Plane;
                    break;
                case "sub":
                    retVehicleType = VehicleType.SubMarine;
                    break;
                case "hel":
                    retVehicleType = VehicleType.Helicopter;
                    break;
                case "can":
                    retVehicleType = VehicleType.Cannon;
                    break;

            }
            return retVehicleType;
        }

        private void OnDisable()
        {
            _watcher.Created -= OnCreated;
            _watcher.Error -= OnError;
        }
    }


}
