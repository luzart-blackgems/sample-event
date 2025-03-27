namespace Luzart
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITicketTally : UIBase
    {
        private TicketTallyManager ticketTallyManager
        {
            get
            {
                return EventManager.Instance.ticketTallyManager;
            }
        }
        public TMP_Text txtTime;
        public ProgressBarUI progressBarUI;
        public TMP_Text txtAmount;
        public ResUI resUI;

        //
        public ScrollRect scrollView;
        public Transform parentSpawn;
        public ItemTicketTallyUI itemTicketTallyPf;
        private List<ItemTicketTallyUI> listItemTicket = new List<ItemTicketTallyUI>();
        protected override void Setup()
        {
            isAnimBtnClose = true;
            base.Setup();
        }

        private void Awake()
        {
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
            long timeContain = ticketTallyManager.dataTicketTally.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
            txtTime.text = GameUtil.LongTimeSecondToUnixTime(timeContain);
        }
        public override void Show(Action onHideDone)
        {
            base.Show(onHideDone);
            RefreshUI();
            OnTimePerSecond(null);
        }
        public override void RefreshUI()
        {
            base.RefreshUI();
            int levelCurrent = ticketTallyManager.dataTicketTally.LevelCurrent;
            int contain = ticketTallyManager.dataTicketTally.ContainItem;
            GiftEvent data = new GiftEvent();
            if (levelCurrent >= ticketTallyManager.dataTicketTally.MaxReward())
            {
                int maxLevel = ticketTallyManager.dataTicketTally.dbEvent.GetRewardType(ETypeResource.None).gifts.Count - 1;
                data = ticketTallyManager.dataTicketTally.dbEvent.GetRewardType(ETypeResource.None).gifts[maxLevel];
            }
            else
            {
                data = ticketTallyManager.dataTicketTally.dbEvent.GetRewardType(ETypeResource.None).gifts[levelCurrent];
            }

            int totalRequire = data.require;
            float percent = (float)((float)contain / (float)totalRequire);
            txtAmount.text = $"{contain}/{totalRequire}";
            resUI.InitData(data.groupGift.dataResources[0]);
            progressBarUI.SetSlider(percent, percent, 0, null);
            InitListItem();
            ShowPopUpTicketIfHas();
        }
        private void InitListItem()
        {
            int levelCurrent = ticketTallyManager.dataTicketTally.LevelCurrent;
            var data = ticketTallyManager.dataTicketTally.dbEvent.GetRewardType(ETypeResource.None).gifts;
            int length = data.Count;
            MasterHelper.InitListObj(length, itemTicketTallyPf, listItemTicket, parentSpawn, (item, index) =>
            {
                item.gameObject.SetActive(true);
                var dataEach = data[index];
                EStateClaim eState = EStateClaim.WillClaim;
                if (dataEach.isClaimed)
                {
                    eState = EStateClaim.Claimed;
                }
                else if (levelCurrent == index)
                {
                    eState = EStateClaim.CanClaim;
                }
                item.Initialize(index, dataEach, eState, Click);
            });
            if (!ticketTallyManager.dataTicketTally.IsMaxLevel)
            {
                itemTicketCur = listItemTicket[levelCurrent];
            }
            else
            {
                int max = listItemTicket.Count - 1;
                itemTicketCur = listItemTicket[max];
            }

            GameUtil.Instance.WaitAndDo(0.1f, MoveToTarget);

        }
        private ItemTicketTallyUI itemTicketCur = null;
        private void MoveToTarget()
        {
            scrollView.FocusOnRectTransform(itemTicketCur.GetComponent<RectTransform>());
        }
        private void Click(ItemTicketTallyUI item)
        {
            item.boxInforMess.gameObject.SetActive(true);
        }
        private void ShowPopUpTicketIfHas()
        {
            if (!ticketTallyManager.IsHasDataDontClaim())
            {
                return;
            }
            int levelCurrent = ticketTallyManager.dataTicketTally.LevelCurrent;
            List<DataResource> listDataRes = new List<DataResource>();
            for (int i = 0; i < levelCurrent; i++)
            {
                var data = ticketTallyManager.dataTicketTally.GetDBGiftEvent(i);
                if (!data.isClaimed)
                {
                    var listRes = data.groupGift.dataResources;
                    listDataRes.AddRange(listRes);
                    ticketTallyManager.UseItem(i);
                }
            }
            DataWrapperGame.ReceiveReward(ValueFirebase.TicketTally, listDataRes.ToArray());
            var ui = UIManager.Instance.ShowUI<UIReceiveRewardEventTicket>(UIName.ReceiveRewardEventTicket, UIManager.Instance.RefreshUI);
            ui.Initialize(listDataRes.ToArray());
        }
    }
}

