﻿using System;

namespace durashopcommunication
{
    public class NotificationGridEventData
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class NotificationGridEvent
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string EventType { get; set; }
        public NotificationGridEventData Data { get; set; }
        public DateTime EventTime { get; set; }
    }
}
