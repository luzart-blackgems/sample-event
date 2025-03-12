namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    
    public class BattlePassItemUI : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if(_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }
        public ItemResBattlePass itemResFree;
        public ItemResBattlePass itemResIAP;
        public TMP_Text txtLineLevel;
    
        public BaseSelect selectOnComplete;
        public BaseSelect selectCurrent;
    
        public int index;
    
        public void InitItemFree(DataResource[] itemRes, EStateClaim state, Action<ItemResBattlePass> action, DataTypeResource chest)
        {
            itemResFree.InitVisual(index, chest,state, action, itemRes);
        }
        public void InitItemIAP(DataResource[] itemRes, EStateClaim state, Action<ItemResBattlePass> action, DataTypeResource chest)
        {
            itemResIAP.InitVisual(index,chest, state, action, itemRes);
        }
        public void InitSliderBattlePass(EStatePass eStatePass, int index ,float percent)
        {
            this.index = index;
            txtLineLevel.text = (index+1).ToString();
            SwitchMedal(eStatePass);
        }
        private void DisableAllSelect()
        {
            selectCurrent.Select(false);
            selectOnComplete.Select(false);
        }
        private void SwitchMedal(EStatePass eState)
        {
            DisableAllSelect();
            switch (eState)
            {
                case EStatePass.Pass:
                    {
                        selectOnComplete.Select(true);
                        break;
                    }
                case EStatePass.Current:
                    {
                        selectCurrent.Select(true);
                        break;
                    }
                default:
                    break;
            }
        }
    }
    public enum EStatePass
    {
        Pass,
        Current,
        WillPass,
    }
}
