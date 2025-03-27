namespace Luzart
{
    using System.Collections.Generic;
    using UnityEngine;

    public partial class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }
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
            Initialize();
        }
        private void Start()
        {
            StartFuncionInGame();
        }
        //private void OnDestroy()
        //{
        //    DestroyFuncionInGame();
        //}
        [Space]
        [Header("Event Manager")]
        public BaseEventManager[] baseEventManagers;

        private const string PATH_EVENT = "data_event";
        public bool IsInit { get; set; } = false;
        public List<DB_Event> list_Event = new List<DB_Event>();

        //
        private Dictionary<string, DB_Event> dictDBEvent = new Dictionary<string, DB_Event>();

        private Dictionary<EEventName, DB_Event> dictListEvent = new Dictionary<EEventName, DB_Event>();

        private Dictionary<EEventName, DB_Event> dictDBEEvent = new Dictionary<EEventName, DB_Event>();

        private Dictionary<EEventName, BaseEventManager> dictBaseEventManager = new Dictionary<EEventName, BaseEventManager>();
        //

        public List<EventUnlockLevel> list_EventUnlockLevel = new List<EventUnlockLevel>();
        private Dictionary<EEventName, int> dictEventUnlock = new Dictionary<EEventName, int>();
        //


        [Space]
        [Header("Only Read")]
        public List<DB_Event> events = new List<DB_Event>();

        public DataEvent dataEvent
        {
            get => dbEventJson.dataEvent;
            set => dbEventJson.dataEvent = value;
        }
        public long TimeFirstTimeStartGame
        {
            get
            {
                return dbEventJson.dataEvent.timeFirstTime;
            }
        }
        private DB_EventJSon dbEventJson;
        //

        public void Initialize()
        {
            LoadEvent();
            InitDictEvent();
            ConfigDBEvent_Firebase();
            //

            ConfigDB_Event();
            InitAllEventLocal();

        }
        private void InitAllEventLocal()
        {
            int length = baseEventManagers.Length;
            for (int i = 0; i < length; i++)
            {
                baseEventManagers[i].LoadData();
            }
        }
        public void SaveDataEvent()
        {
            int length = baseEventManagers.Length;
            for (int i = 0; i < length; i++)
            {
                baseEventManagers[i].SaveData();
            }
        }
        private void InitDictEvent()
        {

            int lengthListEvent = list_Event.Count;
            dictListEvent.Clear();
            for (int i = 0; i < lengthListEvent; i++)
            {
                dictListEvent.Add(list_Event[i].eventName, list_Event[i]);
            }


            int length = events.Count;
            //dictDBEvent.Clear();
            //for (int i = 0; i < length; i++)
            //{
            //    DB_Event e = events[i];
            //    dictDBEvent.Add(e.idEvent, e);
            //}

            dictDBEEvent.Clear();
            for (int i = 0; i < length; i++)
            {
                DB_Event e = events[i];
                dictDBEEvent.Add(e.eventName, e);
            }


            dictBaseEventManager.Clear();
            int lengthBaseEvent = baseEventManagers.Length;
            for (int i = 0; i < lengthBaseEvent; i++)
            {
                var baseEvent = baseEventManagers[i];
                dictBaseEventManager.Add(baseEvent.eEvent, baseEvent);
            }

            dictEventUnlock.Clear();
            int lengthEventUnlock = list_EventUnlockLevel.Count;
            for (int i = 0; i < lengthEventUnlock; i++)
            {
                var eventUnlock = list_EventUnlockLevel[i];
                dictEventUnlock.Add(eventUnlock.eventName, eventUnlock.level);
            }
        }
        private void OnApplicationPause()
        {
            SaveEvent();
        }
        public void LoadEvent()
        {
            dbEventJson = SaveLoadUtil.LoadDataPrefs<DB_EventJSon>(PATH_EVENT);
            if (dbEventJson == null)
            {
                dbEventJson = new DB_EventJSon();
                dbEventJson.dataEvent = new DataEvent();
                events = new List<DB_Event>();
            }
            else
            {
                events = dbEventJson.events;
            }
            if (dbEventJson.dataEvent.timeFirstTime == -1)
            {
                dbEventJson.dataEvent.timeFirstTime = TimeUtils.GetLongTimeCurrent;
            }
            IsInit = true;
        }
        private void ConfigDB_Event()
        {
            if (events != null)
            {
                events.RemoveAll((db) => db.eventStatus == EEventStatus.Finish);
                int length = events.Count;
                for (int i = 0; i < length; i++)
                {
                    var db_Event = events[i];
                    if (db_Event == null)
                    {
                        continue;
                    }
                    if (db_Event.allTypeResources == null)
                    {
                        db_Event.allTypeResources = dictListEvent[db_Event.eventName].allTypeResources;
                    }
                }
            }
            if (IsGetDataFirebase())
            {
                events = listEventToDo;
                if (db_EventJsonFirebase.listUnlockLevel.Count == list_EventUnlockLevel.Count)
                {
                    list_EventUnlockLevel = db_EventJsonFirebase.listUnlockLevel;
                }
            }
            ConfigEventLocal();
            InitDictEvent();

        }
        private void ConfigEventLocal()
        {
            int lengthEvent = list_Event.Count;
            for (int i = 0; i < lengthEvent; i++)
            {
                if (IsGetDataFirebase() && !IsNeedInitDBEventLocal_Firebase(list_Event[i].eventName))
                {
                    continue;
                }
                var db_Event = list_Event[i];
                db_Event.timeEvent = dictBaseEventManager[db_Event.eventName].GetTimeEvent;
                if (db_Event.eventStatus != EEventStatus.Finish)
                {
                    bool isHasEvent = false;
                    int length = events.Count;
                    for (int j = 0; j < length; j++)
                    {
                        if (events[j].idEvent.Equals($"{db_Event.eventName.ToString()}_{db_Event.timeEvent.timeStart}") || events[j].idEvent.Contains(db_Event.eventName.ToString()))
                        {
                            isHasEvent = true;
                            db_Event.idEvent = $"{db_Event.eventName.ToString()}_{db_Event.timeEvent.timeStart}";
                            events[j] = db_Event;
                        }
                    }
                    if (!isHasEvent)
                    {
                        db_Event.idEvent = $"{db_Event.eventName.ToString()}_{db_Event.timeEvent.timeStart}";
                        events.Add(db_Event);
                    }
                }

            }
            SaveEvent();
        }
        public void SaveEvent()
        {
            if (!IsInit)
            {
                return;
            }
            dbEventJson.events = events;
            SaveLoadUtil.SaveDataPrefs<DB_EventJSon>(dbEventJson, PATH_EVENT);
            SaveDataEventFirebase();

        }

        public DB_Event GetEvent(string idEvent)
        {
            if (dictDBEvent.ContainsKey(idEvent))
            {
                return dictDBEvent[idEvent];
            }
            return null;
        }
        public DB_Event GetEvent(EEventName eventName)
        {
            if (dictDBEEvent.ContainsKey(eventName))
            {
                return dictDBEEvent[eventName];
            }
            return null;
        }
        public bool IsHasEvent(EEventName eventName)
        {
            DB_Event db = GetEvent(eventName);
            if (db == null || db.eventStatus == EEventStatus.Finish)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public bool IsUnlockLevel(EEventName eEventName)
        {
            return IsUnlockLevel(eEventName, DataWrapperGame.CurrentLevel);
        }

        public bool IsUnlockLevel(EEventName eEventName, int level)
        {
            if (dictEventUnlock.ContainsKey(eEventName))
            {
                return level >= dictEventUnlock[eEventName];
            }
            else
            {
                return false;
            }

        }
        //public bool IsLevelUnlockExactly(EEventName eEventName)
        //{
        //    return IsLevelUnlockExactly(eEventName, DataManager.Instance.CurrentLevel);
        //}
        public bool IsLevelUnlockExactly(EEventName eEventName, int level)
        {
            if (dictEventUnlock.ContainsKey(eEventName))
            {
                return level == dictEventUnlock[eEventName];
            }
            else
            {
                return false;
            }
        }


        public List<EEventName> GetEEventCurrent()
        {
            List<EEventName> list = new List<EEventName>();
            int length = events.Count;
            for (int i = 0; i < length; i++)
            {
                var baseEvent = events[i];
                if (IsHasEvent(baseEvent.eventName) && IsUnlockLevel(baseEvent.eventName))
                {
                    list.Add(baseEvent.eventName);
                }
            }
            return list;
        }
#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button]
#endif
        public void SaveAllData()
        {
            SaveDataEvent();
            SaveEvent();
        }











    }

    [System.Serializable]
    public class DB_Event
    {
        public string idEvent;
        public EEventName eventName;
        public TimeEvent timeEvent;
        public DB_GiftEvent[] allTypeResources;

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
        // 

        [System.NonSerialized]
        private Dictionary<ETypeResource, DB_GiftEvent> dictReward = new Dictionary<ETypeResource, DB_GiftEvent>();
        private void InitDictionaryTypeResource()
        {
            if (dictReward == null || dictReward.Count <= 0)
            {
                dictReward.Clear();
                int length = allTypeResources.Length;
                for (int i = 0; i < length; i++)
                {
                    dictReward.Add(allTypeResources[i].type, allTypeResources[i]);
                }
            }
        }
        public DB_GiftEvent GetRewardType(ETypeResource type)
        {
            InitDictionaryTypeResource();
            if (dictReward.ContainsKey(type))
            {
                return dictReward[type];
            }
            else
            {
                return default;
            }
        }
    }
    [System.Serializable]
    public struct TimeEvent
    {
        // Time timeStart --- timeStart + timeEndPreview --- timeStart + timeEnd --- timeStartReview + timeReview
        //AWAKE = PREVIEW  ============   START   ==========    END   ==========           REVIEW = DESTROY

        // Thoi gian event start, lay time Unix
        public long timeStart;
        // Thoi gian tinh tu timeStart
        public long timeEndPreview;
        // Thoi gian ket thuc game, tinh tu timeStart
        public long timeStartReview;
        // Thoi gian sau khi ket thuc game, de user doi thuong, tinh tu timeStart
        public long timeEnd;
        public long TimeStartActionUnixTime
        {
            get
            {
                return timeStart + timeEndPreview;
            }
        }
        public long TimeEndActionUnixTime
        {
            get
            {
                return TimeStartActionUnixTime + timeStartReview;
            }
        }
        public long TimeEndUnixTime
        {
            get
            {
                return TimeEndActionUnixTime + timeEnd;
            }
        }
    }
    [System.Serializable]
    public enum EEventStatus
    {
        Preview = 0,
        Running = 1,
        Review = 2,
        Finish = 3,
    }

    [System.Serializable]
    public enum EEventName
    {
        None = 0,
        BattlePass = 1,
        TicketTally = 2,
        JourneyToSuccess = 3,
        FlightEndurance = 4,
        RiseOfKittens = 5,
        Cup = 6,
        DailyReward = 7,
    }
    [System.Serializable]
    public enum ETypeResource
    {
        None = 0,
        VIP = 1,
    }
    [System.Serializable]
    public struct DB_GiftEvent
    {
        public ETypeResource type;
        public List<GiftEvent> gifts;
    }
    [System.Serializable]
    public struct GiftEvent
    {
        public int require;
        public GroupDataResources groupGift;
        public bool isClaimed;

        public bool IsHasChest
        {
            get
            {
                return typeChest.id != 0;
            }
        }
        public DataTypeResource typeChest
        {
            get
            {
                return groupGift.typeChest;
            }
        }

    }
    [System.Serializable]
    public struct GroupDataResources
    {
        public List<DataResource> dataResources;
        public DataTypeResource typeChest;
        public bool IsHasChest
        {
            get
            {
                return typeChest.type == RES_type.Chest && typeChest.id != 0;
            }
        }
    }

    [System.Serializable]
    public struct EventUnlockLevel
    {
        public EEventName eventName;
        public int level;
    }

    [System.Serializable]
    public class DB_EventJSon
    {
        public DataEvent dataEvent = new DataEvent();
        public List<DB_Event> events = new List<DB_Event>();
    }

    [System.Serializable]
    public class DataEvent
    {
        public long timeFirstTime = -1;
        public bool isCompleteTutorialTicket = false;
    }
}
