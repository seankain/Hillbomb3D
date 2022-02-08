using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    Menu,
    AttractMode,
    Playing
}
public class GameStateManager : MonoBehaviour
{

    public GameState CurrentState { get; private set; }
    [SerializeField]
    private Crossfader crossFader;
    private float MenuIdleMaxTime = 10f;
    private float AttractModeTime = 40f;
    private float currentStateElapsedTime = 0;
    private float idleTime = 0;
    [SerializeField]
    private Camera AttractModeCamera;
    [SerializeField]
    private Camera MenuCamera;
    [SerializeField]
    private BotRiderController demoBot;
    private ChunkCycler chunkCycler;


    private void Awake()
    {
        chunkCycler = FindObjectOfType<ChunkCycler>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //TransitionToMenu();
        demoBot.PlayerBailed += DemoBot_PlayerBailed;
        crossFader.FadeOut();
        CurrentState = GameState.Menu;
        AttractModeCamera.gameObject.SetActive(false);
        MenuCamera.gameObject.SetActive(true);
        crossFader.FadeIn();
        
    }

    private void DemoBot_PlayerBailed(object sender, BailEventArgs e)
    {
        StartCoroutine(BotBailedTransitionCoroutine());
    }

    private IEnumerator BotBailedTransitionCoroutine()
    {
        var elapsed = 0f;
        while(elapsed < 5f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        TransitionToMenu();
    }

    // Update is called once per frame
    void Update()
    {
        currentStateElapsedTime += Time.deltaTime;
        if (Input.anyKeyDown)
        {
            idleTime = 0;
            if(CurrentState == GameState.AttractMode)
            {
                idleTime = 0;
                currentStateElapsedTime = 0;
                TransitionToMenu();
            }
        }
        else { idleTime += Time.deltaTime; }
        switch (CurrentState)
        {
            case (GameState.Menu):
                if(idleTime > MenuIdleMaxTime)
                {
                    idleTime = 0;
                    currentStateElapsedTime = 0;
                    TransitionToAttractMode();
                }
                break;
            case (GameState.AttractMode):
                if(idleTime > AttractModeTime)
                {
                    idleTime = 0;
                    currentStateElapsedTime = 0;
                    TransitionToMenu();
                }
                break;
            case (GameState.Playing):
                idleTime = 0;
                currentStateElapsedTime = 0;
                break;
            default:
                break;
        }
    }


    private void TransitionToMenu() 
    {
        CurrentState = GameState.Menu;
        Debug.Log("Transitioning to menu");
        crossFader.FadeOut();
        chunkCycler.ResetChunks();
        AttractModeCamera.gameObject.SetActive(false);
        MenuCamera.gameObject.SetActive(true);
        demoBot.gameObject.SetActive(false);
        crossFader.FadeIn();
    }
    private void TransitionToAttractMode()
    {
        CurrentState = GameState.AttractMode;
        Debug.Log("Transitioning to attract mode");
        crossFader.FadeOut();
        MenuCamera.gameObject.SetActive(false);
        AttractModeCamera.gameObject.SetActive(true);
        demoBot.gameObject.SetActive(true);
        demoBot.Respawn();
        crossFader.FadeIn();
    }

    private void TransitionToPlaying()
    {
        CurrentState = GameState.Playing;
    }



}
