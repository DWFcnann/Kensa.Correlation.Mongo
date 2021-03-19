using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    class PasInfo
    {
        [BsonElement("PASiD")]
        public string PASiD { get; set; }
    
        [BsonElement("Customer")]
        public string Customer { get; set; }
    }
}
