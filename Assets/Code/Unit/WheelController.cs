using UnityEngine;

namespace HocaInk.InteractiveWall
{
    [RequireComponent(typeof(Dreamteck.Splines.SplineFollower))]
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private float _speedModifier = 1;
        [SerializeField] private Transform[] _wheels;

        private Dreamteck.Splines.SplineFollower _follower;
        private float _deltaAngle = 0;
        private Quaternion _wheelRotation;

        private void Start()
        {
            _follower = GetComponent<Dreamteck.Splines.SplineFollower>();
        }

        void Update()
        {
            if (_wheels != null && _wheels.Length > 0)
            {
                _deltaAngle = 360 * _speedModifier * _follower.followSpeed * Time.deltaTime;
                for (int i = 0; i < _wheels.Length; i++)
                {
                    _wheels[i].Rotate(Vector3.right, _deltaAngle);
                }
            }
        }
    }
}
