using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsAvoidance : MonoBehaviour
{



    private void OnTriggerEnter(Collider triggeringCollider)
    {

        PlayerController playerMovement = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerController>();

        if(!playerMovement.currentlySelectedPlayer.GetHashCode().Equals(this.gameObject.GetHashCode())) { return; }

        if (triggeringCollider.gameObject.tag == "wall" || triggeringCollider.gameObject.tag == "Player")
        {
            Debug.Log("Collided with walls or other players. Getting back");;
            transform.position = playerMovement.previousPlayerPos;
            playerMovement.moveNow = false;

        }
    }
}
