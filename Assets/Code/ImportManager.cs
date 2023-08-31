using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace HocaInk.InteractiveWall
{


    public class ImportManager : MonoBehaviour
    {
        [SerializeField] private Shader _shader;
        [SerializeField] private SpawnManager _spawnManager;
        private FileSystemWatcher _watcher;
        private List<string> _fileNames = new List<string>();
        private List<Material> _materials = new List<Material>();
        private string _path = "/Drawings";

        void Start()
        {
            if (!Directory.Exists(Application.dataPath + _path))
            {
                try
                {
                    Directory.CreateDirectory(Application.dataPath + _path);
                    Debug.Log("Exist");
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            Debug.Log(Application.dataPath + _path);
            _watcher = new FileSystemWatcher(Application.dataPath + _path);
            _watcher.NotifyFilter = NotifyFilters.DirectoryName |
                                    NotifyFilters.FileName;
            _watcher.Created += OnCreated;
            _watcher.Error += OnError;
            _watcher.Filter = "*.png";
            _watcher.EnableRaisingEvents = true;
        }

        void Update()
        {
            if(_fileNames.Count > 0)
            {
                string pictureName = _fileNames[0];
                _spawnManager.AddMaterial(CreateMaterial(Application.dataPath +  _path + "/" + pictureName), GetObjectType(pictureName));
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

        private Material CreateMaterial(string pathToImage)
        {
            byte[] pictureBytes = File.ReadAllBytes(pathToImage);
            Texture2D texture = new Texture2D(2048, 2048);
            Material newMaterial = new Material(_shader);
            try
            {
                texture.LoadImage(pictureBytes);
                newMaterial.mainTexture = texture;
            }
            catch
            {
                Debug.Log("error");
            }
            return newMaterial;
        }


        private void OnError(object sender, ErrorEventArgs ea)
        {
            Debug.Log(ea.GetException().Message);
        }

        private ObjectType GetObjectType(string pictureName)
        {
            ObjectType retObjectType = ObjectType.Plane;
            switch (pictureName.Substring(0, 3))
            {
                case "car":
                    retObjectType = ObjectType.Car;
                    break;
                case "bot":
                    retObjectType = ObjectType.Boat;
                    break;
                case "pln":
                    retObjectType = ObjectType.Plane;
                    break;
                case "prs":
                    retObjectType = ObjectType.Parachute;
                    break;
            }
            return retObjectType;
        }

        private void OnDisable()
        {
            _watcher.Created -= OnCreated;
            _watcher.Error -= OnError;
        }
    }
}
