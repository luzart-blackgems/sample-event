using DG.Tweening;
using Luzart;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SequenceActionEventTicketTally : SequenceActionEvent
{
    [SerializeField]
    private ButtonEventTicketTally _btnTicketTally;
    public ButtonEventTicketTally btnTicketTally
    {
        get
        {
            if (_btnTicketTally == null)
            {
                _btnTicketTally = FindAnyObjectByType<ButtonEventTicketTally>();
            }
            return _btnTicketTally;
        }
    }

    public RectTransform rtTicketTally;
    public TMP_Text txtValueTicketTally;

    public override void PreInit()
    {
        TicketTallyManager ticketTallyManager = EventManager.Instance.ticketTallyManager;
        int countKey = ticketTallyManager.valueKey;
        int totalCurrent = ticketTallyManager.dataTicketTally.totalUse;
        int preTotalCurrent = totalCurrent - countKey;
        btnTicketTally.SetVisual(preTotalCurrent);
    }

    public override void Init(Action callback)
    {
        GameUtil.StepToStep(new Action<Action>[]
        {
            OnShowTutorialTicketTally,
            MoveTicketTally,
            OnDoneCallBack
        });

        void OnDoneCallBack(Action onDone)
        {
            callback?.Invoke();
        }
    }


    private void OnShowTutorialTicketTally(Action onDone)
    {
        bool isHasEvent = EventManager.Instance.IsHasEvent(EEventName.TicketTally);
        bool isUnlockEvent = EventManager.Instance.IsUnlockLevel(EEventName.TicketTally);
        bool isTutorial = EventManager.Instance.dataEvent.isCompleteTutorialTicket;
        if (isHasEvent && isUnlockEvent && !isTutorial)
        {
            var uiNoti = Luzart.UIManager.Instance.ShowUI<UITicketTallyNoti>(UIName.TicketTallyNoti, () =>
            {
                var ui = Luzart.UIManager.Instance.ShowUI<UITutorialStepTicketTally>(UIName.TutorialStepTicketTally);
                ui.btnTicket = btnTicketTally;
                ui.InitTutorial(onDone);
            });
        }
        else
        {
            onDone?.Invoke();
        }
    }
    private void MoveTicketTally(Action onDone)
    {
        AnimationAndCheckTicket(onDone);
    }

    public void AnimationAndCheckTicket(Action onDone)
    {
        if (IsHasAnimTicketDaily)
        {
            AnimationTicketTally(onDone);
        }
        else
        {
            onDone?.Invoke();
        }
    }
    public void AnimationTicketTally(Action onDone = null)
    {
        TicketTallyManager ticketTallyManager = EventManager.Instance.ticketTallyManager;
        ticketTallyManager.IsCacheShowVisual = false;
        int countKey = ticketTallyManager.valueKey;
        int totalCurrent = ticketTallyManager.dataTicketTally.totalUse;
        int preTotalCurrent = totalCurrent - countKey;
        btnTicketTally.SetVisual(preTotalCurrent);

        txtValueTicketTally.text = $"x{countKey}";
        Sequence sq = DOTween.Sequence();
        sq.AppendCallback(() => BlockScreen(true));
        sq.AppendCallback(() => rtTicketTally.gameObject.SetActive(true));
        sq.AppendInterval(0.2f);
        sq.Append(rtTicketTally.transform.DOJump(btnTicketTally.transform.position, 1, 1, 1f));
        sq.AppendCallback(() => rtTicketTally.gameObject.SetActive(false));
        sq.Append(btnTicketTally.transform.DOPunchScale(Vector3.one * 1.2f - Vector3.one, 0.5f, 1, 1));
        sq.Append(btnTicketTally.SetSlider(preTotalCurrent, totalCurrent, null));
        sq.AppendCallback(() =>
        {
            OnShowRewardTicket(preTotalCurrent, totalCurrent, onDone);
            BlockScreen(false);
        });
    }
    public bool IsHasAnimTicketDaily
    {
        get
        {
            if (EventManager.Instance.IsHasEvent(EEventName.TicketTally) && EventManager.Instance.IsUnlockLevel(EEventName.TicketTally, DataWrapperGame.CurrentLevel - 1))
            {
                TicketTallyManager ticketTallyManager = EventManager.Instance.ticketTallyManager;
                if (ticketTallyManager != null)
                {
                    if (ticketTallyManager.IsCacheShowVisual && ticketTallyManager.valueKey > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    private void OnShowRewardTicket(int preTotal, int total, Action onDone)
    {
        var dataTicket = EventManager.Instance.ticketTallyManager.dataTicketTally;

        int preIndex = dataTicket.LevelByTotalUse(preTotal);
        int curIndex = dataTicket.LevelByTotalUse(total);
        if (curIndex <= preIndex)
        {
            onDone?.Invoke();
            return;
        }
        List<DataResource> listDataRes = new List<DataResource>();
        for (int i = preIndex; i < curIndex; i++)
        {
            var listRes = dataTicket.GetDBGiftEvent(i).groupGift.dataResources;
            listDataRes.AddRange(listRes);
            EventManager.Instance.ticketTallyManager.UseItem(i);
        }
        DataWrapperGame.ReceiveReward(ValueFirebase.TicketTally, listDataRes.ToArray());
        var ui = Luzart.UIManager.Instance.ShowUI<UIReceiveRewardEventTicket>(UIName.ReceiveRewardEventTicket, onDone);
        ui.Initialize(listDataRes.ToArray());
    }
    private void BlockScreen(bool isBlock)
    {
        Debug.LogError("BlockScreen");
    }
}
