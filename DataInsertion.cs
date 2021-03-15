using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kensa.Correlation.Mongo.DATA_COLLECTIONS;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

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
            AddGaugeBlockResult(CorrelationResult.BuildGaugeBlock(TestNumber));                                           //Build test result from text files, then add to database
        }

        public void AddGaugeBlockResult(CorrelationResult testResult)
        {
            var list = MongoDB.GetCollection<CorrelationResult>(Properties.MONGO_RESULTS_GAUGEBLOCK_COLLECTION);

            if (list.Find(x => x.TestNumber == testResult.TestNumber).CountDocuments() != 0)
            {
                CorrelationResult oldTestResult = list.Find(x => x.TestNumber == testResult.TestNumber).First();
                if (!UpdateResult("Gauge Block", testResult, oldTestResult)) { return; }
            }

            
            //list.InsertOne(testResult);


            IGridFSBucket bucket = new GridFSBucket(MongoDB);
            Stream source = File.Open(@"\\dwffs08\ToolNet\ZeroTouch\Correlation\MachineFiles\PointClouds\GaugeBlock\GaugeBlock_13.1.psl", FileMode.Open);

            var id = bucket.UploadFromStream("pointcloud", source);

            //MessageBox.Show("Test Result Added");
        }
        public void AddAssemblyResult(int TestNumber)
        {
            AddAssemblyResult(CorrelationResult.BuildAssembly(TestNumber));                                           //Build test result from text files, then add to database
        }

        public void AddAssemblyResult(CorrelationResult testResult)
        {
            var list = MongoDB.GetCollection<CorrelationResult>(Properties.MONGO_RESULTS_ASSEMBLY_COLLECTION);

            if (list.Find(x => x.TestNumber == testResult.TestNumber).CountDocuments() != 0)
            {
                CorrelationResult oldTestResult = list.Find(x => x.TestNumber == testResult.TestNumber).First();
                if (!UpdateResult("Assembly", testResult, oldTestResult)) { return; }
                else
                {
                    FilterDefinition<CorrelationResult> filterDefinition = Builders<CorrelationResult>.Filter.Eq("TestNumber", testResult.TestNumber);
                    list.ReplaceOne(filterDefinition, testResult);
                }
            }
            else { list.InsertOne(testResult); }

            //MessageBox.Show("Test Result Added");
        }


        #region Utilities
        static bool UpdateResult(string testType, CorrelationResult newTestResult, CorrelationResult oldTestResult)
        {
            if (newTestResult == null || oldTestResult == null) { MessageBox.Show("This is something null in a test result"); return false; }

            bool fileToUpdate = Comparer<CorrelationResult>.Equals(newTestResult, oldTestResult); //TODO fix this, it always shows false

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
