namespace Luzart
{
    using DG.Tweening;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIReceiveRes : UIBase
    {
        public GameObject fxFirework;
        public ListResUI[] listResUI;
        public void Initialize(params DataResource[] dataResources)
        {
            int lengthDataRes = dataResources.Length;
            List<DataResource> listGold = new List<DataResource>();
            List<DataResource> listOther = new List<DataResource>();
            for (int i = 0; i < lengthDataRes; i++)
            {
                if (dataResources[i].type.type == RES_type.Gold)
                {
                    listGold.Add(dataResources[i]);
                }
                else
                {
                    listOther.Add(dataResources[i]);
                }
            }
            InitResUI(listResUI[0], listGold);
            InitResUI(listResUI[1], listOther);


            List<ResUI> list = new List<ResUI>();
            list.AddRange(listResUI[0].listResUI);
            list.AddRange(listResUI[1].listResUI);
            int length = list.Count;
            closeBtn.gameObject.SetActive(false);
            Sequence sq = DOTween.Sequence();
            sq.AppendInterval(0.05f);
            sq.AppendCallback(() => fxFirework.SetActive(true));
            for (int i = 0; i < length; i++)
            {
                int index = i;
                list[index].gameObject.SetActive(false);
                sq.AppendCallback(() => list[index].gameObject.SetActive(true));
                sq.AppendInterval(0.1f);
            }
            sq.AppendInterval(0.5f);
            sq.OnComplete(() => closeBtn.gameObject.SetActive(true));

        }
        private void InitResUI(ListResUI listRes, List<DataResource> list)
        {
            if (list.Count == 0)
            {
                listRes.gameObject.SetActive(false);
            }
            else
            {
                listRes.gameObject.SetActive(true);
                listRes.InitResUI(list.ToArray());
            }
        }
    }

}