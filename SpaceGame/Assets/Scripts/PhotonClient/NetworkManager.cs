using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Net.Sockets;
using UnityEngine.UI;

public class NetworkManager : Photon.MonoBehaviour
{
    [SerializeField]
    public Text[] txtTeam;

    [SerializeField]
    Text connectionText;
    [SerializeField]
    Transform[] spawnPoints;
    [SerializeField]
    Camera sceneCam;
    [SerializeField]
    GameObject serverWindow;

    [SerializeField]
    Material[] materials;

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
    ObjectPool bulletPools;

    Dictionary<int, GameObject> players;
    
    //public GameObject spawnEnemyPreab;
    //public GameObject goblinSpawn;
    public byte version = 1;

    public Rect guiRect = new Rect(0, 0, 250, 300);
    public bool isVisible = true;
    public bool alignedBottom = false;

    private string inputLine = "";
    private Vector2 scrollPos = Vector2.zero;
    Color target = Color.green;
    PlayerController playerController;
    GameObject playerObject;

    void Start()
    {
        if(this.alignedBottom)
        {
            this.guiRect.y = Screen.height - this.guiRect.height;
        }
        photonView = GetComponent<PhotonView>();
        messages = new Queue<string>(messageCount);

        players = new Dictionary<int, GameObject>();

        PhotonNetwork.ConnectUsingSettings("0.1");
        StartCoroutine("UpdateConnectionString");
        StartCoroutine(MyCoroutine());
        //playerObject = GameObject.Find("Player1");
        //if (playerObject)
        //{
        //    Debug.Log("Found " + playerObject.name);
        //    playerController = playerObject.GetComponent<PlayerController>();
        //}
        //else
        //{
        //    Debug.Log("Not Found.");
        //}
        
    }

    IEnumerator MyCoroutine()
    {
        yield return new WaitForSeconds(3f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
    {
        if (stream.isWriting)
        {
            stream.SendNext(txtTeam[0].text);
            stream.SendNext(txtTeam[1].text);
            stream.SendNext(txtTeam[2].text);
            stream.SendNext(txtTeam[3].text);
        }
        else if (stream.isReading)
        {
            txtTeam[0].text = (string)stream.ReceiveNext();
            txtTeam[1].text = (string)stream.ReceiveNext();
            txtTeam[2].text = (string)stream.ReceiveNext();
            txtTeam[3].text = (string)stream.ReceiveNext();
        }
    }

    public void OnGUI()
    {
        if(!this.isVisible || PhotonNetwork.connectionStateDetailed != ClientState.Joined)
        {
            return;
        }

        if(Event.current.type == EventType.keyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if(!string.IsNullOrEmpty(this.inputLine))
            {
                this.photonView.RPC("ChatLine", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
                return;
            }
            else
            {
                GUI.FocusControl("ChatInput");
            }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.guiRect);
        if(guiRect.Contains(Event.current.mousePosition))
        {
            if(Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 0;
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 1;
            }
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.FlexibleSpace();

        foreach(string message in messages)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;

            GUILayout.Label(message, style);
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        inputLine = GUILayout.TextField(inputLine);

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    [PunRPC]
    public void ChatLine(string newLine, PhotonMessageInfo info)
    {
        string senderName = "anonymous";
        if (info != null && info.sender != null)
        {
            if (!string.IsNullOrEmpty(username.text))
            {
                senderName = info.sender.name;
            }
            else
            {
                senderName = "player " + info.sender.ID;
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
            connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();//connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
            yield return null;
        }
    }
    public void OnJoinedLobby()
    {
        //serverWindow.SetActive(true);
        Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");
        JoinRoom();
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
        PhotonNetwork.player.name = "Player";//username.text; //spawn player's name
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = 4
        };
        PhotonNetwork.JoinRandomRoom();
        PhotonNetwork.JoinOrCreateRoom(roomName.text, roomOptions, TypedLobby.Default);
    }

    public void OnJoinedRoom()
    {
        //serverWindow.SetActive(false);
        StopCoroutine("UpdateConnectionString");
        connectionText.text = "";
        StartSpawnProcess(0f);
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
        //SpawnPlayer(3f);
        //SpawnEnemy();
    }
    void StartSpawnProcess(float respawnTime)
    {
        sceneCam.enabled = true;
        StartCoroutine("SpawnPlayer", respawnTime);
    }
    IEnumerator SpawnPlayer(float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);

        int index = Random.Range(0, spawnPoints.Length);
        player = PhotonNetwork.Instantiate("Player1",
                                spawnPoints[index].position,
                                spawnPoints[index].rotation, 0);
        player.GetComponent<PlayerNetwork>().RespawnMe += StartSpawnProcess;
        player.GetComponent<PlayerNetwork>().SendNetworkMessage += AddLine;
        int id = PhotonNetwork.countOfPlayers;
        string type = "";
        switch((PlayerController.Type)id)
        {
            case PlayerController.Type.Leader:
                player.AddComponent<Leader>();
                type = "Leader";
                player.GetComponent<Renderer>().material = materials[0];
                break;
            case PlayerController.Type.Healer:
                type = "Healer";
                player.AddComponent<Healer>();
                player.GetComponent<Renderer>().material = materials[1];
                break;
            case PlayerController.Type.Defender:
                type = "Defender";
                player.AddComponent<Defender>();
                player.GetComponent<Renderer>().material = materials[2];
                break;
            case PlayerController.Type.Heavy:
                type = "Heavy";
                player.AddComponent<Heavy>();
                player.GetComponent<Renderer>().material = materials[3];
                break;
        }
        PhotonNetwork.player.name = type;
        player.GetComponent<PlayerController>().playerType = (PlayerController.Type)PhotonNetwork.countOfPlayers;
        players.Add(PhotonNetwork.countOfPlayers, player);
        Debug.Log("Number of players " + PhotonNetwork.countOfPlayers);
        sceneCam.enabled = false;
        txtTeam[id - 1].text = type + " " + player.GetComponent<PlayerNetwork>().GetHealth().ToString();
        txtTeam[id - 1].enabled = true;
        /*AddMessage("Joined player: " + PhotonNetwork.player.name);*/
        AddLine(PhotonNetwork.player.name + " has joined the room as " + type.ToString());
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
        if (PhotonNetwork.isMasterClient)
        {
            //PhotonNetwork.Instantiate(GoblinSpawner.name, GoblinSpawner.transform.position, Quaternion.identity, 0);
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
