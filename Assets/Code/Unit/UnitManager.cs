using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private TrackType _trackType;
        [SerializeField] private string _soundName;
        [SerializeField] private float _followSpeed;
        [SerializeField] private bool _hasUV;
        //[SerializeField] private float _currentSpeed;
        [SerializeField] private Renderer[] _vehiclePartsRenderers;

        private SplineFollower _vehicleAhead;
        private Animator _animator;

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

            foreach (Renderer renderer in _vehiclePartsRenderers)
            {
                renderer.material = material;
            }
        }

        private void OnMouseDown()
        {
            _animator.SetTrigger("OnClick");
        }

        public void PlaySound(string soundName)
        {
            AudioManager.instance.PlaySound(soundName);
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
