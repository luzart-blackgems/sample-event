namespace Luzart
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class RiseOfKittensManager : BaseEventManager
    {
        public override EEventName eEvent => EEventName.RiseOfKittens;
    
        public override TimeEvent GetTimeEvent 
        {
            get
            {
                TimeEvent timeEvent = new TimeEvent();
                var datetimeStartDayGame = TimeUtils.GetDateTimeStartDay(EventManager.Instance.TimeFirstTimeStartGame);
                var dayOfWeek = datetimeStartDayGame.DayOfWeek;
                long longTimeDayOfWeekCurrent = TimeUtils.GetLongTimeDayOfCurrentWeek(dayOfWeek);
    
                timeEvent.timeStart = longTimeDayOfWeekCurrent;
                timeEvent.timeEndPreview = 0;
                timeEvent.timeStartReview = 3*24*60*60;
                timeEvent.timeEnd = 0;
                return timeEvent;
            }
        }
        public DataRiseOfKittens dataRiseOfKittens;
        private const string PATH_RISE_OF_KITTENS = "rise_of_kittens";
        public override void LoadData()
        {
            if (!EventManager.Instance.IsHasEvent(EEventName.RiseOfKittens))
            {
                return;
            }
            DB_Event dbEvent = EventManager.Instance.GetEvent(EEventName.RiseOfKittens);
            dataRiseOfKittens = SaveLoadUtil.LoadDataPrefs<DataRiseOfKittens>(PATH_RISE_OF_KITTENS);
            if (dataRiseOfKittens == null)
            {
                dataRiseOfKittens = new DataRiseOfKittens();
                dataRiseOfKittens.dbEvent = new DB_Event();
            }
            DB_Event db_EventLocal = dataRiseOfKittens.dbEvent;
            // Kiểm tra nếu sự kiện mới hoặc cấp độ tối đa đã đạt
            bool isNewEvent = string.IsNullOrEmpty(db_EventLocal.idEvent) || !db_EventLocal.idEvent.Equals(dbEvent.idEvent);
            if (isNewEvent || dataRiseOfKittens.IsMaxLevel)
            {
                if (isNewEvent || dataRiseOfKittens.IsMaxLevel)
                {
                    dataRiseOfKittens = new DataRiseOfKittens { dbEvent = dbEvent };
                    return;
                }
            }
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
        
        public override void SaveData()
        {
            SaveLoadUtil.SaveDataPrefs(dataRiseOfKittens, PATH_RISE_OF_KITTENS);
        }
    
        public bool IsHasDataDontClaim()
        {
            var data = dataRiseOfKittens.dbEvent.GetRewardType(ETypeResource.None).gifts;
            int length = data.Count;
            int levelCurrent = dataRiseOfKittens.LevelCurrent;
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
        public bool IsMaxReceive()
        {
            var gift = dataRiseOfKittens.dbEvent.GetRewardType(ETypeResource.None).gifts;
            if (gift == null || gift.Count == 0)
            {
                return true;
            }
            int levelCurrent = dataRiseOfKittens.LevelCurrent;
            if (levelCurrent >= gift.Count)
            {
                return true;
            }
            return false;
        }
        public void UseItem(int level)
        {
            var gifts = dataRiseOfKittens.dbEvent.GetRewardType(ETypeResource.None);
            var giftClone = gifts.gifts[level];
            giftClone.isClaimed = true;
            gifts.gifts[level] = giftClone;
            SaveData();
        }
        public override void CompleteLevelData(int level)
        {
            base.CompleteLevelData(level);
            if (!EventManager.Instance.IsHasEvent(EEventName.RiseOfKittens) || !EventManager.Instance.IsUnlockLevel(EEventName.RiseOfKittens))
            {
                return;
            }
            dataRiseOfKittens.totalUse++;
            SaveData();
        }
        public override void OnLoseLevelData(int level)
        {
            base.OnLoseLevelData(level);
            if(!EventManager.Instance.IsHasEvent(EEventName.RiseOfKittens) || !EventManager.Instance.IsUnlockLevel(EEventName.RiseOfKittens))
            {
                return;
            }
            if (dataRiseOfKittens.ContainItem <= 0)
            {
                return;
            }
            int contain = dataRiseOfKittens.ContainItem;
            dataRiseOfKittens.totalUse -= contain;
            SaveData();
        }
    }
    [System.Serializable]
    public class DataRiseOfKittens
    {
        public DB_Event dbEvent;
        public int totalUse;
    
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
            level = Mathf.Clamp(level, 0, dataAll.Count - 1);
            return dataAll[level].require;
        }
        public int TotalRequireByLevel(int level)
        {
            int total = 0;
            for (int i = 0; i <= level; i++)
            {
                total += RequireByLevel(level);
            }
            return total;   
        }
    
        public int Require
        {
            get
            {
                return RequireByTotalUse(totalUse);
            }
        }
    
        public int ContainItem
        {
            get
            {
                return ContainByTotalUse(totalUse);
            }
        }
        public int LevelCurrent
        {
            get
            {
                return LevelByTotalUse(totalUse);
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
