using UnityEngine;


namespace HocaInk.InteractiveWall
{


    public class EffectsController : MonoBehaviour
    {
        [SerializeField] private GameObject _spawnObject;

        public void DestroyObject()
        {
            Destroy(gameObject);
        }

        public void AppearObject()
        {
            _spawnObject.SetActive(true);
        }
    }


}
