
//#define FIREBASE_LOG
//#define FIREBASE_ENABLE
namespace Luzart
{
#if FIREBASE_ENABLE
    using BG_Library.Common;
    using BG_Library.NET;
    using Firebase.Analytics;
    using System.Text;
#endif

    public static class FirebaseWrapperLog
    {
        public static void LogEvent(string nameEvent)
        {
#if FIREBASE_ENABLE
            SetUserProperty();
            FirebaseEvent.LogEvent(nameEvent);
            LogLocal($"Log Firebase ( KEY: {nameEvent} )");
#endif
        }
        public static void LogWithLevel(string key, params ParameterFirebaseCustom[] paramCustomOthers)
        {
#if FIREBASE_ENABLE
            int level = DataWrapperGame.CurrentLevel;
            ParameterFirebaseCustom paramLevel = new ParameterFirebaseCustom(TypeFirebase.LevelID, level.ToString());

            int length = paramCustomOthers.Length;
            ParameterFirebaseCustom[] newArrayParams = new ParameterFirebaseCustom[length + 1];
            for (int i = 0; i < length; i++)
            {
                newArrayParams[i] = paramCustomOthers[i];
            }
            newArrayParams[length] = paramLevel;
            LogWithParam(key, newArrayParams);
#endif
        }
        public static void LogWithParam(string key, params ParameterFirebaseCustom[] paramCustomOthers)
        {
#if FIREBASE_ENABLE
            int length = paramCustomOthers.Length;
            Parameter[] param = new Parameter[length];
            for (int i = 0; i < length; i++)
            {
                var data = paramCustomOthers[i];
                param[i] = new Parameter(data.Type, data.param);
            }
            SetUserProperty();
            FirebaseEvent.LogEvent(key, param);
            LogLocal(key, paramCustomOthers);
#endif
        }
        private static void LogLocal(string key, params ParameterFirebaseCustom[] param)
        {
#if FIREBASE_LOG
            StringBuilder stringBuilder = new StringBuilder();
            int length = param.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append("\t( ");
                stringBuilder.Append(param[i].Type);
                stringBuilder.Append(" : ");
                stringBuilder.Append(param[i].param);
                stringBuilder.Append(" )\t");
            }
            LogLocal($"Log Firebase [KEY: {key}] on param [ {stringBuilder} ]");
#endif
        }
        private static void SetUserProperty()
        {
#if FIREBASE_ENABLE
            bool connection = InternetManager.IsInternetAvailable;
            bool isUserIAP = false; // Tu Code
            string userIAP = isUserIAP ? "iap" : "free";
            int level = 0; // Tu code

            FirebaseEvent.SetUserProperty(EventNameUserProperties.Connection, connection.ToString());
            FirebaseEvent.SetUserProperty(EventNameUserProperties.TypeUser, userIAP.ToString());
            FirebaseEvent.SetUserProperty(EventNameUserProperties.BestLevel, level.ToString());
#endif
        }
        private static void LogLocal(string debug)
        {
#if FIREBASE_LOG
            UnityEngine.Debug.Log($"<color={ConstColor.ColorBlueLight}>DEBUG_DA: {debug}</color>");
#endif
        }

        private const string ColorBlueLight = "#00FFF0";
    }
    public struct ParameterFirebaseCustom
    {
        public ParameterFirebaseCustom(string type, string param)
        {
            this.Type = type;
            this.param = param;
        }
        public string Type;
        public string param;
    }

    public static class EventNameUserProperties
    {
        public static string Connection = "connection";
        public static string TypeUser = "type";
        public static string BestLevel = "best_level";
    }

    public static class KeyFirebase
    {
        public static string LevelStart = "level_start";
        public static string LevelFailed = "level_fail";
        public static string LevelEnd = "level_end";
        public static string LevelResult = "level_result";
        public static string ResEarn = "res_earn";
        public static string ResSpend = "res_spend";
        public static string ButtonClick = "button_click";
        //
        public static string IAP_Show = "iap_Show";
        public static string BattlePassClaimed = "battle_pass_claimed";
        public static string TicketTallyClaimed = "ticket_tally_claimed";
        public static string JourneyToSuccessClaimed = "journey_to_success_claimed";
        public static string FlightEnduranceStart = "flight_endurance_start";
        public static string FlightEnduranceClaimed = "flight_endurance_claimed";
        public static string BattlePassPremiumClick = "battle_pass_premium_click";
        public static string BattlePassUnlockPremium = "battle_pass_unlock_premium";

    }
    public static class TypeFirebase
    {
        public static string LevelID = "level_id";
        public static string Amount = "amount";
        public static string Home = "home";
        public static string Type = "type";
        public static string Location = "location";
        //
        public static string LevelReward = "level_reward";
        public static string LevelDifficulty = "level_difficulty";
        public static string LevelProgress = "level_progress";
        public static string LevelTime = "level_time";
        public static string LevelPlaytime = "level_playtime";
        public static string RemainingSeat = "remaining_seats";
        public static string RemainingPeople = "remaining_people";
        public static string ExtraTimePurchased = "extra_time_purchased";
        public static string TimeBooster = "time_booster";
        public static string JumpBooster = "jump_booster";
        public static string AreaBooster = "area_booster";
        public static string AmountEarn = "amount_earn";
        public static string AmountSpend = "amount_spend";
        public static string EventType = "event_type";
        public static string TypeRes = "type";
        public static string TimeAdd = "time_add";
        public static string IDEvent = "id_event";
        public static string Duration = "duration";
        public static string Players = "players";
        public static string Rewards = "rewards";
        public static string PopUpUnder = "popup_under";
        public static string PopUp = "popup";
        public static string Button = "button_iap";
    }
    public static class ValueFirebase
    {
        public static string Normal = "normal";
        public static string Hard = "hard";
        public static string UltraHard = "ultra_hard";
        public static string FirstTime = "first_time";

        // Location
        public static string LevelFinish = "level_finish";
        public static string LevelFinishDoubleCoin = "level_doubleCoin";
        public static string BattlePassFree = "batllePass_free";
        public static string BattlePassPay = "batllePass_pay";
        public static string RefillYourGold = "refill_your_gold";
        public static string RewardAds = "reward_ads";
        public static string TicketTally = "ticket_tally";
        public static string JourneyToSuccess = "journey_to_success";

        public static string LimitTimeEvent = "limit_time_event";
        public static string DailyReward = "daily_reward";

        //
        public static string ReFillHeart = "refill_heart";
        public static string TimeOut = "time_out";
        public static string TimeBooster = "time_booster";
        public static string JumpBooster = "jump_booster";
        public static string AreaBooster = "area_booster";
        public static string TimeOutTicket = "time_out_ticket";
        public static string TimeOutAreYouSure = "time_out_are_you_sure";
        //


        public static string Btn_HomeHeart = "heart";
        public static string Btn_HomeCoin = "coin";
        public static string Btn_HomeStarterPack = "starter_pack";
        public static string Btn_HomeVIPPack = "VIP_pack";
        public static string Btn_HomeMiniPack = "mini_pack";
        public static string Btn_HomeLargePack = "large_pack";
        public static string Btn_HomeSuperPack = "super_pack";
        public static string Btn_HomeLifeAndCoinPack = "lifeandcoin_pack";
        public static string Btn_HomeValentinePack = "valentine_pack";
        public static string Btn_HomeNoMoreAdsPack = "no_more_ads_pack";
        public static string Btn_HomeTicketTally = "ticket_tally";
        public static string Btn_HomeBattlePass = "batlle_pass";
        public static string Btn_HomeJourneyToSuccess = "journey_to_success";
        public static string Btn_RiseOfKittens = "rise_of_kittens";
        public static string Btn_Cup = "cup";

        //
        public static string OpenApp = "open_app";
        public static string Home = "home";
        public static string InGame = "ingame";
        public static string Shop = "shop";
        public static string Settings = "settings";
        public static string BattlePassPopUp = "battlePass";

        //
        public static string AutoShow_NoMoreAdsPack = "no_more_ads";
        public static string AutoShow_Heart = "heart";
        public static string AutoShow_Coin = "coin";
        public static string AutoShow_BoosterPack = "booster_pack";
        public static string AutoShow_TicketTally = "ticket_tally";
        public static string AutoShow_Ratting = "ratting";
        public static string AutoShow_LevelFail = "level_fail";
        public static string AutoShow_LimitTimeEvent = "limit_time_event";
        //
        public static string Free = "free";
        public static string Pay = "pay";
        public static string Ads = "ads";
        //
        public static string PackStarter = "pack_starter";
        public static string PackMini = "pack_mini";
        public static string PackLarge = "pack_large";
        public static string PackSuper = "pack_super";
        public static string PackVIP = "pack_vip";
        public static string PackLargeAndCoin = "pack_large_and_coin";
        public static string PackStarterAds = "pack_starter_ads";
        public static string PackNoAds = "pack_no_ads";
        public static string PackBattlePass = "pack_battlePass";

        //
        public static string HeartAutoAdd = "heart_auto_add";

        //
        public static string FlightEnduranceReceive = "flight_endurance_receive";
        public static string RiseOfKittensReceive = "rise_of_kittens_receive";
        public static string CaptainTrophyReceive = "captain_trophy_receive";

        //
        public static string UIAddCoin = "ui_add_coin";
        public static string RewardHeartAds = "reward_heart_ads";

        public static string OnContinueByCoin = "on_continue_by_coin";
        public static string OnDoubleCoinWin = "on_double_coin_win";

        public static string ResDailyReward = "received_daily_reward";

        public static string ResDailyQuest = "received_daily_quest";
        public static string ResDailyQuestProcess = "received_daily_quest_process";

        public static string ResX2CoinWinGame = "x2_coin_win_game";
        public static string Tutorial = "tutorial";
    }
}
