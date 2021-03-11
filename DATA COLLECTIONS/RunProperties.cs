using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    
    public class RunProperties
    {
        [BsonElement("Tempuratures")]
        public Tempuratures MachineTempuratures { get; set; }

        [BsonElement("Times")]
        public RunDurations RunTimes { get; set; }
        
        [BsonElement("StartTime")]
        public DateTime StartTime { get; set; }
        //[BsonElement("AlignmentValues")]
        //how to do this?
    }


}
