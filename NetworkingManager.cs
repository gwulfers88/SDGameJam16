using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class NetworkingManager : Photon.MonoBehaviour
{
    [SerializeField]
    Text connectionText;
    [SerializeField]
    Transform[] spawnPoints;
    [SerializeField]
    Camera sceneCamera;
    [SerializeField]
    GameObject serverWindow;

    //[SerializeField]
    public InputField username;
    [SerializeField]
    InputField roomName;
    [SerializeField]
    InputField roomList;
    [SerializeField]
    InputField messageWindow;

    Queue<string> messages;
    const int messageCount = 6;
    new PhotonView photonView;

    GameObject player;
    public GameObject spawnEnemyPrefab;
    public GameObject GoblinSpawner;
    public byte Version = 1;

    public Rect GuiRect = new Rect(0, 0, 250, 300);
    public bool IsVisible = true;
    public bool AlignBottom = false;
    //public List<string> messages = new List<string>();
    private string inputLine = "";
    private Vector2 scrollPos = Vector2.zero;
    Color target = Color.green;
    PlayerController playerController;
    GameObject playerObject;
    // Use this for initialization
    void Start()
    {
        
        if (this.AlignBottom)
        {
            this.GuiRect.y = Screen.height - this.GuiRect.height;
        }
        photonView = GetComponent<PhotonView>();
        messages = new Queue<string>(messageCount);
        
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        //spawnPlayer = GameObject.Find("Player1");
        //GoblinSpawner = GameObject.Find("GoblinSpawn");
        PhotonNetwork.ConnectUsingSettings("0.1");
        StartCoroutine("UpdateConnectionString");
        StartCoroutine(MyCoroutine());
        playerObject = GameObject.Find("Player1(Clone)");
        playerController = playerObject.GetComponent<PlayerController>();
        
        //PhotonNetwork.autoJoinLobby = false;
    }
    IEnumerator MyCoroutine()
    {
        yield return new WaitForSeconds(3f);
    }

    public void OnGUI()
    {
        
        if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
        {
            
            return;
        }

        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            
            if (!string.IsNullOrEmpty(this.inputLine))
            {
                
                this.photonView.RPC("ChatLine", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            }
            else
            {
                
                GUI.FocusControl("ChatInput");
            }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.GuiRect);
        if(GuiRect.Contains(Event.current.mousePosition))
        {
            if(Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 0;
                //Debug.Log("Disable mouse control over here!!");
            }
            
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 1;
                //Debug.Log("Disable mouse control over here!!");
            }
        }
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.FlexibleSpace();

        /* Defualt chat display system
        for (int i = messages.Count - 1; i >= 0; i--) // default script loop  this needed the change to for(int i =0; i < messages.[URL='http://unity3d.com/support/documentation/ScriptReference/30_search.html?q=Count']Count[/URL]; i++)
        {
            GUILayout.Label(messages[i]);
        }
        */

        foreach (string message in messages)
        {
            GUILayout.Label(message);
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatInput");
        inputLine = GUILayout.TextField(inputLine);

        /*    send button
        if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
        {
            this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
            this.inputLine = "";
            GUI.FocusControl("");
        }*/
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    [PunRPC]
    public void ChatLine(string newLine, PhotonMessageInfo mi)
    {

        string senderName = "anonymous";

        if (mi != null && mi.sender != null)
        {
            if (!string.IsNullOrEmpty(username.text))
            {
                senderName = mi.sender.name;
            }
            else
            {
                senderName = "player " + mi.sender.ID;
            }
        }

        this.messages.Enqueue("[" + senderName + "]" + ": " + newLine);
        if (messages.Count > 25)
        {
            messages.Dequeue();
        }
        scrollPos.y = 100000;
    }

    public void AddLine(string newLine)
    {
        this.messages.Enqueue(newLine);
    }
    IEnumerator UpdateConnectionString()
    {
        while (true)
        {
            connectionText.text=PhotonNetwork.connectionStateDetailed.ToString();//connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
            yield return null;
        }
    }
    public void OnJoinedLobby()
    {
        serverWindow.SetActive(true);
        Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");
    }
    void OnReceivedRoomListUpdate()
    {
        roomList.text = ""; //to leave the room and come back to lobby
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in rooms)
            roomList.text += room.name + "\n";
    }
    // Update is called once per frame
    //void Update()
    //{
    //    if (!PhotonNetwork.connected)
    //    {
    //        PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManager.GetActiveScene().buildIndex);
    //    }
    //}

    public virtual void OnConnectedToMaster()
    {
        if (PhotonNetwork.networkingPeer.AvailableRegions != null)
        {
            Debug.LogWarning("List of available regions counts " + PhotonNetwork.networkingPeer.AvailableRegions.Count + ". First: " + PhotonNetwork.networkingPeer.AvailableRegions[0] + " \t Current Region: " + PhotonNetwork.networkingPeer.CloudRegion);
        }

        //PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        //PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 4 }, null);
        PhotonNetwork.CreateRoom(null);
    }

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void JoinRoom()
    {
        PhotonNetwork.player.name = username.text; //spawn player's name
        RoomOptions roomOptions = new RoomOptions()
        {
            isVisible = true, maxPlayers = 10
        };
        PhotonNetwork.JoinRandomRoom();
        PhotonNetwork.JoinOrCreateRoom(roomName.text, roomOptions, TypedLobby.Default);
    }
    public void OnJoinedRoom()
    {
       serverWindow.SetActive(false);
        StopCoroutine("UpdateConnectionString");
        connectionText.text = "";
        StartSpawnProcess(0f);
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
        //SpawnPlayer();
        SpawnEnemy();
    }
    void StartSpawnProcess(float respawnTime)
    {
        sceneCamera.enabled = true;
        StartCoroutine("SpawnPlayer", respawnTime);
    }
    IEnumerator SpawnPlayer(float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);

        int index = Random.Range(0, spawnPoints.Length);
        player = PhotonNetwork.Instantiate("Player1", 
                                spawnPoints[index].position, 
                                spawnPoints[index].rotation, 0);
        player.GetComponent<PlayerNetworkMover>().RespawnMe += StartSpawnProcess;
        player.GetComponent<PlayerNetworkMover>().SendNetworkMessage += AddLine;
        sceneCamera.enabled = false;
        /*AddMessage("Joined player: " + PhotonNetwork.player.name);*/
        AddLine(PhotonNetwork.player.name + " has joined the room");
    }
    void AddMessage(string message)
    {
        photonView.RPC("AddMessage_RPC", PhotonTargets.All, message);

    }
   

    //void SpawnPlayer()
    //{
    //    PhotonNetwork.Instantiate(player.name, player.transform.position, Quaternion.identity, 0);
    //}
    void SpawnEnemy()
    {
        if(PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Instantiate(GoblinSpawner.name, GoblinSpawner.transform.position, Quaternion.identity, 0);
        }
    }
    
    [PunRPC]
    void AddMessage_RPC(string message)
    {
        messages.Enqueue(message);

        if (messages.Count > messageCount)
        {
            messages.Dequeue();
        }

        messageWindow.text = "";
        foreach (string m in messages)
        {
            messageWindow.text += m + "\n";
        }
    }
}
