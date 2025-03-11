namespace Luzart
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Observer : MonoBehaviour
    {
        public static Observer Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public delegate void CallBackObserver(object data);
    
        Dictionary<string, HashSet<CallBackObserver>> dictObserver = new Dictionary<string, HashSet<CallBackObserver>>();
        // Use this for initialization
        public void AddObserver(string topicName, CallBackObserver callbackObserver)
        {
            HashSet<CallBackObserver> listObserver = CreateListObserverForTopic(topicName);
            listObserver.Add(callbackObserver);
        }
    
        public void RemoveObserver(string topicName, CallBackObserver callbackObserver)
        {
            HashSet<CallBackObserver> listObserver = CreateListObserverForTopic(topicName);
            if (listObserver.Contains(callbackObserver))
            {
                listObserver.Remove(callbackObserver);
            }
        }
    
        public void Notify(string topicName, object Data)
        {
            HashSet<CallBackObserver> listObserver = CreateListObserverForTopic(topicName);
            HashSet<CallBackObserver> listObserverClone = new HashSet<CallBackObserver>(listObserver);
            foreach (CallBackObserver observer in listObserverClone)
            {
                observer(Data);
            }
        }
    
        public void Notify(string topicName)
        {
            Notify(topicName, null);
        }
    
        protected HashSet<CallBackObserver> CreateListObserverForTopic(string topicName)
        {
            if (!dictObserver.ContainsKey(topicName))
                dictObserver.Add(topicName, new HashSet<CallBackObserver>());
            return dictObserver[topicName];
        }
    }
    public static class ObserverKey
    {
        public static string TimeActionPerSecond = "TimeActionPerSecond";
        public static string CoinObserverNormal = "CoinObserverNormal";
        public static string CoinObserverTextRun = "CoinObserverTextRun";
        public static string CoinObserverDontAuto = "CoinObserverDontAuto";
        public static string OnNewDay = "OnNewDay";
        public static string QuestKey = "QuestKey";
        public static string OnDoneATrayTrue = "OnDoneATrayTrue";
        public static string OnCompleteStage = "OnCompleteStage";
        public static string OnTutorial = "OnTutorial";
        public static string OnChangeAmountBooster = "OnChangeAmountBooster";
        public static string RegisterBoosterRocket = "RegisterBoosterRocket";
        public static string OnAddTicket = "OnAddTicket";
    }
}
