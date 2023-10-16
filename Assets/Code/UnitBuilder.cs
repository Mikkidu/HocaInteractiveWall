using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{
    public class UnitBuilder 
    {

        private SpawnContainer _container;
        private UnitManager _vehicle;
        private TrackType _trackType;
        private float _startPoint = 0;

        public UnitBuilder SetContainer(SpawnContainer containerPrefab, Transform parent)
        {
            _container = containerPrefab;
            _container.SetParent(parent);
            return this;
        }

        public UnitBuilder SetVehicle(UnitManager vehiclePrefab)
        {
            _vehicle = vehiclePrefab;
            return this;
        }

        public UnitBuilder SetTrackType(TrackType track)
        {
            _trackType = track;
            return this;
        }

        public UnitBuilder SetStartPoint(float startPoint)
        {
            //Debug.Log("StartPoint " + startPoint);
            if (startPoint >= 0 || startPoint <= 1)
                _startPoint = startPoint;
            return this;
        }

        public SpawnContainer Build()
        {
            var buildContainer = _container;
            var buildVehicle = _vehicle;
            buildContainer
                .SetTrackType(buildVehicle.GetTrackType)
                .SetStartPoint(_startPoint)
                .SetVehicle(buildVehicle);

            return buildContainer;
        }

        public UnitBuilder Reset()
        {
            _container = null;
            _vehicle = null;
            _startPoint = 0;
            return this;
        }
    }
}
