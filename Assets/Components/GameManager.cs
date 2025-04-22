using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TelemetrySystem;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager instance = null;

    public static GameManager Instance { get { return instance; } private set { } }

    bool savedGame = false;

    public bool SavedGame { get { return savedGame; } private set { savedGame = value; } }

    Animator sceneChanger = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            if(PlayerPrefs.GetInt("SavedGame") == 1)
            {
                savedGame = true;
            }

            checkpoints = new GameObject[numCheckpoints];
            for(int i = 0; i < numCheckpoints; ++i)
            {
                checkpoints[i] = null;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Nombres de Escenas y Jugadores")]
    [SerializeField]
    string MENU_NAME = "StartMenu";
    [SerializeField]
    public string PLAY_NAME = "GameScene";

    [SerializeField]
    public string CINEMATIC_NAME = "StartCinematic";

    [SerializeField]
    string PLAYER_1_NAME = "Marvin";
    [SerializeField]
    string PLAYER_2_NAME = "Bo";

    [SerializeField]
    private int numCheckpoints;

    private List<MovableObject> movableObjects = new List<MovableObject>();

    bool continuedGame = false;
    
    private GameObject[] checkpoints;

    private GameObject[] players;

    private GameObject tongue;

    private int numPlayers = 2;

    //Telemetry
    private bool beginingGameScene = true;

    public void OnSceneLoaded(Scene a, LoadSceneMode b)
    {
        if (SceneManager.GetActiveScene().name == PLAY_NAME)
        {
            players[0] = GameObject.Find("Marvin");
            players[1] = GameObject.Find("Bo");
           
            //Telemetry
            if (beginingGameScene)
            {
                Tracker.Instance.PushEvent(new LevelStartEvent(PLAY_NAME));
                beginingGameScene = false;
            }
            Tracker.Instance.TrackPersistentEvent(new PlayerPositionEvent(players[0].transform, 100));
            Tracker.Instance.TrackPersistentEvent(new PlayerPositionEvent(players[1].transform, 100));
        }
    }

    public void AddCheckpoint(GameObject checkpoint, int id)
    {
        checkpoints[id] = checkpoint;
    }

    public void ToggleFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void LoadPlayScene()
    {
        if (SceneManager.GetActiveScene().name == PLAY_NAME)
        {
            players[0] = GameObject.Find(PLAYER_1_NAME);
            players[1] = GameObject.Find(PLAYER_2_NAME);
            tongue = GameObject.Find("Tongue");
            if (continuedGame)
            {
                currentCheckpoint = PlayerPrefs.GetInt("LastCheckpoint");
            }
            else
            {
                currentCheckpoint = 0;
            }

            ResetPlayerPosition();
            Tracker.Instance.TrackPersistentEvent(new PlayerPositionEvent(players[0].transform, 100));
            Tracker.Instance.TrackPersistentEvent(new PlayerPositionEvent(players[1].transform, 100));
        }
    }

    public void ResetScene()
    {
        continuedGame = true;
        CheckpointComponent.ChangePositionOnLoad(true);
        PlayerPrefs.SetInt("LastCheckpoint", currentCheckpoint);
        sceneChanger.SetTrigger("FadeOut");
    }

    public void HideTongue()
    {
        tongue.SetActive(false);
    }

    private void ResetPlayerPosition()
    {
        for(int i = 0; i < numPlayers; ++i)
        {
            Transform position = checkpoints[currentCheckpoint].transform.GetChild(i);
            players[i].GetComponent<Transform>().SetPositionAndRotation(position.position, position.rotation);
        }
    }

    public void FreezePlayers(bool enable)
    {
        //Telemetry
        Tracker.Instance.PushEvent(new PlayerDeathEvent(players[0].transform.position));
        Tracker.Instance.PushEvent(new PlayerDeathEvent(players[1].transform.position));
        Tracker.Instance.StopTrackingPersistentEvent("PlayerPosition");

        for (int i = 0; i < numPlayers; ++i)
        {
            players[i].GetComponent<MovementComponent>().FreezePlayer(enable);
        }
    }

    public void LoadScene(string newScene)
    {
        if (newScene == PLAY_NAME || newScene == CINEMATIC_NAME) sceneChanger.SetTrigger("FadeIn");

        //Telemetry
        Tracker.Instance.PushEvent(new ChangeSceneEvent(SceneManager.GetActiveScene().name, newScene));

        SceneManager.LoadScene(newScene);
    }

    public void LoadCinematic(string newScene)
    {
        LoadScene(newScene);
    }

    public void LoadSceneNoFade(string newScene)
    {
        //Telemetry
        Tracker.Instance.PushEvent(new ChangeSceneEvent(SceneManager.GetActiveScene().name, newScene));

        SceneManager.LoadScene(newScene);
    }

    public void LoadPlaySceneFade()
    {
        sceneChanger.SetTrigger("FadeOut");
    }

    public void PlayersDie()
    {
        sceneChanger.SetTrigger("DieFadeOut");
    }

    public void LoadCredits()
    {
        LoadScene("Credits");
    }

    public void GameWasContinued()
    {
        continuedGame = true;
    }

    public void BackToMenu()
    {
        //Telemetry
        //Tracker.Instance.PushEvent(new LevelUnpauseEvent(PLAY_NAME));
        Tracker.Instance.PushEvent(new ChangeSceneEvent(SceneManager.GetActiveScene().name, MENU_NAME));
        beginingGameScene = true;

        SceneManager.LoadScene(MENU_NAME);
    }

    public void ExitApplication()
    {
        //Telemetry
        Tracker.Instance.PushEvent(new GameEndEvent());

        Application.Quit();
    }

    public void PauseGame()
    {
        // Hacer movidas con los objetos que tengan movimiento para prevenirlo. 
        // Probablemente haga falta una lista de objetos que tengan rigidbody o algo así,
        // que se metan a la lista y se guarde su estado antes de la pausa y se reanude

        foreach(MovableObject obj in movableObjects)
        {
            obj.Pause();
        }

        //Telemetry
        Tracker.Instance.PushEvent(new LevelPauseEvent(PLAY_NAME));
        Tracker.Instance.StopTrackingPersistentEvent("PlayerPosition");
    }

    public void ResumeGame()
    {
        // Lee PauseGame y te juro que lo entenderás
        foreach (MovableObject obj in movableObjects)
        {
            obj.Resume();
        }

        //Telemetry
        Tracker.Instance.PushEvent(new LevelUnpauseEvent(PLAY_NAME));
        Tracker.Instance.TrackPersistentEvent(new PlayerPositionEvent(players[0].transform, 100));
        Tracker.Instance.TrackPersistentEvent(new PlayerPositionEvent(players[1].transform, 100));
    }

    public void AddMovableObject(MovableObject newObject)
    {
        if (!movableObjects.Contains(newObject)) movableObjects.Add(newObject);
    }

    public void RemoveMovableObject(MovableObject delObject)
    {
        if(movableObjects.Contains(delObject)) movableObjects.Remove(delObject);
    }


    private int currentCheckpoint = 0;

    public int getCurrentCheckpoint()
    {
        return currentCheckpoint;
    }

    public void setCurrentCheckpoint(int checkpoint)
    {
        currentCheckpoint = checkpoint;
        PlayerPrefs.SetInt("LastCheckpoint", currentCheckpoint);
        PlayerPrefs.SetInt("SavedGame", 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new GameObject[numPlayers];
        sceneChanger = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == PLAY_NAME && Input.GetKeyDown(KeyCode.R))
        {
            //Telemetry
            Tracker.Instance.PushEvent(new LevelRestartEvent(PLAY_NAME, players[0].transform.position));
            Tracker.Instance.StopTrackingPersistentEvent("PlayerPosition");

            ResetScene();
        }
    }
}
