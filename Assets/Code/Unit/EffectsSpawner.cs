using UnityEngine;

namespace HocaInk.InteractiveWall
{


    public class EffectsSpawner : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _effects;
        [SerializeField] private IndexRange[] _effectRanges;

        public void AddParticles(int index)
        {
            Instantiate(
                _effects[index], 
                _effects[index].transform.position, 
                _effects[index].transform.rotation, 
                _effects[index].transform.parent)
                .gameObject.SetActive(true);
        }

        public void AddParticlesRange(int rangeIndex)
        {
            if (_effects == null || _effectRanges == null) 
                return;

            int firstIndex = _effectRanges[rangeIndex].firstIndex;
            int lastIndex = _effectRanges[rangeIndex].lastIndex;
            if (firstIndex < lastIndex)
            {
                try
                {
                    for (int i = firstIndex; i <= lastIndex; i++)
                    {
                        AddParticles(i);
                    }
                }
                catch (System.IndexOutOfRangeException e)
                {
                    Debug.Log("Error!" + e.Message);
                }
            }
            else
            {
                Debug.Log("Wrong indexes");
            }
        }

        [System.Serializable]
        public struct IndexRange
        {
            public int firstIndex;
            public int lastIndex;
        }
    }


}
