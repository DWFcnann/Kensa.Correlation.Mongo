using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    public class Machine
    {
        [BsonElement("Number")]
        public int Number { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Brand")]
        public string Brand { get; set; }

    }
}
