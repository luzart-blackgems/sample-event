namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ItemTicketTallyUI : MonoBehaviour
    {
        public Button btnClick;
        public int level;
        public List<DataResource> dataRes;
        public ResUI resUI;
        public TMP_Text txtStt;
        public GameObject obLock, obComplete;
        public BoxInforMess boxInforMess;
        void Start()
        {
            GameUtil.ButtonOnClick(btnClick, Click);
        }
        private Action<ItemTicketTallyUI> ActionClick = null;
        private void Click()
        {
            ActionClick?.Invoke(this);
        }
        public void Initialize(int level, GiftEvent giftEvent, EStateClaim eState, Action<ItemTicketTallyUI> actionClick)
        {
            this.ActionClick = actionClick;
            this.level = level;
            this.dataRes = giftEvent.groupGift.dataResources;
            txtStt.text = $"{level + 1}";
            if (giftEvent.IsHasChest)
            {
                resUI.InitData(new DataResource(giftEvent.groupGift.typeChest, 1));
            }
            else
            {
                resUI.InitData(giftEvent.groupGift.dataResources[0]);
            }
            SwitchState(eState);
            boxInforMess.InitMess(eState, null);
        }
        private void SwitchState(EStateClaim eState)
        {
            DisableAllButton();
            switch (eState)
            {
                case EStateClaim.CanClaim:
                    {
                        break;
                    }
                case EStateClaim.Claimed:
                    {
                        GameUtil.SetActiveCheckNull(obComplete, true);
                        break;
                    }
                case EStateClaim.WillClaim:
                    {
                        GameUtil.SetActiveCheckNull(obLock, true);
                        break;
                    }
                default:
                    break;
            }
        }
        private void DisableAllButton()
        {
            GameUtil.SetActiveCheckNull(obLock, false);
            GameUtil.SetActiveCheckNull(obComplete, false);
        }
    }
}

