namespace Luzart
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    
    public class ButtonEventRiseOfKittens : ButtonEvent
    {
        private DB_Event db_Event;
        public TextMeshProUGUI txtTime;
        public GameObject obNoti;
        private RiseOfKittensManager riseOfKittensManager
        {
            get
            {
                return EventManager.Instance.riseOfKittensManager;
            }
        }
        private void Awake()
        {
            if (IsActiveEvent)
                Observer.Instance.AddObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
        }
        private void OnDestroy()
        {
            if (Observer.Instance != null)
                Observer.Instance.RemoveObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
        }
        private void OnTimePerSecond(object data)
        {
            long timeCurrent = TimeUtils.GetLongTimeCurrent;
            long timeContain = riseOfKittensManager.dataRiseOfKittens.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
            txtTime.text = GameUtil.LongTimeSecondToUnixTime(timeContain);
        }
        private void CheckNoti()
        {
            bool isNoti = riseOfKittensManager.IsHasDataDontClaim();
            GameUtil.SetActiveCheckNull(obNoti, isNoti);
        }
        protected override void InitButton()
        {
            if (riseOfKittensManager.IsMaxReceive())
            {
                gameObject.SetActive(false);
                return;
            }
            CheckNoti();
            OnTimePerSecond(null);
    
        }
    }
}
