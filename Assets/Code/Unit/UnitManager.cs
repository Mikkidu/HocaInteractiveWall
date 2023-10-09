using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private TrackType _trackType;
        [SerializeField] private string _soundName;
        [SerializeField] private float _followSpeed;
        [SerializeField] private Renderer[] _vehiclePartsRenderers;

        private SplineFollower _vehicleAhead;
        private Animator _animator;

        public TrackType GetTrackType
        {
            get { return _trackType; }
        }

        private void Start()
        {
            SplineFollower follower = GetComponent<SplineFollower>();
            follower.spline = TrackManager.instance.GetTrack(_trackType);
            _animator = GetComponent<Animator>();
        }

        public UnitManager Initialize(Material material)
        {
            foreach (Renderer renderer in _vehiclePartsRenderers)
            {
                renderer.material = material;
            }
            return this;
        }

        private void OnMouseDown()
        {
            _animator.SetTrigger("OnClick");
        }

        public void PlaySound(string soundName)
        {
            AudioManager.instance.PlaySound(soundName);
        }

        public void SetDistance(float distance)
        {
            Debug.Log(distance);
            GetComponent<SplineFollower>().SetDistance(distance);
        }
        private void OnTriggerEnter(Collider other)
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
        }
    }
}
