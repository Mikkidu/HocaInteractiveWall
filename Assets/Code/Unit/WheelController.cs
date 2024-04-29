using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{
    [RequireComponent(typeof(SplineFollower))]
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private float _wheelRadius = 1; // Радиус колеса
        [SerializeField] private float _rotationCorrection = 1; // Поправка по скорости

        [SerializeField] private Transform[] _wheels;

        private SplineFollower _follower;
        private float _deltaAngle = 0;
        private float _speedModifier = 0;

        private void Start()
        {
            _follower = GetComponent<SplineFollower>();
            _speedModifier = 1 / (2 * Mathf.PI * _wheelRadius); // Вычисление коэффициента скорости
        }

        void Update()
        {
            if (_wheels != null && _wheels.Length > 0)
            {
                _deltaAngle = 360 * (_speedModifier * (_follower.followSpeed + _rotationCorrection)) * Time.deltaTime; // Учет поправки по скорости
                for (int i = 0; i < _wheels.Length; i++)
                {
                    _wheels[i].Rotate(Vector3.right, _deltaAngle);
                }
            }
        }
    }
}