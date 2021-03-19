using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    class PasResult : InspectionInfo
    {
        [BsonElement("PASinfo")]
        public PasInfo PASinfo { get; set; }

    }
}
