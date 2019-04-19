using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;
using UnityEngine;

public class Board : ScriptableObject
{
    public class WallValidation
    {
        //state and node are used to track board state
        public struct State
        {
            public Board.Move position1;
            public Board.Move position2;

            public State(Board.Move first, Board.Move second)
            {
                position1 = new Board.Move(first);
                position2 = new Board.Move(second);
            }
        }
        public struct Node
        {
            public Node(Board currentboard, Board.Move currentmove, State state)
            {
                gameboard = new Board(currentboard);
                mv = new Board.Move(currentmove);
                currentstate = state;
            }

            public State currentstate;
            public readonly Board gameboard;
            public readonly Board.Move mv;
        }

        //this is necessary for the HashSet to not put duplicates in the set
        public class MoveComparer : EqualityComparer<Board.Move>
        {
            public override bool Equals(Board.Move x, Board.Move y)
            {
                return x == y;
            }

            public override int GetHashCode(Board.Move obj)
            {
                int hash = obj.Row ^ obj.Column ^ obj.Value;
                return hash.GetHashCode();
            }
        }

        //this set is used to make sure we don't back track and get infinite recursion
        HashSet<Board.Move> PlacesBeen;

        public WallValidation()
        {
            PlacesBeen = new HashSet<Board.Move>(new MoveComparer());
        }

        public bool Validate(Board boardSent, Board.Move placement)
        {
            if (placement.Row < 1 || placement.Column < 1 || placement.Row >= 9 || placement.Column >= 9)
            {
                return false;
            }

            //if the space is occupied
            if (boardSent.GameBoard[placement.Row, placement.Column] != 0)
            {
                return false;
            }
            //if wall is horizontal, check left & right for horizontal walls
            else if (placement.Value == 1 &&
                (boardSent.GameBoard[placement.Row, placement.Column - 1] == 1 ||
                boardSent.GameBoard[placement.Row, placement.Column + 1] == 1))
            {
                return false;
            }
            //if wall is vertical, check up & down for vertical walls
            else if (placement.Value == -1 &&
                (boardSent.GameBoard[placement.Row - 1, placement.Column] == -1 ||
                boardSent.GameBoard[placement.Row + 1, placement.Column] == -1))
            {
                return false;
            }

            //check both player positions
            for (int i = 0; i <= 1; i++)
            {
                State startingState = new State(boardSent.PlayerPositions[0], boardSent.PlayerPositions[1]);
                Node startingNode = new Node(boardSent, placement, startingState);
                startingNode.gameboard.MakeMove(placement);
                startingNode.gameboard.Current = (byte)i;

                if (PathToGoal(startingNode, i) == false)
                {
                    return false;
                }
                PlacesBeen.Clear();
            }

            return true;
        }

        //depth-first recursive search
        private bool PathToGoal(Node node, int playerNumber)
        {
            Board.Move moveRight;
            Board.Move moveLeft;
            Board.Move moveDown;
            Board.Move moveUp;

            bool result = false;

            if (PlacesBeen.Count == 0)
            {
                PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
            }

            moveUp = new Board.Move((byte)(node.gameboard.PlayerPositions[playerNumber].Row - 1), node.gameboard.PlayerPositions[playerNumber].Column, 0);
            moveDown = new Board.Move((byte)(node.gameboard.PlayerPositions[playerNumber].Row + 1), node.gameboard.PlayerPositions[playerNumber].Column, 0);
            moveLeft = new Board.Move(node.gameboard.PlayerPositions[playerNumber].Row, (byte)(node.gameboard.PlayerPositions[playerNumber].Column - 1), 0);
            moveRight = new Board.Move(node.gameboard.PlayerPositions[playerNumber].Row, (byte)(node.gameboard.PlayerPositions[playerNumber].Column + 1), 0);


            //if current player can reach other side, wall is legal
            if ((playerNumber == 0 && node.gameboard.PlayerPositions[0].Row == 1) ||
                (playerNumber == 1 && node.gameboard.PlayerPositions[1].Row == 9))
            {
                return true;
            }

            if (PlacesBeen.Count == 71)
            {
                playerNumber = playerNumber;
            }

            //check each direction
            if (node.gameboard.ValidateMove(moveUp) &&
                !PlacesBeen.Contains(moveUp) &&
                result != true)
            {
                Node newNode = new Node(node.gameboard, node.mv, node.currentstate);
                newNode.gameboard.MakeMove(moveUp);
                newNode.gameboard.Current = (byte)playerNumber;

                PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
                result = PathToGoal(newNode, playerNumber);
            }
            if (node.gameboard.ValidateMove(moveLeft) &&
                !PlacesBeen.Contains(moveLeft) &&
                result != true)
            {
                Node newNode = new Node(node.gameboard, node.mv, node.currentstate);
                newNode.gameboard.MakeMove(moveLeft);
                newNode.gameboard.Current = (byte)playerNumber;

                PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
                result = PathToGoal(newNode, playerNumber);
            }
            if (node.gameboard.ValidateMove(moveRight) &&
                !PlacesBeen.Contains(moveRight) &&
                result != true)
            {
                Node newNode = new Node(node.gameboard, node.mv, node.currentstate);
                newNode.gameboard.MakeMove(moveRight);
                newNode.gameboard.Current = (byte)playerNumber;

                PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
                result = PathToGoal(newNode, playerNumber);
            }
            if (node.gameboard.ValidateMove(moveDown) &&
                !PlacesBeen.Contains(moveDown) &&
                result != true)
            {
                Node newNode = new Node(node.gameboard, node.mv, node.currentstate);
                newNode.gameboard.MakeMove(moveDown);
                newNode.gameboard.Current = (byte)playerNumber;

                PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
                result = PathToGoal(newNode, playerNumber);
            }

            //if no more possible moves, then wall is illegal
            // if other side was reached, wall is legal
            return result;
        }
    }

    //move class
    public class Move
    {
        public byte Row;
        public byte Column;
        public sbyte Value;

        public static bool operator <(Move lValue, Move rValue)
        {
            return ((lValue.Row < rValue.Row) ||
                    (lValue.Column < rValue.Column));
        }
        public static bool operator >(Move lValue, Move rValue)
        {
            return ((lValue.Row > rValue.Row) ||
                    (lValue.Column > rValue.Column));
        }

        public static bool operator ==(Move lValue, Move rValue)
        {
            return ((lValue.Row == rValue.Row) &&
                    (lValue.Column == rValue.Column) &&
                    lValue.Value == rValue.Value);
        }

        public static bool operator !=(Move lValue, Move rValue)
        {
            return !(lValue == rValue);
        }

        public Move(byte r, byte c, sbyte v)
        {
            Row = r;
            Column = c;
            Value = v;
        }

        public Move(Move otherMove)
        {
            Row = otherMove.Row;
            Column = otherMove.Column;
            Value = otherMove.Value;
        }

        public Move() { }
    }

    //used for validating wall placements
    WallValidation Validator;

    //member variables
    public sbyte[,] GameBoard;
    const byte BoardSize = 10;

    public Move[] PlayerPositions;
    public byte Current;
    public byte NotCurrent;
    public byte Turns;
    public byte[] Walls;

    //constructor
    public Board()
    {
        GameBoard = new sbyte[BoardSize, BoardSize];
        Walls = new byte[2];
        PlayerPositions = new Move[] {
            new Move(9,5,0),
            new Move(1,5,0) };

        Validator = new WallValidation();

        //build walls around border
        //top
        for (int i = 0; i < 10; i++)
        {
            GameBoard[0, i] = 1;
        }
        //bottom
        for (int i = 0; i < 10; i++)
        {
            GameBoard[9, i] = 1;
        }
        //left
        for (int i = 0; i < 10; i++)
        {
            GameBoard[i, 0] = -1;
        }
        //right
        for (int i = 0; i < 10; i++)
        {
            GameBoard[i, 9] = -1;
        }

        Turns = 0;
        Current = 0;
        NotCurrent = 1;
        for (int i = 0; i <= 1; i++)
        {
            Walls[i] = 10;
        }
    }

    //copy constructor
    public Board(Board otherBoard)
    {
        GameBoard = new sbyte[BoardSize, BoardSize];
        PlayerPositions = new Move[2];
        Walls = new byte[2];

        Buffer.BlockCopy(otherBoard.GameBoard, 0, GameBoard, 0, 100);

        PlayerPositions[0] = new Move(otherBoard.PlayerPositions[0]);
        PlayerPositions[1] = new Move(otherBoard.PlayerPositions[1]);

        Current = otherBoard.Current;
        NotCurrent = otherBoard.NotCurrent;
        Turns = otherBoard.Turns;
        Validator = new WallValidation();

        Walls[0] = otherBoard.Walls[0];
        Walls[1] = otherBoard.Walls[1];
		//Debug.Log (Walls [0]);
    }

    //public functions

    public Move ConvertStringToMove(string moveSent)
    {
        Move move = new Move();

        if (moveSent.Length == 2)
        {
            move.Row = (byte)(10 - int.Parse(moveSent[1].ToString()));
            move.Column = (byte)(moveSent[0] - 96);
            move.Value = 0;
			Debug.Log (move.Row);
        }
        else
        {
            move.Row = (byte)(9 - int.Parse(moveSent[1].ToString()));
            move.Column = (byte)(moveSent[0] - 96);
            if (moveSent[2] == 'h')
            {
                move.Value = 1;
            }
            else if (moveSent[2] == 'v')
            {
                move.Value = -1;
            }
            else
            {
                //error code
                move.Row = 0;
                move.Column = 0;
                move.Value = 0;
            }
        }
        return move;
    }

    public string ConvertMoveToString(Move move)
    {
        string moveS = "";

        moveS = ((char)(move.Column + 96)).ToString();
        if (move.Value == 0)
        {
            moveS += (10 - move.Row).ToString();
        }
        else
        {
            moveS += (9 - move.Row).ToString();
            if (move.Value == 1)
            {
                moveS += "h";
            }
            else
            {
                moveS += "v";
            }
        }
        return moveS;
    }

    public bool ValidateMove(string moveString)
    {
        Move moveSent = ConvertStringToMove(moveString);

        return ValidateMove(moveSent);
    }

    public bool ValidateMove(Move moveSent)
    {
        bool valid = false;
        Move move;

        /***************checks to make sure invalid moves don't cause out of bounds errors********************/
        if (moveSent.Row < 1 || moveSent.Column < 1 || moveSent.Row > 9 || moveSent.Column > 9)
        {
            return false;
        }

        foreach (Move m in PlayerPositions)
        {
            if (m.Row > 9 || m.Row < 1 || m.Column > 9 || m.Row < 1)
                return false;
        }
        /*****************************************************************************************************/

        //if wall
        if (moveSent.Value != 0)
        {
            if (Walls[Current] == 0)
            {
                valid = false;
            }
            else
            {
                valid = Validator.Validate(this, moveSent);
            }
        }
        //if movement
        else
        {
            //reference as if moving from smaller point to larger point
            if (moveSent < PlayerPositions[Current])
            {
                move = new Move(moveSent);
            }
            else
            {
                move = new Move(PlayerPositions[Current]);
            }

            if (MoveIsJump(moveSent))
            {
                valid = true;
            }
            else if (moveSent != PlayerPositions[NotCurrent])
            {
                if (Adjacent(moveSent, PlayerPositions[Current]))
                {
                    //if moving up or down
                    if (moveSent.Row != PlayerPositions[Current].Row)
                    {
                        if (GameBoard[move.Row, move.Column - 1] <= 0 &&
                            GameBoard[move.Row, move.Column] <= 0)
                        {
                            valid = true;
                        }
                    }
                    //if moving left or right
                    else
                    {
                        if (GameBoard[move.Row - 1, move.Column] >= 0 &&
                            GameBoard[move.Row, move.Column] >= 0)
                        {
                            valid = true;
                        }
                    }
                }
            }
        }

        return valid;
    }

    public bool MoveIsJump(Move move)
    {
        foreach (Move m in PlayerPositions)
        {
            if (m.Row > 9 || m.Row < 1 || m.Column > 9 || m.Row < 1)
                return false;
        }

        bool isJump = false;
        if (!(PlayerPositions[NotCurrent].Column > 9 || PlayerPositions[NotCurrent].Column < 1))
        {
            if (Adjacent(PlayerPositions[0], PlayerPositions[1]))
            {
                //jumping forward
                if (move.Row > PlayerPositions[Current].Row)
                {
                    //if not wall in front
                    if(GameBoard[PlayerPositions[Current].Row, PlayerPositions[Current].Column] != 1 &&
                        GameBoard[PlayerPositions[Current].Row, PlayerPositions[Current].Column - 1] != 1)
                    { 
                        //if move is diagonal
                        if (Math.Abs(move.Row - PlayerPositions[Current].Row) != 2)
                        {
                            //if wall is present
                            if ((GameBoard[PlayerPositions[NotCurrent].Row, PlayerPositions[NotCurrent].Column] > 0 ||
                                 GameBoard[PlayerPositions[NotCurrent].Row, PlayerPositions[NotCurrent].Column - 1] > 0))
                            {
                                isJump = true;
                            }
                        }
                        //if move is not a diagonal jump
                        else if (Math.Abs(move.Row - PlayerPositions[Current].Row) == 2)
                        {
                            isJump = true;
                        }
                    }
                    
                }
                //jumping backwards
                else if (move.Row < PlayerPositions[Current].Row)
                {
                    //if not wall in front
                    if(GameBoard[PlayerPositions[Current].Row, PlayerPositions[Current].Column - 1] != 1 &&
                        GameBoard[PlayerPositions[Current].Row - 1, PlayerPositions[Current].Column - 1] != 1)
                    {
                        //if move is diagonal
                        if (Math.Abs(move.Row - PlayerPositions[Current].Row) != 2)
                        {
                            //if wall is present
                            if ((GameBoard[PlayerPositions[NotCurrent].Row - 1, PlayerPositions[NotCurrent].Column - 1] > 0 ||
                                    GameBoard[PlayerPositions[NotCurrent].Row - 1, PlayerPositions[NotCurrent].Column] > 0))
                            {
                                isJump = true;
                            }
                        }
                        //if move is not a diagonal jump
                        else if (Math.Abs(move.Row - PlayerPositions[Current].Row) == 2)
                        {
                            isJump = true;
                        }
                    }
                }
                //jumping right
                else if (move.Column > PlayerPositions[Current].Column)
                {
                    //if not wall in front
                    if(GameBoard[PlayerPositions[Current].Row - 1, PlayerPositions[Current].Column] != -1 &&
                        GameBoard[PlayerPositions[Current].Row, PlayerPositions[Current].Column] != -1)
                    {
                        //if move is diagonal
                        if (Math.Abs(move.Row - PlayerPositions[Current].Row) != 2)
                        {
                            //if wall is present
                            if ((GameBoard[PlayerPositions[NotCurrent].Row, PlayerPositions[NotCurrent].Column] < 0 ||
                                 GameBoard[PlayerPositions[NotCurrent].Row - 1, PlayerPositions[NotCurrent].Column] < 0))
                            {
                                isJump = true;
                            }
                        }
                        //if move is not a diagonal jump
                        else if (Math.Abs(move.Row - PlayerPositions[Current].Row) == 2)
                        {
                            isJump = true;
                        }
                    }
                }
                //jumping left
                else if (move.Column < PlayerPositions[Current].Column)
                {
                    //if not wall in front
                    if(GameBoard[PlayerPositions[Current].Row - 1, PlayerPositions[Current].Column] != -1 &&
                        GameBoard[PlayerPositions[Current].Row - 1, PlayerPositions[Current].Column - 1] != -1)
                    {
                        //if move is diagonal
                        if (Math.Abs(move.Row - PlayerPositions[Current].Row) != 2)
                        {
                            //if wall is present
                            if ((GameBoard[PlayerPositions[NotCurrent].Row - 1, PlayerPositions[NotCurrent].Column - 1] > 0 ||
                                 GameBoard[PlayerPositions[NotCurrent].Row - 1, PlayerPositions[NotCurrent].Column] > 0))
                            {
                                isJump = true;
                            }
                        }
                        //if move is not a diagonal jump
                        else if (Math.Abs(move.Row - PlayerPositions[Current].Row) == 2)
                        {
                            isJump = true;
                        }
                    }
                }
            }
        }
        return isJump;
    }

    public void MakeMove(string moveSent)
    {
        Move move = ConvertStringToMove(moveSent);
        //a value of 0 means it is a pawn move
        if (move.Value == 0)
        {
            SetNewPlayerPosition(move);
        }
        else
        {
            SetNewWall(move);
        }

        //flip current player
        Current = (byte)Math.Abs(Current - 1);
        NotCurrent = (byte)Math.Abs(Current - 1);
        Turns++;
    }

    //overloaded to deal with either representation of a move
    public void MakeMove(Move move)
    {
        if (move.Value == 0)
        {
            SetNewPlayerPosition(move);
        }
        else
        {
            SetNewWall(move);
        }

        Current = (byte)Math.Abs(Current - 1);
        NotCurrent = (byte)Math.Abs(Current - 1);
        Turns++;
    }

    public bool CheckForEndGame()
    {
        bool end = false;
        if (PlayerPositions[0].Row == 1 ||
            PlayerPositions[1].Row == 9)
        {
            end = true;
        }

        return end;
    }

    private bool Adjacent(Move to, Move start)
    {
        bool adj = false;
        //if rows are adjacent
        if (Math.Abs(to.Row - start.Row) == 1)
        {
            // if in same column
            if (to.Column == start.Column)
            {
                adj = true;
            }
        }
        //if columns are adjacent
        else if(Math.Abs(to.Column - start.Column) == 1)
        {
            //if in same row
            if (to.Row == start.Row)
            {
                adj = true;
            }
        }

        //return (Math.Abs(to.Row - start.Row) == 1 ^
        //        Math.Abs(to.Column - start.Column) == 1);
        return adj;
    }
    private void SetNewPlayerPosition(Move move)
    {
        PlayerPositions[Current] = move;
    }

    private void SetNewWall(Move move)
    {
        GameBoard[move.Row, move.Column] = move.Value;
        Walls[Current]--;
    }
}