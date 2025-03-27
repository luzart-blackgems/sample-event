namespace Luzart
{
    using System;
    
    public static class TimeUtils
    {
        public static long GetLongTimeCurrent
        {
            get
            {
                return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public static long GetLongTimeDayInMonth(int day)
        {
            DateTimeOffset dayOfMonth = GetDateTimeDayInMonth(day);
            return dayOfMonth.ToUnixTimeSeconds();
        }
        public static DateTimeOffset GetDateTimeDayInMonth(int day)
        {
            DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(GetLongTimeCurrent);
            return new DateTimeOffset(now.Year, now.Month, day, 0, 0, 0, TimeSpan.Zero);
        }
        public static DateTimeOffset GetDateTimeTimeByDay(long time, int space)
        {
            DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(time);
            DateTimeOffset dayIt = now.AddDays(space);
            return new DateTimeOffset(now.Year, now.Month, dayIt.Day, 0, 0, 0, TimeSpan.Zero);
        }
        public static long GetLongTimeByDay(long time, int space)
        {
            DateTimeOffset now = GetDateTimeTimeByDay(time, space);
            return now.ToUnixTimeSeconds();
        }
        public static long GetLongTimeFirstTimeOfCurrentMonth
        {
            get
            {
                return GetLongTimeDayInMonth(1);
            }
        }
        public static DateTimeOffset GetDateTimeLastDayOfCurrentMonth
        {
            get
            {
                // Lấy thời gian hiện tại theo UTC
                DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(GetLongTimeCurrent);
    
                // Lấy ngày đầu tiên của tháng kế tiếp
                DateTimeOffset firstDayOfNextMonth = new DateTimeOffset(now.Year, now.Month, 1, 23, 59, 59, TimeSpan.Zero).AddMonths(1);
    
                // Trừ đi một ngày để có ngày cuối cùng của tháng hiện tại
                DateTimeOffset lastDayOfMonth = firstDayOfNextMonth.AddDays(-1);
    
    
                return lastDayOfMonth;
            }
    
        }
        public static long GetLongTimeLastDayOfCurrentMonth
        {
            get
            {
                DateTimeOffset dayOfMonth = GetDateTimeLastDayOfCurrentMonth;
                return dayOfMonth.ToUnixTimeSeconds();
            }
        }
        // Lấy ngày đầu tiên của tuần hiện tại (tính từ Thứ Hai)
        public static DateTimeOffset GetDateTimeFirstDayOfCurrentWeek
        {
            get
            {
                DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(GetLongTimeCurrent);
                int diff = (int)now.DayOfWeek - 1; // 0 = Sunday, 1 = Monday, ..., 6 = Saturday
                if (diff < 0) diff = 6; // Nếu là Chủ Nhật (Sunday) thì chuyển về 6
                DateTimeOffset firstDayOfWeek = now.AddDays(-diff);
    
                // Trả về ngày đầu tiên của tuần (giờ đặt về 00:00 UTC)
                var first = new DateTimeOffset(firstDayOfWeek.Date, TimeSpan.Zero); // TimeSpan.Zero để đảm bảo múi giờ UTC
                return first;
            }
        }
        // Lấy thời gian của một ngày cụ thể trong tuần
        public static DateTimeOffset GetDateTimeDayOfCurrentWeek(DayOfWeek dayOfWeek)
        {
            int diff = (int)dayOfWeek - 1;
            if (diff < 0)
            {
                diff = 6;
            }
            DateTimeOffset firstDayOfWeek = GetDateTimeFirstDayOfCurrentWeek;
            DateTimeOffset lastDayOfWeek = firstDayOfWeek.AddDays(diff); // Thêm 6 ngày vào ngày đầu tuần
            return lastDayOfWeek;
        }
        public static long GetLongTimeDayOfCurrentWeek(DayOfWeek dOW)
        {
            return GetDateTimeDayOfCurrentWeek(dOW).ToUnixTimeSeconds();
        }
    
        // Lấy thời gian Unix của ngày đầu tiên trong tuần hiện tại
        public static long GetLongTimeFirstDayOfCurrentWeek
        {
            get
            {
                return GetDateTimeFirstDayOfCurrentWeek.ToUnixTimeSeconds();
            }
        }
    
        // 0 là thứ 2
        public static DateTimeOffset GetDateTimeDayOfCurrentWeek(int dayOfWeek)
        {
            DateTimeOffset firstDayOfWeek = GetDateTimeFirstDayOfCurrentWeek;
            DateTimeOffset lastDayOfWeek = firstDayOfWeek.AddDays(dayOfWeek); // Thêm 6 ngày vào ngày đầu tuần
            return lastDayOfWeek;
        }
        // Lấy ngày cuối cùng của tuần hiện tại (tính là Chủ Nhật)
        public static DateTimeOffset GetDateTimeLastDayOfCurrentWeek
        {
            get
            {
                DateTimeOffset lastDayOfWeek = GetDateTimeDayOfCurrentWeek(6);
                return lastDayOfWeek;
            }
        }
        // Lấy thời gian Unix của ngày cuối cùng trong tuần hiện tại
        public static long GetLongTimeLastDayOfCurrentWeek
        {
            get
            {
                return GetDateTimeLastDayOfCurrentWeek.ToUnixTimeSeconds();
            }
        }
        public static long GetLongTimeStartToday
        {
            get
            {
                return GetDateTimeStartToday.ToUnixTimeSeconds();
            }
        }
    
        public static DateTimeOffset GetDateTimeStartToday
        {
            get
            {
                return GetDateTimeStartDay(GetLongTimeCurrent);
            }
        }
        public static long GetLongTimeStartDay(long time)
        {
            return GetDateTimeStartDay(time).ToUnixTimeSeconds();
        }
        public static DateTimeOffset GetDateTimeStartDay(long time)
        {
            DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(time);
            return new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
        }
    
        public static long GetLongTimeStartTomorrow
        {
            get
            {
                return GetDateTimeStartTomorrow.ToUnixTimeSeconds();
            }
        }
        public static DateTimeOffset GetDateTimeStartTomorrow
        {
            get
            {
                DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(GetLongTimeCurrent);
                now = now.AddDays(1);
                return new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
            }
        }
        public static DateTimeOffset GetDateTimeDayOfCustomWeeks(DayOfWeek targetDay, int weeksAgo)
        {
            // Lấy ngày đầu tiên của tuần hiện tại (thứ Hai)
            DateTimeOffset firstDayOfCurrentWeek = GetDateTimeFirstDayOfCurrentWeek;
    
            // Tính chênh lệch từ ngày đầu tuần đến targetDay
            int dayOffset = (int)targetDay - (int)DayOfWeek.Monday;
            if (dayOffset < 0)
            {
                dayOffset += 7; // Đảm bảo giá trị dương nếu targetDay là Chủ Nhật
            }
    
            // Lùi hoặc tiến lại số tuần theo yêu cầu
            DateTimeOffset targetDate = firstDayOfCurrentWeek.AddDays(dayOffset + (7 * weeksAgo));
            return targetDate;
        }
        public static double GetTotalMilliseaconds()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks);
            double ms = ts.TotalMilliseconds;
            return ms;
        }

        public static double GetTotalDayMilliseaconds()
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            double hours = ts.Hours;
            double minutes = ts.Minutes + (hours * 60);
            double seconds = ts.Seconds + (minutes * 60);
            double ms = seconds * 1000;
            return ms;
        }

        public static string ConvertMilliseacondsToMinuteAndSeconds(double time)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(time);

            if (t.Hours > 0)
                return ConvertMilliseacondsToHourAndMintue(time);

            string dateTime = string.Format("{0:D1}M {1:D2}S", t.Minutes, t.Seconds);
            return dateTime;
        }

        public static string ConvertMilliseacondsToMinute(double time)
        {

            TimeSpan t = TimeSpan.FromMilliseconds(time);

            if (t.Hours > 0)
                return ConvertMilliseacondsToHour(time);

            string dateTime = string.Format("{0:D1}M", t.Minutes);
            return dateTime;
        }

        public static string ConvertMilliseacondsToHour(double time)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(time);
            string dateTime = string.Format("{0:D1}H", t.Hours);
            return dateTime;
        }

        public static string ConvertMilliseacondsToHourAndMintue(double time)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(time);
            string dateTime = string.Format("{0:D1}H {1:D2}M", t.Hours, t.Minutes);
            return dateTime;
        }

        public static int ConvertMilliseacondsToMintue(double time)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(time);
            return t.Minutes;
        }

        public static string ConvertMilliseacondsToDayAndHour(double time)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(time);
            string dateTime = string.Format("{0:D1}D {1:D2}H", t.Days, t.Hours);
            return dateTime;
        }

        public static int GetCurrentDayOfYear()
        {
            return DateTime.Now.DayOfYear;
        }
    }
}
