using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HocaInk.InteractiveWall
{
    public class UnitManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        public void Initialize(Material material)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = material;
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
