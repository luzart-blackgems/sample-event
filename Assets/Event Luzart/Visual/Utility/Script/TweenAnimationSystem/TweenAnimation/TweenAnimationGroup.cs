using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Eco.TweenAnimation
{
    public class TweenAnimationGroup : TweenAnimationBase
    {
        [SerializeField] private Transform _parent;
        [HideLabel] public AnimationDebug AnimationDebug;
        private List<TweenAnimationBase> _tweenAnimations = new ();

        private void Awake()
        {
            AnimationDebug = new AnimationDebug(this);
            int childCount = _parent.childCount;
            for (int i = 0; i < childCount; i++)
                _tweenAnimations.AddRange(_parent.GetChild(i).GetComponents<TweenAnimationBase>());
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
            throw new NotImplementedException();
        }

        public override void Complete()
        {
            throw new NotImplementedException();
        }
    }
}