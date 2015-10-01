﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentScheduler;
using LanguageExt;
using NBlast.Rest.Model.Converters;
using NBlast.Rest.Model.Dto;
using NBlast.Rest.Model.Write;
using NBlast.Rest.Services.Write;
using Serilog;
using static LanguageExt.Prelude;

namespace NBlast.Rest.Async
{
    public interface IQueueProcessingTask : ITask
    {
    }

    public class QueueProcessingTask: IQueueProcessingTask
    {
        private readonly IIndexingQueueKeeper _indexingQueueKeeper;
        private readonly ILogEntryIndexationService _logEntryIndexationService;
        private readonly ILogModelEntryConverter _logModelEntryConverter;
        private static readonly ILogger Logger = Log.Logger.ForContext<QueueProcessingTask>();

        public QueueProcessingTask(IIndexingQueueKeeper indexingQueueKeeper,
                                   ILogEntryIndexationService logEntryIndexationService,
                                   ILogModelEntryConverter logModelEntryConverter)
        {
            if (indexingQueueKeeper == null) throw new ArgumentNullException(nameof(indexingQueueKeeper));
            if (logEntryIndexationService == null) throw new ArgumentNullException(nameof(logEntryIndexationService));
            if (logModelEntryConverter == null) throw new ArgumentNullException(nameof(logModelEntryConverter));

            _indexingQueueKeeper = indexingQueueKeeper;
            _logEntryIndexationService = logEntryIndexationService;
            _logModelEntryConverter = logModelEntryConverter;
        }

        private Unit ProcessModels(IReadOnlyList<LogModel> models)
        {
            var sw = new Stopwatch();
            sw.Start();

            var logEntries = models.Select(Transform).ToList();
            _logEntryIndexationService.IndexMany(logEntries);

            sw.Stop();
            Logger.Debug($"Import process has took ${sw.ElapsedMilliseconds} msec(s)");
            return unit;
        }

        private LogEntry Transform(LogModel model) => _logModelEntryConverter.Convert(model);

        public void Execute()
        {
            var count = _indexingQueueKeeper.Count();
            if(count < 1) return;
            Logger.Verbose("Scheduled task executed, queue contains {0} element(s)", count);
            ProcessModels(_indexingQueueKeeper.ConsumeTop(400));
        }
    }
}