using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dreamteck.Splines;

using UnityEngine;
using UnityEditor;


public class FileManager : MonoBehaviour
{
    private FileSystemWatcher watcher;
    [SerializeField] private GameObject _carModel;
    [SerializeField] private Texture2D texture;
    [SerializeField] private SplineComputer _spline;
    FileSystemEventArgs arg;
    public Shader shader;
    private Vector3 spawnPosition = Vector3.zero;

    private bool isCatchFile;
    private string filePath;

    private string path = "Paintings/Cars";

    void Start()
    {
        //watcher = new FileSystemWatcher(Application.dataPath + "/Resources/" + path);
        watcher = new FileSystemWatcher("D:/Test");
        watcher.NotifyFilter = NotifyFilters.DirectoryName |
                                NotifyFilters.FileName;
        watcher.Created += OnCreated;
        watcher.Error += OnError;
        watcher.Changed += OnChange;
        watcher.Renamed += OnRename;

        watcher.Filter = "*.png";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        texture = Resources.Load<Texture2D>(path + "/" + "CarTeamplateMapping");
        ExtractMaterials("Assets/Art/Models/CarModelWheels.fbx", "Assets/Art/Materials");

    }

    void Update()
    {
        if (isCatchFile)
        {
            isCatchFile = false;
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            texture = new Texture2D(1024, 1024);
            texture.LoadImage(bytes);
            Debug.Log(filePath);
            Material mat = new Material(shader);
            mat.mainTexture = texture;

            Renderer[] modelRenderers = _carModel.GetComponentsInChildren<Renderer>();//.material = new Material(Shader.Find("Standart"));
            foreach (Renderer renderer in modelRenderers)
            {
                renderer.material = mat;
            }
            Instantiate(_carModel, spawnPosition, Quaternion.identity).GetComponent<SplineFollower>().spline = _spline;

            spawnPosition.z += 3;
        }
    }

    private void OnCreated(object sender, FileSystemEventArgs ea)
    {
        Debug.Log(ea.FullPath);
        try
        {
            filePath = ea.FullPath;
            isCatchFile = true;
            /*byte[] bytes = System.IO.File.ReadAllBytes(ea.FullPath);
            texture = new Texture2D(1024, 1024);
            texture.LoadImage(bytes);
            Debug.Log(path + "/" + arg.Name.Replace(".png", ""));
            _carModel.GetComponent<Renderer>().material = new Material(Shader.Find("Standart"));
            Instantiate(_carModel);*/
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            foreach (var dat in e.Data.Keys)
            {
                Debug.Log(dat);
            }
        }
    }
    private void OnChange(object sender, FileSystemEventArgs ea)
    {
        string value = $"Changed: {ea.FullPath}";
        Debug.Log(value);
    }
    private void OnRename(object sender, FileSystemEventArgs ea)
    {
        string value = $"Renamed: {ea.FullPath}";
        Debug.Log(value);
    }

    private void OnError(object sender, ErrorEventArgs ea)
    {
        Debug.Log(ea.GetException().Message);
    }

    private void OnDisable()
    {
        watcher.Created -= OnCreated;
        watcher.Error -= OnError;
    }


    public static void ExtractMaterials(string assetPath, string destinationPath)
    {
        Debug.Log("Extract");
        HashSet<string> hashSet = new HashSet<string>();
        IEnumerable<Object> enumerable = from x in AssetDatabase.LoadAllAssetsAtPath(assetPath)
                                         where x.GetType() == typeof(Material)
                                         select x;
        foreach (Object item in enumerable)
        {
            Debug.Log(item.name);
            string path = System.IO.Path.Combine(destinationPath, item.name) + ".mat";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            string value = AssetDatabase.ExtractAsset(item, path);
            if (string.IsNullOrEmpty(value))
            {
                hashSet.Add(assetPath);
            }
        }

        foreach (string item2 in hashSet)
        {
            Debug.Log(item2);
            AssetDatabase.WriteImportSettingsIfDirty(item2);
            AssetDatabase.ImportAsset(item2, ImportAssetOptions.ForceUpdate);
        }
    }

}
