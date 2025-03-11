namespace Luzart
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIDailyReward : UIBase
    {
        private DB_Event dBEvent;
        public ItemDailyReward[] itemDailyRewards;
        public BaseSelect bsButton;
        public Button btnClick, btnClickAds, btnClickAdsX2;
        public TMP_Text txtTime;
        private enum EStateButton
        {
            Hide = 0,
            Claim = 1,
            Ads = 2,
        }
        private EStateButton eState;

        protected override void Setup()
        {
            base.Setup();
            GameUtil.ButtonOnClick(btnClick, ClickButton, true);
            GameUtil.ButtonOnClick(btnClickAds, ClickButtonAds, true);
            GameUtil.ButtonOnClick(btnClickAdsX2, ClickButtonAdsX2, true);
        }
        private DailyRewardManager dailyRewardManager
        {
            get
            {
                return EventManager.Instance.dailyRewardManager;
            }
        }
        public override void Show(Action onHideDone)
        {
            base.Show(onHideDone);
            RefreshUI();
        }
        private void Awake()
        {
            Observer.Instance?.AddObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);

        }
        private void OnDestroy()
        {
            Observer.Instance?.RemoveObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
        }
        private void OnTimePerSecond(object data)
        {
            long timeCurrent = TimeUtils.GetLongTimeCurrent;
            long timeContain = TimeUtils.GetLongTimeStartTomorrow - timeCurrent;
            string strTimeCurrent = GameUtil.LongTimeSecondToUnixTime(timeContain, true);
            txtTime.text = $"{strTimeCurrent}";
        }
        public void ClickButton()
        {
            OnDone();
        }
        public void ClickButtonAds()
        {
            AdsWrapperManager.ShowReward(KeyAds.AdsReceivedDailyReward, OnDone, UIManager.Instance.ShowToastInternet);
        }
        public void ClickButtonAdsX2()
        {
            AdsWrapperManager.ShowReward(KeyAds.AdsReceivedDailyReward, OnDoneX2, UIManager.Instance.ShowToastInternet);
        }
        private void InitList()
        {
            var allGift = dBEvent.GetRewardType(ETypeResource.None).gifts;
            if (allGift == null)
            {
                return;
            }
            int length = allGift.Count;
            for (int i = 0; i < length; i++)
            {
                var gift = allGift[i];
                DBDailyReward newDB = new DBDailyReward();
                newDB.index = i;
                newDB.dataRes = gift.groupGift;
                itemDailyRewards[i].Initialize(newDB, ActionClick);
            }
            ActionClick(itemDailyRewards[dailyRewardManager.Today]);
        }
        private ItemDailyReward cacheItemDailyReward;
        private void ActionClick(ItemDailyReward itemDailyReward)
        {
            if (cacheItemDailyReward != null)
            {
                cacheItemDailyReward.Select(false);
            }
            this.cacheItemDailyReward = itemDailyReward;
            this.cacheItemDailyReward.Select(true);
            OnRefreshButton();
        }
        private void OnRefreshButton()
        {
            if (cacheItemDailyReward.eStateClaim == EStateClaim.CanClaim)
            {
                bsButton.Select(1);
            }
            else if(cacheItemDailyReward.eStateClaim == EStateClaim.CanClaimDontClaimed)
            {
                bsButton.Select(2);
            }
            else if(cacheItemDailyReward.eStateClaim == EStateClaim.Claimed)
            {
                bsButton.Select(3);
            }
            else
            {
                bsButton.Select(0);
            }
        }
        private void OnDone()
        {
            var listData = cacheItemDailyReward.dbDailyReward.dataRes.dataResources;
            dailyRewardManager.ClaimReward(cacheItemDailyReward.dbDailyReward.index);
            DataWrapperGame.ReceiveRewardShowPopUp(ValueFirebase.ResDailyReward, RefreshUIOnDone, listData.ToArray());

        }
        private void OnDoneX2()
        {
            var listData = cacheItemDailyReward.dbDailyReward.dataRes.dataResources;
            int length = listData.Count;
            DataResource[] arrayDataResources = new DataResource[length];
            for (int i = 0; i < length; i++)
            {
                int index = i;
                var item = listData[index].Clone();
                item.amount = item.amount * 2;
                arrayDataResources[index] = item;
            }
            dailyRewardManager.ClaimReward(cacheItemDailyReward.dbDailyReward.index);
            DataWrapperGame.ReceiveRewardShowPopUp(ValueFirebase.ResDailyReward, RefreshUIOnDone, arrayDataResources);

        }
        private void RefreshUIOnDone()
        {
            UIManager.Instance.RefreshUI();
        }
        public override void RefreshUI()
        {
            base.RefreshUI();
            dBEvent = dailyRewardManager.dataDailyReward.dbEvent;
            if (dBEvent == null || dBEvent.eventStatus == EEventStatus.Finish)
            {
                Hide();
                return;
            }
            InitList();
        }
    }

}