using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{


    [Tooltip("Set the speed with which the walls can be moved")]
    public float moveSpeed;
    public Transform selectedWall { private set; get; }      // The wall to move



    internal static bool isWallMoving { private set; get; }  // Flag indicates whether the wall is being moved or not

    private bool allowMovement;                              // Flag indicates whether to allow walls movement or not
    private MovementDirections movementDirectionAllowed;     // Indicates which movement direction to allow for a wall      
    private PlayerController playerController;


    private enum MovementDirections
    {
        verticalMovement,
        horizontalMovement,
        none
    }


    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;



        if (Input.GetMouseButtonUp(0))
        {

            if(allowMovement == true) { allowMovement = false; isWallMoving = false; }

            else if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMasks.instance.wallLayerOnly))
            {

                // The current player should only move walls that are allowed for him
        
                string allowableWallsType = playerController.currentlySelectedPlayer.allowableWallToMove.ToString();
                if (!hitInfo.transform.tag.Equals(allowableWallsType)) { return; }


                BoardSetup.instance.EnableDisablePlacementArrows(true);

                allowMovement = true;
                isWallMoving = true;

                selectedWall = hitInfo.transform;
                Debug.Log("Selected wall name  " + selectedWall.name);

                //Debug.Log("Hit wall rotation is    " +hitInfo.transform.localEulerAngles);

                int yAngle = (int)hitInfo.transform.localEulerAngles.y;
               
            }
        }


                /*
                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMasks.instance.wallAndBlockLayer))
                    {
                        if (hitInfo.transform.gameObject.layer == LayerMasks.instance.wallsLayerNumber)
                        {
                            allowMovement = true;
                            isWallMoving  = true;

                            selectedWall  = hitInfo.transform;
                            Debug.Log("Selected wall name  " + selectedWall.name);

                            //Debug.Log("Hit wall rotation is    " +hitInfo.transform.localEulerAngles);

                            int yAngle = (int)hitInfo.transform.localEulerAngles.y;

                            if (Mathf.Abs(yAngle) == 90)
                            {
                                movementDirectionAllowed = MovementDirections.horizontalMovement;
                                Debug.Log("Allow horizontal movement only");
                            }

                            else if((Mathf.Abs(yAngle) == 0))
                            {
                                movementDirectionAllowed = MovementDirections.verticalMovement;
                                Debug.Log("Allow vertical movement only");
                            }

                            else
                            {
                                movementDirectionAllowed = MovementDirections.none;
                                allowMovement = false;
                                Debug.LogWarning("No movement is allowed for walls oriented at y: " +yAngle + " deg");
                            }


                        }

                        // Hit the block
                        else
                        {
                        }

                    }
                }


                if(Input.GetMouseButtonUp(0))
                {
                    allowMovement = false;
                    UtilityServices.instance.RunDelayedCommand(1f , ()=> { isWallMoving = false; });       
                }


                if(allowMovement)
                {

                    float mouseHorz = Input.GetAxis("Mouse X") * 10;
                    float mouseVert = Input.GetAxis("Mouse Y") * 10;

                    if (movementDirectionAllowed == MovementDirections.horizontalMovement)
                    {
                        //Debug.Log("Moving horizontally");
                        selectedWall.Translate( mouseHorz * moveSpeed  * Time.deltaTime,0 ,0 , Space.World);
                    } 

                    else if(movementDirectionAllowed == MovementDirections.verticalMovement)
                    {
                        //Debug.Log("Moving Vertically");
                        selectedWall.Translate(0, 0, mouseVert * moveSpeed * Time.deltaTime, Space.World);
                    }

                }
                */

            }
}
