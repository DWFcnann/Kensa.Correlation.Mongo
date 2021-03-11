using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    public class Tempuratures
    {
        [BsonElement("Bridge1")]
        public double[] Bridge1 { get; set; }

        [BsonElement("Bridge2")]
        public double[] Bridge2 { get; set; }

        [BsonElement("Granite")]
        public double[] Granite { get; set; }

        [BsonElement("Part")]
        public double[] Part { get; set; }

        [BsonElement("Laser")]
        public double[] Laser { get; set; }

        public Tempuratures(int RunCount)
        {
            Bridge1 = new double[RunCount];
            Bridge2 = new double[RunCount];
            Granite = new double[RunCount];
            Part = new double[RunCount];
            Laser = new double[RunCount];
        }
    }
}
