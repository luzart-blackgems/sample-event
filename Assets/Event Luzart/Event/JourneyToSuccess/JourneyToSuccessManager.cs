namespace Luzart
{
    using System;
    using System.Collections.Generic;
    
    public class JourneyToSuccessManager : BaseEventManager
    {
        private const string PATH_JTS = "journey_to_success";
        public DataJourneyToSuccess dataJourneyToSuccess;
        public DB_IAPJourney[] dbIAP; 
        public override EEventName eEvent => EEventName.JourneyToSuccess;
    
        public override TimeEvent GetTimeEvent
        {
            get
            {
                TimeEvent timeEvent = new TimeEvent();
                timeEvent.timeStart = WednesdayOrSaturday();
                timeEvent.timeEndPreview = 0;
                timeEvent.timeStartReview = FridayOrTuesday() - WednesdayOrSaturday();
                timeEvent.timeEnd = 0;
                return timeEvent;
            }
        }
    
        public long WednesdayOrSaturday()
        {
            DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(TimeUtils.GetLongTimeCurrent);
            int day = (int)now.DayOfWeek;
            if(day >= 3 && day < 6)
            {
                return TimeUtils.GetDateTimeDayOfCurrentWeek(DayOfWeek.Wednesday).ToUnixTimeSeconds();
            }
            else if(day == 0 || day == 6)
            {
                return TimeUtils.GetDateTimeDayOfCurrentWeek(DayOfWeek.Saturday).ToUnixTimeSeconds();
            }
            else
            {
                return TimeUtils.GetDateTimeDayOfCustomWeeks(DayOfWeek.Saturday, -1).ToUnixTimeSeconds();
            }
        }
        public long FridayOrTuesday()
        {
            DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(TimeUtils.GetLongTimeCurrent);
            int day = (int)now.DayOfWeek;
            if (day >= 3 && day < 6)
            {
                return TimeUtils.GetDateTimeDayOfCurrentWeek(DayOfWeek.Saturday).ToUnixTimeSeconds();
            }
            else if (day == 0 || day == 6)
            {
                return TimeUtils.GetDateTimeDayOfCustomWeeks(DayOfWeek.Wednesday,1).ToUnixTimeSeconds();
            }
            else
            {
                return TimeUtils.GetDateTimeDayOfCurrentWeek(DayOfWeek.Wednesday).ToUnixTimeSeconds();
            }
        }
        public override void LoadData()
        {
            DB_Event dbEvent = EventManager.Instance.GetEvent(EEventName.JourneyToSuccess);
            if (dbEvent == null || dbEvent.eventStatus == EEventStatus.Finish)
            {
                return;
            }
            dataJourneyToSuccess = SaveLoadUtil.LoadDataPrefs<DataJourneyToSuccess>(PATH_JTS);
            if (dataJourneyToSuccess == null)
            {
                dataJourneyToSuccess = new DataJourneyToSuccess();
                dataJourneyToSuccess.dbEvent = new DB_Event();
                if(dbIAP!=null && dbIAP.Length>0)
                {
                    dataJourneyToSuccess.isBuyIAP = new bool[dbIAP.Length];
                }
                else
                {
                    dataJourneyToSuccess.isBuyIAP = new bool[0];
                }
    
            }
            DB_Event db_EventLocal = dataJourneyToSuccess.dbEvent;
            if (string.IsNullOrEmpty(db_EventLocal.idEvent) || !db_EventLocal.idEvent.Equals(dbEvent.idEvent))
            {
                dataJourneyToSuccess = new DataJourneyToSuccess();
                dataJourneyToSuccess.dbEvent = new DB_Event();
                dataJourneyToSuccess.dbEvent = dbEvent;
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
            SaveLoadUtil.SaveDataPrefs(dataJourneyToSuccess, PATH_JTS);
        }
        public bool IsHasLevelIAP(int level)
        {
            var db = GetDBIAP(level);
            return !db.IsDefault;
        }
        public DB_IAPJourney GetDBIAP(int index)
        {
            int length = dbIAP.Length;
            for (int i = 0; i < length; i++)
            {
                var iap = dbIAP[i];
                if(index == iap.index)
                {
                    return iap;
                }
            }
            return default;
        }
        public bool IsHasClaimedFull()
        {
            return dataJourneyToSuccess.level >= dataJourneyToSuccess.dbEvent.GetRewardType(ETypeResource.None).gifts.Count;
        }
        public bool IsHasDataFreeDontReceive()
        {
            var gift = dataJourneyToSuccess.dbEvent.GetRewardType(ETypeResource.None).gifts;
            if(gift == null || gift.Count == 0)
            {
                return false;
            }
            int levelCurrent = dataJourneyToSuccess.level;
            bool isIAP = IsHasLevelIAP(levelCurrent);
            if(isIAP)
            {
                return false;
            }
            if(levelCurrent>= gift.Count)
            {
                return false;
            }
            if (gift[levelCurrent].isClaimed)
            {
                return false;
            }
            return true;
        }
        public bool IsMaxReceive()
        {
            var gift = dataJourneyToSuccess.dbEvent.GetRewardType(ETypeResource.None).gifts;
            if (gift == null || gift.Count == 0)
            {
                return true;
            }
            int levelCurrent = dataJourneyToSuccess.level;
            if (levelCurrent >= gift.Count)
            {
                return true;
            }
            return false;
        }
        public long TimeContain
        {
            get
            {
                long timeCurrent = TimeUtils.GetLongTimeCurrent;
                long timeContain = dataJourneyToSuccess.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
                return timeContain;
            }
        }
        public void ClaimItem(int index)
        {
            var gift = dataJourneyToSuccess.dbEvent.GetRewardType(ETypeResource.None).gifts;
            if (gift == null || gift.Count == 0 || index >= gift.Count)
            {
                return;
            }
            var giftClone = gift[index];
            giftClone.isClaimed = true;
            gift[index] = giftClone;
            string strType = IsHasLevelIAP(index) ? "iap" : "normal";
            //FirebaseWrapperLog.LogWithParam(KeyFirebase.JourneyToSuccessClaimed, new ParameterFirebaseCustom[]
            //{
            //    new ParameterFirebaseCustom(TypeFirebase.IDEvent, dataJourneyToSuccess.dbEvent.idEvent),
            //    new ParameterFirebaseCustom(TypeFirebase.LevelReward,index.ToString()),
            //    new ParameterFirebaseCustom(TypeFirebase.Duration,TimeContain.ToString()),
            //    new ParameterFirebaseCustom(TypeFirebase.EventType, strType),
            //});
            SaveData();
        }
    }
    
    [System.Serializable]
    public class DataJourneyToSuccess
    {
        public DB_Event dbEvent;
        public bool[] isBuyIAP;
        public int level
        {
            get
            {
                var gift = dbEvent.GetRewardType(ETypeResource.None).gifts;
                int length = gift.Count;
                for (int i = 0; i < length; i++)
                {
                    if (!gift[i].isClaimed)
                    {
                        return i;
                    }
                }
                return length;
    
            }
        }
    }
    
    [System.Serializable]
    public struct DB_IAPJourney
    {
        public int index;
        public DB_Pack dbPack;
        //public IAPProductStats stats;
        public bool IsDefault
        {
            get
            {
                return dbPack == null || string.IsNullOrEmpty(dbPack.productId);
            }
        }
    }
}
