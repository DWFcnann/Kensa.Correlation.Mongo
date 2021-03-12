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
    [BsonIgnoreExtraElements]
    public class TestResult
    {
        public const double FAKE_DATA_FREQUENCY = 5;
        public const string UNREFERENCED = "UNREFERENCED";
        public const double dUNREFERENCED = -1.0;
        public const int nUNREFERENCED = -1;



        //Collection
        //[BsonId]
        //public ObjectId Id { get; set; }

        [BsonElement("TestNumber")]
        public int TestNumber { get; set; }

        [BsonElement("StartTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("Machine")]
        public Machine MachineInfo { get; set; }
        
        [BsonElement("SerialNumber")]
        public string SerialNumber { get; set; }

        [BsonElement("Operator")]
        public string Operator { get; set; }

        [BsonElement("PIP")]
        public PartInspectionPlan PIP { get; set; }

        [BsonElement("TestType")]
        public string TestType { get; set; }

        [BsonElement("Measured")]
        public Characteristic[] Characteristics { get; set; }

        [BsonElement("Times")]
        public RunDurations RunDurations { get; set; }

        [BsonElement("Tempuratures")]
        public Tempuratures Tempuratures { get; set; }


        /// <summary>
        /// Default contr
        /// </summary>
        public TestResult()
        {

        }


        internal static TestResult BuildAssembly(int testNumber)
        {
            TestInfo pointCloudInfo = Assembly.PointCloudInfo.TestInfos(testNumber);

            Machine machine = new Machine();
            machine.Brand = pointCloudInfo.MachineType;
            machine.Type = pointCloudInfo.InspectionType;
            machine.Number = pointCloudInfo.MachineNumber;

            PartInspectionPlan pip = new PartInspectionPlan();
            pip.Description = pointCloudInfo.PIPName;
            pip.ID = pointCloudInfo.PIPRev;


            string folder = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\ResultFiles"; // TODO move result files to \Assembly
            string serialNumber = pointCloudInfo.SerialNumber;
            DirectoryInfo dir = new DirectoryInfo(folder);
            FileInfo[] files = dir.GetFiles();
            List<FileInfo> testFiles = files.Where(x => x.Name.Contains("Correlation_" + serialNumber + "." + testNumber + ".")).ToList();
            Characteristic[] characteristics = Characteristic.Extract(testFiles);


            RunDurations runDurations = new RunDurations(testFiles.Count);

            Tempuratures tempuratures = new Tempuratures(testFiles.Count);

            folder = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\DataFiles\MachineOutputs"; //TODO move to MachineFiles\MachineOutputs\CorrelationAssembly
            dir = new DirectoryInfo(folder);
            files = dir.GetFiles();
            testFiles = files.Where(x => x.Name.Contains("Correlation_" + testNumber + ".")).ToList();

            int i = 0;
            foreach (var document in testFiles)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(document.FullName);

                XmlNode runInfo = xml.SelectSingleNode("PartRun");

                #region Time
                string timeFull = runInfo.SelectSingleNode("StartTime").InnerText;
                string dateString = timeFull.Split('T')[0];
                string timeString = timeFull.Split('T')[1];
                //Set Date
                int year = Convert.ToInt32(dateString.Split('-')[0]);
                int month = Convert.ToInt32(dateString.Split('-')[1]);
                int day = Convert.ToInt32(dateString.Split('-')[2]);

                DateTime sdateTime = DateTime.MinValue;
                sdateTime = sdateTime.AddYears(year - 1);
                sdateTime = sdateTime.AddMonths(month - 1);
                sdateTime = sdateTime.AddDays(day - 1);
                //Set Time
                int hour = Convert.ToInt32(timeString.Split(':')[0]);
                int minute = Convert.ToInt32(timeString.Split(':')[1]);
                int second = Convert.ToInt32(timeString.Split(':', '.')[2]);
                sdateTime = sdateTime.AddHours(hour);
                sdateTime = sdateTime.AddMinutes(minute);
                sdateTime = sdateTime.AddSeconds(second);



                timeFull = runInfo.SelectSingleNode("FinishTime").InnerText;
                dateString = timeFull.Split('T')[0];
                timeString = timeFull.Split('T')[1];
                //Set Date
                year = Convert.ToInt32(dateString.Split('-')[0]);
                month = Convert.ToInt32(dateString.Split('-')[1]);
                day = Convert.ToInt32(dateString.Split('-')[2]);


                DateTime edateTime = DateTime.MinValue;
                edateTime = edateTime.AddYears(year - 1);
                edateTime = edateTime.AddMonths(month - 1);
                edateTime = edateTime.AddDays(day - 1);
                //Set Time
                hour = Convert.ToInt32(timeString.Split(':')[0]);
                minute = Convert.ToInt32(timeString.Split(':')[1]);
                second = Convert.ToInt32(timeString.Split(':', '.')[2]);
                edateTime = edateTime.AddHours(hour);
                edateTime = edateTime.AddMinutes(minute);
                edateTime = edateTime.AddSeconds(second);
                #endregion  

                runDurations.RunStart[i] = sdateTime;
                runDurations.RunFinish[i] = edateTime;

                tempuratures.Laser[i] = Convert.ToDouble(runInfo.SelectSingleNode("DLMTemperature").InnerText);
                tempuratures.Bridge1[i] = Convert.ToDouble(runInfo.SelectSingleNode("Bridge1Temperature").InnerText);
                tempuratures.Bridge2[i] = Convert.ToDouble(runInfo.SelectSingleNode("Bridge2Temperature").InnerText);
                tempuratures.Granite[i] = Convert.ToDouble(runInfo.SelectSingleNode("GraniteTemperature").InnerText);
                tempuratures.Part[i] = Convert.ToDouble(runInfo.SelectSingleNode("PartTemperature").InnerText);

                i++;
            }



            TestResult testResult = new TestResult()
            {
                TestNumber = testNumber,

                //read point cloud info
                MachineInfo = machine,
                SerialNumber = pointCloudInfo.SerialNumber,
                Operator = pointCloudInfo.OperatorName,
                PIP = pip,
                TestType = pointCloudInfo.RunType,

                //Run Info (from xml)
                RunDurations = runDurations,
                Tempuratures = tempuratures,
                StartTime = runDurations.RunStart[0],

                //get measured values
                Characteristics = characteristics
            };



            return testResult;
        }


        internal static TestResult BuildGaugeBlock(int testNumber) 
        { 
            TestInfo pointCloudInfo = GaugeBlock.PointCloudInfo.TestInfos(testNumber);

            Machine machine = new Machine();
            machine.Brand = pointCloudInfo.MachineType;
            machine.Type = pointCloudInfo.InspectionType;
            machine.Number = pointCloudInfo.MachineNumber;

            PartInspectionPlan pip = new PartInspectionPlan();
            pip.Description = pointCloudInfo.PIPName;
            pip.ID = pointCloudInfo.PIPRev;

            string folder = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\ResultFiles\GaugeBlock";
            string size = pointCloudInfo.SerialNumber.Substring(0, 2);
            DirectoryInfo dir = new DirectoryInfo(folder);
            FileInfo[] files = dir.GetFiles();
            List<FileInfo> testFiles = files.Where(x => x.Name.Contains("GaugeBlock_" + size + "." + testNumber + ".")).ToList();
            Characteristic[] characteristics = Characteristic.Extract(testFiles);


            RunDurations runDurations = new RunDurations(testFiles.Count);

            Tempuratures tempuratures = new Tempuratures(testFiles.Count);

            folder = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\MachineFiles\MachineOutputs\GaugeBlock";
            dir = new DirectoryInfo(folder);
            files = dir.GetFiles();
            testFiles = files.Where(x => x.Name.Contains("GaugeBlock_" + testNumber + ".")).ToList();

            int i = 0;
            foreach (var document in testFiles)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(document.FullName);

                XmlNode runInfo = xml.SelectSingleNode("PartRun");

                #region Time
                string timeFull = runInfo.SelectSingleNode("StartTime").InnerText;
                string dateString = timeFull.Split('T')[0];
                string timeString = timeFull.Split('T')[1];
                //Set Date
                int year = Convert.ToInt32(dateString.Split('-')[0]);
                int month = Convert.ToInt32(dateString.Split('-')[1]);
                int day = Convert.ToInt32(dateString.Split('-')[2]);

                DateTime sdateTime = DateTime.MinValue;
                sdateTime = sdateTime.AddYears(year - 1);
                sdateTime = sdateTime.AddMonths(month - 1);
                sdateTime = sdateTime.AddDays(day - 1);
                //Set Time
                int hour = Convert.ToInt32(timeString.Split(':')[0]);
                int minute = Convert.ToInt32(timeString.Split(':')[1]);
                int second = Convert.ToInt32(timeString.Split(':', '.')[2]);
                sdateTime = sdateTime.AddHours(hour);
                sdateTime = sdateTime.AddMinutes(minute);
                sdateTime = sdateTime.AddSeconds(second);



                timeFull = runInfo.SelectSingleNode("FinishTime").InnerText;
                dateString = timeFull.Split('T')[0];
                timeString = timeFull.Split('T')[1];
                //Set Date
                year = Convert.ToInt32(dateString.Split('-')[0]);
                month = Convert.ToInt32(dateString.Split('-')[1]);
                day = Convert.ToInt32(dateString.Split('-')[2]);


                DateTime edateTime = DateTime.MinValue;
                edateTime = edateTime.AddYears(year - 1);
                edateTime = edateTime.AddMonths(month - 1);
                edateTime = edateTime.AddDays(day - 1);
                //Set Time
                hour = Convert.ToInt32(timeString.Split(':')[0]);
                minute = Convert.ToInt32(timeString.Split(':')[1]);
                second = Convert.ToInt32(timeString.Split(':', '.')[2]);
                edateTime = edateTime.AddHours(hour);
                edateTime = edateTime.AddMinutes(minute);
                edateTime = edateTime.AddSeconds(second);
                #endregion  

                runDurations.RunStart[i] = sdateTime;
                runDurations.RunFinish[i] = edateTime;

                tempuratures.Laser[i] = Convert.ToDouble(runInfo.SelectSingleNode("DLMTemperature").InnerText);
                tempuratures.Bridge1[i] = Convert.ToDouble(runInfo.SelectSingleNode("Bridge1Temperature").InnerText);
                tempuratures.Bridge2[i] = Convert.ToDouble(runInfo.SelectSingleNode("Bridge2Temperature").InnerText);
                tempuratures.Granite[i] = Convert.ToDouble(runInfo.SelectSingleNode("GraniteTemperature").InnerText);
                tempuratures.Part[i] = Convert.ToDouble(runInfo.SelectSingleNode("PartTemperature").InnerText);

                i++;
            }



            TestResult testResult = new TestResult()
            {
                TestNumber = testNumber,

                //read point cloud info
                MachineInfo = machine,
                SerialNumber = pointCloudInfo.SerialNumber,
                Operator = pointCloudInfo.OperatorName,
                PIP = pip,
                TestType = pointCloudInfo.RunType,

                //Run Info (from xml)
                RunDurations = runDurations,
                Tempuratures = tempuratures,
                StartTime = runDurations.RunStart[0],

                //get measured values
                Characteristics = characteristics
            };
            
            

            return testResult;
        }
    }

    


}
