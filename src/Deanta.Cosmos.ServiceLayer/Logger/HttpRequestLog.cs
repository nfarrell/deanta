﻿
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Deanta.Cosmos.Test")]


namespace Deanta.Cosmos.ServiceLayer.Logger
{
    /// <summary>
    /// This class handles the storing/retrieval of logs for each Http request, as defined by 
    /// ASP.NET Core's TraceIdentifier. 
    /// It uses a static ConcurrentDictionary to hold the logs. 
    /// NOTE: THIS WILL NOT WORK WITH SCALE OUT, i.e. it will not work if multiple instances of the web app are running
    /// </summary>
    public class HttpRequestLog
    {
        private const int MaxKeepLogMinutes = 10;
        private const int MaxLogsInOneTrace = 500;
        private const int ExceedMaxLogsTrimTo = MaxLogsInOneTrace/2;

        private static readonly ConcurrentDictionary<string, HttpRequestLog> AllHttpRequestLogs = new ConcurrentDictionary<string, HttpRequestLog>();

        private List<LogParts> _requestLogs;

        private HttpRequestLog(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
            LastAccessed = DateTime.UtcNow;
            _requestLogs = new List<LogParts>();

            //now clear old request logs
            ClearOldLogs(MaxKeepLogMinutes);
        }

        public string TraceIdentifier { get; }

        public DateTime LastAccessed { get; private set; }

        public ImmutableList<LogParts> RequestLogs => _requestLogs.ToImmutableList();

        public override string ToString()
        {
            return $"At time: {LastAccessed:s}, Logs : {string.Join("/n", _requestLogs.Select(x => x.ToString()))}";
        }

        public static void AddLog(string traceIdentifier, LogLevel logLevel, EventId eventId, string eventString)
        {
            var thisSessionLog = AllHttpRequestLogs.GetOrAdd(traceIdentifier,
                x => new HttpRequestLog(traceIdentifier));

            //This stops a single session having too many logs
            TrimLogsIfTooLong(thisSessionLog);

            thisSessionLog._requestLogs.Add(new LogParts(logLevel, eventId, eventString));
            thisSessionLog.LastAccessed = DateTime.UtcNow;
        }

        /// <summary>
        /// This returns the HttpRequestLog for the given traceIdentifier
        /// </summary>
        /// <param name="traceIdentifier"></param>
        /// <returns>found HttpRequestLog. returns null of not found (log might be old)</returns>
        public static HttpRequestLog GetHttpRequestLog(string traceIdentifier)
        {
            if (AllHttpRequestLogs.TryGetValue(traceIdentifier, out var result)) return result;

            //No log so make up one to say what has happened.
            result = new HttpRequestLog(traceIdentifier);
            var oldest = AllHttpRequestLogs.Values.OrderBy(x => x.LastAccessed).FirstOrDefault();
            result._requestLogs.Add(new LogParts(LogLevel.Warning, new EventId(1, "EfCoreInAction"), 
                $"Could not find the log you asked for. I have {AllHttpRequestLogs.Keys.Count} logs" +
                (oldest == null ? "." : $" the oldest is {oldest.LastAccessed:s}")));

            return result;
        }

        //-----------------------------------
        //private methods

        /// <summary>
        /// Made internal so Unit Tests can get at it (not ideal, but needed)
        /// </summary>
        /// <param name="maxKeepLogMinutes"></param>
        internal static void ClearOldLogs(int maxKeepLogMinutes)
        {
            var logsToRemove =
                AllHttpRequestLogs.Values.OrderBy(x => x.LastAccessed).Where(
                    x => DateTime.UtcNow.Subtract(x.LastAccessed).TotalMinutes > maxKeepLogMinutes);

            RemoveLogs(logsToRemove);
        }
        private static void TrimLogsIfTooLong(HttpRequestLog thisSessionLog)
        {
            if (thisSessionLog._requestLogs.Count > MaxLogsInOneTrace)
            {
                thisSessionLog._requestLogs = thisSessionLog._requestLogs.Skip(thisSessionLog._requestLogs.Count - ExceedMaxLogsTrimTo).ToList();
                thisSessionLog._requestLogs.Insert(0, new LogParts(LogLevel.Warning, new EventId(1, "EfCoreInAction"),
                    $"The number of logs exceeded {MaxLogsInOneTrace}. I have removed older logs so that the display of logs won't break."));
            }
        }

        private static void RemoveLogs(IEnumerable<HttpRequestLog> logsToRemove)
        {
            foreach (var logToRemove in logsToRemove)
            {
                HttpRequestLog value;
                AllHttpRequestLogs.TryRemove(logToRemove.TraceIdentifier, out value);
            }
        }
    }
}