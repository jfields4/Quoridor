using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;

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
            return (lValue.Row == rValue.Row && lValue.Column == rValue.Column && lValue.Value == rValue.Value);
        }

        public static bool operator !=(Move lValue, Move rValue)
        {
            return !(lValue == rValue);
        }

        public Move (byte r, byte c, sbyte v)
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

    //member variables
    private sbyte[,] GameBoard;
    const byte BoardSize = 10;

    private Move[] PlayerPositions;
    private byte Current;
    private byte NotCurrent;

    //constructor
    public Board()
    {
        Current = 0;
        NotCurrent = 1;
        GameBoard = new sbyte[BoardSize,BoardSize];
        PlayerPositions = new Move[] {
            new Move(9,5,0),
            new Move(1,5,0) };
    }

    //public functions

    public static bool operator ==(Board lValue, Board rValue)
    {
        bool result = true;

        for (int i = 0; result && i < 10; i++)
        {
            for (int j = 0; result && j < 10; j++)
            {
                if (lValue.GameBoard[i,j] != rValue.GameBoard[i,j])
                {
                    result = false;
                }
            }
        }
        if (result)
        {
            result = lValue.PlayerPositions[0] == rValue.PlayerPositions[0] && lValue.PlayerPositions[1] == rValue.PlayerPositions[1];
        }
        return result;
    }

    public static bool operator !=(Board lValue, Board rValue)
    {
        bool result = !(lValue == rValue);
        return result;
    }


    public Move ConvertStringToMove(string moveSent)
    {
        Move move = new Move();

        move.Row = (byte)(10 - Int32.Parse(moveSent[1].ToString()));
        move.Column = (byte)(moveSent[0] - 96);

        if (moveSent.Length == 2)
        {
            move.Value = 0;
        }
        else
        {
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
        moveS += (10 - move.Row).ToString();

        if (move.Value != 0)
        {
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
        bool valid = false;

        Move moveSent = ConvertStringToMove(moveString);
        
        //reference as if moving from smaller point to larger point
        Move move;
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
        else
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

        return valid;
    }

    public bool ValidateMove(Move moveSent)
    {
        bool valid = false;

        Move move = new Move(0,0,0);

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
        else
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
        Debug.Log("Valid " + valid);
        return valid;
    }

    public bool MoveIsJump(Move move)
    {
        bool isJump = false;

        if (Adjacent(PlayerPositions[0], PlayerPositions[1]))
        {
            //jumping forward
            if (move.Row > PlayerPositions[Current].Row)
            {
                //if move is diagonal
                if (Math.Abs(move.Row - PlayerPositions[Current].Row) != 2)
                {
                    //if wall is present
                    if ((GameBoard[PlayerPositions[NotCurrent].Row + 1, PlayerPositions[NotCurrent].Column - 1] < 0 &&
                         GameBoard[PlayerPositions[NotCurrent].Row + 1, PlayerPositions[NotCurrent].Column] < 0))
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
                    if ((GameBoard[PlayerPositions[NotCurrent].Row - 1, PlayerPositions[NotCurrent].Column + 1] < 0 &&
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
            //jumping right
            else if (move.Column > PlayerPositions[Current].Column)
            {
                //if move is diagonal
                if (Math.Abs(move.Row - PlayerPositions[Current].Row) != 2)
                {
                    //if wall is present
                    if ((GameBoard[PlayerPositions[NotCurrent].Row, PlayerPositions[NotCurrent].Column + 1] > 0 &&
                         GameBoard[PlayerPositions[NotCurrent].Row - 1, PlayerPositions[NotCurrent].Column + 1] > 0))
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
                    if ((GameBoard[PlayerPositions[NotCurrent].Row, PlayerPositions[NotCurrent].Column - 1] > 0 &&
                         GameBoard[PlayerPositions[NotCurrent].Row - 1, PlayerPositions[NotCurrent].Column - 1] > 0))
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
        Debug.Log("isJump " + isJump);
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
    }

    //overloaded to deal with either representation of a move
    public void MakeMove(Move move)
    {
        Debug.Log("Making move: " + move.Row + ", " + move.Column + ", ");

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

    //private functions
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
        else if (Math.Abs(to.Column - start.Column) == 1)
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
        PlayerPositions[Current] = new Move(move);
    }

    private void SetNewWall(Move move)
    {
        GameBoard[move.Row, move.Column] = move.Value;
    }
}