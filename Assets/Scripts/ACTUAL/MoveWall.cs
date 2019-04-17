using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gameboard = Board;
public class MoveWall : MonoBehaviour
{


    [Tooltip("Set the speed with which the walls can be moved")]
    public float moveSpeed;
    public Transform selectedWall { private set; get; }      // The wall to move
    NewLogic newLogic;
    public byte row;
    public char col;
    public Text text;
    public GameObject[] TopWall;
    int j = 0;
    int Counter2 = 0;

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
        newLogic = GameObject.Find("NewLogic").GetComponent<NewLogic>();
        playerController = GetComponent<PlayerController>();

    }

    // Update is called once per frame


    public void MoveFinalWall(Transform wallGameObject)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;



        if (newLogic.playerMove == true)
        {

            //			Debug.Log (wallGameObject.transform.position);
            if (allowMovement == true) { allowMovement = false; isWallMoving = false; }

            // else if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMasks.instance.wallLayerOnly))
            //		else if (wallGameObject.gameObject.layer==LayerMasks.instance.wallLayerOnly)

            //	{

            // The current player should only move walls that are allowed for him

            string allowableWallsType = playerController.currentlySelectedPlayer.allowableWallToMove.ToString();
            //			Debug.Log (allowableWallsType);

            //   if (!hitInfo.transform.tag.Equals(allowableWallsType)) { return; }
            if (!wallGameObject.transform.tag.Equals(allowableWallsType)) { return; }
            //   Debug.Log("Selected wall name  " + selectedWall.name);

            BoardSetup.instance.EnableDisablePlacementArrows(true);


            allowMovement = true;
            isWallMoving = true;

            //selectedWall = hitInfo.transform;
            selectedWall = wallGameObject;
            //	        MoveWallWithQuoridoorNation("b8","v");
            //			Debug.Log("Selected wall name  " + selectedWall.name);
            //			Debug.Log("Selected wall Postion  " + selectedWall.position);
            gameboard.Move selectedMove = new gameboard.Move((byte)(10 - row), (byte)(col - 64), 0);
            //Debug.Log("Move wall select move: " + selectedMove.Row + " " + selectedMove.Column);


            //	Debug.Log("Hit wall rotation is    " +hitInfo.transform.localEulerAngles);
            //			Debug.Log("Hit wall rotation is    " +wallGameObject.transform.localEulerAngles);
            int yAngle = (int)wallGameObject.transform.localEulerAngles.y;

            //	}





        }

        //	return wallGameObject;
    }














    void Update()
    {


        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //   RaycastHit hitInfo;



        // if (Input.GetMouseButtonUp(0))
        //	{


        //       if(allowMovement == true) { allowMovement = false; isWallMoving = false; }

        //     else if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMasks.instance.wallLayerOnly))
        //   {

        // The current player should only move walls that are allowed for him

        //     string allowableWallsType = playerController.currentlySelectedPlayer.allowableWallToMove.ToString();
        //    if (!hitInfo.transform.tag.Equals(allowableWallsType)) { return; }


        //                BoardSetup.instance.EnableDisablePlacementArrows(true);


        //              allowMovement = true;
        //            isWallMoving = true;

        //        selectedWall = hitInfo.transform;
        //          Debug.Log("Selected wall name  " + selectedWall.name);

        //Debug.Log("Hit wall rotation is    " +hitInfo.transform.localEulerAngles);

        //      int yAngle = (int)hitInfo.transform.localEulerAngles.y;

        //    }
        //}


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



    public void MoveWallWithQuoridorNotation(char col, int row, int angle)
    {
        for (j = Counter2; j < TopWall.Length; j++)
        {
            if (j == Counter2)
            {
                Counter2++;
                TopWall[j].gameObject.SetActive(true);
                j = Counter2;
            }
            else
            {
                TopWall[j].gameObject.SetActive(false);

            }

        }

        //Debug.Log(Counter2);

        //	GameObject selectedWall2=newLogic.wallLogic ();
        GameObject finDDot = GameObject.Find(col.ToString() + row.ToString());
        finDDot = GameObject.Find(col.ToString() + row.ToString());
        //Debug.Log("gameobjectname" + col.ToString() + row.ToString());

        Debug.Log("Wall angle " + angle);
        if (angle == -1)
        {
            //			Debug.Log("gameobjectname"+finDDot.gameObject.name);
            TopWall[Counter2 - 1].transform.position = new Vector3(finDDot.transform.position.x, 20, finDDot.transform.position.z);  //finDDot.transform.position;
            TopWall[Counter2 - 1].transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (angle == 1)
        {
            TopWall[Counter2 - 1].transform.position = new Vector3(finDDot.transform.position.x, 20, finDDot.transform.position.z);
            TopWall[Counter2 - 1].transform.rotation = Quaternion.Euler(0, 90, 90);
        }



    }




}//class
