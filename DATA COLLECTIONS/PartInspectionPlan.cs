using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    public class PartInspectionPlan
    {
        [BsonElement("ID")]
        public string ID { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }
    }
}
