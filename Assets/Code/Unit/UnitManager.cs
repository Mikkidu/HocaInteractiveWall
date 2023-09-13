using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{
    public class UnitManager : test
    {
        [SerializeField] private TrackType _trackType;
        [SerializeField] private string _soundName;
        [SerializeField] private float _followSpeed;
        [SerializeField] private bool _hasUV;


        [SerializeField]
        private float _currentSpeed;
        private SplineFollower _vehicleAhead;
        private Animator _animator;

        public override float Test { 
            get 
            { 
                Debug.Log("Dai");
                return _currentSpeed;
            }
            set
            {
                Debug.Log("Hai");
                if (_vehicleAhead != null && value > _vehicleAhead.followSpeed)
                {
                    value = _vehicleAhead.followSpeed;
                }
                _currentSpeed = value;

            }
        }


       /* [field: SerializeField]
        public float Speed
        {
            get { return _currentSpeed; }
            set
            {
                if (_vehicleAhead != null && value > _vehicleAhead.followSpeed)
                {
                    value = _vehicleAhead.followSpeed;
                }
                _currentSpeed = value;

            }
        }*/

        void Start()
        {

            SplineFollower follower = GetComponent<SplineFollower>();
            follower.spline = TrackManager.instance.GetTrack(_trackType);
            _animator = GetComponent<Animator>();
            
        }

        public void Initialize(Material material)
        {
            if (!_hasUV)
            {
                material.color = GetRandomColor();
            }
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = material;
            }
        }

        private void OnMouseDown()
        {
            _animator.SetTrigger("OnClick");
        }

        public void PlaySound()
        {
            AudioManager.instance.PlaySound("BOT1");
        }

        private Color GetRandomColor()
        {
            switch(Random.Range(0, 5))
            {
                case 0:
                    return Color.red;
                case 1:
                    return Color.green;
                case 2:
                    return Color.blue;
                case 3:
                    return Color.yellow;
                case 4:
                    return Color.magenta;
                default:
                    return Color.white;
            }
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
