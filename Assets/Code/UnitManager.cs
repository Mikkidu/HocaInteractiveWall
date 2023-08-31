using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private TrackType _trackType;

        void Start()
        {
            GetComponent<SplineFollower>().spline = TrackManager.instance.GetTrack(_trackType);
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
