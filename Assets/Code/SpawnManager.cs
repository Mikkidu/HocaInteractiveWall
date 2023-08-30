using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HocaInk.InteractiveWall
{
    public class SpawnManager : MonoBehaviour
    {
        List<MaterialTemplate> _materials = new List<MaterialTemplate>();

        private void Update()
        {
            if (_materials.Count > 0)
            {
                SpawnObject(_materials[0]);
            }
        }

        public void AddMaterial(Material material, ObjectType objectType)
        {
            _materials.Add(new MaterialTemplate(material, objectType));
        }

        private void SpawnObject(MaterialTemplate materialTemplate)
        {
            GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newObject.GetComponent<Renderer>().material = materialTemplate.material;
            newObject.transform.position = transform.position;
            _materials.Remove(materialTemplate);
        }

        private struct MaterialTemplate
        {
            public Material material;
            public ObjectType type;

            public MaterialTemplate(Material newMaterial, ObjectType objectType)
            {
                material = newMaterial;
                type = objectType;
            }
        }
    }
}
