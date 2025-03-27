using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Eco.TweenAnimation
{
    public class FillAmountAnimation : IAnimation
    {
        private AnimationFactory _factory;
        private Image _image;
        private BaseOptions _options;
        private FloatOptions _customOptions;
        
        public void Initialized(AnimationFactory animationFactory)
        {
            _factory = animationFactory;
            _image = animationFactory.TweenAnimation.Image;
            _options = _factory.TweenAnimation.BaseOptions;
            _customOptions = _factory.TweenAnimation.FloatOptions;
            if(_customOptions.EndTo == -1)
                _customOptions.EndTo = _image.fillAmount;
            
        }

        public void SetAnimationFrom()
        {
            _image.fillAmount = _customOptions.From;
        }

        public Tweener Show(float durationDelta = 1f)
        {
            SetAnimationFrom();
            return _image
                .DOFillAmount(_customOptions.EndTo, _options.Duration * durationDelta)
                .SetEase(_options.ShowEase)
                .SetUpdate(_options.IgnoreTimeScale)
                .SetDelay(_options.StartDelay * durationDelta);
        }

        public Tweener Hide(float durationDelta = 1f)
        {
            _image.fillAmount = _customOptions.EndTo;
            return _image
                .DOFillAmount(_customOptions.From, _options.Duration * durationDelta)
                .SetEase(_options.HideEase)
                .SetUpdate(_options.IgnoreTimeScale)
                .SetDelay(_options.StartDelay * durationDelta);
        }
    }
}