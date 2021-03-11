using Kensa.Correlation.Mongo.TCP_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using Kensa.Correlation.Mongo.DATA_COLLECTIONS;
using System.IO;

namespace Kensa.Correlation.Mongo
{


    public class Class1
    {
        public const string TERMINATOR = "\r\n";                                                        //I'LL BE BACK
        public const string COMMA = ",";                                                                //WAIT...
        public const string SPACE = " ";                                                                //THE FINAL FRONTIER
        public const string COLON = ":";                                                                //COLON BLOW
        public const string TAB = "\t ";                                                                //RUN ONE UP
        public const string CONNECTED = "CONNECTED";                                                    //
        public const string DISCONNECT = "DISCONNECT";                                                  //



        public void Testtt(object sender, EventArgs e)
        {
            

            var dbClient = new MongoClient(Properties.MONGO_CONNECTION_STRING);


            var dbList = dbClient.ListDatabases().ToList();
            foreach(var item in dbList)
            {
                Console.WriteLine(item.ToString());
            }
            IMongoDatabase db = dbClient.GetDatabase(Properties.MONGO_DATABASE_STRING);

            var list = db.GetCollection<TestResult>(Properties.MONGO_RESULTS_GAUGEBLOCK_COLLECTION);
            //var docs = list.Find(new BsonDocument()).ToList();

            TestResult onlyTest = list.Find(x => x.TestNumber == 13).FirstOrDefault();
            



            string[] info = File.ReadAllLines(@"\\dwffs08\ToolNet\ZeroTouch\Correlation\DataFiles\TestInfo\GaugeBlock_TestInfo.csv");
            int lastTest = info.Length - 1;
            list.InsertOne(TestResult.Build(28));
            list.InsertOne(TestResult.Build(29));
            
            for (int i = 25; i <= lastTest; i++)
            {
                TestResult testResult = TestResult.Build(i);

                try
                {
                    list.InsertOne(testResult);
                }
                catch {
                    FilterDefinition<TestResult> filterDefinition = Builders<TestResult>.Filter.Eq("TestNumber", i);
                    list.ReplaceOne(filterDefinition, testResult); }
            }

            //list.InsertMany(results);
        }

    }
}
