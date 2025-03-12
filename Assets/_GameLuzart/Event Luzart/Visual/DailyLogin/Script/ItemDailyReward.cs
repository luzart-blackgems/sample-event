namespace Luzart
{
    using Luzart;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ItemDailyReward : MonoBehaviour
    {
        public Button btnClick;
        public DBDailyReward dbDailyReward;
        public ResUI resUI;
        public BaseSelect bsFrameCurrentDay;
        public BaseSelect bsSelect;
        public BaseSelect bsComplete;
        public EStateClaim eStateClaim;
        public TMP_Text txtDay;
        private Action<ItemDailyReward> actionClick;
        private DailyRewardManager dailyRewardManager
        {
            get
            {
                return EventManager.Instance.dailyRewardManager;
            }
        }
        private void Start()
        {
            GameUtil.ButtonOnClick(btnClick, Click);
        }
    
        public void Initialize(DBDailyReward db, Action<ItemDailyReward> actionClick)
        {
            this.dbDailyReward = db;
            this.actionClick = actionClick;
            txtDay.text = $"Day {dbDailyReward.index + 1}";
    
            CheckState();
            ChangeVisualOnState();
            InitResUI();
        }
        private void CheckState()
        {
            if (dailyRewardManager.dataDailyReward.IsClaimedDay(dbDailyReward.index))
            {
                eStateClaim = EStateClaim.Claimed;
                return;
            }
            int today = dailyRewardManager.Today;
            if (today > dbDailyReward.index)
            {
                eStateClaim = EStateClaim.CanClaimDontClaimed;
            }
            else if (today == dbDailyReward.index)
            {
                eStateClaim = EStateClaim.CanClaim;
            }
            else
            {
                eStateClaim = EStateClaim.WillClaim;
            }
        }
        private void ChangeVisualOnState()
        {
            int today = dailyRewardManager.Today;
            bool isToday = today == dbDailyReward.index;
            bsFrameCurrentDay.Select(isToday);
            switch (eStateClaim)
            {
                case EStateClaim.Claimed:
                    {
                        bsComplete.Select(2);
                        break;
                    }
                case EStateClaim.CanClaimDontClaimed:
                    {
                        bsComplete.Select(0);
                        break;
                    }
                case EStateClaim.WillClaim:
                    {
                        bsComplete.Select(1);
                        break;
                    }
                case EStateClaim.CanClaim:
                    {
                        bsComplete.Select(1);
                        break;
                    }
            }
        }
        private void Click()
        {
            actionClick?.Invoke(this);
        }
        public void Select(bool isSelect)
        {
            bsSelect.Select(isSelect);
        }
        private void InitResUI()
        {
            if(resUI!= null)
            {
                resUI.InitData(dbDailyReward.dataRes.dataResources[0]);
            }
        }
    
    }
    public class DBDailyReward
    {
        public int index;
        public GroupDataResources dataRes;
    }
}
