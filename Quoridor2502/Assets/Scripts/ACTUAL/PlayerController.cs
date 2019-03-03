using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamecore = GameCore;
using gameboard = Board;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    gamecore core = new gamecore(true);
    public Player player1;                          // Reference to Player1 gameobject
    public Player player2;                          // Reference to player2 gameobject
    public float speed;                             // The speed with which the player will move to other blocks
    [HideInInspector]
    public Vector3 previousPlayerPos;               // The previous position of the player before moving to this next block
    public Text currentlyActivePlayer;              // The UI text that shows the current active player
    [Range(0.1f,10)]
    public float timeToStayInAir;                   // The time the player will take to jump or the time it will stay in the air.
    [Range(50, 400)]
    public float jumpHeight;                        // The maximum height the player will reach when jumping.



    private bool moveNow;                           // Flag indicates whether the player should move now or not
    internal Player currentlySelectedPlayer;        // The player that is currently selected
    internal Players lastMoveBy;                    // The player that made the last move
    private bool isPlayer1Selected;                 // Shows which player is selected corrently
    private Vector3 playerNextDestination;                         // The interpolation end point
    private bool allowToggle = true;                // Flag indicates whether to allow switching between players
    private bool shouldJump;                        // Flag indicates whether the player should jump or not
    private float jumpAnim;                         // The jump animation time keeper
    private Vector3 playerStationaryPos;            // The position of the player when it was stationary before jumping
    private bool allowPlayerAction = true;                  // Should the player be allowed to make a move now?


    public enum Players
    {
       Player1,
       Player2
    }


    public enum wallTypes
    {
        topWall,
        bottomWall
    }



    [System.Serializable]
    public class Player
    {
        public Players playerType;
        public GameObject playerGameObject;
        public wallTypes allowableWallToMove;
    }





    // Start is called before the first frame update
    void Start()
    {
        currentlySelectedPlayer = player1;
        currentlyActivePlayer.text = "Active Player : " + $"<color=#00ff00ff>{currentlySelectedPlayer.playerType}</color>";
    }

    // Update is called once per frame
    void Update()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        int temp = 0;

        if(Input.GetMouseButtonUp(0) && !MoveWall.isWallMoving && allowPlayerAction)
        {

            if (Physics.Raycast(ray, out hitInfo , Mathf.Infinity , LayerMasks.instance.blockLayerOnly | LayerMasks.instance.playersLayerOnly , QueryTriggerInteraction.Collide))
            {
                 
                if (hitInfo.transform.gameObject.layer == LayerMasks.instance.blocksLayerNumber)
                {

                    bool movingOnOtherPlayer = false;

                    Transform blockHit = hitInfo.transform;
                    float blockSize = blockHit.localScale.x;
                    int playerPos = GetPlayerBoardPosition(currentlySelectedPlayer);
                    temp = playerPos;
                    int hitBlockPos = BoardSetup.instance.gridArray.IndexOf(blockHit.name) + 1;


                    if (currentlySelectedPlayer.playerType == Players.Player1)
                    {
                        if(GetPlayerBoardPosition(player2) == hitBlockPos) { movingOnOtherPlayer = true; }
                    }
                        
                    else
                    {
                        if (GetPlayerBoardPosition(player1) == hitBlockPos) { movingOnOtherPlayer = true; }
                    }



                    if(!movingOnOtherPlayer)
                    {


                        //Debug.Log("Index: " + hitBlockIndexPos + " is "+ blockHit.position.x + " " + currentlySelectedPlayer.transform.position.y + " " + blockHit.position.z);               
                        List<int> allowablePositions = GetAllowablePositions(playerPos);

                        previousPlayerPos = currentlySelectedPlayer.playerGameObject.transform.position;
                        byte row = (byte)(hitBlockPos / 9 + 1);
                        byte col = (byte)(hitBlockPos % 9);
                        gameboard.Move move = new gameboard.Move(row, col, 0);


                        if (allowablePositions.Contains(hitBlockPos))
                        {

                            Transform selectedPlayer = currentlySelectedPlayer.playerGameObject.transform;

                            Vector3 rayOrigin = new Vector3(selectedPlayer.position.x, selectedPlayer.position.y - (selectedPlayer.localScale.y / 2), selectedPlayer.position.z);

                            // If there is no wall infront of the player then move
                            if (!Physics.Raycast(rayOrigin, blockHit.position - rayOrigin, Mathf.Infinity, LayerMasks.instance.placedWallsOnly))
                            {
                                previousPlayerPos = currentlySelectedPlayer.playerGameObject.transform.position;
                                //currentlySelectedPlayer.transform.position = new Vector3(blockHit.position.x , currentlySelectedPlayer.transform.position.y, blockHit.position.z);
                                playerNextDestination = new Vector3(blockHit.position.x, currentlySelectedPlayer.playerGameObject.transform.position.y, blockHit.position.z);
                                moveNow   = true;        
                                lastMoveBy = currentlySelectedPlayer.playerType;
                            }


                        }

                        else
                        {
                            // might be able to jump
                            int jumpablePos = GetJumpablePosition();

                            if (hitBlockPos == jumpablePos)
                            {
                                //check for wall 
                                Transform selectedPlayer = currentlySelectedPlayer.playerGameObject.transform;

                                Vector3 rayOrigin = new Vector3(selectedPlayer.position.x , selectedPlayer.position.y - (selectedPlayer.localScale.y / 2), selectedPlayer.position.z);

                                if (Physics.Raycast(rayOrigin , blockHit.position - rayOrigin , Mathf.Infinity , LayerMasks.instance.placedWallsOnly))
                                {
                                    Debug.Log("Don't jump i see a wall");
                                }

                                else
                                {
                                    /* Jump animation */

                                    lastMoveBy = currentlySelectedPlayer.playerType;          

                                    // move parabola
                                    shouldJump = true;
                                    playerStationaryPos = currentlySelectedPlayer.playerGameObject.transform.position;
                                    playerNextDestination = new Vector3(blockHit.position.x, currentlySelectedPlayer.playerGameObject.transform.position.y, blockHit.position.z);
                                    jumpAnim = 0;
                                }

                            }

                        }

                    }

                    else { Debug.Log("Moving on the other player is not allowed"); }


     


                    /*
                    if (allowableIndices.Contains(hitBlockIndexPos) ||
                         core.IsJump(move))
                    {
                        Debug.Log("Processing move...");
                        bool tempBool = core.ProcessMove(move);
                        Debug.Log("Processed: " + tempBool);
                        if (tempBool)
                        {
                            //currentlySelectedPlayer.transform.position = new Vector3(blockHit.position.x , currentlySelectedPlayer.transform.position.y, blockHit.position.z);
                            lerpTo = new Vector3(blockHit.position.x, currentlySelectedPlayer.transform.position.y, blockHit.position.z);

                            moveNow = true;
                            if (core.CheckForVictory())
                            {
                                Debug.Log(currentlySelectedPlayer.ToString() + "Wins!");
                            }
                        }
                    } */

                }

                // A player was clicked
                else
                {
                    Debug.Log("Moving on the other player is not allowed");
                }

            }
        }

        
        if(moveNow)
        {

            // As soon as the player starts to move stop his next actions until the move is complete
             allowPlayerAction = false;

             GameObject actingPlayer = currentlySelectedPlayer.playerGameObject;

             actingPlayer.transform.position = Vector3.Lerp(actingPlayer.transform.position , playerNextDestination , Time.deltaTime * speed);

            if (Vector3.Distance(actingPlayer.transform.position, playerNextDestination) <= 0.3f)
            {
                allowPlayerAction = true;
                moveNow = false;
                actingPlayer.transform.position = playerNextDestination;

                // Change player as the move has been completely made now
                ToggleActivePlayer();

            }
        }


        else if(shouldJump)
        {

            // As soon as the player starts to jump stop his next actions until the jump is complete
            allowPlayerAction = false;
            Transform actingPlayer = currentlySelectedPlayer.playerGameObject.transform;

            float currentPlayerHeight = actingPlayer.position.y;
            float playerInitialHeight = playerStationaryPos.y;

            jumpAnim += Time.deltaTime;
            actingPlayer.position = MathParabola.Parabola(playerStationaryPos, playerNextDestination, jumpHeight, jumpAnim / timeToStayInAir);

            
            /*  This is one way to know if the player has reached the target grid so stop the jump. The other approach is for the player to inform if it is triggered by a block
            if (Mathf.Abs(currentPlayerHeight - playerInitialHeight) <= 2f && Vector3.Distance(playerNextDestination, actingPlayer.position) <= 5f)
            {
                currentlySelectedPlayer.playerGameObject.transform.position = new Vector3(playerNextDestination.x, playerInitialHeight, playerNextDestination.z);
                allowMove = true;
                shouldJump = false;

                // Change player as the move has been completely made now
                ToggleActivePlayer();
            }
            */

        }
        
    }




    public void ChangeToPlayer1(bool selected)
    {
        if(selected)
        {
            isPlayer1Selected = true; currentlySelectedPlayer = player1;
            currentlyActivePlayer.text = "Active Player : " + $"<color=#00ff00ff>{currentlySelectedPlayer.playerType}</color>";
        }

    }

    public void ChangeToPlayer2(bool selected)
    {
        if (selected)
        {
            isPlayer1Selected = false; currentlySelectedPlayer = player2;
            currentlyActivePlayer.text = "Active Player : " + $"<color=#00ff00ff>{currentlySelectedPlayer.playerType}</color>";
        }
    }




    public List<int> GetAllowablePositions(int index)
    {
        List<int> allowables = new List<int>();

        // lies in the left most column on the grid
        if (((index - 1) % 9) == 0)
        {

            // top left block
            if (index == 1)
            {
                allowables.Add(index + 1);
                allowables.Add(index + 9);
            }

            //bottom left block
            else if (index == 82)
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

        foreach (int allow in allowables)
        {
            allows += "  " + allow;
        }

        return allowables;
    }




    public int GetPlayerBoardPosition(Player player)
    {
        RaycastHit hitInfo;
        GameObject playerObject = player.playerGameObject;


        if (Physics.Raycast(playerObject.transform.position, -playerObject.transform.up, out hitInfo, 100 , LayerMasks.instance.blockLayerOnly))
        {

            //if (hitInfo.transform.gameObject.layer != LayerMasks.instance.blocksLayerNumber) { return -1; }

            int index = BoardSetup.instance.gridArray.IndexOf(hitInfo.transform.name) + 1;
            //Debug.Log("Hit for player position check " + hitInfo.transform.name + "  Index is is  " + index);
            return index;
        }

        return -1;
    }




    public int GetJumpablePosition()
    {

        int currentPlayerPos = GetPlayerBoardPosition(currentlySelectedPlayer);
        int otherPlayerPos   = GetPlayerBoardPosition((currentlySelectedPlayer.playerType == Players.Player1) ? player2 : player1);

        // Vertical jumps
        int jump1 = currentPlayerPos + 18;
        int jump2 = currentPlayerPos - 18;

        // Horizontal jumps
        int jump3 = currentPlayerPos + 2;
        int jump4 = currentPlayerPos - 2;



        if (!PositionLiesInBoard(jump1) && !PositionLiesInBoard(jump2) && !PositionLiesInBoard(jump3) && !PositionLiesInBoard(jump4))
        {
            //Debug.Log("NO JUMP ALLOWED NO POS ON BOARD currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump1 " + jump1 + " jump2 " + jump2);
            return -1;
        }


        if ((currentPlayerPos + 9) == otherPlayerPos)
        {
            // jump 1 is allowed
            //Debug.Log("Jump1 is allowed  pos is  " + jump1 + " currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump1 " + jump1 + " jump2 " + jump2);
            return jump1;
        }

        else if ((currentPlayerPos - 9) == otherPlayerPos)
        {
            // jump 1 is allowed
            //Debug.Log("Jump2 is allowed  pos is  " + jump2 + " currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump1 " + jump1 + " jump2 " + jump2);
            return jump2;
        }


        else if ((currentPlayerPos + 1) == otherPlayerPos)
        {
            // jump 3 is allowed
            //Debug.Log("Jump3 is allowed  pos is  " + jump3 + " currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump3 " + jump3 + " jump4 " + jump4);
            return jump3;
        }

        else if ((currentPlayerPos - 1) == otherPlayerPos)
        {
            // jump 4 is allowed
            //Debug.Log("Jump4 is allowed  pos is  " + jump4 + " currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump3 " + jump3 + " jump4 " + jump4);
            return jump4;
        }

        else
        {
            //Debug.Log("NO VALID JUMP FOUND FOR THE ACTIVE PLAYER:  " + currentlySelectedPlayer.playerType + "  currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump1 " + jump1 + " jump2 " + jump2 + " jump3 " + jump3 + " jump4 " + jump4);
            return -1;
        }

    }




    public void StopJump(Players requestingPlayer)
    {
        Debug.Log(requestingPlayer + " is requesting to stop the jump ");
        if(requestingPlayer == currentlySelectedPlayer.playerType && shouldJump)
        {
            currentlySelectedPlayer.playerGameObject.transform.position = playerNextDestination;
            allowPlayerAction = true;
            shouldJump = false;

            // Change player as the move has been completely made now
            ToggleActivePlayer();
        }
    }



    public bool PositionLiesInBoard(int index)
    {
        int gridArraySize = BoardSetup.instance.gridArray.Count;

        return (index >= 1 && index <= gridArraySize);
    }



    public void ToggleActivePlayer()
    {
        if (currentlySelectedPlayer.GetHashCode().Equals(player1.GetHashCode())) { ChangeToPlayer2(true); /*Debug.Log("Toggled active player now becomes PLAYER2"); */} 
        else {  ChangeToPlayer1(true);/* Debug.Log("Toggled active player now becomes PLAYER1");*/ }
    }
}
