namespace Luzart
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class UIRiseOfKittens : UIBase
    {
        public ScrollRect scrollRect;
        public Transform parentSpawn;
        public ItemRiseOfKittens itemRisePf;
        public Transform parentLine, parentMedal;
        private List<ItemRiseOfKittens> listItemRiseOfKittens = new List<ItemRiseOfKittens> ();
    
        private RiseOfKittensManager riseOfKittensManager
        {
            get
            {
                return EventManager.Instance.riseOfKittensManager;
            }
        }
        private ItemRiseOfKittens itemRiseCache;
    
        protected override void Setup()
        {
            base.Setup();
        }
        public override void Show(Action onHideDone)
        {
            base.Show(onHideDone);
            RefreshUI();
        }
        public override void RefreshUI()
        {
            base.RefreshUI();
            Initialize();
        }
    
        private void Initialize()
        {
            var dbGiftEvent = riseOfKittensManager.dataRiseOfKittens.dbEvent.GetRewardType(ETypeResource.None);
            var list = dbGiftEvent.gifts;
            int length = list.Count;
            int levelCurrent = riseOfKittensManager.dataRiseOfKittens.LevelCurrent;
            MasterHelper.InitListObj(length, itemRisePf, listItemRiseOfKittens, parentSpawn, (item, index) =>
            {
                item.gameObject.SetActive(true);
                var db = list[index];
                item.Initialize(index, levelCurrent, db, ClickItem);
            });
            levelCurrent--;
            if (itemRiseCache == null)
            {
                if(levelCurrent < 0)
                {
                    scrollRect.verticalNormalizedPosition = 0;
                    itemRiseCache = listItemRiseOfKittens[0];
                }
                else
                {
                    itemRiseCache = listItemRiseOfKittens[levelCurrent];
                }
    
                GameUtil.Instance.WaitAndDo(0, () =>
                {
                    for (int i = 0; i < length; i++)
                    {
                        var item = listItemRiseOfKittens[i];
                        item.bsMedal.transform.SetParent(parentMedal);
                        item.progressBarLine.transform.SetParent(parentLine);
                    }
                    if (levelCurrent < 0)
                    {
                        return;
                    }
                    var rt = itemRiseCache.GetComponent<RectTransform>();
                    // Tính giá trị cuộn từ dưới lên
                    float normalizedPosition = Mathf.Clamp01(Mathf.Abs(rt.anchoredPosition.y) / scrollRect.content.rect.height);
    
                    // Cuộn đến phần tử
                    scrollRect.verticalNormalizedPosition = 1 - normalizedPosition;
                });
            }
    
            
    
        }
        private void ClickItem(ItemRiseOfKittens item)
        {
            itemRiseCache = item;
            switch (item.eState)
            {
                case EStateClaim.CanClaim:
                    {
                        ClaimChest(item);
                        break;
                    }
                default:
                    {
                        item.ShowChest();
                        break;
                    }
            }
        }
        private void ClaimChest(ItemRiseOfKittens item)
        {
            item.SelectAnim(true);
            SetBlock(true);
            GameUtil.Instance.WaitAndDo(0.6f, () =>
            {
                SetBlock(false);
                var data = item.dataRes;
                DataWrapperGame.ReceiveRewardShowPopUp(ValueFirebase.RiseOfKittensReceive, UIManager.Instance.RefreshUI, data);
                riseOfKittensManager.UseItem(item.index);
                UIManager.Instance.RefreshUI();
            }
        );
        }
        public override void Hide()
        {
            base.Hide();

        }
        public GameObject obBlock;
        public void SetBlock(bool isStatus)
        {
            obBlock.SetActive(isStatus);
        }
    
    }
}
