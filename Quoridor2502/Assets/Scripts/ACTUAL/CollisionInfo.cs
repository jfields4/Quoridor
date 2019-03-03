using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInfo : MonoBehaviour
{


    private PlayerController playerController;
    private PlayerController.Players playerType;


    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerController>();

        if(this.gameObject.GetHashCode().Equals(playerController.player1.playerGameObject.GetHashCode())) { playerType = PlayerController.Players.Player1; }
        else { playerType = PlayerController.Players.Player2; }

    }

    private void OnTriggerEnter(Collider other)
    {
        // Hit the ground
        if(other.gameObject.layer == LayerMasks.instance.blocksLayerNumber)
        {
            playerController.StopJump(playerType);
        }
    }

}
