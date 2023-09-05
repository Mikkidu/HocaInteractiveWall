using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace HocaInk.InteractiveWall
{
    public class TrackManager : MonoBehaviour
    {
        public static TrackManager instance;

        [SerializeField] private SplineComputer _airTrack;
        [SerializeField] private SplineComputer _groundTrack;
        [SerializeField] private SplineComputer _underWaterTrack;
        [SerializeField] private SplineComputer _waterTrack;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        public SplineComputer GetTrack(TrackType trackType)
        {
            SplineComputer track = _groundTrack;
            switch (trackType)
            {
                case TrackType.Air:
                    track = _airTrack;
                    break;
                case TrackType.Ground:
                    track = _groundTrack;
                    break;
                case TrackType.Water:
                    track = _waterTrack;
                    break;
                case TrackType.UnderWater:
                    track = _underWaterTrack;
                    break;
            }
            return track;
        }
    }
}
