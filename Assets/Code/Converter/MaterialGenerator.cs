using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace HocaInk.InteractiveWall
{


    public class MaterialGenerator : MonoBehaviour
    {
        [SerializeField] private Shader _shader;
        [SerializeField] private Material _material;
        [SerializeField] private UnitSpawner _spawnManager;
        
        public Texture2D _textureTest;

        private FileSystemWatcher _watcher;
        private List<string> _fileNames = new List<string>();
        private ImageConverter _converter;
        private string _path = "c:/InteractiveSoftware/Scans";
        private bool _isREadyForImport = true;


        void Start()
        {
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
            
            if(_fileNames.Count > 0 & _isREadyForImport)
            {
                string pictureName = _fileNames[0];
                _isREadyForImport = false;
                _converter.GenerateTexture(_path + "/" + pictureName);
                _fileNames.Remove(pictureName);
            }
        }


        private void OnCreated(object sender, FileSystemEventArgs ea)
        {
            try
            {
                _fileNames.Add(ea.Name);
            }
            catch(System.Exception e)
            {
                Debug.Log(e.Message);
            }
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
