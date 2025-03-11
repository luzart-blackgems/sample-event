namespace Luzart
{

    using DG.Tweening;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class ButtonEventTicketTally : ButtonEvent
    {
        public TextMeshProUGUI txtTime;
        public TextMeshProUGUI txtTicketCurrent;
        public ResUI resUI;
        public ProgressBarUI progressBarUI;
        public GameObject obNoti;

        private TicketTallyManager ticketTallyManager
        {
            get
            {
                return EventManager.Instance.ticketTallyManager;
            }
        }
        private void Awake()
        {
            if (IsActiveEvent)
            {
                Observer.Instance.AddObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
            }

            UIManager.AddActionRefreshUI(RefreshUI);
        }
        private void OnDestroy()
        {
            Observer.Instance?.RemoveObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
            UIManager.RemoveActionRefreshUI(RefreshUI);
        }
        public void RefreshUI()
        {
            InitEvent(null);
        }
        private void OnTimePerSecond(object data)
        {
            if (!IsActiveEvent)
            {
                return;
            }
            long timeCurrent = TimeUtils.GetLongTimeCurrent;
            long timeContain = EventManager.Instance.ticketTallyManager.dataTicketTally.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
            txtTime.text = GameUtil.LongTimeSecondToUnixTime(timeContain);
        }
        protected override void InitButton()
        {
            base.InitButton();

            SetVisual(ticketTallyManager.dataTicketTally.totalUse);
            CheckNoti();
            OnTimePerSecond(null);

        }
        private void CheckNoti()
        {
            bool isNoti = ticketTallyManager.IsHasDataDontClaim();
            GameUtil.SetActiveCheckNull(obNoti, isNoti);
        }
        public void SetVisual(int totalUse)
        {
            int levelCurrent = ticketTallyManager.dataTicketTally.LevelByTotalUse(totalUse);
            int contain = ticketTallyManager.dataTicketTally.ContainByTotalUse(totalUse);
            int totalRequire = ticketTallyManager.dataTicketTally.RequireByTotalUse(totalUse);
            float percent = (float)((float)contain / (float)totalRequire);
            txtTicketCurrent.text = $"{contain}/{totalRequire}";
            progressBarUI.SetSlider(percent, percent, 0f, null);
            SetResUI(totalUse);
        }
        private void SetResUI(int totalUse)
        {
            int levelCurrent = ticketTallyManager.dataTicketTally.LevelByTotalUse(totalUse);
            levelCurrent = Mathf.Clamp(levelCurrent, 0, ticketTallyManager.dataTicketTally.MaxReward() - 1);
            var data = ticketTallyManager.dataTicketTally.dbEvent.GetRewardType(ETypeResource.None).gifts;
            DataResource dataResource = new DataResource();
            if (data[levelCurrent].groupGift.IsHasChest)
            {
                dataResource = new DataResource(data[levelCurrent].groupGift.typeChest, 1);
            }
            else
            {
                dataResource = data[levelCurrent].groupGift.dataResources[0];
            }
            resUI.InitData(dataResource);
        }

        public Tween SetSlider(int preTotalUse, int totalUse, Action onDone = null)
        {
            float timeFloat = 0.8f;

            int levelPre = ticketTallyManager.dataTicketTally.LevelByTotalUse(preTotalUse);
            int containPre = ticketTallyManager.dataTicketTally.ContainByTotalUse(preTotalUse);
            int totalRequirePre = ticketTallyManager.dataTicketTally.RequireByTotalUse(preTotalUse);
            float percentPre = (float)((float)containPre / (float)totalRequirePre);


            int levelCurrent = ticketTallyManager.dataTicketTally.LevelByTotalUse(totalUse);
            int containCurrent = ticketTallyManager.dataTicketTally.ContainByTotalUse(totalUse);
            int totalRequireCurrent = ticketTallyManager.dataTicketTally.RequireByTotalUse(totalUse);
            float percentCurrent = (float)((float)containCurrent / (float)totalRequireCurrent);

            Sequence sq = DOTween.Sequence();
            txtTicketCurrent.text = $"{containPre}/{totalRequirePre}";
            if (levelCurrent > levelPre)
            {
                float timePre = (1 - percentPre) * timeFloat;
                float timeEnd = (percentCurrent) * timeFloat;
                sq.AppendCallback(() =>
                {
                    SetResUI(preTotalUse);
                });
                sq.Append(progressBarUI.SetSliderTween(percentPre, 1, timePre, null));
                sq.Join(DOVirtual.Int(containPre, totalRequirePre, timePre, (x) => txtTicketCurrent.text = $"{x}/{totalRequirePre}"));
                for (int i = levelPre + 1; i < levelCurrent; i++)
                {
                    int total = ticketTallyManager.dataTicketTally.TotalUseByLevel(i);
                    sq.Append(SetSliderFull(total, timeFloat));
                }
                sq.AppendCallback(() =>
                {
                    SetResUI(totalUse);
                });
                sq.Append(progressBarUI.SetSliderTween(0, percentCurrent, timeEnd, null));
                sq.Join(DOVirtual.Int(0, containCurrent, timeEnd, (x) => txtTicketCurrent.text = $"{x}/{totalRequireCurrent}"));
            }
            else
            {
                sq.AppendCallback(() =>
                {
                    SetResUI(totalUse);
                });
                sq.Append(progressBarUI.SetSliderTween(percentPre, percentCurrent, timeFloat, null));
                sq.Join(DOVirtual.Int(containPre, containCurrent, timeFloat, (x) => txtTicketCurrent.text = $"{x}/{totalRequireCurrent}"));
            }
            return sq;
        }
        private Tween SetSliderFull(int totalUse, float timeFloat)
        {
            totalUse = Mathf.Clamp(totalUse - 1, 0, 1000000);
            int totalRequire = ticketTallyManager.dataTicketTally.RequireByTotalUse(totalUse);
            Sequence sq = DOTween.Sequence();
            sq.AppendCallback(() =>
            {
                SetResUI(totalUse);
            });
            sq.Append(progressBarUI.SetSliderTween(0, 1, timeFloat, null));
            sq.Join(DOVirtual.Int(0, totalRequire, timeFloat, (x) => txtTicketCurrent.text = $"{x}/{totalRequire}"));
            return sq;
        }
        public void SetSlider(int preIndex, int curIndex, float prePercent, float curPercent, Action onDone = null)
        {

            int preRequire = ticketTallyManager.dataTicketTally.RequireByLevel(preIndex);
            int curRequire = ticketTallyManager.dataTicketTally.RequireByLevel(curIndex);
            int curContain = ticketTallyManager.dataTicketTally.ContainItem;

        }
        public override void InitEvent(Action action)
        {
            base.InitEvent(ClickTicketTally);
        }
        private void ClickTicketTally()
        {
            UIManager.Instance.ShowUI<UITicketTally>(UIName.TicketTally);
        }
    }

}