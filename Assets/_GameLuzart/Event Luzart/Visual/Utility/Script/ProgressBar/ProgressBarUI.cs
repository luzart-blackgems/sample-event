namespace Luzart
{
    using DG.Tweening;
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Luzart;
    
    public class ProgressBarUI : MonoBehaviour
    {
        public Image imFill;
        public virtual void SetSlider(float prePercent, float targetPercent, float time, Action onDone, Action<float> actionUpdate = null)
        {
            prePercent = Mathf.Clamp01(prePercent);
            targetPercent = Mathf.Clamp01(targetPercent);
            if (prePercent == targetPercent || time <= 0)
            {
                imFill.fillAmount = targetPercent;
                onDone?.Invoke();
                return;
            }
            GameUtil.Instance.StartLerpValue(this, prePercent, targetPercent, time, (x) =>
            {
                imFill.fillAmount = x;
                actionUpdate?.Invoke(x);
            }, onDone);
        }
        public virtual Tween SetSliderTween(float prePercent, float targetPercent, float time, Action onDone, Action<float> actionUpdate = null)
        {
            prePercent = Mathf.Clamp01(prePercent);
            targetPercent = Mathf.Clamp01(targetPercent);
            if (prePercent == targetPercent || time <= 0)
            {
                imFill.fillAmount = targetPercent;
                onDone?.Invoke();
                return null;
            }
            else
            {
                return DOVirtual.Float(prePercent, targetPercent, time, (x) =>
                {
                    imFill.fillAmount = x;
                    actionUpdate?.Invoke(x);
                }).OnComplete(()=> onDone?.Invoke()).SetId(this);
            }
    
        }
        private void OnDisable()
        {
            this.DOKill(true);
        }
    }
}
