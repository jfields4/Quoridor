using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using System.Collections;
using static WallValidation;
public class Board
{
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
                //jumping backwards
                else if (move.Row < PlayerPositions[Current].Row)
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
                //jumping right
                else if (move.Column > PlayerPositions[Current].Column)
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
                //jumping left
                else if (move.Column < PlayerPositions[Current].Column)
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
        if (PlayerPositions[0].Row == 9 ||
            PlayerPositions[0].Row == 1)
        {
            end = true;
        }

        return end;
    }

    //private functions
    private bool Adjacent(Move to, Move start)
    {
        return (Math.Abs(to.Row - start.Row) == 1 ^
                Math.Abs(to.Column - start.Column) == 1);
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