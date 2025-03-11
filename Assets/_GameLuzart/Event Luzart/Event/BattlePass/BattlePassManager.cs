namespace Luzart
{
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    
    public class BattlePassManager : BaseEventManager
    {
        private const string PATH_BATTLEPASS = "battle_pass";
        public DataBattlePass dataBattlePass;
        public DifficultyKey[] diffKey = new DifficultyKey[3];
        public bool IsCacheShowVisual = false;
        public int valueKey = 0;
        private Dictionary<Difficulty, int> _dictDiffKey = null;
        public Dictionary<Difficulty, int> dictDiffKey
        {
            get
            {
                if(_dictDiffKey == null)
                {
                    _dictDiffKey = new Dictionary<Difficulty, int>();
                    int length = diffKey.Length;
                    for (int i = 0; i < length; i++)
                    {
                        var item = diffKey[i];
                        _dictDiffKey.Add(item.difficulty, item.amountKey);
                    }
                }
                return _dictDiffKey;
            }
        }
    
        public override EEventName eEvent => EEventName.BattlePass;
        public override TimeEvent GetTimeEvent
        {
            get
            {
                TimeEvent timeEvent = new TimeEvent();
                timeEvent.timeStart = TimeUtils.GetLongTimeFirstTimeOfCurrentMonth;
                timeEvent.timeEndPreview = 0;
                timeEvent.timeStartReview = TimeUtils.GetLongTimeLastDayOfCurrentMonth - TimeUtils.GetLongTimeFirstTimeOfCurrentMonth;
                timeEvent.timeEnd = 0;
                return timeEvent;
            }
    
        }
    
        public override void LoadData()
        {
            if(!EventManager.Instance.IsHasEvent(eEvent))
            {
                return;
            }
            DB_Event dbEvent = EventManager.Instance.GetEvent(EEventName.BattlePass);
            dataBattlePass = SaveLoadUtil.LoadDataPrefs<DataBattlePass>(PATH_BATTLEPASS);
            if (dataBattlePass == null)
            {
                dataBattlePass = new DataBattlePass();
                dataBattlePass.dbEvent = new DB_Event();
            }
            DB_Event db_EventLocal = dataBattlePass.dbEvent;
            if (string.IsNullOrEmpty(db_EventLocal.idEvent) || !db_EventLocal.idEvent.Equals(dbEvent.idEvent))
            {
                dataBattlePass = new DataBattlePass();
                dataBattlePass.dbEvent = new DB_Event();
                dataBattlePass.dbEvent = dbEvent;
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
            SaveLoadUtil.SaveDataPrefs(dataBattlePass, PATH_BATTLEPASS);
        }
        public override void CompleteLevelData(int level)
        {
            if (!EventManager.Instance.IsHasEvent(EEventName.BattlePass) || !EventManager.Instance.IsUnlockLevel(EEventName.BattlePass, DataWrapperGame.CurrentLevel - 1))
            {
                return;
            }
            Difficulty diff = DataWrapperGame.diff;
            int key = dictDiffKey[diff];
            dataBattlePass.totalUse += key;
            valueKey = key;
            IsCacheShowVisual = true;
            SaveData();
        }
        public override void CompleteLevelVisual(int level)
        {
            base.CompleteLevelVisual(level);    
        }
        public override void OnLoseLevelData(int level)
        {
            base.OnLoseLevelData(level);
            IsCacheShowVisual = false;
        }
        public void UseItemNormal(int level)
        {
            var gifts = dataBattlePass.dbEvent.GetRewardType(ETypeResource.None);
            var giftClone = gifts.gifts[level] ;
            giftClone.isClaimed = true;
            gifts.gifts[level] = giftClone;
            PushFirebaseClaimed(level, ETypeResource.None);
            SaveData() ;
        }
        public void UseItemVIP(int level)
        {
            var gifts = dataBattlePass.dbEvent.GetRewardType(ETypeResource.VIP);
            var giftClone = gifts.gifts[level];
            giftClone.isClaimed = true;
            gifts.gifts[level] = giftClone;
            PushFirebaseClaimed(level,ETypeResource.VIP);
            SaveData();
    
        }
        private void PushFirebaseClaimed(int level, ETypeResource type)
        {
            //string strType = "free";
            //if(type == ETypeResource.VIP)
            //{
            //    strType = "pay";
            //}
            //FirebaseWrapperLog.LogWithLevel(KeyFirebase.BattlePassClaimed, new ParameterFirebaseCustom[]
            //{
            //    new ParameterFirebaseCustom(TypeFirebase.LevelReward,level.ToString()),
            //    new ParameterFirebaseCustom(TypeFirebase.EventType, strType),
            //});
    
        }
        public bool IsHasDataDontClaim()
        {
            bool isNormal = IsHasDataDontClaim(ETypeResource.None);
            bool isVIP = false;
            if (dataBattlePass.isBuyIAP)
            {
                isVIP = IsHasDataDontClaim(ETypeResource.VIP);
            }
            return (isNormal || isVIP);
        }
        private bool IsHasDataDontClaim(ETypeResource eType)
        {
            return CountDataDontClaim(eType) > 0;
        }
        public int CountDataDontClaim()
        {
            return CountDataDontClaim(ETypeResource.None, dataBattlePass.totalUse) ;
        }
        public int CounDataDontClaim(int total)
        {
            int value = CountDataDontClaim(ETypeResource.None, total);
            int valueVIP = 0;
            if (dataBattlePass.isBuyIAP)
            {
                valueVIP = CountDataDontClaim(ETypeResource.VIP, total);
            }
    
            return value + valueVIP;
        }
        public int CountDataDontClaim(ETypeResource eType)
        {
            return CountDataDontClaim(eType, dataBattlePass.totalUse);
        }
        public int CountDataDontClaim(ETypeResource eType, int total)
        {
            int value = 0;
            var data = dataBattlePass.dbEvent.GetRewardType(eType).gifts;
            int length = data.Count;
            int levelCurrent = dataBattlePass.LevelByTotalUse(total);
            for (int i = 0; i < length; i++)
            {
                int index = i;
                if (index < levelCurrent)
                {
                    if (!data[index].isClaimed)
                    {
                        value++;
                    }
                }
    
            }
            return value;
        }
    
        public long TimeContain
        {
            get
            {
                long timeCurrent = TimeUtils.GetLongTimeCurrent;
                long timeContain = dataBattlePass.dbEvent.timeEvent.TimeEndUnixTime - timeCurrent;
                return timeContain;
            }
        }
    
        [System.Serializable]
        public struct DifficultyKey
        {
            public Difficulty difficulty;
            public int amountKey;
        }
    }
    [System.Serializable]
    public class DataBattlePass
    {
        public DB_Event dbEvent;
        public bool isBuyIAP;
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
    public enum Difficulty
    {
        Normal = 0,
        Hard =1,
        SuperHard =2,
    }
}
