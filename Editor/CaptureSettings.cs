using UnityEditor;
using UnityEngine;

namespace Game.Editor.PhotoBooth
{
    [CreateAssetMenu(menuName = "PhotoBooth/CaptureSettings")]
    public class CaptureSettings : ScriptableObject
    {
        [field: SerializeField]
        public SceneAsset Scene { get; private set; }
        [field: SerializeField]
        public Vector2 Size { get; set; }
    }
}
