﻿namespace BrotatoServer.Utilities;

public static class DateTimeExtensions
{
    public static string TimeAgo(this DateTime dateTime)
    {
        var timeSpan = DateTime.Now.Subtract(dateTime);
        
        return FormatTimespan(timeSpan);
    }

    public static string TimeAgo(this DateTimeOffset dateTimeOffset)
    {
        var timeSpan = DateTimeOffset.Now.Subtract(dateTimeOffset);

        return FormatTimespan(timeSpan);
    }

    private static string FormatTimespan(TimeSpan timeSpan)
    {

        if (timeSpan <= TimeSpan.FromSeconds(60))
        {
            var plural = timeSpan.Seconds == 1 ? "" : "s";
            return $"{timeSpan.Seconds} second{plural} ago";
        }

        if (timeSpan <= TimeSpan.FromMinutes(60))
        {
            return timeSpan.Minutes > 1 ? 
                $"about {timeSpan.Minutes} minutes ago" :
                "about a minute ago";
        }
        
        if (timeSpan <= TimeSpan.FromHours(24))
        {
            return timeSpan.Hours > 1 ? 
                $"about {timeSpan.Hours} hours ago" : 
                "about an hour ago";
        }
        
        if (timeSpan <= TimeSpan.FromDays(30))
        {
            return timeSpan.Days > 1 ? 
                $"about {timeSpan.Days} days ago" : 
                "yesterday";
        }
        
        if (timeSpan <= TimeSpan.FromDays(365))
        {
            return timeSpan.Days > 30 ? 
                $"about {timeSpan.Days / 30} months ago" : 
                "about a month ago";
        }
        
        return timeSpan.Days > 365 ? 
            $"about {timeSpan.Days / 365} years ago" : 
            "about a year ago";
    }
}