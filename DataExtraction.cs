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


        #region Gauge Block Extraction
        /// <summary>
        /// Returns Gauge block TestResult for given test number
        /// </summary>
        /// <param name="TestNumber"></param>
        /// <returns></returns>
        public TestResult GetGaugeBlockResults(int testNumber)
        {
            TestResult testResult = new TestResult();

            var list = MongoDB.GetCollection<TestResult>(Properties.MONGO_RESULTS_GAUGEBLOCK_COLLECTION);
            testResult = list.Find(x => x.TestNumber == testNumber).FirstOrDefault();

            return testResult;
        }

        /// <summary>
        /// Returns list of gauge block TestResults for given test numbers
        /// </summary>
        /// <param name="testNumbers"></param>
        /// <returns></returns>
        public List<TestResult> GetGaugeBlockResults(List<int> testNumbers)
        {
            List<TestResult> testResults = new List<TestResult>();

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
        public TestResult GetRingGaugeResults(int testNumber)
        {
            throw new NotImplementedException();

            TestResult testResult = new TestResult();

            var list = MongoDB.GetCollection<TestResult>(Properties.MONGO_RESULTS_RINGGAUGE_COLLECTION);
            testResult = list.Find(x => x.TestNumber == testNumber).FirstOrDefault();

            return testResult;
        }
        /// <summary>
        /// Returns list of ring gauge TestResults for given test numbers
        /// </summary>
        /// <param name="testNumbers"></param>
        /// <returns></returns>
        public List<TestResult> GetRingGaugeResults(List<int> testNumbers)
        {
            List<TestResult> testResults = new List<TestResult>();

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