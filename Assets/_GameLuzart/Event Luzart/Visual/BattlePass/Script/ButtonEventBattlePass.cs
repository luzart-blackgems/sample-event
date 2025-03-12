namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    
    public class ButtonEventBattlePass : ButtonEvent
    {
        public TextMeshProUGUI txtTime;
        public TextMeshProUGUI txtLevelCurrent;
        public ProgressBarUI progressBarUI;
        public GameObject obNoti;
        private BattlePassManager battlePassManager 
        {
            get
            {
                return EventManager.Instance.battlePassManager;
            }
        }
        protected override void Start()
        {
            base.Start();
            if (IsActiveEvent)
            {
                Observer.Instance.AddObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
            }
        }
        protected override void OnDestroy()
        {
            Observer.Instance?.RemoveObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
        }
        private void OnTimePerSecond(object data)
        {
            long timeCurrent = TimeUtils.GetLongTimeCurrent;
            long timeContain = EventManager.Instance.battlePassManager.dataBattlePass.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
            txtTime.text = GameUtil.LongTimeSecondToUnixTime(timeContain);
        }
        private void CheckNoti()
        {
            bool isNoti = battlePassManager.IsHasDataDontClaim();
            GameUtil.SetActiveCheckNull(obNoti, isNoti);
        }
        protected override void InitButton()
        {
            SetItem(battlePassManager.dataBattlePass.totalUse);
            CheckNoti();
            OnTimePerSecond(null);
        }
        public void SetItem(int totalUse)
        {
            int levelCurrent = battlePassManager.CounDataDontClaim(totalUse);
            int contain = battlePassManager.dataBattlePass.ContainByTotalUse(totalUse);
            int totalRequire = battlePassManager.dataBattlePass.RequireByTotalUse(totalUse);
            float percent = (float)contain / (float)totalRequire;
            txtLevelCurrent.text = $"{levelCurrent}";
            progressBarUI.SetSlider(percent, percent, 0f, null);
        }
        public void SetSlider(int preTotalUse, int totalUse, Action onDone =null)
        {
            int levelPre = battlePassManager.dataBattlePass.LevelByTotalUse(preTotalUse);
            int containPre = battlePassManager.dataBattlePass.ContainByTotalUse(preTotalUse);
            int totalRequirePre = battlePassManager.dataBattlePass.RequireByTotalUse(preTotalUse);
            float percentPre = (float)((float)containPre / (float)totalRequirePre);
    
    
            int levelCurrent = battlePassManager.dataBattlePass.LevelByTotalUse(totalUse);
            int containCurrent = battlePassManager.dataBattlePass.ContainByTotalUse(totalUse);
            int totalRequireCurrent = battlePassManager.dataBattlePass.RequireByTotalUse(totalUse);
            float percentCurrent = (float)((float)containCurrent / (float)totalRequireCurrent);
    
            SetSlider(levelPre,levelCurrent,percentPre, percentCurrent, onDone);
        }
        public void SetSlider(int preIndex, int curIndex, float prePercent, float curPercent, Action onDone= null)
        {
            float time = 0.5f;
            float velocity = (curIndex - preIndex + curPercent - prePercent) / (float)time;
            if(curIndex > preIndex)
            {
                float timeFloat = (1 - prePercent)*velocity;
                progressBarUI.SetSlider(prePercent, 1, timeFloat, () =>
                {
                    int curReward = battlePassManager.CounDataDontClaim(battlePassManager.dataBattlePass.totalUse);
                    txtLevelCurrent.text = $"{curReward}";
                    progressBarUI.SetSlider(0, curPercent, time - timeFloat, onDone);
                });
            }
            else
            {
                progressBarUI.SetSlider(prePercent, curPercent, time, onDone);
            }
    
        }
    }
}
