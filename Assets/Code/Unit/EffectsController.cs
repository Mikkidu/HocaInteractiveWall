using UnityEngine;

namespace HocaInk.InteractiveWall
{
    public class EffectsController : MonoBehaviour
    {
        [SerializeField] private GameObject _spawnObject;
        public void DestroyObject()//, float delay = 0)
        {
            Destroy(gameObject);//, delay);
        }

        public void AppearObject()
        {
            _spawnObject.SetActive(true);
        }

    }
}
