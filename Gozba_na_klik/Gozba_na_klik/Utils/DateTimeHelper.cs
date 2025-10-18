namespace Gozba_na_klik.Utils
{
    public static class DateTimeHelper
    {
        public static string GetDayName(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Sunday => "Nedelja",
                DayOfWeek.Monday => "Ponedeljak",
                DayOfWeek.Tuesday => "Utorak",
                DayOfWeek.Wednesday => "Sreda",
                DayOfWeek.Thursday => "Četvrtak",
                DayOfWeek.Friday => "Petak",
                DayOfWeek.Saturday => "Subota",
                _ => day.ToString()
            };
        }
    }
}