using UnityEngine;


namespace HocaInk.InteractiveWall
{


    public class AnimationController : MonoBehaviour
    {
        [Range(1,10)]
        [SerializeField] private byte _actionCount;
        [SerializeField] private bool _hasSounds;
        [SerializeField] private string _soundName;

        private Animator _animator;
        private byte _lastActionNumber = 0;
        private bool _isSecondSameAction;
        private AudioManager _audioManager;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _audioManager = AudioManager.instance;
        }

        private void OnMouseDown()
        {
            if (_actionCount == 1)
            {
                _animator.SetTrigger("Action0");
                return;
            }

            var actionIndex = (byte)Random.Range(0, _actionCount);
            if (actionIndex == _lastActionNumber)
            {
                if (_isSecondSameAction)
                {
                    actionIndex = (byte)((actionIndex + 1) % _actionCount);
                    SetNewAction(actionIndex);
                }
                else
                {
                    _isSecondSameAction = true;
                }
            }
            else
            {
                SetNewAction(actionIndex);
            }
            _animator.SetTrigger("Action" + actionIndex);

            if (_hasSounds)
            {
                _audioManager.PlayRandomAnimalSound(_soundName);
            }
        }

        private void SetNewAction(byte actionIndex)
        {
            _isSecondSameAction = false;
            _lastActionNumber = actionIndex;
        }

        public void PlaySound(string soundName)
        {
            _audioManager.PlaySound(soundName);
        }

        public void PlayRandomSound(string nameStarts)
        {
            _audioManager.PlayRandomSound(nameStarts);
        }
    }


}
