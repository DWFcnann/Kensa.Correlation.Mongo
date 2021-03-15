using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    /// <summary>
    /// Inspection Report Oject
    /// </summary>
    [BsonIgnoreExtraElements]
    public class InspectionInfo
    {
        public const double FAKE_DATA_FREQUENCY = 5;
        public const string UNREFERENCED = "UNREFERENCED";
        public const double dUNREFERENCED = -1.0;
        public const int nUNREFERENCED = -1;



        //Collection
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("StartTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("Machine")]
        public Machine MachineInfo { get; set; }
        
        [BsonElement("PartInfo")]
        public Part PartInfo { get; set; }

        [BsonElement("Operator")]
        public string Operator { get; set; }

        [BsonElement("PIP")]
        public PartInspectionPlan PIP { get; set; }

        [BsonElement("Measured")]
        public Characteristic[] Characteristics { get; set; }

        [BsonElement("Times")]
        public RunDurations RunDurations { get; set; }

        [BsonElement("Tempuratures")]
        public Tempuratures Tempuratures { get; set; }


        /// <summary>
        /// Default contr
        /// </summary>
        public InspectionInfo()
        {

        }

    }
}
