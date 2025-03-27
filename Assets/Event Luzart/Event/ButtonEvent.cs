namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ButtonEvent : MonoBehaviour
    {
        public EEventName eEventName;
        public Button btn;
        public bool isAnimButton = true;
        public bool isAutoInit = true;
        protected virtual void Start()
        {
            GameUtil.ButtonOnClick(btn, ClickBtnEvent, isAnimButton);
            if (isAutoInit)
            {
                InitEvent(null);
            }
        }

        public Action actionClick;
        public bool IsActiveEvent
        {
            get
            {
                return EventManager.Instance.IsHasEvent(eEventName);
            }
        }
        public bool IsUnlockLevel
        {
            get
            {
                return EventManager.Instance.IsUnlockLevel(eEventName);
            }
        }
        public virtual void InitEvent(Action action)
        {
            this.actionClick = action;
            if (IsActiveEvent && IsUnlockLevel)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }
            InitButton();
        }
        protected virtual void InitButton()
        {

        }
        private void ClickBtnEvent()
        {
            actionClick?.Invoke();
        }
    }
}
