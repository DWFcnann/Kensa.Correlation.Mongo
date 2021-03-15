using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace GenericClass
{
    public class Part
    {

        [BsonElement("SerialNumber")]
        public string SerialNumber { get; set; }

        [BsonElement("PartNumber")]
        public string PartNumber { get; set; }

        [BsonElement("DrawingNumber")]
        public string DrawingNumber { get; set; }
    }
}
