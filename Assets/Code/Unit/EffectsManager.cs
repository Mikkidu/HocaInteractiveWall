using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HocaInk.InteractiveWall
{
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _effects;
        [SerializeField] private IndexRange[] effectRanges;


        public void AddParticles(int index)
        {
            Instantiate(_effects[index], _effects[index].transform.position, _effects[index].transform.rotation)
                .gameObject.SetActive(true);
        }

        public void AddParticlesRange(int rangeIndex)
        {
            int firstIndex = effectRanges[rangeIndex].firstIndex;
            int lastIndex = effectRanges[rangeIndex].lastIndex;
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
