using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Kensa.Correlation.Mongo.TCP_Classes
{
  /// <summary>
  /// This client requires the use of a MongoDB Driver
  /// You'll need to add this through a nuget package.  Use the following command
  /// on the command line inteface:  dotnet add package MongoDB.Driver --version 2.8.1
  /// or using the package manager using the following line: Install-Package MongoDB.Driver -Version 2.8.1
  /// Maybe I'm becoming an old man, but I really dont like the Package Manager. It seems too easy for MS to shove
  /// extra trash in your system. Anyway, this should result in the following referenced added to the project:
  /// MongoDB.Bson
  /// MongoDB.Driver
  /// MongoDB.Driver.Core
  /// Among other System like assemblies
  /// Note that to connect to a database - first you'll need network access - then a connection string
  /// The connection string typically looks like this: mongodb://localhost:27017/databasename.collectionname
  /// Let's say it's in another PC, then it would look like this:   mongodb://mongodb0.example.com:27017/mydatabase_name
  ///                                                         or:   mongodb://192.168.1.15:27017/mydatabase_name
  /// Let's say you needed a login and a password then  use this:   mongodb://booRadly:MrFinchSucks@mongodb0.example.com:27017/AtticusDataBase
  /// where booRadly is the login, MrFinchSucks is the password, and AtticusDataBase is the database name 
  /// 
  /// This class is really pretty simple. It connects, allows the user to check if we're connected. 
  /// It also sends/requests PARTDATA to the specified DATABASE. Feel free to add more trash in here, but at this point I'm pretty 
  /// focused on the immediate job at hand. 
  /// 
  ///     5446 was my number.... 
  ///     right now someone else has that number....
  ///     https://www.youtube.com/watch?v=yjg6flu3zuc
  ///                                                   - Toots and the Maytals
  /// </summary>
  class MongoDBClientExtract
  {
    MongoClient ClientObject = null;

    string _connectionString = String.Empty;
    public string GetConnectionString { get { return _connectionString; } }

    string _ipString = String.Empty;
    public string IPAddressString { get { return _ipString; } }

    string _portstring = String.Empty;
    public string PortString { get { return _portstring; } }

    MongoDB.Driver.Core.Servers.ServerDescription _serverDescrip;
    public MongoDB.Driver.Core.Servers.ServerDescription GetServerDescription { get { return _serverDescrip; } }

    string _databaseName = String.Empty;
    public string GetDatabaseName { get { return _databaseName; } }

    IMongoDatabase _dbObject;
    public IMongoDatabase GetDataBase { get { return _dbObject; } }

   // public const string PART_DATA_DATABASE_NAME = "CalFix_Test";
   // public const string FAI_FLAT_DATA_COLLECTION_NAME = "FAI_Collection";

    public const string FAI_FLAT_DATABASE_NAME = "CorrelationFAIDatabase";
    public const string PART_DATA_COLLECTION_NAME = "FixtureData";


    

    //"mongodb://10.60.1.15:27017"
    public const string FWD_SLASH = "//";
    public const string COMMA = ",";
    public const string COLON = ":";

    /// <summary>
    /// DEFAULT
    /// </summary>
    public MongoDBClientExtract()
    {

    }

      /// <summary>
      /// Simple Constructor  THIS ACTUALLY ATTEMPTS TO OPEN THE CONNECTION
      /// </summary>
      /// <param name="szConnectionString"></param>
      /// <param name="szDatabaseName"></param>
      public MongoDBClientExtract(string szConnectionString, string szdatabase)
    {

      string[] mySplits;
      string[] splitArray = new string[] { FWD_SLASH, COLON };
      mySplits = szConnectionString.Split(splitArray, StringSplitOptions.None);
      if (mySplits.Length >= 4)
      {
        _ipString = mySplits[2];
        _portstring = mySplits[3];
      }


      _connectionString = szConnectionString + "/" + szdatabase;                                 //Initialize 

      ClientObject = new MongoClient(_connectionString);                                          //Tries to connect

      MongoClientSettings mysettings = ClientObject.Settings;                                     //Used to verify that we're really connected 
      //mysettings.ConnectTimeout = TimeSpan.FromMilliseconds(1000);                              //Give it a second - THROWS AN EXCEPTION

      _serverDescrip = ClientObject.Cluster.Description.Servers.FirstOrDefault();                 //Find out about the server
      _dbObject = ClientObject.GetDatabase(szdatabase);                                           //Get database object

    }

    /// <summary>
    /// Does a check and returns true of we are connected
    /// </summary>
    /// <returns></returns>
    public bool IsConnected()
    {
      bool retVal = false;
      try
      {
        //not necessary but any command that talk to database would be fine.
        BsonDocument command = new BsonDocument { { "connectionStatus", 1 }, { "showPrivileges", true } };
        BsonDocument result = _dbObject.RunCommand<BsonDocument>(command);
        // if(result != null) { retVal = true; } //one way to do this
        if (ClientObject.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
          retVal = true;
          return retVal;
        }

        return retVal;
      }
      catch (Exception ex)
      {
        var t = ex.Message;
        return retVal;
      }
    }

    ///// <summary>
    ///// Inserts a PArtData class into the Database. Uses the constant: FULL_DATA_PATH
    ///// Users will need to change this as required
    ///// </summary>
    ///// <param name="mypart"></param>
    ///// <param name="szCollectionName"></param>
    //public void InsertPartDataIntoCollection(PartData mypart)
    //{
    //  IMongoCollection<PartData> myCollection = GetDataBase.GetCollection<PartData>(PART_DATA_COLLECTION_NAME);    //Get the collection
    //  myCollection.InsertOne(mypart);                                                                             //Insert
    //}


    ///// <summary>
    ///// OVERLOADED: Inserts the Partdata object into the specified collection 
    ///// </summary>
    ///// <param name="mypart"></param>
    ///// <param name="szCollectionName"></param>
    //public void InsertPartDataIntoCollection(PartData mypart, string szCollectionName)
    //{
    //  IMongoCollection<PartData> myCollection = GetDataBase.GetCollection<PartData>(szCollectionName); //Get the collection
    //  myCollection.InsertOne(mypart);                                                                   //Insert
    //}

    /*
    /// <summary>
    /// Inserts the specified FAIFlat class into the Database. Uses the constant: FULL_DATA_PATH
    /// Users will need to change this as required
    /// </summary>
    /// <param name="myFAI"></param>
    public void InsertFAIFlatIntoCollection(FAI_FLAT_Data myFAI)
    {
      IMongoCollection<FAI_FLAT_Data> myCollection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);    //Get the collection
      myCollection.InsertOne(myFAI);                                                                             //Insert
    }


    /// <summary>
    /// THIS NEEDS TO BE TESTED - BASICALLY GETS ALL THE RECORDS IN THE PARTDATA COLLECTION 
    /// </summary>
    /// <returns></returns>
    public List<FAI_FLAT_Data> GetAllPartDataRecords()
    {
      List<FAI_FLAT_Data> retList = null;
      retList = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME).AsQueryable().ToList();
        //.Where(x => x.RecipeName == recipeName)
        //.Select(i => i.Revision)
        //.FirstOrDefault();
     
      return retList;
    }

    /// <summary>
    /// Returns all records with the specified TestID
    /// </summary>
    /// <param name="szTestID"></param>
    /// <returns></returns>
    public List<FAI_FLAT_Data> GetAllPartDataRecords(string szTestID)
    {
      List<FAI_FLAT_Data> retList = null;
      retList = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME).AsQueryable()
      .Where(x => x.TestID == szTestID).ToList();
      return retList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="szTestID"></param>
    /// <param name="evalID"></param>
    /// <returns></returns>
    public List<FAI_FLAT_Data> GetAllPartDataRecords(string szTestID, string evalID)
    {
      List<FAI_FLAT_Data> retList = null;
      retList = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME).AsQueryable()
      .Where(x => x.TestID == szTestID && x.EvaluationID == evalID).ToList();
      return retList;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="szTestID"></param>
    /// <param name="evalID"></param>
    /// <param name="toolID"></param>
    /// <returns></returns>
    public List<FAI_FLAT_Data> GetAllPartDataRecords(string szTestID, string evalID, string toolID)
    {
      List<FAI_FLAT_Data> retList = null;
      retList = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME).AsQueryable()
      .Where(x => x.TestID == szTestID && x.EvaluationID == evalID && x.CollectionToolID == toolID).ToList();
      return retList;
    }

   

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<String> GetAllDistinctTestID()
    {
      List<string> myStringList = new List<string>();
     
      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);

      var Thisfilter = new BsonDocument();
      var MyData = collection.Distinct<string>("TestID", Thisfilter);
      myStringList = MyData.ToList<string>();

      return myStringList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<String> GetAllDistinctEvaluationID()
    {
      List<string> myStringList = new List<string>();

      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);

      var Thisfilter = new BsonDocument();
      var MyData = collection.Distinct<string>("EvaluationID", Thisfilter);
      myStringList = MyData.ToList<string>();

      return myStringList;
    }

    /// <summary>
    /// For the given TestID, return all the EvaluationID's
    /// </summary>
    /// <param name="szTestID"></param>
    public void MyTest(string szTestID)
    {
      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);
      var condition = Builders<FAI_FLAT_Data>.Filter.Eq(p => p.TestID, "42.2");                                  //ONLY LOOK FOR TEST ID 42
      var fields = Builders<FAI_FLAT_Data>.Projection.Include(p => p.TestID).Include(p => p.EvaluationID);       //HOW TO GET MULTIPLE FIELDS
      //var fields = Builders<FAI_FLAT_Data>.Projection.Include(p => p.EvaluationID);                            //JUST RETURN THE EVALUATION ID
      var results = collection.Find(condition).Project<FAI_FLAT_Data>(fields).ToList().AsQueryable();
    
      List<string> evaluationIDList = new List<string>();
      foreach(FAI_FLAT_Data FAI in results)
      {
         evaluationIDList.Add(FAI.EvaluationID);
      }

      var distinctNames = (from d in evaluationIDList select d).Distinct();

      List<string> distinctevaluationIDList = new List<string>();
      foreach (string mystring in distinctNames)
      {
        distinctevaluationIDList.Add(mystring);
      }

      // return distinctevaluationIDList;
    }

    /// <summary>
    /// Returns a list of PartID's for the given Test_ID, Eval_ID, AND Part_ID 
    /// </summary>
    /// <param name="szTestID"></param>
    /// <param name="szEvalID"></param>
    /// <param name="szpartID"></param>
    public List<FAI_FLAT_Data> GetDistinctGetAllEntries(string szTestID, string szEvalID, string szpartID)
    {
      List<string> evaluationIDList = new List<string>();

      //GET THE COLLECTION
      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);
      //FIND ONLY THOSE DATA ENTRIES IN THE COLLECTION THAT MEET THE SPECIFIED CRITERIA 
      var myResults = collection.Find(dataEntry => dataEntry.TestID == szTestID && dataEntry.EvaluationID == szEvalID && dataEntry.PartID == szpartID);
      //TURN THE RESULTS INTO A LIST OF FAI_FLAT_Data
      List<FAI_FLAT_Data> tempList = myResults.ToList<FAI_FLAT_Data>();

      return tempList;
    }



    /// <summary>
    /// Returns a list of PartID's for the given Test_ID, Eval_ID, Part_ID, and an FAI string list 
    /// </summary>
    /// <param name="szTestID"></param>
    /// <param name="szEvalID"></param>
    /// <param name="szpartID"></param>
    /// <param name="FAIList"></param>
    /// <returns></returns>
    public List<FAI_FLAT_Data> GetDistinctGetAllEntries(string szTestID, string szEvalID, string szpartID, List<string> FAIList)
    {
      List<string> evaluationIDList = new List<string>();
      List<FAI_FLAT_Data> DataPileList = new List<FAI_FLAT_Data>();
     // IFindFluent<FAI_FLAT_Data, FAI_FLAT_Data> myPile;

      //GET THE COLLECTION
      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);

      for(int idx = 0; idx < FAIList.Count; idx++)
      {
        //FIND ONLY THOSE DATA ENTRIES IN THE COLLECTION THAT MEET THE SPECIFIED CRITERIA 
        // var myResults = collection.Find(dataEntry => dataEntry.TestID == szTestID && dataEntry.EvaluationID == szEvalID && dataEntry.PartID == szpartID && dataEntry.FAI_ID == FAIList[idx]);
        var myPile = collection.Find(dataEntry => dataEntry.TestID == szTestID && dataEntry.EvaluationID == szEvalID && dataEntry.PartID == szpartID && dataEntry.FAI_ID == FAIList[idx]);
        List<FAI_FLAT_Data> tempList = myPile.ToList<FAI_FLAT_Data>();

        for(int ndx = 0; ndx < tempList.Count; ndx++)
        {
          DataPileList.Add(tempList[ndx]);
        }
      }

      return DataPileList;
    }



    /// <summary>
    /// Returns a string list of the partID string associated 
    /// with the given TestID and the EvalID
    /// </summary>
    /// <param name="szTestID"></param>
    /// <param name="szevalID"></param>
    /// <returns></returns>
    public List<string> GetDistinctPartIDs(string szTestID, string szEvalID)
    {
      List<string> partIDList = new List<string>();

      //USING THE COLLECTION AND THE FOLOWING CONDITIONS AND FIELD DEF's GET THE RESULT
      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);

      //MAKE UP A FILTER -"AND" A COUPLE TOGETHER
      var myFilter = Builders<FAI_FLAT_Data>.Filter.Eq(p => p.TestID, szTestID);                                //ONLY LOOK FOR THE GIVEN TEST ID 
      myFilter = myFilter & Builders<FAI_FLAT_Data>.Filter.Eq(p => p.EvaluationID, szEvalID);

      //var fields = Builders<FAI_FLAT_Data>.Projection.Include(p => p.TestID).Include(p => p.PartID);       //HOW TO GET MULTIPLE FIELDS
      var myProjection = Builders<FAI_FLAT_Data>.Projection.Include(p => p.PartID);
      
      //GET THE REULTS AND TURN THEM INTO A LIST
      var results = collection.Find(myFilter).Project<FAI_FLAT_Data>(myProjection).ToList().AsQueryable();

      ///CONVERT THE RESULT TO A STRING LIST
      List<string> evaluationIDList = new List<string>();
      foreach (FAI_FLAT_Data FAI in results)
      {
        partIDList.Add(FAI.PartID);
      }

      //RETURN ONLY THE DISTINCT VALUES
      var distinctPartID = (from d in partIDList select d).Distinct();

      //PLACE THEM IN THE FORM OF A LIST OF STRINGS
      List<string> distinctPartIDList = new List<string>();
      foreach (string mystring in distinctPartID)
      {
        distinctPartIDList.Add(mystring);
      }

      return distinctPartIDList;
    }


      /// <summary>
      /// For the given test ID - get a list of distinct evaluation ID's
      /// </summary>
      /// <param name="szTestID"></param>
      /// <returns></returns>
      public List<string> GetDistinctEvaluationIDs(string szTestID)
    {
      //USING THE COLLECTION AND THE FOLOWING CONDITIONS AND FIELD DEF's GET THE RESULT
      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);
      var condition = Builders<FAI_FLAT_Data>.Filter.Eq(p => p.TestID, szTestID);                                //ONLY LOOK FOR TEST ID 42
      var fields = Builders<FAI_FLAT_Data>.Projection.Include(p => p.TestID).Include(p => p.EvaluationID);       //HOW TO GET MULTIPLE FIELDS
      //var fields = Builders<FAI_FLAT_Data>.Projection.Include(p => p.EvaluationID);                            //JUST RETURN THE EVALUATION ID
      var results = collection.Find(condition).Project<FAI_FLAT_Data>(fields).ToList().AsQueryable();


      ///CONVERT THE RESULT TO A STRING LIST
      List<string> evaluationIDList = new List<string>();
      foreach (FAI_FLAT_Data FAI in results)
      {
        evaluationIDList.Add(FAI.EvaluationID);
      }

      //RETURN ONLY THE DISTINCT VALUES
      var distinctNames = (from d in evaluationIDList select d).Distinct();

      //PLACE THEM IN THE FORM OF A LIST OF STRINGS
      List<string> distinctevaluationIDList = new List<string>();
      foreach (string mystring in distinctNames)
      {
        distinctevaluationIDList.Add(mystring);
      }

       return distinctevaluationIDList;
    }

    /// <summary>
    /// GENERATES A SET OF FAKE FAI - NOTE:NOT FULLY POPULATED
    /// </summary>
    /// <returns></returns>
    public List<FAI_FLAT_Data> GenerateSomeFakeData()
    {
      List<FAI_FLAT_Data> myFakeDataList = new List<FAI_FLAT_Data>();
      
      string[] IDArray = new string[] { "34.1", "34.2", "34.3", "34.4", "34.5", "34.6", "34.7", "34.8", "35", "36", "11",
                                        "12.1", "12.2", "14", "25", "9.1", "16", "30", "31.1", "31.2", "32.1", "32.2", "6",
                                           "7", "8" };

      // GIVE IT 5 PARTS - START WITH 1
      for (int run = 1; run < 6; run++)
      {
        //25 FAI FOR EACH RUN ID
        for (int idx = 0; idx < 25; idx++)
        {
          myFakeDataList.Add(new FAI_FLAT_Data("FAKE-171", "999", "123", run, IDArray[idx], idx));
        }//FAI FOR
      }//OUTSIDE FOR

      return myFakeDataList;
    }


    ///// <summary>
    ///// Returns a list of the the Test and Evaluation ID's in the collection
    ///// </summary>
    ///// <returns></returns>
    public List<FAI_TestAndEval> GetAllTestIDAndEvalID()
    {

    //  List<string> myStringList = new List<string>();
    //  List<FAI_TestAndEval> retList = new List<FAI_TestAndEval>();

      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);

      return GetAllTestIDAndEvalID(collection);
    }



    /// <summary>
    /// Returns a list of the the TesT and evaluation ID's in the collection
    /// </summary>
    /// <param name="myCollection"></param>
    /// <returns></returns>
    public List<FAI_TestAndEval> GetAllTestIDAndEvalID(IMongoCollection<FAI_FLAT_Data> myCollection)
    {

      List<string> myTestIDStringList = new List<string>();
      List<FAI_TestAndEval> retList = new List<FAI_TestAndEval>();

   
      var Thisfilter = new BsonDocument();
      var MyTestIDs = myCollection.Distinct<string>("TestID", Thisfilter);
      myTestIDStringList = MyTestIDs.ToList<string>();     //MAKE A LIST OF THE UNIQUE TEST ID STRINGS
     



      for (int idx = 0; idx < myTestIDStringList.Count; idx++)
      {
        retList.Add(new FAI_TestAndEval(myTestIDStringList[idx], GetDistinctEvaluationIDs(myTestIDStringList[idx])));
      }
      return retList;
    }

    /// <summary>
    /// Deletes all of the records with the specified TestID string - ASYNCHRONOUSLY
    /// </summary>
    /// <param name="szTestID"></param>
    /// <returns></returns>
    public async Task DeleteAllRecordsAsyncWithTestID(string szTestID)
    {
      // GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME).DeleteMany("TestID":{ szTestID});
      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);
      var multipleDelete = await collection.DeleteManyAsync(Builders<FAI_FLAT_Data>.Filter.Eq("Test_ID", szTestID));
      long nVal = multipleDelete.DeletedCount;

    }

    /// <summary>
    /// Deletes all of the records with the specified TestID string - Blocks
    /// </summary>
    /// <param name="szTestID"></param>
    public void DeleteAllRecordsWithTestID(string szTestID)
    {
      var collection = GetDataBase.GetCollection<FAI_FLAT_Data>(PART_DATA_COLLECTION_NAME);
      var multipleDelete = collection.DeleteMany(Builders<FAI_FLAT_Data>.Filter.Eq("Test_ID", szTestID));
      long nVal = multipleDelete.DeletedCount;
    }
        */
  }//CLASS
}//NAMESPACE
