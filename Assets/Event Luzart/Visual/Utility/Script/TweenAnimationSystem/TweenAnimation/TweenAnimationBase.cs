using DG.Tweening;
using UnityEngine;

namespace Eco.TweenAnimation
{
    public abstract class TweenAnimationBase : MonoBehaviour
    {
        public abstract void Show(float durationDelta = 1f, TweenCallback onComplete = null);
        public abstract void Hide(float durationDelta = 1f, TweenCallback onComplete = null);
        public abstract void Kill();
        public abstract void Complete();
    }
}