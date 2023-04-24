namespace PersonalAPI.Models
{
    public class WholeHourModel
    {
        public static DateTime RoundUpToNextWholeHour(DateTime dateTime)
        {
            return dateTime.AddHours(1).Date.AddHours(dateTime.Hour);
        }
    }
}
