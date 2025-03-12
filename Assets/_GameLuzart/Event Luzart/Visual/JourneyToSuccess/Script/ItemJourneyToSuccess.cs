namespace Luzart
{
#if ENABLE_IAP
    using BG_Library.IAP;
#endif
    using DG.Tweening;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ItemJourneyToSuccess : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public CanvasGroup canvasGroupLock;
        public Button btn;
        public ListResUI listResUI;
        public BaseSelect selectLock;
        public BaseSelect selectIAP ;
        private Action<ItemJourneyToSuccess> actionClick;
        private DataResource[] dataRes;
#if ENABLE_IAP
        public IAPProductPriceLegacy iapPrice;
#endif
        public int index;
        public EStateClaim eState;
        public DB_Pack dbPack;
        private void Start()
        {
            GameUtil.ButtonOnClick(btn, Click, true);
        }
        private void Click()
        {
            actionClick?.Invoke(this);
        }
        public void InitializeData(int index, EStateClaim eStateClaim, Action<ItemJourneyToSuccess> actionClick,  params DataResource[] dataRes)
        {
            this.actionClick = actionClick;
            this.dataRes = dataRes;
            this.index = index;
            this.eState = eStateClaim;
            bool isReceive = eStateClaim == EStateClaim.Claimed;
            if (isReceive)
            {
                gameObject.SetActive(false);
                return;
            }
            bool isLock = eStateClaim == EStateClaim.WillClaim;
            selectLock.Select(isLock);
            var db = EventManager.Instance.journeyToSuccessManager.GetDBIAP(index);
            bool isIAP = !db.IsDefault;
            selectIAP.Select(isIAP);
            listResUI.InitResUI(dataRes);
            if (isIAP)
            {
                this.dbPack = db.dbPack;
#if ENABLE_IAP
                iapPrice.SetIAPProductStats(db.stats);
#endif
            }
        }
        public void OnHide(Action onDone, Action onUpdate)
        {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            Vector2 size = rt.sizeDelta;
            Sequence sq = DOTween.Sequence();
            sq.Append(canvasGroup.DOFade(0, 0.35f));
            sq.Append(rt.DOSizeDelta(new Vector2(size.x, 0), 0.2f).OnUpdate(() =>
            {
                onUpdate?.Invoke();
            }));
            sq.AppendCallback(()=> onDone?.Invoke());
            sq.SetId(this);
        }
        public void UnlockAnim(Action onDone)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(selectLock.transform.DOScale(2.5f, 0.35f));
            sequence.Join(canvasGroupLock.DOFade(0, 0.35f));
            sequence.AppendCallback(() => { onDone?.Invoke(); });
            sequence.SetId(this);
    
            
        }
        private void OnDisable()
        {
            this.DOKill(false);
        }
    }
}
