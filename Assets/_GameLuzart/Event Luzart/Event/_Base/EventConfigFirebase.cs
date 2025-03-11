namespace Luzart
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    public partial class EventManager
    {
        private DB_EventJsonFirebase db_EventJsonFirebase;
        private List<DB_EventFirebase> listEventFirebaseToDo = new List<DB_EventFirebase>();
        private List<DB_Event> listEventToDo = new List<DB_Event>();
        private Dictionary<EEventName, List<DB_EventFirebase>> dictEvent = new Dictionary<EEventName, List<DB_EventFirebase>>();
        private Dictionary<EEventName, Config_EventGameFirebase> dictConfigEventGame = new Dictionary<EEventName, Config_EventGameFirebase>();
        private const string PATH_EVENTFIREBASE = "eventFirebase";
        private void ClearAllDBEvent_Firebase()
        {
            listEventFirebaseToDo.Clear();
            dictEvent.Clear();
            dictConfigEventGame.Clear();
        }
        public void SaveDataEventFirebase(DB_EventJsonFirebase db)
        {
            this.db_EventJsonFirebase = db;
            SaveLoadUtil.SaveDataPrefs<DB_EventJsonFirebase>(db, PATH_EVENTFIREBASE);
        }
        public void SaveDataEventFirebase()
        {
            SaveDataEventFirebase(db_EventJsonFirebase);
        }
    
        public void ConfigDBEvent_Firebase()
        {
            ClearAllDBEvent_Firebase();
            db_EventJsonFirebase = SaveLoadUtil.LoadDataPrefs<DB_EventJsonFirebase>(PATH_EVENTFIREBASE);
            if (!IsGetDataFirebase())
            {
                return;
            }
            InitDict_Firebase();
            ConfigDBEventFirebaseAvaiable();
            ConfigDBEvent();
        }
        private void ConfigDBEvent()
        {
            listEventToDo.Clear();
            int length = listEventFirebaseToDo.Count;
            for (int i = 0; i < length; i++)
            {
                var eventFirebase = listEventFirebaseToDo[i];
                EEventName eventName = eventFirebase.eventName;
                DB_Event dbEvent = new DB_Event();
                dbEvent.timeEvent = eventFirebase.timeEvent;
                dbEvent.allTypeResources = dictListEvent[eventName].allTypeResources;
                dbEvent.idEvent = $"{dbEvent.eventName.ToString()}_{dbEvent.timeEvent.timeStart}";
                listEventToDo.Add(dbEvent);
            }
        }
        private void ConfigDBEventFirebaseAvaiable()
        {
            long timeCurrent = TimeUtils.GetLongTimeCurrent;
            EEventName[] eEventNames = (EEventName[])Enum.GetValues(typeof(EEventName));
            int lengthEnum = eEventNames.Length;
            for (int i = 0; i < lengthEnum; i++)
            {
                EEventName eEventName = eEventNames[i];
                List<DB_EventFirebase> listEvent = new List<DB_EventFirebase>();
                if (dictEvent.ContainsKey(eEventName))
                {
                    listEvent = dictEvent[eEventName];
                }
                int lengthListEvent = listEvent.Count;
    
                if (IsPassLastEvent_Firebase(timeCurrent, listEvent))
                {
                    if (IsUseEventLocalEnd_Firebase(eEventName))
                    {
                        dictConfigEventGame[eEventName].isNeedInitEvent = true;
                    }
                }
                else
                {
                    for (int j = 0; j < lengthListEvent; j++)
                    {
                        var eachEvent = listEvent[j];
                        if (IsHasTimeEvent_Firebase(timeCurrent, eachEvent.timeEvent))
                        {
                            listEventFirebaseToDo.Add(eachEvent);
                            break;
                        }
                    }
                }
            }
        }
        private void InitDict_Firebase()
        {
            InitDictEvent_Firebase();
            InitDictConfigEventGame_Firebase();
        }
        private void InitDictEvent_Firebase()
        {
            var list = db_EventJsonFirebase.list_EventFirebase;
            int length = list.Count;
            for (int i = 0; i < length; i++)
            {
                var dbEvent = list[i];
                List<DB_EventFirebase> listDB = new List<DB_EventFirebase>();
                if (dictEvent.ContainsKey(dbEvent.eventName))
                {
                    listDB = dictEvent[dbEvent.eventName];
                }
                else
                {
                    dictEvent.Add(dbEvent.eventName, listDB);
                }
                listDB.Add(dbEvent);
            }
        }
        private bool IsGetDataFirebase()
        {
            if (db_EventJsonFirebase == null ||
                (
                (db_EventJsonFirebase.list_EventFirebase == null || db_EventJsonFirebase.list_EventFirebase.Count == 0)
                &&
                (db_EventJsonFirebase.listUnlockLevel == null || db_EventJsonFirebase.listUnlockLevel.Count == 0)
                &&
                (db_EventJsonFirebase.listConfigEvent == null || db_EventJsonFirebase.listConfigEvent.Count == 0)
                ))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool IsPassLastEvent_Firebase(long timeCurrent, List<DB_EventFirebase> _listDBEvent)
        {
            if (_listDBEvent == null ||  _listDBEvent.Count == 0)
            {
                return false;
            }
            int indexLastEvent = _listDBEvent.Count - 1;
            if(timeCurrent > _listDBEvent[indexLastEvent].timeEvent.TimeEndUnixTime)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        private void InitDictConfigEventGame_Firebase()
        {
            var list = db_EventJsonFirebase.listConfigEvent;
            int length = list.Count;
            for (int i = 0; i < length; i++)
            {
                var config = list[i];
                if (!dictConfigEventGame.ContainsKey(config.eventName))
                {
                    dictConfigEventGame.Add(config.eventName, config);
                }
            }
        }
        private bool IsNeedInitDBEventLocal_Firebase(EEventName eEventName)
        {
            if (dictConfigEventGame.ContainsKey(eEventName))
            {
                return dictConfigEventGame[eEventName].isNeedInitEvent;
            }
            else
            {
                return false;
            }
        }
        private bool IsUseEventLocalEnd_Firebase(EEventName eEventName)
        {
            if (dictConfigEventGame.ContainsKey(eEventName))
            {
                return dictConfigEventGame[eEventName].isUseEventLocalEnd;
            }
            else
            {
                return false;
            }
        }
        private bool IsHasTimeEvent_Firebase(long timeCurrent, TimeEvent timeEvent)
        { 
            return timeCurrent >= timeEvent.timeStart && timeCurrent <= timeEvent.TimeEndUnixTime;
        }
    }
    [System.Serializable]
    public class DB_EventJsonFirebase
    {
        public List<DB_EventFirebase> list_EventFirebase = new List<DB_EventFirebase>();
        public List<EventUnlockLevel> listUnlockLevel = new List<EventUnlockLevel>();
        public List<Config_EventGameFirebase> listConfigEvent = new List<Config_EventGameFirebase>();
    }
    [System.Serializable]
    public class Config_EventGameFirebase
    {
        public EEventName eventName;
        public bool isUseEventLocalEnd = false;
        [System.NonSerialized]
        public bool isNeedInitEvent = false;
    }
    [System.Serializable]
    public class DB_EventFirebase
    { 
        public EEventName eventName;
        public TimeEvent timeEvent;
    
    #if UNITY_EDITOR && ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
    #endif
        public EEventStatus eventStatus
        {
            get
            {
                long myTime = TimeUtils.GetLongTimeCurrent;
                if (myTime > timeEvent.timeStart && myTime < timeEvent.TimeStartActionUnixTime)
                {
                    return EEventStatus.Preview;
                }
                else if (myTime > timeEvent.TimeStartActionUnixTime && myTime < timeEvent.TimeEndActionUnixTime)
                {
                    return EEventStatus.Running;
                }
                else if (myTime > timeEvent.TimeEndActionUnixTime && myTime < timeEvent.TimeEndUnixTime)
                {
                    return EEventStatus.Review;
                }
                else
                {
                    return EEventStatus.Finish;
                }
            }
        }
    }
}
