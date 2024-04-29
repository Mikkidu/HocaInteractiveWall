using UnityEngine;

namespace HocaInk.InteractiveWall
{
    [ExecuteInEditMode]
    public class CollisionTracker : MonoBehaviour
    {
        [SerializeField] private float _stopDistance = 2;
        [SerializeField] private Transform _raycastPoint;
        [SerializeField] private LayerMask _targetLayers;
        [SerializeField] private AnimationController _animController;
        [SerializeField] private float _stoppingInterval = 3.5f;

        private bool _isBlocked = false;
        private float _endStoppingTime;



        void Update()
        {
            if (Time.time > _endStoppingTime)
            {
                // RaycastHit hit;
                // Ray forwardRay = new Ray(_raycastPoint.position, _raycastPoint.forward);
                // if (Physics.Raycast(forwardRay, out hit, _stopDistance, _targetLayers))
                Debug.DrawRay(_raycastPoint.position, _raycastPoint.forward * _stopDistance, Color.red);
                Vector3 boxCenter = _raycastPoint.position + _raycastPoint.forward * _stopDistance / 2;
                Collider[] colliders = Physics.OverlapBox(boxCenter, new Vector3(0.25f, 0.25f, _stopDistance / 2), _raycastPoint.rotation, _targetLayers);
                if (colliders.Length > 0)
                {
                    if (!_isBlocked)
                    {
                        _isBlocked = true;
                        _animController.BlockUnit();
                        Debug.Log("BLocked! by " + colliders[0].name);
                        _endStoppingTime = Time.time + _stoppingInterval;
                    }
                }
                else if (_isBlocked)
                {
                    _isBlocked = false;
                    _animController.UnlockUnit();
                }
            }
        }
    }
}
