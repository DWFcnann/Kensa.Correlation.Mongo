using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    public class RunDurations
    {
        [BsonElement("Start")]
        public DateTime[] RunStart { get; set; }


        [BsonElement("Finish")]
        public DateTime[] RunFinish { get; set; }


        public RunDurations(int RunCount)
        {
            RunStart = new DateTime[RunCount];
            RunFinish = new DateTime[RunCount];
        }
    }
}
