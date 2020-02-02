using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    Platform,
    Flight,
    GameOver,
    Transition
}

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameState State { get; private set; } = GameState.Menu;

    public float PlatformPlayTime = 180; // seconds
    public float FlightPlayTime = 300;

    public PlatformingPlayer PlatformingPlayer1;
    public PlatformingPlayer PlatformingPlayer2;

    public GameObject Map;

    public SpaceShip SpaceShipP1;
    public SpaceShip SpaceShipP2;

    public SpaceBattleController SpaceBattleController;

    public AudioSource MusicSource;
    public AudioClip MusicMenu;
    public AudioClip MusicPlatform;
    public AudioClip MusicFlight;

    private Camera camera;
    private CameraPan cameraPan;
    private float timeTaken = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            cameraPan = GetComponent<CameraPan>();
            camera = Camera.main;
            State = GameState.Menu;
            PlatformingPlayer1.enabled = false;
            PlatformingPlayer2.enabled = false;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        // For now until we have start menu
        //State = GameState.Platform;
        //MusicSource.clip = MusicPlatform;
        //MusicSource.Play();
    }

    private void Update()
    {
        if (State == GameState.Transition) return;

        if (State == GameState.Menu)
        {
            if (Input.anyKey)
            {
                State = GameState.Transition;
                cameraPan.SetState(TransitionState.FromMenu, BeginPlatform);
                StartCoroutine(ChangeAudioclipOnEnd(MusicPlatform));
            }
        }

        if (State == GameState.Platform)
        {
            timeTaken += Time.deltaTime;
            if (timeTaken > PlatformPlayTime)
            {
                cameraPan.SetState(TransitionState.FromPlatformer, BeginFlight);
                State = GameState.Transition;
                StartCoroutine(ChangeAudioclipOnEnd(MusicFlight));
            }
        }
    }

    void BeginPlatform()
    {
        PlatformingPlayer1.enabled = true;
        PlatformingPlayer2.enabled = true;
        State = GameState.Platform;
    }

    void BeginFlight()
    {
        State = GameState.Flight;
        SpaceShipP1.BasePart.SetPilotsInside();
        SpaceShipP2.BasePart.SetPilotsInside();
        Destroy(Map);
        Destroy(PlatformingPlayer1.gameObject);
        Destroy(PlatformingPlayer2.gameObject);
        StartCoroutine(SpaceShipeRiseCoroutine());
    }

    IEnumerator SpaceShipeRiseCoroutine()
    {
        while (true)
        {
            var p1 = new Vector3(SpaceShipP1.transform.position.x, camera.transform.position.y, 0);
            var p2 = new Vector3(SpaceShipP2.transform.position.x, camera.transform.position.y, 0);
            SpaceShipP1.transform.position = Vector3.Lerp(SpaceShipP1.transform.position, p1, 0.1f);
            SpaceShipP2.transform.position = Vector3.Lerp(SpaceShipP2.transform.position, p2, 0.1f);
            print(p1.y - camera.transform.position.y);
            if (Mathf.Abs(p1.y - SpaceShipP1.transform.position.y) < 0.5f)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        State = GameState.Flight;
        SpaceShipP1.IsFunctional = true;
        SpaceShipP2.IsFunctional = true;
        var p1par = SpaceShipP1.transform.parent;
        var p2par = SpaceShipP2.transform.parent;
        SpaceShipP1.transform.parent = null;
        SpaceShipP2.transform.parent = null;
        Destroy(p1par.gameObject);
        Destroy(p2par.gameObject);
        SpaceBattleController.transform.position = camera.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
        SpaceBattleController.gameObject.SetActive(true);
    }

    IEnumerator ChangeAudioclipOnEnd(AudioClip newClip)
    {
        var timeLeft = MusicSource.clip.length - MusicSource.time;
        yield return new WaitForSecondsRealtime(timeLeft);
        MusicSource.clip = newClip;
        MusicSource.Play();
    }
}
