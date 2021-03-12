using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kensa.Correlation.Mongo.DATA_COLLECTIONS;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace Kensa.Correlation.Mongo
{
    public class DataInsertion
    {
        public DataInsertion()
        {
            dbClient = new MongoClient(Properties.MONGO_CONNECTION_STRING);
            MongoDB = dbClient.GetDatabase(Properties.MONGO_DATABASE_STRING);
        }
        MongoClient dbClient = null;
        IMongoDatabase MongoDB = null;

        public void AddGaugeBlockResult(int TestNumber)
        {
            AddGaugeBlockResult(TestResult.BuildGaugeBlock(TestNumber));                                           //Build test result from text files, then add to database
        }

        public void AddGaugeBlockResult(TestResult testResult)
        {
            var list = MongoDB.GetCollection<TestResult>(Properties.MONGO_RESULTS_GAUGEBLOCK_COLLECTION);

            if (list.Find(x => x.TestNumber == testResult.TestNumber).CountDocuments() != 0)
            {
                TestResult oldTestResult = list.Find(x => x.TestNumber == testResult.TestNumber).First();
                if (!UpdateResult("Gauge Block", testResult, oldTestResult)) { return; }
            }

            list.InsertOne(testResult);
            MessageBox.Show("Test Result Added");
        }
        public void AddAssemblyResult(int TestNumber)
        {
            AddAssemblyResult(TestResult.BuildAssembly(TestNumber));                                           //Build test result from text files, then add to database
        }

        public void AddAssemblyResult(TestResult testResult)
        {
            var list = MongoDB.GetCollection<TestResult>(Properties.MONGO_RESULTS_ASSEMBLY_COLLECTION);

            if (list.Find(x => x.TestNumber == testResult.TestNumber).CountDocuments() != 0)
            {
                TestResult oldTestResult = list.Find(x => x.TestNumber == testResult.TestNumber).First();
                if (!UpdateResult("Assembly", testResult, oldTestResult)) { return; }
                else
                {
                    FilterDefinition<TestResult> filterDefinition = Builders<TestResult>.Filter.Eq("TestNumber", testResult.TestNumber);
                    list.ReplaceOne(filterDefinition, testResult);
                }
            }
            else { list.InsertOne(testResult); }

            MessageBox.Show("Test Result Added");
        }


        #region Utilities
        static bool UpdateResult(string testType, TestResult newTestResult, TestResult oldTestResult)
        {
            if (newTestResult == null || oldTestResult == null) { MessageBox.Show("This is something null in a test result"); return false; }

            bool fileToUpdate = Comparer<TestResult>.Equals(newTestResult, oldTestResult); //TODO fix this, it always shows false

            if (fileToUpdate)
            { MessageBox.Show("Test Result was previously added, no update."); return false; }
            else
            {
                DialogResult update = MessageBox.Show(testType + " test number: " + newTestResult.TestNumber + " has already been added. \r\n Would you like to update?", "Update Result?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (update == DialogResult.No) { return false; }

            }

            return true;
        }
        #endregion
    }
}
