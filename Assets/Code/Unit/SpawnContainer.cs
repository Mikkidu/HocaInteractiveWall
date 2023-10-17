using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{


    public class SpawnContainer : MonoBehaviour
    {
        [SerializeField] private UnitManager _unit;
        [SerializeField] private float _startPoint;
        [SerializeField] private float _startDistance;
        [SerializeField] private Transform _parent;
        [SerializeField] private SplineFollower _splineFollower;
        [SerializeField] private TrackType _trackType;

        private void Start()
        {
            //Debug.developerConsoleVisible = true;
            transform.SetParent(_parent);
            _startDistance = _splineFollower.spline.CalculateLength() * _startPoint;
            _splineFollower.SetDistance(_startDistance);
            SpawnVehicle();
        }

        public SpawnContainer SetParent(Transform parent)
        {
            _parent = parent;
            return this;
        }

        public SpawnContainer SetVehicle(UnitManager unit)
        {
            _unit = unit;
            return this;
        }

        public SpawnContainer SetTrackType(TrackType trackType)
        {
            _trackType = trackType;
            _splineFollower = GetComponent<SplineFollower>();
            _splineFollower.spline = TrackManager.instance.GetTrack(trackType);
            if (_splineFollower.spline == null)
                Debug.Log("Spline is null");
            return this;
        }

        public SpawnContainer SetStartPoint(float startPoint)
        {
            _startPoint = startPoint;
            return this;
        }

        public void SpawnVehicle()
        {
            _unit.gameObject.SetActive(false);
            if (_trackType == TrackType.Ground)
            {
                _unit = Instantiate(_unit, transform.position + Vector3.up * 0.5f, transform.rotation);
            }
            else
            {
            _unit = Instantiate(_unit);
            }
            _unit.transform.SetParent(transform);
            _unit.GetComponent<SplineFollower>().spline = _splineFollower.spline;
            _unit.distance = _startDistance;
        }

        public void AppearObject()
        {
            _unit.gameObject.SetActive(true);
            _unit.transform.SetParent(transform.parent);
        }

        public void DestroyObject()
        {
            Destroy(gameObject);
        }
    }


}
