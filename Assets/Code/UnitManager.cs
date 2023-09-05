using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private TrackType _trackType;

        private Animator _animator;

        void Start()
        {
            SplineFollower follower = GetComponent<SplineFollower>();
            follower.spline = TrackManager.instance.GetTrack(_trackType);
            _animator = GetComponent<Animator>();

        }

        public void Initialize(Material material)
        {
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
    }
}
