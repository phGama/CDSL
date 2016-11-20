using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContentDeliverySystem.WEB.Models
{
    public class InternalNotification
    {
        private static string SessionMessageKey = "INTERNALNOTIFICATION";

        public static void Subscribe(AlertMessage info)
        {
            HttpContext.Current.Session[SessionMessageKey] = info;

        }

        public static void Success(string Message)
        {
            Subscribe(new AlertMessage()
            {
                Message = Message,
                MessageType = InternalNotificationType.success
            });
        }

        public static void Error(Exception ex)
        {
            Subscribe(new AlertMessage()
            {
                Message = ex.Message,
                MessageType = InternalNotificationType.error
            });
        }

        public static void Error(string Message)
        {
            Subscribe(new AlertMessage()
            {
                Message = Message,
                MessageType = InternalNotificationType.error
            });
        }

        public static AlertMessage Retrieve()
        {
            var LastMessage = (AlertMessage)HttpContext.Current.Session[SessionMessageKey];
            HttpContext.Current.Session[SessionMessageKey] = null;
            return LastMessage;
        }

    }

    public enum InternalNotificationType
    {
        success, error, warning, info
    }

    public class AlertMessage
    {
        public string Message { get; set; }
        public InternalNotificationType MessageType { get; set; }

        public string MessageTypeText
        {
            get
            {
                switch (this.MessageType)
                {
                    case InternalNotificationType.success:
                        return "success";
                    case InternalNotificationType.error:
                        return "error";
                    case InternalNotificationType.warning:
                        return "warning";
                    case InternalNotificationType.info:
                        return "info";
                    default:
                        return "serverError";
                }
            }
        }
    }
}