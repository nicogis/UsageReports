﻿// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Newtonsoft.Json;

namespace ArcGIS.Server.Rest.Classes
{
    [JsonObject]
    public class LogMessage
    {
        public LogType type { get; set; }
        public string message { get; set; }
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime time { get; set; }
        public string source { get; set; }
        public string machine { get; set; }
        public string user { get; set; }
        public int code { get; set; }
        public string elapsed { get; set; }
        public string process { get; set; }
        public string thread { get; set; }
        public string methodName { get; set; }

        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum LogType
        {
            Severe,
            Warning,
            Info,
            Fine,
            Verbose,
            Debug,
        }


    }
}
