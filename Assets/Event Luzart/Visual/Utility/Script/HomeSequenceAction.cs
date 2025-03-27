using Luzart;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSequenceAction : MonoBehaviour
{
    public UIMoveToTicketTally uiMoveToTicketTally;

    private void Start()
    {
        GameUtil.StepToStep(new Action<Action>[]
        {

            OnShowTutorialTicketTally,
            MoveTicketTally

        });
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
        uiMoveToTicketTally.AnimationAndCheckTicket(onDone);
    }
}
