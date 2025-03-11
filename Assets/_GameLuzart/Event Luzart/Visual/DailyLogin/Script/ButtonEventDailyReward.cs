namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class ButtonEventDailyReward : ButtonEvent
    {
        public GameObject obNoti;
        private DailyRewardManager dailyRewardManager
        {
            get
            {
                return EventManager.Instance.dailyRewardManager;
            }
        }
        public void CheckNoti()
        {
            bool isNoti = dailyRewardManager.IsHasDataFreeDontReceive();
            GameUtil.SetActiveCheckNull(obNoti, isNoti);
        }
        protected override void InitButton()
        {
            base.InitButton();
            CheckNoti();
        }
        public override void InitEvent(Action action)
        {
            base.InitEvent(ClickDailyReward);
        }
        private void ClickDailyReward()
        {
            UIManager.Instance.ShowUI<UIDailyReward>(UIName.DailyReward);
        }
    }
}

