namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ItemResBattlePass : MonoBehaviour
    {
        private RectTransform _rt;
        public RectTransform rectTransform
        {
            get
            {
                if (_rt == null)
                {
                    _rt = GetComponent<RectTransform>();
                }
                return _rt;
            }
        }
        public int index { get; set; } = 0;
        public Button btnClick;
        private Action<ItemResBattlePass> actionClick;
        public ResUI resUI;
        public GameObject obClaimed;
        public GameObject obCanClaim;
        public GameObject obWillClaim;
        public GameObject obIAP;
        public BoxInforMess boxInforMess;
        
        void Start()
        {
            GameUtil.ButtonOnClick(btnClick, ClickButton);
        }
        private void ClickButton()
        {
            actionClick?.Invoke(this);
        }
        public DataResource[] dataRes;
        public EStateClaim eState;
        public void InitVisual(int index,DataTypeResource chest,EStateClaim stateClaim, Action<ItemResBattlePass> actionClick, params DataResource[] data)
        {
            this.index = index;
            this.dataRes = data;
            eState = stateClaim;
            this.actionClick = actionClick;
            if (chest.id != 0)
            {
                resUI.InitData(new DataResource(chest, 1));
            }
            else
            {
                resUI.InitData(data[0]);
            }
    
            SwitchState();
            if(chest.id != 0)
            {
                stateClaim = EStateClaim.Chest;
            }
            boxInforMess.InitMess(stateClaim, data);
    
        }
        private void SwitchState()
        {
            DisableAllButton();
            switch (eState)
            {
                case EStateClaim.CanClaim:
                    {
                        GameUtil.SetActiveCheckNull(obCanClaim, true);
                        break;
                    }
                case EStateClaim.Claimed:
                    {
                        GameUtil.SetActiveCheckNull(obClaimed, true);
                        break;
                    }
                case EStateClaim.WillClaim:
                    {
                        GameUtil.SetActiveCheckNull(obWillClaim, true);
                        break;
                    }
                case EStateClaim.NeedIAP:
                    {
                        GameUtil.SetActiveCheckNull(obIAP, true);
                        break;
                    }
                default: 
                            break;
            }
        }
        private void DisableAllButton()
        {
            GameUtil.SetActiveCheckNull(obCanClaim, false);
            GameUtil.SetActiveCheckNull(obClaimed, false);
            GameUtil.SetActiveCheckNull(obWillClaim, false);
            GameUtil.SetActiveCheckNull(obIAP, false);
        }
    }
    
}
