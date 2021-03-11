using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace Kensa.Correlation.Mongo.TCP_Classes
{
  //THESE DELGATES DEFINE ESSENTIALLY EVENTS THAT THE SERVER CLASS BELOW ISSUES 
  public delegate void AsyncTCPServerDataReceivedEvent(ClientStateObject clientObj);
  public delegate void AsyncTCPServerDataSentEvent(ClientStateObject clientObj);
  public delegate void AsyncTCPServerConnectionEvent(ClientStateObject clientObj, int connectionCount);
  public delegate void AsyncTCPServerStatusEvent(string szData);

  /// <summary>
  /// This class is the basic object that gets thrown around
  /// the server class below. It essentially holds all the 
  /// current information for a particular client and it's
  /// latests transactions
  /// </summary>
  public class ClientStateObject
  {
    public Socket workSocket = null;                //Specific Client Socket
    public const int BUFFER_SIZE = 3200000;         //Receive Buffer size in Bytes
    public byte[] buffer = new byte[BUFFER_SIZE];
    public StringBuilder sb = new StringBuilder();
    public IPEndPoint SocketEndpoint = null;
    public String Transaction = String.Empty;       //DATA TRANSACTION 
    public DateTime TimeTag;
    public int RequestedProcessID = -1;
  }//CLASS


  /// <summary>
  /// This is a simple Asynch TCP server Class. 
  /// </summary>
  class AsyncTCPServer
  {
    //COMMON DIALECT 
    public const string STATUS = "STATUS: ";
    public const string INSTANTIATED = "Instantiated";
    public const string CLIENTLIST_COUNT_STRING = "Client List Count:";
    public const string SPACE = " ";
    public const string COLON = ":";
    public const string SLASH = "/";
    public const string PERIOD = ".";
    public const string TERMINATOR = "\r\n";
    public const string EXIT = "<EXIT>";
    public const string STATUS_IPADRESS_STRING = "Server Listening at IPAddress: ";
    public const string STATUS_PORT_STRING = ", on PORT: ";
    public bool SendWithTerminator { get; set; }
    public string Terminator { get; set; }

    //SERVER SPECIFIC STUFF
    private readonly Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static ArrayList ClientList = new ArrayList();                       //HOLDS ALL THE CLIENTS
    public ArrayList GetClientList { get { return ClientList; } }
    public int ClientCount { get { return ClientList.Count; } }
    string _IPListeningOn = "0.0.0.0";
    public string IPListeningOn { get { return _IPListeningOn; } }
    private int _Port;
    public int Port { get { return _Port; } }
    private string _ServerDescription;
    public string ServerDescription { get { return _ServerDescription; } set { _ServerDescription = value; } }

    //EVENT/DELEGATE DECLARARION SECTION - BABY ITS A RED FLAG DAY - BABY LET'S GET INTO THE WATER
    public event AsyncTCPServerDataReceivedEvent DataReceivedNotification;
    public event AsyncTCPServerDataSentEvent DataSentNotification;
    public event AsyncTCPServerConnectionEvent ConnectionNotification;
    public event AsyncTCPServerStatusEvent StatusNotification;

    /// <summary>
    /// Simple Constructor
    /// </summary>
    /// <param name="port"></param>
    /// <param name="description"></param>
    public AsyncTCPServer(int port, string description)
    {
      _ServerDescription = description;                                 //SIMPLE DESCRIPTION STRING
      _Port = port;                                                     //PORT TO LISTEN ON
      Terminator = TERMINATOR;                                          //INITIALIZE
      OnStatusNotification(ServerDescription + SPACE + INSTANTIATED);   //LET US ALL KNOW
    }

    /// <summary>
    /// Constructor - includes the terminator
    /// </summary>
    /// <param name="port"></param>
    /// <param name="description"></param>
    /// <param name="szTerminator"></param>
    public AsyncTCPServer(int port, string description, string szTerminator)
    {
      _ServerDescription = description;                                 //SIMPLE DESCRIPTION STRING
      _Port = port;                                                     //PORT TO LISTEN ON
      Terminator = szTerminator;                                          //INITIALIZE
      OnStatusNotification(ServerDescription + SPACE + INSTANTIATED);   //LET US ALL KNOW
    }


    #region START/STOP FUNCTIONS

    /// <summary>
    /// Start the whole damn thing. " I had to start it somewhere, so I started it there" - Shatner
    /// Pass in the index for the network interface card you wish to use
    /// </summary>
    /// <param name="NIC_Index"></param>
    public void Start(int NIC_Index)
    {
      byte[] bytes = new Byte[2048];

      //FIGURE OUT WHO WE ARE AND ESTABLISH A LOCAL END POINT - NOTE
      //IF YOUR SYSTEM IS MULTI HOMED, THEN YOU'LL HAVE TO FIGURE
      //OUT WHAT INDX LINKS TO THE ETHERNET CONNECTION YOU WANT
      IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
      IPAddress ipAddress = ipHostInfo.AddressList[NIC_Index];
      IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, Port);
      _IPListeningOn = ipAddress.ToString();

      OnStatusNotification(STATUS_IPADRESS_STRING + ipAddress.ToString() + STATUS_PORT_STRING + Port.ToString());
      ServerSocket.Bind(ipLocalEndPoint);                                 //STRAP IT DOWN
      ServerSocket.Listen(0);                                             //NO SOCKET LEFT BEHIND
      ServerSocket.BeginAccept(AcceptConnectionCallback, ServerSocket);   //GO HERE WHEN SOMEONE KNOCKS
    }

    /// <summary>
    /// Start Listening on the specified ip address and port.
    /// "I had to start it somewhere, so I started there."
    /// </summary>
    /// <param name="ipString"></param>
    /// <param name="nPort"></param>
    public void Start(string ipString, int nPort)
    {
      byte[] bytes = new Byte[2048];
      _Port = nPort;
      IPAddress ipAddress = IPAddress.Parse(ipString);
      IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, Port);

      OnStatusNotification("SERVER LISTENING AT: " + ipAddress.ToString() + " PORT: " + Port.ToString());
      ServerSocket.Bind(ipLocalEndPoint);                         //STRAP IT DOWN
      _IPListeningOn = ipString;
      ServerSocket.Listen(0);                                     //NO SOCKET LEFT BEHIND
      ServerSocket.BeginAccept(AcceptConnectionCallback, null);   //GO HERE WHEN SOMEONE KNOCKS
    }

    #endregion

    #region CALL BACK FUNCTIONALITY

    /// <summary>
    /// Call Back that is fired when someone is connecting
    /// </summary>
    /// <param name="AR"></param>
    private void AcceptConnectionCallback(IAsyncResult AR)
    {
      ClientStateObject state = new ClientStateObject();

      try
      {
        state.workSocket = ServerSocket.EndAccept(AR);                          //TRANSFER FROM LISTENER
                                                                                //      state.workSocket = (Socket)AR;                                          //TRANSFER FROM LISTENER
        state.SocketEndpoint = state.workSocket.RemoteEndPoint as IPEndPoint;   //UPDATE ID
        state.TimeTag = DateTime.Now;
      }

      catch (ObjectDisposedException)
      {
        return;
      }
      //SEND THIS OFF TO READ
      state.workSocket.BeginReceive(state.buffer, 0, ClientStateObject.BUFFER_SIZE, 0, new AsyncCallback(ReadDataCallback), state);
      ClientList.Add(state);                                               //ADD THIS TO THE CLIENTLIST PILE
      OnConnectionNotification(state);
      OnStatusNotification(CLIENTLIST_COUNT_STRING + ClientList.Count.ToString());
      ServerSocket.BeginAccept(AcceptConnectionCallback, null);            //SEND THE SERVER SOCKET BACK TO BEGIN LISTENING AGAIN
    }


    /// <summary>
    /// Read the data 
    /// </summary>
    /// <param name="AR"></param>
    private void ReadDataCallback(IAsyncResult Ar)
    {
      String content = String.Empty;
      ClientStateObject state = (ClientStateObject)Ar.AsyncState;
      Socket socketToClient = state.workSocket;
      String ClientAddressString = GetIPAddressString(socketToClient);

      try
      {
        //TRY TO READ DATA FROM THE CLIENT SOCKET
        int bytesRead = socketToClient.EndReceive(Ar);
        state.Transaction = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);

        if (bytesRead > 0)
        {
          //THERE MIGHT BE MORE DATA - SO STORE WHAT WE'VE RECEIVED SO FAR
          state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
          content = state.sb.ToString();

          //CHECK AND SEE IF WE'VE RECEIVED THE <TERMINATOR>. SERIOUSLY IT NOTES THE END OF LINE OF THIS SPECIFIC DATA
          if (content.IndexOf(TERMINATOR) > -1)
          {
            //ALL DATA HAS BEEN READ
            state.Transaction = content;                        //ACCUMULATED DATA STRING
            state.TimeTag = DateTime.Now;                       //THIS HAPPENNED .... NOW.
            state.sb.Clear();                                   //CLEAR OUT THE STRING BUILDER

            //START LISTENING AGAIN
            socketToClient.BeginReceive(state.buffer, 0, ClientStateObject.BUFFER_SIZE, 0, new AsyncCallback(ReadDataCallback), state);
            OnMyDataReceivedNotification(state);                //LET ALL SUBSCRIBERS KNOW - NOTE THAT THIS GUY KILLS THE THREAD OF EXECUTION
                                                                //AND NEEDS TO BE THE LAST CALL - LOCATED HERE
          }
          else   //WE DIDNT FIND THE TERMINATOR SO READ AGAIN
          {
            socketToClient.BeginReceive(state.buffer, 0, ClientStateObject.BUFFER_SIZE, 0, new AsyncCallback(ReadDataCallback), state);
          }
        }
        else
        {  //NO DATA WAS THERE - MUST BE DISCONNECTED - CLEANT UP THE MESS AND START ACCEPTING AGAIN
          socketToClient.Close();
          ClientList.Remove(state);                               //REMOVE CLIENTSTATEOBJECT FROM CLIENTLIST
          state.TimeTag = DateTime.Now;
          OnConnectionNotification(state);
          OnStatusNotification(CLIENTLIST_COUNT_STRING + ClientList.Count.ToString());
          ServerSocket.BeginAccept(AcceptConnectionCallback, null);
        }

      }
      catch (SocketException)
      {
        // Don't shutdown because the socket may be disposed and its disconnected anyway.
        socketToClient.Close();
        ClientList.Remove(state);                                  //REMOVE CLIENTSTATEOBJECT FROM CLIENTLIST
        state.TimeTag = DateTime.Now;
        OnConnectionNotification(state);
        return;
      }

      //CHECK TO SEE IF THE DATA HOLDS A <EXIT> COMMAND. IF SO, DISCONNECT GRACEFULLY
      if (content.ToUpper().IndexOf(EXIT) > -1)                     // Client wants to exit 
      {
        // Always Shutdown before closing
        state.TimeTag = DateTime.Now;
        socketToClient.Shutdown(SocketShutdown.Both);
        socketToClient.Close();
        ClientList.Remove(state);                               //REMOVE CLIENTSTATEOBJECT FROM CLIENTLIST
        OnConnectionNotification(state);
        return;
      }
      // }//IF CONNECTED
    }

    /// <summary>
    /// Broadcast message to all clients
    /// </summary>
    /// <param name="ar"></param>
    private void OnSendMessageCallBack(IAsyncResult ar)
    {
      try
      {
        Socket client = (Socket)ar.AsyncState;
        client.EndSend(ar);
        //DO WE NEED TO REQUEST THAT THIS THING BEGINS LISTENING AGAIN? - GUESS NOT.
      }

      catch (Exception ex)
      {
        //LET EVERYONE KNOW
        OnStatusNotification("EXCEPTION: SendCallBack Function: " + ex.ToString());
        //SPEW TO UI
        Console.WriteLine("Exception Thrown in TcpServer  HandleClientRequest Function:" + ex.ToString());
      }

    }

    #endregion

    #region SEND FUNCTIONS
    /// <summary>
    /// Send data function - FRONT END FUNCTION
    /// </summary>
    /// <param name="ClientIndex"></param>
    /// <param name="szData"></param>
    public void SendData(int ClientIndex, string szData)
    {
      ClientStateObject state = (ClientStateObject)ClientList[ClientIndex];
      if (true == SendWithTerminator)
      {
        state.Transaction = szData + Terminator;
      }
      else
      {
        state.Transaction = szData;
      }

      SendData(state);
    }

    /// <summary>
    /// Send data function - ASYNCHRONOUS
    /// </summary>
    /// <param name="tcpClient"></param>
    /// <param name="szData"></param>
    public void SendData(ClientStateObject targetClient)
    {
      if (true == SendWithTerminator)
      {
        targetClient.Transaction += Terminator;
      }
      targetClient.TimeTag = DateTime.Now;
      OnDataSentNotification(targetClient);                                 //NOTIFY
      var databytes = Encoding.ASCII.GetBytes(targetClient.Transaction);    //CONVERT TO BYTE ARRAY

      //MAKE SURE WE ARE CONNECTED STILL 
      targetClient.workSocket.BeginSend(databytes, 0, databytes.Length, 0, new AsyncCallback(OnSendMessageCallBack), targetClient.workSocket);
    }

    #endregion

    #region HELPER FUNCTIONS SECTION

    /// <summary>
    /// Returns all the NETWORK INTERFACE CONNECTION ADDRESSES
    /// </summary>
    /// <returns></returns>
    public IPAddress[] GetAllActiveSystemNICS()
    {
      IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
      return ipHostInfo.AddressList;
    }


    /// <summary>
    /// Simply returns the address of the server socket as a string
    /// </summary>
    /// <returns></returns>
    public string GetServerAddress()
    {
      String retString = "UNKNOWN";
      retString = GetIPAddressString(ServerSocket);
      return retString;
    }

    /// <summary>
    /// Close all the sockets that are in the client list and then
    /// close the server socket
    /// </summary>
    public void CloseAllSockets()
    {
      String ClientAddressString = String.Empty;
      try
      {
        foreach (ClientStateObject state in ClientList)
        {
          state.TimeTag = DateTime.Now;
          state.workSocket.Shutdown(SocketShutdown.Both);
          state.workSocket.Close();
          ClientList.Remove(state);
          OnConnectionNotification(state);
        }
      }
      catch (Exception)
      {

      }
      ServerSocket.Close();                                    //SHUT DOWN MAIN SERVER SOCKET
      OnStatusNotification("All Sockets Closed");
    }

    /// <summary>
    /// Generates a Simple Time Stamp
    /// 2017/12/25 13:15:59.345
    /// </summary>
    /// <returns></returns>
    public string GenerateTimeStamp()
    {
      //timeStamp = DateTime.Now.ToString() + ": ";
      string timeStamp;
      DateTime Now = DateTime.Now;
      timeStamp = Now.Year.ToString() + SLASH + Now.Month.ToString("D2") + SLASH + Now.Day.ToString("D2") + SPACE + SPACE;
      timeStamp += "[" + Now.Hour.ToString("D2") + COLON + Now.Minute.ToString("D2") + COLON + Now.Second.ToString("D2") + PERIOD;
      timeStamp += Now.Millisecond.ToString("D3") + "]" + SPACE + COLON + SPACE;
      return timeStamp;
    }

    /// <summary>
    /// Makes a formatted string from a DateTime object
    /// 2017/12/25 13:15:59.345
    /// </summary>
    /// <param name="Now"></param>
    /// <returns></returns>
    public string GenerateTimeStamp(DateTime Now)
    {
      string timeStamp;
      timeStamp = Now.Year.ToString() + SLASH + Now.Month.ToString("D2") + SLASH + Now.Day.ToString("D2") + SPACE + SPACE;
      timeStamp += "[" + Now.Hour.ToString("D2") + COLON + Now.Minute.ToString("D2") + COLON + Now.Second.ToString("D2") + PERIOD;
      timeStamp += Now.Millisecond.ToString("D3") + "]" + SPACE + COLON + SPACE;
      return timeStamp;
    }

    /// <summary>
    /// Returns the IPAddress string
    /// </summary>
    /// <param name="mySocket"></param>
    /// <returns></returns>
    public string GetIPAddressString(Socket mySocket)
    {
      IPEndPoint remoteEndpoint = null;
      IPEndPoint localEndpoint = null;
      string retString = "UNDEFINED";

      try { localEndpoint = mySocket.LocalEndPoint as IPEndPoint; }          //SEE WHAT 
      catch (Exception) { }

      try { remoteEndpoint = mySocket.RemoteEndPoint as IPEndPoint; }
      catch (Exception) { }

      if (remoteEndpoint != null) { retString = remoteEndpoint.Address.ToString(); }

      if (localEndpoint != null) { retString = localEndpoint.Address.ToString(); }

      return retString;                                                   //SEND BACK STRING
    }

    /// <summary>
    ///  Returns the clientstateobject from the pile
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public ClientStateObject GetClientStateObject(int idx)
    {
      return (ClientStateObject)ClientList[idx];
    }

    /// <summary>
    /// Returns the string of the endpoint
    /// </summary>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public string GetIPAddressString(IPEndPoint endPoint)
    {
      return endPoint.ToString();
    }

    /// <summary>
    /// Returns the address string of the requested client
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public string GetClientIPAddress(int idx)
    {
      IPEndPoint remoteEndpoint = null;
      IPEndPoint localEndpoint = null;
      string retString = "UNDEFINED";

      if (idx < ClientList.Count)
      {
        ClientStateObject state = (ClientStateObject)ClientList[idx];

        //try checking if this is a LocalEndpoint - catch exception if not
        try { localEndpoint = state.workSocket.LocalEndPoint as IPEndPoint; }
        catch (SocketException) { }
        //try checking if this is a RemotEndpoint - catch exception if not
        try { remoteEndpoint = state.workSocket.RemoteEndPoint as IPEndPoint; }
        catch (SocketException) { }


        if (remoteEndpoint != null) { retString = remoteEndpoint.Address.ToString(); }

        if (localEndpoint != null) { retString = localEndpoint.Address.ToString(); }
      }

      return retString;                                            //SPEW BACK THE STRING 
    }

    #endregion

    #region SERVER EVENT SECTION

    /// <summary>
    /// Simple Data Received event generator
    /// </summary>
    /// <param name="szData"></param>
    protected virtual void OnMyDataReceivedNotification(ClientStateObject clientOBJ)
    {
      // if(clientOBJ.Transaction.Contains("DISCONNECT"))  //TRY READING - USED FOR DISCONNECTION
      // {
      // int idx = 0;
      // idx++;
      // NEED THIS IN HERE TO DETERMINE IF THE CLIENT DISCONNECTED- NOT SURE WHY THIS WORKS.
      // IT MUST HAVE SOMETHING TO DO WITH THE ACT OF READING FROM A DEAD SOCKET 
      // }          


      if (DataReceivedNotification != null)
      {

        DataReceivedNotification(clientOBJ);                    //SEND IT
      }
    }

    /// <summary>
    /// Simple Data SENT event generator
    /// </summary>
    /// <param name="szData"></param>
    protected virtual void OnDataSentNotification(ClientStateObject clientOBJ)
    {
      if (DataSentNotification != null)
      {
        DataSentNotification(clientOBJ);                       //SEND IT
      }
    }

    /// <summary>
    /// Let me know if we've got a client connected
    /// </summary>
    /// <param name="flag"></param>
    protected virtual void OnConnectionNotification(ClientStateObject clientOBJ)
    {
      if (ConnectionNotification != null)                     //IF WE HAVE LISTENERS TO THIS EVENT
      {
        ConnectionNotification(clientOBJ, ClientList.Count);    //SEND IT
      }
    }

    /// <summary>
    /// All this does is send out STATUS data 
    /// STATUS data concerns the server status- not connection info
    /// </summary>
    /// <param name="szData"></param>
    public virtual void OnStatusNotification(string szData)
    {
      if (StatusNotification != null)         //If someone is listening to this event
      {
        //string szOut = GenerateTimeStamp() + SPACE + STATUS + szData;
        string szOut = STATUS + szData;
        StatusNotification(szOut);                            //SEND IT
      }
    }

    #endregion

  }//CLASS
}
