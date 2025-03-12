namespace Luzart
{
    using DG.Tweening;
    using Eco.TweenAnimation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class UIBattlePass : UIBase
    {
        public TweenAnimation twHide;
        [Space, Header("Scroll View")]
        public ScrollRect scrollRect;
        public BattlePassItemUI battlePassItemPf;
        public Transform parentBattlePassItem;
        public List<BattlePassItemUI> listItem = new List<BattlePassItemUI>();
    
        [Space, Header("Other")]
        public TMP_Text txtContainCurrent;
        public ProgressBarUI progressBarUI;
        public TMP_Text txtLevelCurrent;
        public TMP_Text txtTime;
        public Button btnActive, btnAutumnPass, btnInfor;
        public GameObject obEnergyBar;
    
        private DB_Event dBEvent;
        private DB_GiftEvent dataNormal;
        private DB_GiftEvent dataVIP;
        
        private BattlePassManager battlePassManager
        {
            get
            {
                return EventManager.Instance.battlePassManager;
            }
        }
        protected override void Setup()
        {
            isAnimBtnClose = true;
            base.Setup();
            GameUtil.ButtonOnClick(btnActive,ClickBattlePass);
            GameUtil.ButtonOnClick(btnAutumnPass,ClickBattlePass);
            GameUtil.ButtonOnClick(btnInfor, ClickInforBattlePass);
        }
    
        public override void Show(Action onHideDone)
        {
            base.Show(onHideDone);
            RefreshUI();
            OnTimePerSecond(null);
            int levelCurrent = battlePassManager.dataBattlePass.LevelCurrent;
            if (battlePassManager.dataBattlePass.IsMaxLevel)
            {
                int max = listItem.Count - 1;
                battlePassItemUI = listItem[max];
            }else if (levelCurrent >= 2)
            {
                battlePassItemUI = listItem[levelCurrent];
                OnMoveToAnimation();
            }
        }
        public override void RefreshUI()
        {
            base.RefreshUI();
            dBEvent = battlePassManager.dataBattlePass.dbEvent;
            if (dBEvent == null || dBEvent.eventStatus == EEventStatus.Finish)
            {
                Hide();
                return;
            }
            dataNormal = dBEvent.GetRewardType(ETypeResource.None);
            dataVIP = dBEvent.GetRewardType(ETypeResource.VIP);
            int length = dataNormal.gifts.Count;
            MasterHelper.InitListObj(length, battlePassItemPf, listItem, parentBattlePassItem, (item, index) =>
            {
                item.gameObject.SetActive(true);
                InitItem(item, index);
            });
            SetUpProgressKey();
    
    
    
        }
        private BattlePassItemUI battlePassItemUI = null;
        private void OnMoveToAnimation()
        {
            GameUtil.Instance.WaitAndDo(0.2f, () =>
            {
                var preBattle = listItem[battlePassItemUI.index - 1];
                scrollRect.FocusOnRectTransform(preBattle.rectTransform);
            });
        }
        private void ClickBattlePass()
        {
            if (battlePassManager.dataBattlePass.isBuyIAP)
            {
                return;
            }
            UIManager.Instance.ShowUI(UIName.PackBattlePass);
            FirebaseWrapperLog.LogWithParam(KeyFirebase.BattlePassPremiumClick, new ParameterFirebaseCustom[]
            {
                new ParameterFirebaseCustom(TypeFirebase.IDEvent, dBEvent.idEvent),
                new ParameterFirebaseCustom(TypeFirebase.Duration, battlePassManager.TimeContain.ToString()),
                new ParameterFirebaseCustom(TypeFirebase.Location, ValueFirebase.BattlePassPopUp),
            });
        }
    
        private void ClickInforBattlePass()
        {
            UIManager.Instance.ShowUI(UIName.BattlePassTutorial);
        }
    
        private void SetUpProgressKey()
        {
            int levelCurrent = battlePassManager.dataBattlePass.LevelCurrent;
            int contain = battlePassManager.dataBattlePass.ContainItem;
            int totalRequire = 0;
            if (battlePassManager.dataBattlePass.IsMaxLevel)
            {
                levelCurrent = levelCurrent - 1;
                totalRequire = battlePassManager.dataBattlePass.dbEvent.GetRewardType(ETypeResource.None).gifts[levelCurrent].require;
            }
            else
            {
                totalRequire = battlePassManager.dataBattlePass.dbEvent.GetRewardType(ETypeResource.None).gifts[levelCurrent].require;
            }
            
            txtContainCurrent.text = $"{contain}/{totalRequire}";
            txtLevelCurrent.text = $"{levelCurrent}";
            float percent = (float)contain/(float)totalRequire;
            progressBarUI.SetSlider(percent, percent, 0f, null);
        }
        private void InitItem(BattlePassItemUI item, int index)
        {
            int levelCurrent = battlePassManager.dataBattlePass.LevelCurrent;
            int contain = battlePassManager.dataBattlePass.ContainItem;
            int totalRequire = 0;
            if (battlePassManager.dataBattlePass.IsMaxLevel)
            {
    
            }
            else
            {
                totalRequire = battlePassManager.dataBattlePass.dbEvent.GetRewardType(ETypeResource.None).gifts[levelCurrent].require;
            }
    
            float percent = 0f;
            EStatePass eStatePass = EStatePass.WillPass;
            if (levelCurrent > index)
            {
                percent = 1f;
                eStatePass = EStatePass.Pass;
            }
            else if(levelCurrent == index)
            {
                percent = (float)contain / (float)totalRequire;
                eStatePass = EStatePass.Current;
            }
            var dataNor = dataNormal.gifts[index];
            var dataVip = dataVIP.gifts[index];
            
            item.InitSliderBattlePass(eStatePass,index, percent);
            
    
    
            // Normal
            EStateClaim eStateNormal = EStateClaim.WillClaim;
            if (dataNor.isClaimed)
            {
                eStateNormal = EStateClaim.Claimed;
            }
            else
            {
                if (levelCurrent > index)
                {
                    eStateNormal = EStateClaim.CanClaim;
                }
            }
            item.InitItemFree(dataNor.groupGift.dataResources.ToArray(), eStateNormal, ClickItemNormal,dataNor.typeChest);
    
            // VIP
            EStateClaim eStateVIP = EStateClaim.NeedIAP;
            if (battlePassManager.dataBattlePass.isBuyIAP)
            {
                eStateVIP = EStateClaim.WillClaim;
                if (dataVip.isClaimed)
                {
                    eStateVIP = EStateClaim.Claimed;
                }
                else
                {
                    if (levelCurrent > index)
                    {
                        eStateVIP = EStateClaim.CanClaim;
                    }
                }
            }
    
            item.InitItemIAP(dataVip.groupGift.dataResources.ToArray(), eStateVIP, ClickItemVIP, dataVip.typeChest);
    
        }
    
        private void ClickItemNormal(ItemResBattlePass item)
        {
            switch (item.eState)
            {
                case EStateClaim.CanClaim:
                    {
                        DataWrapperGame.ReceiveRewardShowPopUp(ValueFirebase.BattlePassFree, dataResource: item.dataRes);
                        battlePassManager.UseItemNormal(item.index);
                        RefreshUI();
                        break;
                    }
                default:
                    {
                        item.boxInforMess.gameObject.SetActive(true);
                        break;
                    }
            }
        }
        private void ClickItemVIP(ItemResBattlePass item)
        {
            switch (item.eState)
            {
                case EStateClaim.CanClaim:
                    {
                        DataWrapperGame.ReceiveRewardShowPopUp(ValueFirebase.BattlePassPay ,dataResource: item.dataRes);
                        battlePassManager.UseItemVIP(item.index);
                        RefreshUI();
                        break;
                    }
                default:
                    {
                        item.boxInforMess.gameObject.SetActive(true);
                        break;
                    }
            }
        }
        public override void Hide()
        {
            twHide.Show();
            GameUtil.Instance.WaitAndDo(twHide.BaseOptions.Duration, () =>
            {
                base.Hide();
                UIManager.Instance.RefreshUI();
            });
    
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
            long timeContain = battlePassManager.dataBattlePass.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
            txtTime.text = GameUtil.LongTimeSecondToUnixTime(timeContain);
        }
    
    }
}
