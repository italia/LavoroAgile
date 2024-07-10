using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Extensions
{
    /// <summary>
    /// Questa classe contiene degli extension method per il TempDataDictionary.
    /// </summary>
    public static class TempDataExtensions
    {
        /// <summary>
        /// Registra le informazioni relative ad una notifica da inviare al client.
        /// </summary>
        /// <param name="tempData"></param>
        /// <param name="notificationType"></param>
        /// <param name="message"></param>
        public static void SendNotification(this ITempDataDictionary tempData, NotificationType notificationType, string message)
        {
            tempData["AppNotification"] = JsonConvert.SerializeObject(new Notification(notificationType, message));

        }

        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }

    /// <summary>
    /// Tipo di notifica da inviare.
    /// </summary>
    public enum NotificationType
    {
        Error,
        Success
    }

    public class Notification
    {
        public string Type { get; set; }
        public string Message { get; set; }

        public Notification(NotificationType notificationType, string message)
        {
            this.Type = notificationType.ToString().ToLower();
            this.Message = message;

        }

        public Notification()
        {

        }

    }
}
