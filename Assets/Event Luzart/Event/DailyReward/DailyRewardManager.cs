namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class DailyRewardManager : BaseEventManager
    {
        private const string PATH_DAILY_REWARD = "daily_reward";
        public int Today;
        public override EEventName eEvent => EEventName.DailyReward;

        public override TimeEvent GetTimeEvent
        {
            get
            {
                long timeStartGame = TimeUtils.GetLongTimeStartDay(EventManager.Instance.TimeFirstTimeStartGame);
                long timeStartCurrent = TimeUtils.GetLongTimeStartDay(TimeUtils.GetLongTimeCurrent);
                long deltaTime = timeStartCurrent - timeStartGame;
                TimeSpan timeSpan = TimeSpan.FromSeconds(deltaTime);
                int day = timeSpan.Days;
                int dayNow = day % 7;
                long timeStartEvent = TimeUtils.GetLongTimeByDay(timeStartCurrent, -dayNow);

                TimeEvent timeEvent = new TimeEvent();
                timeEvent.timeStart = timeStartEvent;
                timeEvent.timeStartReview = 24 * 60 * 60 * 7;
                return timeEvent;

            }
        }

        private void Start()
        {
            Observer.Instance?.AddObserver(ObserverKey.OnNewDay, OnNewDay);
        }
        private void OnDestroy()
        {
            Observer.Instance?.RemoveObserver(ObserverKey.OnNewDay, OnNewDay);
        }
        public DataDailyReward dataDailyReward;
        private void OnNewDay(object data)
        {
            int lastTimeLogin = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //dataDailyReward.lastTimeLogin = lastTimeLogin;
            //dataDailyReward.count++;
            SaveData();
            Today = GetToday();
        }
        public override void LoadData()
        {
            if (!EventManager.Instance.IsHasEvent(eEvent))
            {
                return;
            }
            DB_Event dbEvent = EventManager.Instance.GetEvent(EEventName.DailyReward);
            dataDailyReward = SaveLoadUtil.LoadDataPrefs<DataDailyReward>(PATH_DAILY_REWARD);
            if (dataDailyReward == null)
            {
                dataDailyReward = new DataDailyReward();
                dataDailyReward.dbEvent = new DB_Event();
            }
            DB_Event db_EventLocal = dataDailyReward.dbEvent;
            if (string.IsNullOrEmpty(db_EventLocal.idEvent) || !db_EventLocal.idEvent.Equals(dbEvent.idEvent))
            {
                dataDailyReward = new DataDailyReward();
                dataDailyReward.dbEvent = new DB_Event();
                dataDailyReward.dbEvent = dbEvent;
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
            //if (!dataDailyReward.isFirstTimeLogin)
            //{
            //    int lastTimeLogin = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //    dataDailyReward.firstTimeLogin = lastTimeLogin;
            //    dataDailyReward.isFirstTimeLogin = true;
            //    SaveData();
            //}
            Today = GetToday();
        }

        public override void SaveData()
        {
            SaveLoadUtil.SaveDataPrefs(dataDailyReward, PATH_DAILY_REWARD);
        }
        //public int GetTimeFromFirstTimeLogin()
        //{
        //    DateTimeOffset dtNow = DateTime.UtcNow;
        //    DateTimeOffset dtFist = DateTimeOffset.FromUnixTimeSeconds((long)dataDailyReward.firstTimeLogin);
        //    return dtFist.Day - dtNow.Day;
        //}
        //public int GetTimeFromLastTimeLogin()
        //{
        //    DateTimeOffset dtNow = DateTime.UtcNow;
        //    DateTimeOffset dtFist = DateTimeOffset.FromUnixTimeSeconds((long)dataDailyReward.lastTimeLogin);
        //    return dtFist.Day - dtNow.Day;
        //}
        public bool IsClaimDay(int day)
        {
            return dataDailyReward.dbEvent.GetRewardType(ETypeResource.None).gifts[day].isClaimed;
        }
        public void ClaimReward(int day = -1)
        {
            //if(day == -1)
            //{
            //    day = GetTimeFromFirstTimeLogin();
            //}
            //if (day > dataDailyReward.isClaim.Length - 1 )
            //{
            //    GameUtil.LogError("DailyReward False");
            //}
            var gift = dataDailyReward.dbEvent.GetRewardType(ETypeResource.None).gifts[day];
            gift.isClaimed = true;
            dataDailyReward.dbEvent.GetRewardType(ETypeResource.None).gifts[day] = gift;

            SaveData();
        }

    
        public int GetToday()
        {
            long timeStartCurrent = TimeUtils.GetLongTimeStartDay(TimeUtils.GetLongTimeCurrent);
            long deltaTime = timeStartCurrent - dataDailyReward.dbEvent.timeEvent.timeStart;
            TimeSpan timeSpan = TimeSpan.FromSeconds(deltaTime);
            int day = timeSpan.Days;
            int dayNow = day % 7;
            return dayNow;
        }

        public bool IsHasDataFreeDontReceive()
        {
            Today = GetToday();
            return !IsClaimDay(Today);
        }
        //public DBProcessReward[] dataResourceProcess = new DBProcessReward[4];
        //public int MaxDay
        //{
        //    get
        //    {
        //        int length = dataResourceProcess.Length;
        //        return dataResourceProcess[length - 1].day;
        //    }
        //}
        //public bool IsClaimProcess(int index)
        //{
        //    return dataDailyReward.process.isClaim[index];
        //}
        //public void ClaimProcess(int index)
        //{
        //    dataDailyReward.process.isClaim[index] = true;
        //    SaveData();
        //}
        //public int ReturnIndexDayProcess(int day)
        //{
        //    int length = dataResourceProcess.Length;
        //    for (int i = 0; i < length; i++)
        //    {
        //        int index = i;
        //        var process = dataResourceProcess[index];
        //        if(index == length -1 && day == process.day)
        //        {
        //            return index;
        //        }
        //        if(day < process.day)
        //        {
        //            return index -1;
        //        }
        //    }
        //    return -1;
        //}
        //public float ReturnProcessPercent(int day)
        //{
        //    float percent = (float)(day+1) / (float)(MaxDay+1);
        //    return percent;
        //}

        //public bool IsFullProces()
        //{
        //    int length = dataDailyReward.process.isClaim.Length;
        //    for (int i = 0; i < length; i++)
        //    {
        //        if (!dataDailyReward.process.isClaim[i])
        //        {
        //            return false;
        //        }
        //    }
        //    for (int i = 0; i < dataDailyReward.isClaim.Length; i++)
        //    {
        //        if (!dataDailyReward.isClaim[i])
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
    }
    [System.Serializable]
    public class DataDailyReward
    {
        public DB_Event dbEvent;
        public bool IsClaimedDay(int day)
        {
            var db_giftEvent = dbEvent.GetRewardType(ETypeResource.None);
            var allDB = db_giftEvent.gifts;
            int length = allDB.Count;
            if (day < length)
            {
                return allDB[day].isClaimed;
            }
            else
            {
                return false;
            }

        }
    }
    [System.Serializable]
    public class DataProcessReward
    {
        public bool[] isClaim = new bool[4];
    }
    [System.Serializable]
    public class DBProcessReward
    {
        public int day;
        public GroupDataResources groupDataResources;
    }
}
