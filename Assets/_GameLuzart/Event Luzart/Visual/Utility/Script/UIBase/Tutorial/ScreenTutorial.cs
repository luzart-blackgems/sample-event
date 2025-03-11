namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ScreenTutorial : MonoBehaviour
    {
        public Action ActionClick;
        public Button btn;
        public virtual void Setup()
        {
            GameUtil.ButtonOnClick(btn, OnClick);
        }
        private void OnClick()
        {
            ActionClick?.Invoke();
        }
        public virtual void Show(Action ActionClick)
        {
            this.ActionClick = ActionClick;
        }
        public virtual void RefreshUI()
        {

        }
        public virtual void Hide()
        {
            actionHide?.Invoke();
        }
        public Action actionHide;
        public virtual void SpawnItem(GameObject gO)
        {
            if (gO == null)
            {
                return;
            }
            var item = Instantiate(gO, transform);
            item.transform.position = gO.transform.position;
            item.transform.rotation = gO.transform.rotation;
            OnDoneSpawnItem(gO);
        }

        public virtual void OnDoneSpawnItem(GameObject gO)
        {
            RectTransform rt = gO.GetComponent<RectTransform>();
            btn.transform.SetAsLastSibling();
            RectTransform rectTransform = rt;
            RectTransform parentRectTransform = rectTransform.parent as RectTransform;

            if (parentRectTransform != null)
            {
                Vector2 parentSize = parentRectTransform.rect.size;
                Vector2 anchorMin = rectTransform.anchorMin;
                Vector2 anchorMax = rectTransform.anchorMax;

                // Kích thước thực tế của rectTransform khi anchors khác nhau
                Vector2 realSizeDelta = new Vector2(
                    (anchorMax.x - anchorMin.x) * parentSize.x + rectTransform.sizeDelta.x,
                    (anchorMax.y - anchorMin.y) * parentSize.y + rectTransform.sizeDelta.y
                );
                btn.image.rectTransform.sizeDelta = realSizeDelta;
            }

            btn.transform.position = gO.transform.position;
        }
    }

}