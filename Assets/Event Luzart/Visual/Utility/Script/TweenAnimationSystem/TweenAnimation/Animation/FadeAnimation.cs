using DG.Tweening;
using UnityEngine;

namespace Eco.TweenAnimation
{
    public class FadeAnimation : IAnimation
    {
        private AnimationFactory _factory;
        private CanvasGroup _canvasGroup;
        private BaseOptions _options;
        private CanvasGroupOptions _customOptions;
        
        public void Initialized(AnimationFactory animationFactory)
        {
            _factory = animationFactory;
            _canvasGroup = animationFactory.TweenAnimation.CanvasGroup;
            _options = animationFactory.TweenAnimation.BaseOptions;
            _customOptions = animationFactory.TweenAnimation.CanvasGroupOptions;
            if(_customOptions.EndTo == -1)
                _customOptions.EndTo = _canvasGroup.alpha;
        }

        public void SetAnimationFrom()
        {
            _canvasGroup.alpha = _customOptions.From;
        }

        public Tweener Show(float durationDelta = 1f)
        {
            SetAnimationFrom();
            CheckAlpha();
            return _canvasGroup
                .DOFade(_customOptions.EndTo, _options.Duration * durationDelta)
                .SetEase(_options.ShowEase)
                .SetUpdate(_options.IgnoreTimeScale)
                .SetDelay(_options.StartDelay * durationDelta)
                .OnComplete(CheckAlpha);
        }

        public Tweener Hide(float durationDelta = 1f)
        {
            _canvasGroup.alpha = _customOptions.EndTo;
            CheckAlpha();
            return _canvasGroup
                .DOFade(_customOptions.From, _options.Duration * durationDelta)
                .SetEase(_options.ShowEase)
                .SetUpdate(_options.IgnoreTimeScale)
                .SetDelay(_options.StartDelay * durationDelta)
                .OnComplete(CheckAlpha);
        }
        
        private void CheckAlpha()
        {
            _canvasGroup.blocksRaycasts = _canvasGroup.alpha > 0;
        }
    }
}