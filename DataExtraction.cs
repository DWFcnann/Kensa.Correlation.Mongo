using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kensa.Correlation.Mongo.DATA_COLLECTIONS;
using MongoDB.Driver;

namespace Kensa.Correlation.Mongo
{
    public class DataExtraction
    {
        public DataExtraction()
        {
            dbClient = new MongoClient(Properties.MONGO_CONNECTION_STRING);
            MongoDB = dbClient.GetDatabase(Properties.MONGO_DATABASE_STRING);
        }
        MongoClient dbClient = null;
        IMongoDatabase MongoDB = null;

        public CorrelationResult GetResults(int testNumber, string testType)
        {
            CorrelationResult testResult = new CorrelationResult();

            string collection;
            switch (testType)
            {
                case "Assembly":
                    collection = Properties.MONGO_RESULTS_ASSEMBLY_COLLECTION;
                    break;
                case "GaugeBlock":
                    collection = Properties.MONGO_RESULTS_GAUGEBLOCK_COLLECTION;
                    break;
                case "RingGauge":
                    collection = Properties.MONGO_RESULTS_RINGGAUGE_COLLECTION;
                    break;
                case "Plane":
                    collection = Properties.MONGO_RESULTS_PLANE_COLLECTION;
                    break;
                default:
                    throw new Exception(testType + " is not a known type.");
            }

            var list = MongoDB.GetCollection<CorrelationResult>(collection);
            testResult = list.Find(x => x.TestNumber == testNumber).FirstOrDefault();

            return testResult;
        }

        #region Gauge Block Extraction
        /// <summary>
        /// Returns Gauge block TestResult for given test number
        /// </summary>
        /// <param name="TestNumber"></param>
        /// <returns></returns>
        public CorrelationResult GetGaugeBlockResults(int testNumber)
        {
            CorrelationResult testResult = new CorrelationResult();

            var list = MongoDB.GetCollection<CorrelationResult>(Properties.MONGO_RESULTS_GAUGEBLOCK_COLLECTION);
            testResult = list.Find(x => x.TestNumber == testNumber).FirstOrDefault();

            return testResult;
        }

        /// <summary>
        /// Returns list of gauge block TestResults for given test numbers
        /// </summary>
        /// <param name="testNumbers"></param>
        /// <returns></returns>
        public List<CorrelationResult> GetGaugeBlockResults(List<int> testNumbers)
        {
            List<CorrelationResult> testResults = new List<CorrelationResult>();

            foreach (int testNumber in testNumbers)
            {
                testResults.Add(GetGaugeBlockResults(testNumber));
            }

            return testResults;
        }
        #endregion

        #region Ring Gauge Extraction
        /// <summary>
        /// Returns Ring gauge TestResult for given test number
        /// </summary>
        /// <param name="TestNumber"></param>
        /// <returns></returns>
        public CorrelationResult GetRingGaugeResults(int testNumber)
        {
            throw new NotImplementedException();

            CorrelationResult testResult = new CorrelationResult();

            var list = MongoDB.GetCollection<CorrelationResult>(Properties.MONGO_RESULTS_RINGGAUGE_COLLECTION);
            testResult = list.Find(x => x.TestNumber == testNumber).FirstOrDefault();

            return testResult;
        }
        /// <summary>
        /// Returns list of ring gauge TestResults for given test numbers
        /// </summary>
        /// <param name="testNumbers"></param>
        /// <returns></returns>
        public List<CorrelationResult> GetRingGaugeResults(List<int> testNumbers)
        {
            List<CorrelationResult> testResults = new List<CorrelationResult>();

            foreach (int testNumber in testNumbers)
            {
                testResults.Add(GetRingGaugeResults(testNumber));
            }

            return testResults;
        }
        #endregion

        #region Assembly Extraction

        #endregion

        #region Machine Info
        public Machine GetMachineInfo(string serialNumber)
        {
            Machine machine = new Machine();

            var list = MongoDB.GetCollection<Machine>(Properties.MONGO_MACHINES_COLLECTION);
            machine = list.Find(x => x.SerialNumber == serialNumber).FirstOrDefault();

            return machine;
        }
        #endregion
    }
}