using UnityEngine;

namespace HocaInk.InteractiveWall
{
    public class AnimationController : MonoBehaviour
    {
        private Animator _animator;
        private byte _lastActionNumber = 0;
        private bool _isSecondSameAction;
        [Range(0,10)]
        [SerializeField] private byte _actionCount;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnMouseDown()
        {
            if (_actionCount == 1)
            {
                _animator.SetTrigger("Action0");
                return;
            }
            byte actionIndex = (byte)Random.Range(0, _actionCount);
            if (actionIndex == _lastActionNumber)
            {
                if (_isSecondSameAction)
                {
                    actionIndex = (byte)((actionIndex + 1) % _actionCount);
                    _isSecondSameAction = false;
                    _lastActionNumber = actionIndex;
                }
                else
                {
                    _isSecondSameAction = true;
                }
            }
            else
            {
                _isSecondSameAction = false;
                _lastActionNumber = actionIndex;
            }
            _animator.SetTrigger("Action" + actionIndex);
        }

        public void PlaySound(string soundName)
        {
            AudioManager.instance.PlaySound(soundName);
        }

        public void PlayRandomSound(string nameStarts)
        {
            AudioManager.instance.PlayRandomSound(nameStarts);
        }
    }
}
