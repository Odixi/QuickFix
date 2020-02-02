using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Intro,
    Menu,
    Platform,
    Flight,
    Winner,
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
    public GameObject RedWin;
    public GameObject BlueWin;
    public GameObject GameOver;

    public SpaceShip SpaceShipP1;
    public SpaceShip SpaceShipP2;

    public SpaceBattleController SpaceBattleController;

    public AudioSource MusicSource;
    public AudioClip MusicMenu;
    public AudioClip MusicPlatform;
    public AudioClip MusicFlight;

    public GameObject IntroGameObject;
    public Animator IntroAnimator;
    public SpriteRenderer IntroCharacter;
    public Sprite IntroCharacterMouthOpen;
    public Animator IntroTextAnimator;
    public GameObject PressStartButton;

    public Team Winner = Team.Red;

    private Camera camera;
    private CameraPan cameraPan;
    private float timeTaken = 0;
    private bool anyKeyUp = true;

    public enum Team
    {
        Red,
        Blue
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            cameraPan = GetComponent<CameraPan>();
            camera = Camera.main;
            State = GameState.Intro;
            PlatformingPlayer1.enabled = false;
            PlatformingPlayer2.enabled = false;
            IntroAnimator.speed = 0;
            IntroTextAnimator.speed = 0;
            IntroTextAnimator.gameObject.SetActive(false);
            PressStartButton.SetActive(false);
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

        if (State == GameState.Intro)
        {
            StartIntro();
            State = GameState.Transition;
        }

        if (State == GameState.Menu)
        {
            if (Input.anyKey)
            {
                State = GameState.Transition;
                StartCoroutine(EndMenu());
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

        if (State == GameState.Flight)
        {
            timeTaken += Time.deltaTime;
            if (timeTaken > FlightPlayTime)
            {
                cameraPan.SetState(TransitionState.FromFlight, BeginGameOver);
                State = GameState.Transition;
                StartCoroutine(ChangeAudioclipOnEnd(MusicMenu));
            }
        }

        if (Input.anyKey == false) anyKeyUp = true;
        if (State == GameState.GameOver)
        {
            timeTaken += Time.deltaTime;
            GameOver.SetActive(true);
            if (Input.anyKey && anyKeyUp && timeTaken > 2) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else GameOver.SetActive(false);

        if (State == GameState.Winner)
        {
            timeTaken += Time.deltaTime;
            RedWin.SetActive(Winner == Team.Red);
            BlueWin.SetActive(Winner == Team.Blue);
            if (Input.anyKey && anyKeyUp && timeTaken > 2) BeginGameOver();
        }
        else
        {
            BlueWin.SetActive(false);
            RedWin.SetActive(false);
        }
    }

    public void PlayerWin(Team winningTeam)
    {
        timeTaken = 0;
        Winner = winningTeam;
        State = GameState.Winner;
    }

    void BeginGameOver()
    {
        timeTaken = 0;
        State = GameState.GameOver;
    }

    void StartIntro()
    {
        IntroAnimator.speed = 2;
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1.5f); // Wait for Intro animation
        cameraPan.SetState(TransitionState.ToMenu, delegate { });
        StartCoroutine( MoveFromToIn(IntroAnimator.transform, IntroAnimator.transform.position, new Vector3(6.74f, 13.9f, 0), 2.4f));
        yield return new WaitForSeconds(3f);
        IntroTextAnimator.gameObject.SetActive(true);
        IntroTextAnimator.speed = 0.5f;
        yield return new WaitForSeconds(1.5f);
        PressStartButton.SetActive(true);
        State = GameState.Menu;

    }

    IEnumerator MoveFromToIn(Transform trans, Vector3 from, Vector3 to, float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            trans.position = Vector3.Lerp(from, to, Mathf.Min( t / time, 1));
            yield return null;
        }
    }

    IEnumerator EndMenu()
    {
        IntroAnimator.SetBool("Crush", true);
        IntroAnimator.speed = 10;
        IntroCharacter.sprite = IntroCharacterMouthOpen;
        yield return new WaitForSeconds(1);
        cameraPan.SetState(TransitionState.FromMenu, BeginPlatform);
    }

    void BeginPlatform()
    {
        Destroy(IntroGameObject);
        PlatformingPlayer1.enabled = true;
        PlatformingPlayer2.enabled = true;
        State = GameState.Platform;
    }

    void BeginFlight()
    {
        timeTaken = 0;
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
