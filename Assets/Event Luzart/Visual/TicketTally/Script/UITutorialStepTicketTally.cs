namespace Luzart
{
    using System;

    public class UITutorialStepTicketTally : UITutorial
    {
        public ButtonEventTicketTally btnTicket;
        public void InitTutorial(Action onDone)
        {
            if (btnTicket == null)
            {
                btnTicket = FindAnyObjectByType<ButtonEventTicketTally>();
            }

            if(btnTicket == null)
            {
                return;
            }
            ShowScreenTutorial(0, btnTicket.gameObject, OnClickTicketTally);

            void OnClickTicketTally()
            {
                Hide();
                UIManager.Instance.ShowUI(UIName.TicketTally, onDone);
                EventManager.Instance.dataEvent.isCompleteTutorialTicket = true;
                EventManager.Instance.SaveAllData();
            }
        }
    }

}