using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HocaInk.InteractiveWall
{
    public class SpawnManager : MonoBehaviour
    {
        
        [SerializeField] private UnitManager _subMarinePrefab;
        [SerializeField] private UnitManager _planerPrefab;
        [SerializeField] private UnitManager _tankPrefab;
        [SerializeField] private UnitManager _cannonPrefab;
        [SerializeField] private UnitManager _helicoterPrefab;
        [SerializeField] private UnitManager _boatPrefab;


        [SerializeField] private Transform _unitsTranform;

        [SerializeField] private float _spawnInterval = 1f;

        private List<MaterialTemplate> _materials = new List<MaterialTemplate>();
        private float _spawnTrigger = 0;

        private void Update()
        {
            if (_materials.Count > 0 & Time.time > _spawnTrigger)
            {
                SpawnObject(_materials[0]);
                _spawnTrigger = Time.time + _spawnInterval;
            }
        }

        public void AddMaterial(Material material, ObjectType objectType)
        {
            _materials.Add(new MaterialTemplate(material, objectType));
        }

        private void SpawnObject(MaterialTemplate materialTemplate)
        {
            UnitManager newObject = GetUnitByType(materialTemplate.type);
            Instantiate(newObject, _unitsTranform).Initialize(materialTemplate.material);
            _materials.Remove(materialTemplate);
        }

        private UnitManager GetUnitByType(ObjectType objectType)
        {
            UnitManager returnUnit = _planerPrefab;
            switch (objectType)
            {
                case ObjectType.Tank:
                    returnUnit = _tankPrefab;
                    break;
                case ObjectType.Boat:
                    returnUnit = _boatPrefab;
                    break;
                case ObjectType.Plane:
                    returnUnit = _planerPrefab;
                    break;
                case ObjectType.Cannon:
                    returnUnit = _cannonPrefab;
                    break;
                case ObjectType.SubMarine:
                    returnUnit = _subMarinePrefab;
                    break;
                case ObjectType.Helicopter:
                    returnUnit = _helicoterPrefab;
                    break;
            }
            return returnUnit;
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
