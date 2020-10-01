using UnityEngine.Events;

namespace Elendow.SpritedowAnimator
{
    [System.Serializable]
    public class SpriteAnimationEvent : UnityEvent<BaseAnimator>
    {
        public int eventFrame;
        public string eventName;
        public UnityEngine.SendMessageOptions messageOptions;
    }
}