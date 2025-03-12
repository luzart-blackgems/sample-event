namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class UITutorialStepJourneyToSuccess : UITutorial
    {
        // Start is called before the first frame update
        public void InitTutorial(Action onDone)
        {
            //var uiMainMenu = UIManager.Instance.GetUiActive<UIMainMenu>(UIName.MainMenu);
            //var uiMainMenu_Home = uiMainMenu.GetUIScreenMainMenu(EMainMenu.Home) as MainMenu_Home;
            //if (uiMainMenu_Home == null)
            //{
            //    Hide();
            //    return;
            //}
            //ShowScreenTutorial(0, uiMainMenu_Home.btnJourneyToSuccess.gameObject, OnClickJourneyToSuccess);
    
            //void OnClickJourneyToSuccess()
            //{
            //    Hide();
            //    UIManager.Instance.ShowUI(UIName.JourneyToSuccess,onDone);
            //    DataManager.Instance.GameData.isTutorialJourneyToSuccess = true;
            //    DataManager.Instance.SaveGameData();
            //}
        }
    }
}
