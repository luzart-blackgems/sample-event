namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class ButtonEventDailyReward : ButtonEvent
    {
        private DB_Event db_Event;
        public GameObject obNoti;
        private DailyRewardManager dailyRewardManager
        {
            get
            {
                return EventManager.Instance.dailyRewardManager;
            }
        }
        private void Awake()
        {
            UIManager.AddActionRefreshUI(InitRefreshUI);
        }
        private void OnDestroy()
        {
            UIManager.RemoveActionRefreshUI(InitRefreshUI);
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
        public void InitRefreshUI()
        {
            InitEvent(null);
        }
        public override void InitEvent(Action action)
        {
            base.InitEvent(ClickDailyReward);
        }
        private void ClickDailyReward()
        {
            UIManager.Instance.ShowUI<UIDailyReward>(UIName.DailyReward);
        }
        protected override void Start()
        {
            base.Start();

        }
    }
}

