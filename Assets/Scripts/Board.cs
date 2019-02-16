using System.Collections.Generic;
using UnityEngine;
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

        public Move (byte r, byte c, sbyte v)
        {
            Row = r;
            Column = c;
            Value = v;
        }

        public Move() { }
    }

    //member variables
    private sbyte[,] GameBoard;
    const byte BoardSize = 10;

    private Move[] PlayerPositions;
    private byte Current;


    //constructor
    public Board()
    {
        GameBoard = new sbyte[BoardSize,BoardSize];
        PlayerPositions = new Move[] {
            new Move(9,5,0),
            new Move(1,5,0) };
    }

    //public functions

    public Move ConvertStringToMove(string moveSent)
    {
        Move move = new Move();

        move.Row = (byte)(10 - Int32.Parse(moveSent[1].ToString()));
        move.Column = (byte)(moveSent[0] - 140);

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

        moveS = ((char)move.Column).ToString();
        moveS += ((char)move.Row).ToString();

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
            move = moveSent;
        }
        else
        {
            move = PlayerPositions[Current];
        }

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

        return valid;
    }


    public bool ValidateMove(Move moveSent)
    {
        bool valid = false;
            
        Move move = new Move(0,0,0);
        
        if (moveSent < PlayerPositions[Current])
        {
            move = moveSent;
        }
        else
        {
            move = PlayerPositions[Current];
        }

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
        return valid;
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
        Debug.Log("New Location- Row: " + move.Row + " Column: " + move.Column);
    }

    private void SetNewWall(Move move)
    {
        GameBoard[move.Row, move.Column] = move.Value;
    }
}