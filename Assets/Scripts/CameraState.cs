using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraState : MonoBehaviour
{

    public BoardControllerBase boardController;

    public CharacterState characterState;
    public ChaseCamera chaseCamera;
    // Start is called before the first frame update
    void Start()
    {
        chaseCamera = GetComponent<ChaseCamera>();
        boardController.PlayerBailed += BoardController_PlayerBailed;
        boardController.PlayerRespawned += BoardController_PlayerRespawned;
    }

    private void BoardController_PlayerRespawned(object sender, System.EventArgs e)
    {
        chaseCamera.ChaseObject = boardController.gameObject;
    }

    private void BoardController_PlayerBailed(object sender, BailEventArgs e)
    {
        chaseCamera.ChaseObject = e.RagdollInstance;
    }



    

}
