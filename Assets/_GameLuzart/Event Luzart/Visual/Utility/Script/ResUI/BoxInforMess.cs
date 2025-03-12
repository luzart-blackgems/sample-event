namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Luzart;
    
    public class BoxInforMess : MonoBehaviour
    {
        private RectTransform _rtMe = null;
        public RectTransform rectTransform
        {
            get
            {
                if (_rtMe == null)
                {
                    _rtMe = GetComponent<RectTransform>();
                }
                return _rtMe;
            }
        }
        public BaseSelect baseBox;
        public ListResUI listResUI;
        public Action actionClickBox;
        public void InitMess(EStateClaim state, params DataResource[] dataRes )
        {
            switch (state)
            {
                case EStateClaim.Chest:
                    {
                        baseBox.Select(3);
                        if( listResUI != null )
                        {
                            listResUI.InitResUI(dataRes);
                        }
                        break;
                    }
                case EStateClaim.CanClaim:
                    {
                        baseBox.Select(4);
                        break;
                    }
                case EStateClaim.WillClaim:
                    {
                        baseBox.Select(1);
                        break;
                    }
                case EStateClaim.Claimed:
                    {
                        baseBox.Select(0);
                        break;
                    }
                case EStateClaim.NeedIAP:
                    {
                        baseBox.Select(2);
                        break;
                    }
    
    
            }
    
        }
        private void OnEnable()
        {
            StartWaitToUpdate();
        }
        private void OnDisable()
        {
            if (corIEWaitToUpdate != null)
            {
                StopCoroutine(corIEWaitToUpdate);
            }
        }
        private void StartWaitToUpdate()
        {
            if (corIEWaitToUpdate != null)
            {
                StopCoroutine(corIEWaitToUpdate);
            }
            corIEWaitToUpdate = StartCoroutine(IEWaitToUpdate());
        }
        private Coroutine corIEWaitToUpdate = null;
        private IEnumerator IEWaitToUpdate()
        {
            yield return new WaitForSeconds(0.2f);
            while (gameObject.activeInHierarchy)
            {
                if (gameObject.activeSelf)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        gameObject.SetActive(false);
                        actionClickBox?.Invoke();
                    }
                }
                yield return null;
            }
        }
    }
}
