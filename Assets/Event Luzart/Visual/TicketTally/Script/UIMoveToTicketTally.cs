using DG.Tweening;
using Luzart;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMoveToTicketTally : MonoBehaviour
{
    public ButtonEventTicketTally btnTicketTally;

    public RectTransform rtTicketTally;
    public TMP_Text txtValueTicketTally;
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
        Debug.LogError("[UIMoveTOTicketTally] Chua Block Canvas " + isBlock);
    }
}
