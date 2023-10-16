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
            Debug.developerConsoleVisible = true;
            transform.SetParent(_parent);
            _splineFollower.SetDistance(_startDistance);
            /*var follower = GetComponent<SplineFollower>();
            _startDistance = follower.spline.CalculateLength() * _startPoint;
            Debug.Log("Distance " + _startDistance +
                        "Length " + follower.spline.CalculateLength()+
                        "Point " + _startPoint);
            follower.SetDistance(_startDistance);*/
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
            //_unit.transform.parent = transform;
            return this;
        }

        public SpawnContainer SetTrackType(TrackType trackType)
        {
            //Debug.Log(trackType);
            _trackType = trackType;
            GetComponent<SplineFollower>().spline = TrackManager.instance.GetTrack(trackType);
            if (GetComponent<SplineFollower>().spline == null)
                Debug.Log("Spline is null");
            return this;
        }

        public SpawnContainer SetStartPoint(float startPoint)
        {
            _startPoint = startPoint;
            _splineFollower = GetComponent<SplineFollower>();
            _startDistance = _splineFollower.spline.CalculateLength() * _startPoint;
            /*Debug.LogError("Distance " + _startDistance +
                        "Length " + _splineFollower.spline.CalculateLength() +
                        "Point " + _startPoint);*/
            _splineFollower.SetDistance(_startDistance);
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
            //_unit.SetDistance(_startDistance);
            _unit.distance = _startDistance;
        }

        public void AppearObject()
        {
            _unit.gameObject.SetActive(true);
            _unit.transform.SetParent(transform.parent);
        }

        public void DestroyObject()//, float delay = 0)
        {
            Destroy(gameObject);//, delay);
        }

    }
}
