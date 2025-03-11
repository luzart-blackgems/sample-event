namespace Luzart
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class TicketTallyManager : BaseEventManager
    {
        public DataTicketTally dataTicketTally;
        private const string PATH_TICKETTALLY = "ticket_tally";
        public override EEventName eEvent => EEventName.TicketTally;
    
        public bool IsCacheShowVisual = false;
        public int valueKey = 0;
    
        public override TimeEvent GetTimeEvent 
        {
            get
            {
                TimeEvent timeEvent = new TimeEvent();
                timeEvent.timeStart = TimeUtils.GetLongTimeFirstDayOfCurrentWeek;
                timeEvent.timeEndPreview = 0;
                timeEvent.timeStartReview = 60*60*24*7;
                timeEvent.timeEnd = 0;
                return timeEvent;
            }
        }
    
        public override void LoadData()
        {
            if (!EventManager.Instance.IsHasEvent(eEvent))
            {
                return;
            }
            DB_Event dbEvent = EventManager.Instance.GetEvent(EEventName.TicketTally);
            dataTicketTally = SaveLoadUtil.LoadDataPrefs<DataTicketTally>(PATH_TICKETTALLY);
            if (dataTicketTally == null)
            {
                dataTicketTally = new DataTicketTally();
                dataTicketTally.dbEvent = new DB_Event();
            }
    
            DB_Event db_EventLocal = dataTicketTally.dbEvent;
            if (string.IsNullOrEmpty(db_EventLocal.idEvent) || !db_EventLocal.idEvent.Equals(dbEvent.idEvent))
            {
                dataTicketTally = new DataTicketTally();
                dataTicketTally.dbEvent = dbEvent;
                dataTicketTally.totalUse = 0;
            }
            else
            {
                db_EventLocal.idEvent = dbEvent.idEvent;
                db_EventLocal.timeEvent = dbEvent.timeEvent;
                int length = dbEvent.allTypeResources.Length;
                List<DB_GiftEvent> list = new List<DB_GiftEvent>();
                for (int i = 0; i < length; i++)
                {
                    DB_GiftEvent db = new DB_GiftEvent();
                    db.type = dbEvent.allTypeResources[i].type;
                    int lengthGift = dbEvent.allTypeResources[i].gifts.Count;
                    db.gifts = new List<GiftEvent>();
                    for (int j = 0; j < lengthGift; j++)
                    {
                        GiftEvent gift = new GiftEvent();
                        gift.groupGift = dbEvent.allTypeResources[i].gifts[j].groupGift;
                        gift.require = dbEvent.allTypeResources[i].gifts[j].require;
                        gift.isClaimed = db_EventLocal.allTypeResources[i].gifts[j].isClaimed;
                        db.gifts.Add(gift);
                    }
                    list.Add(db);
                }
                db_EventLocal.allTypeResources = list.ToArray();
    
    
            }
    
        }
        public override void SaveData()
        {
            SaveLoadUtil.SaveDataPrefs(dataTicketTally, PATH_TICKETTALLY);
        }
        public override void CompleteLevelData(int level)
        {
            base.CompleteLevelData(level);
    
        }
        public override void OnStartGame(int level)
        {
            base.OnStartGame(level);
            valueKey = 0;
        }
        public override void CompleteLevelVisual(int level)
        {
            base.CompleteLevelVisual(level);
            if (!EventManager.Instance.IsUnlockLevel(EEventName.TicketTally, level))
            {
                return;
            }
            IsCacheShowVisual = true;
            dataTicketTally.totalUse += valueKey;
            SaveData();
        }
        public bool IsHasDataDontClaim()
        {
            var data = dataTicketTally.dbEvent.GetRewardType(ETypeResource.None).gifts;
            int length = data.Count;
            int levelCurrent = dataTicketTally.LevelCurrent;
            for (int i = 0; i < length; i++)
            {
                int index = i;
                if (index < levelCurrent)
                {
                    if (!data[index].isClaimed)
                    {
                        return true;
                    }
                }
    
            }
            return false;
        }
        public void UseItem(int level)
        {
            var gifts = dataTicketTally.dbEvent.GetRewardType(ETypeResource.None);
            var giftClone = gifts.gifts[level];
            giftClone.isClaimed = true;
            gifts.gifts[level] = giftClone;
            PushFirebaseClaimed(level);
            SaveData();
        }
        private void PushFirebaseClaimed(int level)
        {
            //FirebaseWrapperLog.LogWithLevel(KeyFirebase.TicketTallyClaimed, new ParameterFirebaseCustom[]
            //{
            //    new ParameterFirebaseCustom(TypeFirebase.LevelReward,level.ToString()),
            //});
    
        }
        private void Start()
        {
            Observer.Instance?.AddObserver(ObserverKey.OnAddTicket, OnReceiveData);
        }
        private void OnDestroy()
        {
            Observer.Instance?.RemoveObserver(ObserverKey.OnAddTicket, OnReceiveData);
        }
        public void OnReceiveData(object data)
        {
            if (!EventManager.Instance.IsHasEvent(eEvent) || data == null)
            {
                return;
            }
            int add = (int)data;
            valueKey += add;

        }
    }
    [System.Serializable]
    public class DataTicketTally
    {
        public DB_Event dbEvent;
        public int totalUse;
    
        public int MaxReward()
        {
            return dbEvent.GetRewardType(ETypeResource.None).gifts.Count; 
        }
    
        public int LevelByTotalUse(int totalUse)
        {
            int total = totalUse;
            var data = dbEvent.GetRewardType(ETypeResource.None).gifts;
            int length = data.Count;
            for (int i = 0; i < length; i++)
            {
                total = total - data[i].require;
                if (total < 0)
                {
                    return i;
                }
            }
            return length;
        }
        public GiftEvent GetDBGiftEvent(int level)
        {
            var data = dbEvent.GetRewardType(ETypeResource.None).gifts;
            return data[level];
        }
        public int ContainByTotalUse(int totalUse)
        {
            int total = totalUse;
            var data = dbEvent.GetRewardType(ETypeResource.None).gifts;
            int length = data.Count;
            for (int i = 0; i < length; i++)
            {
                int totalBackup = total;
                total = total - data[i].require;
    
                if (total < 0)
                {
                    total = totalBackup;
                    break;
                }
            }
            return total;
        }
        public int RequireByTotalUse(int totalUse)
        {
            int levelCurrent = LevelByTotalUse(totalUse);
            return RequireByLevel(levelCurrent);
        }
        public int RequireByLevel(int level)
        {
            var dataAll = dbEvent.GetRewardType(ETypeResource.None).gifts;
            level = Mathf.Clamp(level, 0, dataAll.Count-1);
            return dataAll[level].require;
        }
        public int TotalUseByLevel(int level)
        {
            var data = dbEvent.GetRewardType(ETypeResource.None).gifts;
            int length = data.Count;
            int total = 0;
            for (int i = 0; i < length; i++)
            {
                total = total + data[i].require;
                if (i >= level)
                {
                    return total;
                }
            }
            return total;
        }
        public int Require
        {
            get
            {
                int _require = RequireByTotalUse(totalUse);
                return _require;
            }
        }
        public int ContainItem
        {
            get
            {
                int _containItem = ContainByTotalUse(totalUse);
                return _containItem;
            }
        }
        public int LevelCurrent
        {
            get
            {
                int _levelCurrent = LevelByTotalUse(totalUse);
                return _levelCurrent;
            }
        }
        public int MaxLevel
        {
            get
            {
                var dataAll = dbEvent.GetRewardType(ETypeResource.None).gifts;
                return dataAll.Count;
    
            }
        }
        public bool IsMaxLevel
        {
            get
            {
                return LevelCurrent == MaxLevel;
            }
        }
    }
}
