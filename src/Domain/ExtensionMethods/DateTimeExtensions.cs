using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static bool VerificaPianificazione(this DateTime? dataInizio, DateTime? dataFine, string pianificazioneDate)
        {
            bool result = false;
            List<DateTime> listPianificazioneDate = pianificazioneDate.Split(",").Select(date => DateTime.Parse(date)).ToList();

            foreach (DateTime dataPianificata in listPianificazioneDate)
            {
                if (dataPianificata < dataInizio || dataPianificata > dataFine)
                    result = true;
            }

            return result;
        }

        public static bool VerificaPianificazione(this DateTime dataInizio, DateTime dataFine, string pianificazioneDate)
        {
            bool result = false;
            List<DateTime> listPianificazioneDate = pianificazioneDate.Split(",").Select(date => DateTime.Parse(date)).ToList();

            foreach (DateTime dataPianificata in listPianificazioneDate)
            {
                if (dataPianificata < dataInizio || dataPianificata > dataFine)
                    result = true;
            }

            return result;
        }

        public static bool VerificaNumeroGiorniSettimana(this DateTime? dataInizio, string pianificazioneDate)
        {
            bool result = false;
            List<DateTime> listPianificazioneDate = pianificazioneDate.Split(",").Select(date => DateTime.Parse(date)).ToList();
            listPianificazioneDate.Sort((x, y) => DateTime.Compare(x.Date, y.Date));

            for (int i = 0; i < listPianificazioneDate.Count - 2; i++)
            {
                var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
                var d1 = listPianificazioneDate[i].Date.AddDays(-1 * (int)cal.GetDayOfWeek(listPianificazioneDate[i]));
                var d2 = listPianificazioneDate[i + 1].Date.AddDays(-1 * (int)cal.GetDayOfWeek(listPianificazioneDate[i + 1]));
                var d3 = listPianificazioneDate[i + 2].Date.AddDays(-1 * (int)cal.GetDayOfWeek(listPianificazioneDate[i + 2]));

                if (d1 == d2 && d2 == d3)
                    result = true;
            }

            return result;
        }
    }
}
