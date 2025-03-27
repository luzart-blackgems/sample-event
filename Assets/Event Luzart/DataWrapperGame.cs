using Luzart;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataWrapperGame
{
    public static int CurrentLevel
    {
        get
        {
            int level = 1000;
            Debug.LogError($"[DataWrapperGame] Don't have level to check in ");
            return level;
        }
    }
    public static Difficulty diff
    {
        get
        {
            Debug.LogError($"[DataWrapperGame] Don't have level difficult to check ");
            return Difficulty.Normal;
        }
    }
    public static string NamePlayer
    {
        get
        {
            Debug.LogError($"[DataWrapperGame] Don't have name player to check ");
            return "Player";
        }
    }

    public static int IDAvatarPlayer
    {
        get
        {
            Debug.LogError($"[DataWrapperGame] Don't have id avatar player to check ");
            return 0;
        }
    }
    public static Sprite[] AllSpriteAvatars
    {
        get
        {
            Debug.LogError($"[DataWrapperGame] Don't have all sprite avatars to check ");
            return new Sprite[0];
        }
    }
    public static void ReceiveReward(string where = null, params DataResource[] dataResource)
    {
        Debug.LogError($"[DataWrapperGame] Don't receive Reward ");
    }
    public static void ReceiveRewardShowPopUp(string where = null, Action onDone = null ,params DataResource [] dataResource)
    {
        Debug.LogError($"[DataWrapperGame] Don't receive Reward ");
        ReceiveReward(where, dataResource);
    }
    public static void BuyReward(DataResource dataRes, Action onDone = null, string where = null)
    {
        Debug.LogError($"[DataWrapperGame] Don't buy Reward ");
    }
    public static int ResourceContinueGame
    {
        get
        {
            Debug.LogError($"[DataWrapperGame] Don't have resource continue game to check ");
            return 0;
        }
    }
}
[System.Serializable]
public enum EStateClaim
{
    CanClaimDontClaimed,
    CanClaim,
    Claimed,
    WillClaim,
    NeedIAP,
    Chest,
}
[System.Serializable]
public class DataResource
{
    public DataResource()
    {

    }
    public DataResource(DataTypeResource type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
    public DataTypeResource type;
    public int amount;
    [System.NonSerialized]
    public int idIcon = 0;
    [System.NonSerialized]
    public Sprite spIcon;

    //[JsonIgnore]
    //public int idBg = 0;
    //[JsonIgnore]
    //[JsonIgnore]
    //public Sprite spBg;
    public DataResource Clone()
    {
        return new DataResource(this.type, this.amount);
    }
}
[System.Serializable]
public struct DataTypeResource
{
    public DataTypeResource(RES_type type, int id = 0)
    {
        this.type = type;
        this.id = id;
    }
    public RES_type type;
    public int id;
    public bool Compare(DataTypeResource dataOther)
    {
        if (type == dataOther.type && id == dataOther.id)
        {
            return true;
        }
        return false;
    }
    public string GetKeyString(RES_type _type, int _id)
    {
        return $"{_type}_{_id}";
    }
    public override string ToString()
    {
        return $"{type}_{id}";
    }
    // Định nghĩa toán tử == để so sánh các instance của struct
    public static bool operator ==(DataTypeResource left, DataTypeResource right)
    {
        return left.type == right.type && left.id == right.id;
    }
    public static bool operator !=(DataTypeResource left, DataTypeResource right)
    {
        return left.type != right.type || left.id != right.id;
    }


    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
public enum RES_type
{
    None = 0,
    Gold = 1,
    Heart = 2,
    HeartTime = 3,
    Booster = 4, // 0: ReRoll, 1: Hammer, 2: Rocket, 3: MoveTray
    Chest = 5, // 0: None, start for 1, 2 , 3,4
    NoAds = 6,
}
