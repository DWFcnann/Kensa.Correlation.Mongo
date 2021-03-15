using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    /// <summary>
    /// Class for a customer
    /// </summary>
    class InspectionResult : InspectionInfo
    {
        [BsonElement("OperationNumber")]
        public string OperationNumber { get; set; }

        [BsonElement("SetupNumber")]
        public int SetupNumber { get; set; }
    }
}
