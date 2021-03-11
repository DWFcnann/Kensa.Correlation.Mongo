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
  /// on the command line inteface: dotnet add package MongoDB.Driver --version 2.8.1
  /// or using the package manager using the following line: Install-Package MongoDB.Driver -Version 2.8.1
  /// Mybe I'm becoming an old man, but I really dont like the Package Manager. It seems to easy for MS to shove
  /// extra trash in your system. Anyway, this should result in the following referenced added to the project:
  /// MongoDB.Bson
  /// MongoDB.Driver
  /// MongoDB.Driver.Core
  /// Among other System like assemblies
  /// Note that to connect to a database - first you'll need network access - then a connection string
  /// the connection string typically looks like this: mongodb://localhost:27017/databasename.collectionname
  /// Let's say it's in another PC, then it would look like this:   mongodb://mongodb0.example.com:27017/mydatabase_name
  ///                                                         or:   mongodb://192.168.1.15:27017/mydatabase_name
  /// Let's say you needed a login and a password then  use this:   mongodb://booRadly:MrFinchSucks@mongodb0.example.com:27017/AtticusDataBase
  /// where booRadly is the login, MrFinchSucks is the password, and AtticusDataBase is the database name 
  /// 
  /// This class is really pretty simple. It connects, allows the user to check if we're connected. 
  /// It also sends PARTDATA to the specified DATABASE. Feel free to add more trash in here, but at this point I'm pretty 
  /// focused on the immediate job at hand. 
  /// 
  /// 5446 was my number.... right now someone else has that number....
  /// 
  /// </summary>
  class MongoDBClientInsert 
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

    public const string PART_DATA_DATABASE_NAME = "CalFix_Test";
    public const string FAI_FLAT_DATABASE_NAME = "CorrelationFAIDatabase";

    public const string PART_DATA_COLLECTION_NAME = "FixtureData";
    public const string FAI_FLAT_DATA_COLLECTION_NAME = "FAI_Collection";

    //"mongodb://10.60.1.15:27017"
    public const string FWD_SLASH = "//";
    public const string COMMA = ",";
    public const string COLON = ":";


    /// <summary>
    /// Simple Constructor  THIS ACTUALLY ATTEMPTS TO OPEN THE CONNECTION
    /// </summary>
    /// <param name="szConnectionString"></param>
    /// <param name="szDatabaseName"></param>
    public MongoDBClientInsert(string szConnectionString, string szdatabase)
    {

      string[] mySplits;
      string[] splitArray = new string[] { FWD_SLASH, COLON };
      mySplits = szConnectionString.Split(splitArray, StringSplitOptions.None);
      if(mySplits.Length >= 4)
      {
        _ipString = mySplits[2];
        _portstring = mySplits[3];
      }
     

      _connectionString = szConnectionString +  "/" + szdatabase;                                 //Initialize 

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
      catch(Exception ex)
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
    //  IMongoCollection<PartData> myCollection = GetDataBase.GetCollection< PartData>(szCollectionName); //Get the collection
    //  myCollection.InsertOne(mypart);                                                                   //Insert
    //}

    /// <summary>
    /// Inserts the specified FAIFlat class into the Database. Uses the constant: FULL_DATA_PATH
    /// Users will need to change this as required
    /// </summary>
    /// <param name="myFAI"></param>
    //public void InsertFAIFlatIntoCollection(FAI_FLAT myFAI)
    //{
    //  IMongoCollection<FAI_FLAT> myCollection = GetDataBase.GetCollection<FAI_FLAT>(PART_DATA_COLLECTION_NAME);    //Get the collection
    //  myCollection.InsertOne(myFAI);                                                                             //Insert
    //}


    ///// <summary>
    ///// THIS NEEDS TO BE TESTED - BASICALLY GETS ALL THE RECORDS IN THE PARTDATA COLLECTION 
    ///// </summary>
    ///// <returns></returns>
    //public List<PartData> GetAllPartDataRecords()
    //{

    //  IMongoCollection<PartData> collection = GetDataBase.GetCollection<PartData>(PART_DATA_COLLECTION_NAME);
    //  return collection.Find(new BsonDocument()).ToList();
    //}

    ///// <summary>
    ///// THIS NEEDS TO BE TESTED - BASICALLY GETS ALL THE RECORDS IN THE PARTDATA COLLECTION 
    ///// </summary>
    ///// <returns></returns>
    //public List<PartData> GetAllPartDataRecords(string szID)
    //{
    //  IMongoCollection<PartData> collection = GetDataBase.GetCollection<PartData>(PART_DATA_COLLECTION_NAME);
    //  var filter = Builders<PartData>.Filter.Eq("PartID", szID);
    //  return collection.Find(filter).ToList<PartData>();

    //  //public IMongoCollection<PartData>  GetPartDataFromCollection(string MyPartID, string szCollectionName)
    //  //{
    //  //  PartData retPart = new PartData();
    //  //  var myCollection = GetDataBase.GetCollection<PartData>(szCollectionName); //Get the collection

    //  //  var filter = Builders<PartData>.Filter.Eq("PartID", MyPartID);
    //  //  return myCollection.Find(filter).FirstOrDefault();

    //  //}
    //}



  }//CLASS
}//NAMESPACE
