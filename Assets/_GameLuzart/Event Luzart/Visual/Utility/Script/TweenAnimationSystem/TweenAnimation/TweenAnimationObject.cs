using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Eco.TweenAnimation
{
    public class TweenAnimationObject : TweenAnimationBase
    {
        [HideLabel] public AnimationDebug AnimationDebug;
        private List<TweenAnimationBase> _tweenAnimations = new ();

        private void Awake()
        {
            AnimationDebug = new AnimationDebug(this);
            TweenAnimationBase[] animations = GetComponents<TweenAnimationBase>();
            for (var i = 0; i < animations.Length; i++)
                if(animations[i] != this && animations[i] is not TweenSequenceCustom) _tweenAnimations.Add(animations[i]);
        }
        public override void Show(float durationDelta = 1, TweenCallback onComplete = null)
        {
            gameObject.SetActive(true);
            for (var i = 0; i < _tweenAnimations.Count; i++)
                _tweenAnimations[i].Show(durationDelta, onComplete);
        }

        public override void Hide(float durationDelta = 1, TweenCallback onComplete = null)
        {
            gameObject.SetActive(true);
            for (var i = 0; i < _tweenAnimations.Count; i++)
                _tweenAnimations[i].Hide(durationDelta, onComplete);
        }

        public override void Kill()
        {
            throw new System.NotImplementedException();
        }

        public override void Complete()
        {
            throw new System.NotImplementedException();
        }
    }
}