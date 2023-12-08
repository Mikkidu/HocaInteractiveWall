using UnityEngine;

namespace HocaInk.InteractiveWall
{


    [CreateAssetMenu(fileName = "AudioData", menuName = "Audio/SoundsObject")]
    public class SoundPack : ScriptableObject
    {
        public string packName;
        public Sound[] Sounds;
    }


}
