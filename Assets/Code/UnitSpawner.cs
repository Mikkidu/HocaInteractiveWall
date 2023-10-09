using System.Collections.Generic;
using UnityEngine;


namespace HocaInk.InteractiveWall
{


    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private UnitManager _subMarinePrefab;
        [SerializeField] private UnitManager _planerPrefab;
        [SerializeField] private UnitManager _tankPrefab;
        [SerializeField] private UnitManager _cannonPrefab;
        [SerializeField] private UnitManager _helicoterPrefab;
        [SerializeField] private UnitManager _boatPrefab;

        [SerializeField] private SpawnContainer _spawnContainer;

        [SerializeField] private Transform _unitsTranform;
        [SerializeField] private float _spawnInterval = 1f;

        private List<MaterialTemplate> _materialsQueue = new List<MaterialTemplate>();
        private float _spawnTrigger = 0;

        private void Update()
        {
            if (_materialsQueue.Count > 0 & Time.time > _spawnTrigger)
            {
                //SpawnObject(_materialsQueue[0]);
                SpawnBuildObject(_materialsQueue[0]);
                _spawnTrigger = Time.time + _spawnInterval;
            }
        }

        public void AddMaterial(Material material, VehicleType objectType)
        {
            _materialsQueue.Add(new MaterialTemplate(material, objectType));
        }

        private void SpawnObject(MaterialTemplate materialTemplate)
        {
            UnitManager newObject = GetUnitByType(materialTemplate.type);
            Instantiate(
                newObject, 
                Vector3.zero, 
                Quaternion.identity, 
                _unitsTranform)
                .Initialize(materialTemplate.material);

            _materialsQueue.Remove(materialTemplate);
        }

        private void SpawnBuildObject(MaterialTemplate materialTemplate)
        {
            var vehicle = GetUnitByType(materialTemplate.type)
                .Initialize(materialTemplate.material);
            var vehicleCOntainer = new UnitBuilder()
                .SetContainer(_spawnContainer, _unitsTranform)
                .SetVehicle(vehicle)
                .SetTrackType(vehicle.GetTrackType)
                .SetStartPoint(Random.Range(0, 1f))
                .Build();
            Instantiate(vehicleCOntainer);
            _materialsQueue.Remove(materialTemplate);
        }

        private UnitManager GetUnitByType(VehicleType objectType)
        {
            var returnUnit = _planerPrefab;
            switch (objectType)
            {
                case VehicleType.Tank:
                    returnUnit = _tankPrefab;
                    break;
                case VehicleType.Boat:
                    returnUnit = _boatPrefab;
                    break;
                case VehicleType.Plane:
                    returnUnit = _planerPrefab;
                    break;
                case VehicleType.Cannon:
                    returnUnit = _cannonPrefab;
                    break;
                case VehicleType.SubMarine:
                    returnUnit = _subMarinePrefab;
                    break;
                case VehicleType.Helicopter:
                    returnUnit = _helicoterPrefab;
                    break;
            }
            return returnUnit;
        }

        private struct MaterialTemplate
        {
            public Material material;
            public VehicleType type;

            public MaterialTemplate(Material newMaterial, VehicleType objectType)
            {
                material = newMaterial;
                type = objectType;
            }
        }
    }
}
