using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamecore = GameCore;
using gameboard = Board;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour
{
    public Player player1;                          // Reference to Player1 gameobject
    public Player player2;                          // Reference to player2 gameobject
	public GameObject Togglimage;
	public Sprite []sprite;
	public float speed;                             // The speed with which the player will move to other blocks
    [HideInInspector]
    public Vector3 previousPlayerPos;               // The previous position of the player before moving to this next block
    public Text currentlyActivePlayer;              // The UI text that shows the current active player
    [Range(0.1f,10)]
    public float timeToStayInAir;                   // The time the player will take to jump or the time it will stay in the air.
    [Range(50, 400)]
    public float jumpHeight;                        // The maximum height the player will reach when jumping.
    
    private gameboard.Move opponentMove = new gameboard.Move(0,0,0);
    public LoadEndScreens endScreens;
    public gamecore Core;
    private bool moveNow;                           // Flag indicates whether the player should move now or not
    internal Player currentlySelectedPlayer;        // The player that is currently selected
    internal Players lastMoveBy;                    // The player that made the last move
    private bool isPlayer1Selected;                 // Shows which player is selected corrently
    private bool weArePlayer1 = true;
    private Vector3 playerNextDestination;          // The interpolation end point
    private bool allowToggle = true;                // Flag indicates whether to allow switching between players
    private bool shouldJump;                        // Flag indicates whether the player should jump or not
    private float jumpAnim;                         // The jump animation time keeper
    private Vector3 playerStationaryPos;            // The position of the player when it was stationary before jumping
    private bool allowPlayerAction = true;          // Should the player be allowed to make a move now?


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




    public class MoveTuple : IEquatable<MoveTuple>
    {
		
        public byte row;
        public char col;
        public byte indexInGrid;

        public override string ToString()
        {
            return ($"[ Row: {row} , Col: {col} , GridIndex: {indexInGrid} ]");
        }

        public bool Equals(MoveTuple other)
        {
            return (this == other);
        }


        public static bool operator == (MoveTuple one, MoveTuple two)
        {
            if ((object)two == null) { return false; }

            return (one.indexInGrid == two.indexInGrid);
        }


        public static bool operator != (MoveTuple one, MoveTuple two)
        {
            if ((object)two == null) { return true; }

            return (one.indexInGrid != two.indexInGrid);
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        Core = gamecore.CreateInstance<gamecore>();
        Core.Init(GameObject.FindObjectOfType<GameSettings>().AIGame);
        currentlySelectedPlayer = player1;
        //currentlyActivePlayer.text = "Active Player : " + $"<color=#00ff00ff>{currentlySelectedPlayer.playerType}</color>";
		currentlyActivePlayer.text =  currentlySelectedPlayer.playerType.ToString();
	}

    // Update is called once per frame
    void Update()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;


        if(Input.GetMouseButtonUp(0) && !MoveWall.isWallMoving && allowPlayerAction)
        {

            if (Physics.Raycast(ray, out hitInfo , Mathf.Infinity , LayerMasks.instance.blockLayerOnly | LayerMasks.instance.playersLayerOnly , QueryTriggerInteraction.Collide))
            {
                 
                if (hitInfo.transform.gameObject.layer == LayerMasks.instance.blocksLayerNumber)
                {

                    bool movingOnOtherPlayer = false;

                    BoardSetup.Block blockHit = new BoardSetup.Block(hitInfo.transform.name, hitInfo.transform);
                    MoveTuple hitBlockPosition = GetBlockPosition(blockHit);

                    //float blockSize = blockHit.blockTransform.localScale.x;


                    //int playerPos = GetPlayerBoardPosition(currentlySelectedPlayer);


                    if (currentlySelectedPlayer.playerType == Players.Player1)
                    {
                        if(GetPlayerBoardPosition(player2) == hitBlockPosition) { movingOnOtherPlayer = true; }
                    }
                        
                    else
                    {
                        if (GetPlayerBoardPosition(player1) == hitBlockPosition) { movingOnOtherPlayer = true; }
                    }



                    if(!movingOnOtherPlayer)
                    {

                        previousPlayerPos = currentlySelectedPlayer.playerGameObject.transform.position;
                        gameboard.Move selectedMove = new gameboard.Move((byte)(10 - hitBlockPosition.row), (byte)(hitBlockPosition.col - 64), 0);

                        if (Core.ValidateMove(selectedMove))
                        {
                            if (MovePlayer(hitBlockPosition.col, hitBlockPosition.row))
                            {
                               
                            }

                            else
                            {
                                // might be able to jump
                                TryJump(hitBlockPosition);
                            }
                        }
                        

                    }

                    else { Debug.LogWarning("Moving on the other player is not allowed"); }

                }

                // A player was clicked
                else
                {
                    Debug.LogWarning("Moving on the other player is not allowed");
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
                if((weArePlayer1 != isPlayer1Selected) && moveNow == false)
                {
                    opponentMove = Core.GetMove();
                    opponentMove.Row = (byte)(10 - opponentMove.Row);
                    string stringMove = Core.ConvertMoveToString(opponentMove);
                    string upper = stringMove.ToUpper();
                    char temp = upper[0];

                    MovePlayer(temp, opponentMove.Row);
                }
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
    

        }
        
    }




    public void ChangeToPlayer1(bool selected)
    {
        if(selected)
        {
			Togglimage.gameObject.GetComponent<Image> ().sprite = sprite [0];
			isPlayer1Selected = true; currentlySelectedPlayer = player1;
			currentlyActivePlayer.text = currentlySelectedPlayer.playerType.ToString();
        }

    }

    public void ChangeToPlayer2(bool selected)
    {
        if (selected)
        {
			Togglimage.gameObject.GetComponent<Image> ().sprite = sprite [0];
			isPlayer1Selected = false; currentlySelectedPlayer = player2;
			currentlyActivePlayer.text = currentlySelectedPlayer.playerType.ToString();
        }
    }




    public List<MoveTuple> GetAllowablePositions(MoveTuple blockPosition)
    {
        List<MoveTuple> allowables = new List<MoveTuple>();

        int index = blockPosition.indexInGrid;

        // lies in the left most column on the grid
        if (((index - 1) % 9) == 0)
        {

            // top left block
            if (index == 1)
            {
                allowables.Add(IndexToMoveTuple(index + 1));
                allowables.Add(IndexToMoveTuple(index + 9));
            }

            //bottom left block
            else if (index == 82)
            {
                allowables.Add(IndexToMoveTuple(index + 1));
                allowables.Add(IndexToMoveTuple(index - 9));
            }

            else
            {
                allowables.Add(IndexToMoveTuple(index + 1));
                allowables.Add(IndexToMoveTuple(index + 9));
                allowables.Add(IndexToMoveTuple(index - 9));
            }
        }



        // lies in the right most column on the grid
        else if ((index % 9) == 0)
        {

            // top right block
            if (index == 9)
            {
                allowables.Add(IndexToMoveTuple(index - 1));
                allowables.Add(IndexToMoveTuple(index + 9));
            }

            //bottom right block
            else if (index == 90)
            {
                allowables.Add(IndexToMoveTuple(index - 1));
                allowables.Add(IndexToMoveTuple(index - 9));
            }

            else
            {
                allowables.Add(IndexToMoveTuple(index - 1));
                allowables.Add(IndexToMoveTuple(index - 9));
                allowables.Add(IndexToMoveTuple(index + 9));
            }
        }



        // lies in the top most row on the grid
        else if (index <= 9)
        {

            // top left block
            if (index == 1)
            {
                allowables.Add(IndexToMoveTuple(index + 1));
                allowables.Add(IndexToMoveTuple(index + 9));
            }

            // top right block
            if (index == 9)
            {
                allowables.Add(IndexToMoveTuple(index - 1));
                allowables.Add(IndexToMoveTuple(index + 9));
            }

            else
            {
                allowables.Add(IndexToMoveTuple(index + 1));
                allowables.Add(IndexToMoveTuple(index - 1));
                allowables.Add(IndexToMoveTuple(index + 9));
            }
        }



        // lies in the bottom most row on the grid
        else if (index >= 73)
        {

            // bottom left block
            if (index == 73)
            {
                allowables.Add(IndexToMoveTuple(index + 1));
                allowables.Add(IndexToMoveTuple(index - 9));
            }

            // bottom right block
            if (index == 81)
            {
                allowables.Add(IndexToMoveTuple(index - 1));
                allowables.Add(IndexToMoveTuple(index - 9));
            }

            else
            {
                allowables.Add(IndexToMoveTuple(index + 1));
                allowables.Add(IndexToMoveTuple(index - 1));
                allowables.Add(IndexToMoveTuple(index - 9));
            }
        }


        // lies in the midle area of the grid
        else
        {
            allowables.Add(IndexToMoveTuple(index + 1));
            allowables.Add(IndexToMoveTuple(index - 1));
            allowables.Add(IndexToMoveTuple(index + 9));
            allowables.Add(IndexToMoveTuple(index - 9));
        }


        foreach (MoveTuple allow in allowables)
        {
            //Debug.Log("Move Allowed:  " + allow);
        }

        return allowables;
    }




    public MoveTuple GetPlayerBoardPosition(Player player)
    {
        RaycastHit hitInfo;
        GameObject playerObject = player.playerGameObject;


        if (Physics.Raycast(playerObject.transform.position, -playerObject.transform.up, out hitInfo, 100 , LayerMasks.instance.blockLayerOnly))
        {

            BoardSetup.Block blockHit = new BoardSetup.Block(hitInfo.transform.name , hitInfo.transform);

            int index = BoardSetup.instance.gridArray.IndexOf(blockHit) + 1;
            return IndexToMoveTuple(index);
        }

        return null;
    }




    public MoveTuple GetJumpablePosition()
    {

        int currentPlayerPos = GetPlayerBoardPosition(currentlySelectedPlayer).indexInGrid;
        int otherPlayerPos   = GetPlayerBoardPosition((currentlySelectedPlayer.playerType == Players.Player1) ? player2 : player1).indexInGrid;

        // Vertical jumps
        int jump1 = currentPlayerPos + 18;
        int jump2 = currentPlayerPos - 18;

        // Horizontal jumps
        int jump3 = currentPlayerPos + 2;
        int jump4 = currentPlayerPos - 2;

        MoveTuple moveTuple;


        if (!PositionLiesInBoard(jump1) && !PositionLiesInBoard(jump2) && !PositionLiesInBoard(jump3) && !PositionLiesInBoard(jump4))
        {
            //Debug.Log("NO JUMP ALLOWED NO POS ON BOARD currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump1 " + jump1 + " jump2 " + jump2);
            moveTuple = null;
        }


        if ((currentPlayerPos + 9) == otherPlayerPos)
        {
            // jump 1 is allowed
            //Debug.Log("Jump1 is allowed  pos is  " + jump1 + " currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump1 " + jump1 + " jump2 " + jump2);
            moveTuple = IndexToMoveTuple(jump1);
        }

        else if ((currentPlayerPos - 9) == otherPlayerPos)
        {
            // jump 1 is allowed
            //Debug.Log("Jump2 is allowed  pos is  " + jump2 + " currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump1 " + jump1 + " jump2 " + jump2);
            moveTuple = IndexToMoveTuple(jump2);
        }


        else if ((currentPlayerPos + 1) == otherPlayerPos)
        {
            // jump 3 is allowed
            //Debug.Log("Jump3 is allowed  pos is  " + jump3 + " currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump3 " + jump3 + " jump4 " + jump4);
            moveTuple = IndexToMoveTuple(jump3);
        }

        else if ((currentPlayerPos - 1) == otherPlayerPos)
        {
            // jump 4 is allowed
            //Debug.Log("Jump4 is allowed  pos is  " + jump4 + " currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump3 " + jump3 + " jump4 " + jump4);
            moveTuple = IndexToMoveTuple(jump4);
        }

        else
        {
            //Debug.Log("NO VALID JUMP FOUND FOR THE ACTIVE PLAYER:  " + currentlySelectedPlayer.playerType + "  currentPlayerPos " + currentPlayerPos + " otherPlayerPos  " + otherPlayerPos + " jump1 " + jump1 + " jump2 " + jump2 + " jump3 " + jump3 + " jump4 " + jump4);
            moveTuple  = null;
        }


        return moveTuple;

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



    public MoveTuple IndexToMoveTuple(int index)
    {
        MoveTuple moveTuple = new MoveTuple();
        int rowEndIndex = ReturnNextMultiple(index, 9);
        

        byte rowNumber = (byte)  (10 - (rowEndIndex / 9));     // The rows start from 9 go in descending order, so the first row is 9 the second is 8.
        int colNumber  = index - (9 * (9 - rowNumber));
        //Debug.Log(" For player  " + currentlySelectedPlayer.playerType + "indexToConvert  " + index + "  rowEndIndex  " + rowEndIndex + " rowNumber " + rowNumber + " colNumber " + colNumber);
        moveTuple.indexInGrid = (byte)index;
        moveTuple.row = rowNumber;
        moveTuple.col = GetCharFromHexCode("4" + colNumber);

        return moveTuple;

    }





    public char GetCharFromHexCode(string hexCode)
    {
        return ((char)int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber));
    }






    public int ReturnNextMultiple(int lookAheadOf , int multipleOf)
    { 
        lookAheadOf++;

        while(true)
        {
            if(lookAheadOf % multipleOf == 0) { return lookAheadOf; }
            lookAheadOf++;
        }
    }




    public void ToggleActivePlayer()
    {
        if (Core.CheckForVictory())
        {
            if (currentlySelectedPlayer.GetHashCode().Equals(player1.GetHashCode()) && weArePlayer1)
            {
                endScreens.ShowWinScreen();
            }
            else
            {
                endScreens.ShowLossScreen();
            }
        }

        if (currentlySelectedPlayer.GetHashCode().Equals(player1.GetHashCode())) { ChangeToPlayer2(true); /*Debug.Log("Toggled active player now becomes PLAYER2"); */} 
        else {  ChangeToPlayer1(true);/* Debug.Log("Toggled active player now becomes PLAYER1");*/ }
    }




    public int GetIndexFromRowCol(char col, int row)
    {
        int colNumber   = Int32.Parse((((int)col).ToString("X"))[1] + "");
        int rowEndIndex = (10 - row) * 9; 
        return  rowEndIndex - (9 - colNumber);     // The row number starts from the bottom
    }




    public bool MovePlayer(char col, int row)
    {
        Debug.Log("Recieved " + row + " " + col);
        gameboard.Move selectedMove = new gameboard.Move((byte)(10 - row), (byte)(col - 64), 0);
        Debug.Log(selectedMove.Row + " " + selectedMove.Column);
        Core.ProcessMove(selectedMove);
        col = (col + "").ToUpper()[0];

        MoveTuple playerPos       = GetPlayerBoardPosition(currentlySelectedPlayer);
        MoveTuple moveTo          = IndexToMoveTuple(GetIndexFromRowCol(col , row));
        BoardSetup.Block blockHit = BoardSetup.instance.gridArray[moveTo.indexInGrid - 1];

        //Debug.Log("Blockhit is  " + blockHit + "  playerPos is  " + playerPos + "  moveTo is  " + moveTo);

        List<MoveTuple> allowablePositions = GetAllowablePositions(playerPos);

        previousPlayerPos = currentlySelectedPlayer.playerGameObject.transform.position;


        if (allowablePositions.Contains(moveTo))
        {
            
            Transform selectedPlayer = currentlySelectedPlayer.playerGameObject.transform;

            Vector3 rayOrigin = new Vector3(selectedPlayer.position.x, selectedPlayer.position.y - (selectedPlayer.localScale.y / 2), selectedPlayer.position.z);

            // If there is no wall infront of the player then move
            if (!Physics.Raycast(rayOrigin, blockHit.blockTransform.position - rayOrigin, Mathf.Infinity, LayerMasks.instance.placedWallsOnly))
            {
                previousPlayerPos = currentlySelectedPlayer.playerGameObject.transform.position;
                //currentlySelectedPlayer.transform.position = new Vector3(blockHit.position.x , currentlySelectedPlayer.transform.position.y, blockHit.position.z);
                playerNextDestination = new Vector3(blockHit.blockTransform.position.x, currentlySelectedPlayer.playerGameObject.transform.position.y, blockHit.blockTransform.position.z);
                moveNow = true;
                lastMoveBy = currentlySelectedPlayer.playerType;
                return true;
            }

            else
            {
                return false;
            }

        }

        else
        {
            return false;
        }

    }




    public bool TryJump(MoveTuple jumpTo)
    {

        MoveTuple jumpablePos = GetJumpablePosition();
        BoardSetup.Block targetBlock = BoardSetup.instance.gridArray[jumpTo.indexInGrid - 1];


        if (jumpTo == jumpablePos)
        {

            //check for wall 
            Transform selectedPlayer = currentlySelectedPlayer.playerGameObject.transform;

            Vector3 rayOrigin = new Vector3(selectedPlayer.position.x, selectedPlayer.position.y - (selectedPlayer.localScale.y / 2), selectedPlayer.position.z);

            if (Physics.Raycast(rayOrigin, targetBlock.blockTransform.position - rayOrigin, Mathf.Infinity, LayerMasks.instance.placedWallsOnly))
            {
                Debug.Log("Don't jump i see a wall");
                return false;
            }

            else
            {
                /* Jump animation */

                lastMoveBy = currentlySelectedPlayer.playerType;

                // move parabola
                shouldJump = true;
                playerStationaryPos = currentlySelectedPlayer.playerGameObject.transform.position;
                playerNextDestination = new Vector3(targetBlock.blockTransform.position.x, currentlySelectedPlayer.playerGameObject.transform.position.y, targetBlock.blockTransform.position.z);
                jumpAnim = 0;

                return true;
            }

        }

        else { return false; }

    }



    public MoveTuple GetBlockPosition(BoardSetup.Block block)
    {
        MoveTuple moveTuple = null;
        int blockIndex = BoardSetup.instance.gridArray.IndexOf(block);

        if(blockIndex != -1)
        {
            moveTuple = IndexToMoveTuple(blockIndex + 1);
        }

        return moveTuple;
    }


}
