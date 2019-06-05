﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Formatters.Json.Formatter;
using FasTnT.Model;
using FasTnT.Model.Responses;
using Newtonsoft.Json;

namespace FasTnT.Formatters.Json
{
    public class JsonResponseFormatter : BaseResponseFormatter<string>
    {
        public override async Task Write(IEpcisResponse entity, Stream output, CancellationToken cancellationToken)
        {
            if (entity == default(IEpcisResponse)) return;

            using var writer = new StreamWriter(output);
            await writer.WriteAsync(new ReadOnlyMemory<char>(Format(entity).ToArray()), cancellationToken);
        }

        protected override string FormatInternal(PollResponse response)
        {
            var dict = response.Entities.Select(x => new JsonEventFormatter().FormatEvent((EpcisEvent)x));

            return JsonConvert.SerializeObject(dict);
        }

        protected override string FormatInternal(GetStandardVersionResponse response) => JsonConvert.SerializeObject(response.Version);
        protected override string FormatInternal(GetVendorVersionResponse response) => JsonConvert.SerializeObject(response.Version);

        protected override string FormatInternal(ExceptionResponse response)
        {
            return JsonConvert.SerializeObject(new Dictionary<string, object> { 
                { "exception", response.Exception },
                { "severity", response.Severity.DisplayName },
                { "message", response.Reason }
            });
        }

        protected override string FormatInternal(GetSubscriptionIdsResult response) => JsonConvert.SerializeObject(response.SubscriptionIds);
        protected override string FormatInternal(GetQueryNamesResponse response) => JsonConvert.SerializeObject(response.QueryNames);
    }
}
