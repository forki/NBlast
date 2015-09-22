﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using LanguageExt;
using Lucene.Net.Linq;
using NBlast.Rest.Index;
using NBlast.Rest.Model.Write;
using static LanguageExt.Prelude;

namespace NBlast.Rest.Services.Write
{
    public class LogEntryIndexationService: ILogEntryIndexationService
    {
        private readonly ILuceneDataProvider _luceneDataProvider;
        private readonly ILogEntryMapperProvider _logEntryMapperProvider;

        public LogEntryIndexationService(ILuceneDataProvider luceneDataProvider, ILogEntryMapperProvider logEntryMapperProvider)
        {
            if (luceneDataProvider == null) throw new ArgumentNullException(nameof(luceneDataProvider));
            if (logEntryMapperProvider == null) throw new ArgumentNullException(nameof(logEntryMapperProvider));
            _luceneDataProvider = luceneDataProvider;
            _logEntryMapperProvider = logEntryMapperProvider;
        }

        public Unit IndexOne(LogEntry entry)
        {
            return Index(session => session.Add(entry));
        }

        public Unit IndexMany(IReadOnlyList<LogEntry> entries)
        {
            return Index(session => entries.ToList().ForEach(entry => session.Add(entry)));
        }

        private Unit Index (Action<ISession<LogEntry>> closure)
        {
            using (var session = _luceneDataProvider.OpenSession(_logEntryMapperProvider.Provide()))
            {
                closure(session);
                return unit;
            }
        }
    }

}