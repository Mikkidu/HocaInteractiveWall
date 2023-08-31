using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HocaInk.InteractiveWall
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private UnitManager _subMarine;
        [SerializeField] private UnitManager _planer;
        [SerializeField] private UnitManager _tank;
        [SerializeField] private UnitManager _parachute;
        [SerializeField] private UnitManager _helicopter;
        [SerializeField] private UnitManager _horse;

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
            UnitManager returnUnit = _planer;
            switch (objectType)
            {
                case ObjectType.Car:
                    returnUnit = _tank;
                    break;
                case ObjectType.Boat:
                    returnUnit = _subMarine;
                    break;
                case ObjectType.Plane:
                    returnUnit = _planer;
                    break;
                case ObjectType.Parachute:
                    returnUnit = _parachute;
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
