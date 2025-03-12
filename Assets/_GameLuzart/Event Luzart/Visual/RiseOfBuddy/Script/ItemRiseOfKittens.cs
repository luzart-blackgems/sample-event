namespace Luzart
{
    using JetBrains.Annotations;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ItemRiseOfKittens : MonoBehaviour
    {
        public BaseSelect bsChest;
        public BaseSelect bsMedal;
        public BaseSelect bsChestAnim;
        public BaseSelect bsChestInChestAnim;
        public BaseSelect bsNotiChest;
        public BaseSelect bsChestOpen;
        public ProgressBarUI progressBarLine;
        public Button btnChest;
        private Action<ItemRiseOfKittens> actionClick = null;
        public DataResource[] dataRes;
        public int index;
        public BoxInforMess boxInforMess;
        public TMP_Text txtIndex;
        private RiseOfKittensManager riseOfKittensManager
        {
            get
            {
                return EventManager.Instance.riseOfKittensManager;
            }
        }
        public EStateClaim eState;
        void Start()
        {
            GameUtil.ButtonOnClick(btnChest, ClickChest, true);
        }
        public void Initialize(int index , int levelCurrent , GiftEvent dB_giftEvent ,Action<ItemRiseOfKittens> actionClick = null)
        {
            this.index = index;
            this.dataRes = dB_giftEvent.groupGift.dataResources.ToArray();
            this.actionClick = actionClick;
            txtIndex.text = riseOfKittensManager.dataRiseOfKittens.TotalRequireByLevel(index).ToString();
            SelectAnim(false);
            int idTypeChest = dB_giftEvent.typeChest.id;
            bsChest.Select(idTypeChest);
            bsChestInChestAnim.Select(idTypeChest);
            eState = EStateClaim.WillClaim;
            if (dB_giftEvent.isClaimed)
            {
                eState = EStateClaim.Claimed;
            }
            else if(index < levelCurrent)
            {
                eState = EStateClaim.CanClaim;
            }
            bsNotiChest.Select(false);
            switch (eState)
            {
                case EStateClaim.Claimed:
                    {
                        bsChestOpen.Select(2);
                        break;
                    }
                case EStateClaim.CanClaim:
                    {
                        bsChestOpen.Select(1);
                        bsNotiChest.Select(true);
                        break;
                    }
                case EStateClaim.WillClaim:
                    {
                        bsChestOpen.Select(0);
                        break;
                    }
            }
            bool isPass = index > levelCurrent;
            if (index < levelCurrent)
            {
                progressBarLine.SetSlider(1, 1, 0, null);
            }
            else if(index == levelCurrent)
            {
                float require = riseOfKittensManager.dataRiseOfKittens.Require;
                float contain = riseOfKittensManager.dataRiseOfKittens.ContainItem;
                float percent = contain/require;
                progressBarLine.SetSlider(percent, percent, 0, null);
    
            }
            else
            {
                progressBarLine.SetSlider(0,0,0, null);
            }
            bsMedal.Select(isPass);
            boxInforMess.InitMess(EStateClaim.Chest, dataRes);
        }
        public void ShowChest()
        {
            boxInforMess.gameObject.SetActive(true);
        }
        private void ClickChest()
        {
            actionClick?.Invoke(this);
        }
        public void SelectAnim(bool isSelect)
        {
            bsChestAnim.Select(isSelect);
        }
    }
}
