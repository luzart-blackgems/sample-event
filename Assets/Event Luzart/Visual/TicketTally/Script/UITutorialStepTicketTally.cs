namespace Luzart
{
    using System;

    public class UITutorialStepTicketTally : UITutorial
    {
        public void InitTutorial(Action onDone)
        {
            var buttonTicketTally = FindAnyObjectByType<ButtonEventTicketTally>();
            ShowScreenTutorial(0, buttonTicketTally.gameObject, OnClickTicketTally);

            void OnClickTicketTally()
            {
                Hide();
                UIManager.Instance.ShowUI(UIName.TicketTally, onDone);
                EventManager.Instance.dataEvent.isCompleteTutorialTicket = true;
            }
        }
    }

}