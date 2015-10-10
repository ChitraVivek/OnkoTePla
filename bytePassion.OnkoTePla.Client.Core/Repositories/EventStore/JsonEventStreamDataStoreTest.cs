﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using bytePassion.OnkoTePla.Client.Core.Domain;
using bytePassion.OnkoTePla.Client.Core.Repositories.SerializationDoubles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace bytePassion.OnkoTePla.Client.Core.Repositories.EventStore
{
    public class JsonEventStreamDataStoreTest : IPersistenceService<IEnumerable<EventStream<AggregateIdentifier>>>
    {
        private readonly string filename;

        public JsonEventStreamDataStoreTest(string filename)
        {
            this.filename = filename;
        }

        public void Persist(IEnumerable<EventStream<AggregateIdentifier>> data)
        {
	        var serializationData = data.Select(eventStream => new EventStreamSerializationDouble(eventStream));

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            using (var output = new StringWriter())
            {
                serializer.Serialize(output, serializationData);
                File.WriteAllText(filename, output.ToString());
            }
        }

        public IEnumerable<EventStream<AggregateIdentifier>> Load()
        {
            List<EventStreamSerializationDouble> eventStreams;
            ITraceWriter traceWriter = new MemoryTraceWriter();

            var serializer = new JsonSerializer() {TraceWriter = traceWriter};
     

            using (StreamReader file = File.OpenText(filename))
            {
                eventStreams = (List<EventStreamSerializationDouble>)serializer.Deserialize(file, typeof(List<EventStreamSerializationDouble>));		
            }

            Debug.WriteLine(traceWriter);
            return eventStreams.Select(eventStreamDouble => eventStreamDouble.GetEventStream());
        }
    }
}