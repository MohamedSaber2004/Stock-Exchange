using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Common.Helpers
{
    public static class TimeAgoHelper
    {
        public static string ToTimeAgo(DateTime dateTime, Language language)
        {
            return language == Language.ar
                ? ToArabicTimeAgo(dateTime)
                : ToEnglishTimeAgo(dateTime);
        }

        public static string ToArabicTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime.ToUniversalTime();
            if (timeSpan.TotalSeconds < 0)
                timeSpan = TimeSpan.Zero;

            if (timeSpan.TotalMinutes < 1)
                return "منذ لحظات";

            var minutes = (int)timeSpan.TotalMinutes;
            if (minutes == 1)
                return "منذ دقيقة";
            if (minutes == 2)
                return "منذ دقيقتين";
            if (minutes >= 3 && minutes <= 10)
                return $"منذ {minutes} دقائق";
            if (minutes < 60)
                return $"منذ {minutes} دقيقة";

            var hours = (int)timeSpan.TotalHours;
            if (hours == 1)
                return "منذ ساعة";
            if (hours == 2)
                return "منذ ساعتين";
            if (hours >= 3 && hours <= 10)
                return $"منذ {hours} ساعات";
            if (hours < 24)
                return $"منذ {hours} ساعة";

            var days = (int)timeSpan.TotalDays;
            if (days == 1)
                return "منذ يوم";
            if (days == 2)
                return "منذ يومين";
            if (days >= 3 && days <= 10)
                return $"منذ {days} أيام";
            if (days < 30)
                return $"منذ {days} يوماً";

            var months = days / 30;
            if (months == 1)
                return "منذ شهر";
            if (months == 2)
                return "منذ شهرين";
            if (months >= 3 && months <= 10)
                return $"منذ {months} أشهر";

            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToEnglishTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime.ToUniversalTime();
            if (timeSpan.TotalSeconds < 0)
                timeSpan = TimeSpan.Zero;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";

            var minutes = (int)timeSpan.TotalMinutes;
            if (minutes == 1)
                return "1 minute ago";
            if (minutes < 60)
                return $"{minutes} minutes ago";

            var hours = (int)timeSpan.TotalHours;
            if (hours == 1)
                return "1 hour ago";
            if (hours < 24)
                return $"{hours} hours ago";

            var days = (int)timeSpan.TotalDays;
            if (days == 1)
                return "1 day ago";
            if (days < 30)
                return $"{days} days ago";

            var months = days / 30;
            if (months == 1)
                return "1 month ago";
            if (months < 12)
                return $"{months} months ago";

            var years = days / 365;
            if (years == 1)
                return "1 year ago";

            return $"{years} years ago";
        }
    }
}
