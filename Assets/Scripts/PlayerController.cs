using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    public GameObject player1;                      // Reference to Player1 gameobject
    public GameObject player2;                      // Reference to player2 gameobject
    public float speed;                             // The speed with which the player will move to other blocks
    [HideInInspector]
    public Vector3 previousPlayerPos;               // The previous position of the player before moving to this next block

    internal bool moveNow;                          // Flag indicates whether the player should move now or not
    internal GameObject currentlySelectedPlayer;    // The player that is currently selected


    private bool isPlayer1Selected;                 // Shows which player is selected corrently
    private Vector3 lerpTo;                         // The interpolation end point
    private bool allowToggle = true;                // Flag indicates whether to allow switching between players





    // Start is called before the first frame update
    void Start()
    {

        currentlySelectedPlayer = player1;

    }

    // Update is called once per frame
    void Update()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;


        if(Input.GetMouseButtonUp(0) && !MoveWall.isWallMoving)
        {
            if (Physics.Raycast(ray, out hitInfo , Mathf.Infinity , LayerMasks.instance.wallAndBlockLayer))
            {

                if (hitInfo.transform.gameObject.layer == LayerMasks.instance.blocksLayerNumber)
                {

                    Transform blockHit = hitInfo.transform;
                    float blockSize = blockHit.localScale.x;
                    int playerIndexPo = GetPlayerIndex(currentlySelectedPlayer);
                    int hitBlockIndexPos = BoardSetup.instance.gridArray.IndexOf(blockHit.name) + 1;
                    

                    Debug.Log("Hit block index is  " + hitBlockIndexPos + "  name is  "+ blockHit.name + "  currently selected player is   "+ currentlySelectedPlayer.name);               
                    List<int> allowableIndices= GetAllowableIndices(playerIndexPo);

                    if(allowableIndices.Contains(hitBlockIndexPos))
                    {
                        previousPlayerPos = currentlySelectedPlayer.transform.position;
                        //currentlySelectedPlayer.transform.position = new Vector3(blockHit.position.x , currentlySelectedPlayer.transform.position.y, blockHit.position.z);
                        lerpTo = new Vector3(blockHit.position.x, currentlySelectedPlayer.transform.position.y, blockHit.position.z);
                        moveNow = true;
                    }

                }

                // wall was clicked
                else
                {
                }

            }
        }


        if(moveNow)
        {
            currentlySelectedPlayer.transform.position = Vector3.Lerp(currentlySelectedPlayer.transform.position , lerpTo , Time.deltaTime * speed);

            if (Vector3.Distance(currentlySelectedPlayer.transform.position, lerpTo) <= 0.3f)
            {
                moveNow = false;
                currentlySelectedPlayer.transform.position = lerpTo;
            }
        }
        
    }




    public void ChangeToPlayer1(bool selected)
    {
        if(selected) { isPlayer1Selected = true; currentlySelectedPlayer = player1; }

    }

    public void ChangeToPlayer2(bool selected)
    {
        if (selected) { isPlayer1Selected = false; currentlySelectedPlayer = player2; }
    }




    public List<int> GetAllowableIndices(int index)
    {
        List<int> allowables = new List<int>();

        // lies in the left most column on the grid
        if(((index - 1) % 9) == 0)
        {

            // top left block
            if(index == 1)
            {
                allowables.Add(index + 1);
                allowables.Add(index + 9);
            }

            //bottom left block
            else if(index == 82)
            {
                allowables.Add(index + 1);
                allowables.Add(index - 9);
            }

            else
            {
                allowables.Add(index + 1);
                allowables.Add(index + 9);
                allowables.Add(index - 9);
            }
        }



        // lies in the right most column on the grid
        else if ((index % 9) == 0)
        {

            // top right block
            if (index == 9)
            {
                allowables.Add(index - 1);
                allowables.Add(index + 9);
            }

            //bottom right block
            else if (index == 90)
            {
                allowables.Add(index - 1);
                allowables.Add(index - 9);
            }

            else
            {
                allowables.Add(index - 1);
                allowables.Add(index - 9);
                allowables.Add(index + 9);
            }
        }



        // lies in the top most row on the grid
        else if (index <= 9)
        {

            // top left block
            if (index == 1)
            {
                allowables.Add(index + 1);
                allowables.Add(index + 9);
            }

            // top right block
            if (index == 9)
            {
                allowables.Add(index - 1);
                allowables.Add(index + 9);
            }

            else
            {
                allowables.Add(index + 1);
                allowables.Add(index - 1);       
                allowables.Add(index + 9);
            }
        }



        // lies in the bottom most row on the grid
        else if (index >= 73)
        {

            // bottom left block
            if (index == 73)
            {
                allowables.Add(index + 1);
                allowables.Add(index - 9);
            }

            // bottom right block
            if (index == 81)
            {
                allowables.Add(index - 1);
                allowables.Add(index - 9);
            }

            else
            {
                allowables.Add(index + 1);
                allowables.Add(index - 1);
                allowables.Add(index - 9);
            }
        }


        // lies in the midle area of the grid
        else
        {
            allowables.Add(index + 1);
            allowables.Add(index - 1);
            allowables.Add(index + 9);
            allowables.Add(index - 9);
        }

        string allows = "";

        foreach(int allow in allowables)
        {
            allows += "  " + allow;
        }
        Debug.Log("Index was  " + index + " allowables are:  " + allows);

        return allowables;
    }




    public int GetPlayerIndex(GameObject player)
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(player.transform.position, -player.transform.up, out hitInfo, 100 , LayerMasks.instance.blockLayerOnly))
        {

            //if (hitInfo.transform.gameObject.layer != LayerMasks.instance.blocksLayerNumber) { return -1; }

            int index = BoardSetup.instance.gridArray.IndexOf(hitInfo.transform.name) + 1;
            Debug.Log("Hit for player position check " + hitInfo.transform.name + "  Index is is  " + index);
            return index;
        }

        return -1;
    }

}
