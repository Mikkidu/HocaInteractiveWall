using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{


    [RequireComponent(typeof(Animator))]
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private TrackType _trackType;
        [SerializeField] private string _soundName;
        [SerializeField] private Renderer[] _vehiclePartsRenderers;

        #region Add Distance control

        //[SerializeField] private float _followSpeed;
        //private SplineFollower _vehicleAhead;

        #endregion

        public float distance;

        public TrackType GetTrackType
        {
            get { return _trackType; }
        }

        private void Start()
        {
            var follower = GetComponent<SplineFollower>();
            follower.spline = TrackManager.instance.GetTrack(_trackType);
            follower.SetDistance(distance);
        }

        public UnitManager Initialize(Material material)
        {
            foreach (Renderer renderer in _vehiclePartsRenderers)
            {
                renderer.material = material;
            }
            return this;
        }

        public void SetDistance(float distance)
        {
            Debug.Log(distance);
            GetComponent<SplineFollower>().SetDistance(distance);
        }

        #region Add Distance Control

        /*private void OnTriggerEnter(Collider other)
        {
            if (_vehicleAhead != null)
            {
                return;
            }
             _vehicleAhead = other.GetComponent<SplineFollower>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (_vehicleAhead == null)
            {
                return;
            }

            if (other.TryGetComponent<SplineFollower>(out SplineFollower vehicleExit))
            {
                if (vehicleExit == _vehicleAhead)
                {
                    _vehicleAhead = null;
                }
            }
        }*/

        #endregion

    }


}
