namespace Luzart
{
    using TMPro;

    public class UIPackBattlePass : UIPack
    {
        public TMP_Text txtTime;
        private void Awake()
        {
            Observer.Instance?.AddObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
        }
        private void OnDestroy()
        {
            Observer.Instance?.RemoveObserver(ObserverKey.TimeActionPerSecond, OnTimePerSecond);
        }
        private void OnTimePerSecond(object data)
        {
            long timeCurrent = TimeUtils.GetLongTimeCurrent;
            long timeContain = EventManager.Instance.battlePassManager.dataBattlePass.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
            txtTime.text = GameUtil.LongTimeSecondToUnixTime(timeContain);
        }
        public override void InitIAP()
        {

            if (!EventManager.Instance.IsHasEvent(EEventName.BattlePass) || EventManager.Instance.battlePassManager.dataBattlePass.isBuyIAP)
            {
                Hide();
                return;
            }
            OnTimePerSecond(null);
        }
        protected override void OnCompleteBuy()
        {
            PackManager.Instance.SaveBuyPack(db_Pack.productId);
            EventManager.Instance.battlePassManager.dataBattlePass.isBuyIAP = true;
            EventManager.Instance.SaveDataEvent();

            Hide();
            UIManager.Instance.RefreshUI();
            PushFirebase();
        }
        private void PushFirebase()
        {
            FirebaseWrapperLog.LogWithParam(KeyFirebase.BattlePassPremiumClick, new ParameterFirebaseCustom[]
    {
            new ParameterFirebaseCustom(TypeFirebase.IDEvent, EventManager.Instance.battlePassManager.dataBattlePass.dbEvent.idEvent),
            new ParameterFirebaseCustom(TypeFirebase.Duration, EventManager.Instance.battlePassManager.TimeContain.ToString()),
            new ParameterFirebaseCustom(TypeFirebase.Location, ValueFirebase.BattlePassPopUp),
    });
        }
        public override void HideCallAnimFly()
        {

        }
    }
}

