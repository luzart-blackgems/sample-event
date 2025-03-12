namespace Luzart
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    
    public class ButtonEventJourneyToSuccess : ButtonEvent
    {
        private DB_Event db_Event;
        public TextMeshProUGUI txtTime;
        public GameObject obNoti;
        private JourneyToSuccessManager journeyToSuccessManager
        {
            get
            {
                return EventManager.Instance.journeyToSuccessManager;
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
            long timeContain = journeyToSuccessManager.dataJourneyToSuccess.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
            txtTime.text = GameUtil.LongTimeSecondToUnixTime(timeContain);
        }
        public void CheckNoti()
        {
            bool isNoti = journeyToSuccessManager.IsHasDataFreeDontReceive();
            GameUtil.SetActiveCheckNull(obNoti, isNoti);
        }
        protected override void InitButton()
        {
            if (journeyToSuccessManager.IsMaxReceive())
            {
                gameObject.SetActive(false);
                return;
            }
            CheckNoti();
            OnTimePerSecond(null);
    
        }
    
    }
}
