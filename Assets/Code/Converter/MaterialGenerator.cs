using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
#if UNITY_WEBGL
using Cysharp.Threading.Tasks;
#endif


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
            _converter = new ImageConverter(this);
#if !UNITY_WEBGL
            StartFileWatcher();
#endif
        }

        void Update()
        {
            if(_filePaths.Count > 0 & _isREadyForImport)
            {
                string picturePath = _filePaths[0];
                _isREadyForImport = false;
                _converter.GenerateTexture(picturePath);
                _filePaths.Remove(picturePath);
            }
        }

        

        /// <summary>
        /// Call select file window for chose image
        /// </summary>
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

        

#if UNITY_WEBGL

        /// <summary>
        /// Create material int WebGL project with custom shader with image transforming
        /// </summary>
        /// <param name="texture">Square texture with taged name</param>
        /// <param name="center">Center of drawing from up left corner</param>
        /// <param name="alightAngle">Angle to change aligment</param>
        /// <param name="correctAngle">Image tilt angle</param>
        /// <param name="tilling">Correction dor the scale of the scanned image</param>
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

        /// <summary>
        /// Get url to image file and save it
        /// </summary>
        /// <param name="url"></param>
        public void FileSelected(string url)
        {
            Debug.Log("File selected:" + url);
            _filePaths.Add(url);
        }

#else
        /// <summary>
        /// Check new images in the folder
        /// </summary>
        private void StartFileWatcher()
        {
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

       /// <summary>
        /// Create material in PC project with toon material
        /// </summary>
        /// <param name="texture">Transformed square texture with taged name</param>
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

        /// <summary>
        /// Catch file file path on file created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
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

        /// <summary>
        /// Check for errors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void OnError(object sender, ErrorEventArgs ea)
        {
            Debug.Log(ea.GetException().Message);
        }

                private void OnDisable()
        {
            _watcher.Created -= OnCreated;
            _watcher.Error -= OnError;
        }

#endif

        /// <summary>
        /// Calls from imageConverter
        /// </summary>
        public void ConversionError()
        {
            Debug.Log("ConversionError");
            _isREadyForImport = true;
        }

        
        /// <summary>
        /// Return enum type of vehicle by picture name.
        /// </summary>
        /// <param name="pictureName">Texture name</param>
        /// <returns></returns>
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



    }


}
