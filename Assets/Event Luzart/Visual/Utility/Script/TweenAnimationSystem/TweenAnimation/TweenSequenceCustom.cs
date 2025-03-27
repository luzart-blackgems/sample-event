using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Eco.TweenAnimation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Eco.TweenAnimation
{
    public class TweenSequenceCustom : TweenAnimationBase
    {
        [SerializeField] private EShow _showOnAction;
        [SerializeField] private bool _ignoreTimeScale;
        [SerializeField] private bool _deActivateOnShow = true;
        [SerializeField] private AnimationCustom[] _showAnimation;
        [SerializeField] private AnimationCustom[] _hideAnimation;
        [SerializeField, HideLabel] private AnimationDebug _debug;

        private void Awake()
        {
            _debug = new AnimationDebug(this);
            if (_showOnAction == EShow.Awake) 
                Show();
        }

        private void OnEnable()
        {
            if (_showOnAction == EShow.Enable) 
                Show();
        }

        public override void Show(float durationDelta = 1, TweenCallback onComplete = null)
        {
            gameObject.SetActive(true);
            foreach (var animationCustom in _showAnimation)
            {
                if(_deActivateOnShow) animationCustom.tweenAnimation.gameObject.SetActive(false);
                DOVirtual.DelayedCall(animationCustom.DelayShow, () =>
                {
                    animationCustom.tweenAnimation.gameObject.SetActive(true);
                    animationCustom.tweenAnimation.Show(durationDelta, () =>
                    {
                        if (animationCustom.DeActivateOnComplete)
                            animationCustom.tweenAnimation.gameObject.SetActive(false);
                        onComplete?.Invoke();
                    });
                }, _ignoreTimeScale);
            }
        }

        public override void Hide(float durationDelta = 1, TweenCallback onComplete = null)
        {
            foreach (var animationCustom in _hideAnimation)
            {
                DOVirtual.DelayedCall(animationCustom.DelayShow, () =>
                {
                    animationCustom.tweenAnimation.gameObject.SetActive(true);
                    if(animationCustom.DeActivateOnComplete)
                        onComplete += () => animationCustom.tweenAnimation.gameObject.SetActive(false);
                    animationCustom.tweenAnimation.Hide(durationDelta, onComplete);
                }, _ignoreTimeScale);
            }
        }

        public override void Kill()
        {
            throw new NotImplementedException();
        }

        public override void Complete()
        {
            throw new NotImplementedException();
        }

        /*
        IEnumerator IEHideTween(float durationDelta = 1, TweenCallback onComplete = null)
        {
            foreach (var animationCustom in _hideAnimation)
            {
                DOVirtual.DelayedCall(animationCustom.DelayShow, () =>
                {
                    animationCustom.tweenAnimation.gameObject.SetActive(true);
                    if(animationCustom.DeActivateOnComplete) onComplete += () => animationCustom.tweenAnimation.gameObject.SetActive(false);
                    animationCustom.tweenAnimation.Hide(durationDelta, onComplete);
                }, false);
            }
            yield return new WaitUntil()
            onComplete?.Invoke();
        }*/

        [System.Serializable]
        public class AnimationCustom
        {
            public TweenAnimationBase tweenAnimation;
            public bool DeActivateOnComplete;
            public float DelayShow;
        }
    }
}