using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using gameBoard = Board;
using AIOpponent = AI;
using Photon.Pun;
using Photon.Realtime;

public class GameCore : ScriptableObject
{
    gameBoard board;

    public bool AIGame;
    public bool AIHard;
    public bool AIF;
    AIOpponent ComputerOpponent;

    gameBoard.Move LastMove = new gameBoard.Move(0, 0, 0);

    public GameCore()
    {
    }

    public void OnEnable()
    {
        board = gameBoard.CreateInstance<gameBoard>();
    }

    public void Init(bool AI, bool AIHard, bool AIFirst)
    {
        AIGame = AI;
        AIF = AIFirst;
        if (AIGame)
        {
            AIOpponent.Parameters prms = new AIOpponent.Parameters();
            prms.FWD = 2.223198962;
            prms.LAT = 9.340232866;
            prms.BKWD = -0.602717177;
            prms.JUMP = 8.222396163;
            prms.WIN = 134.5876635;
            prms.WALL = 5;
            prms.DIST = 10;

            ComputerOpponent = AIOpponent.CreateInstance<AIOpponent>();
            ComputerOpponent.Init(AIHard, AIFirst, prms);
        }
    }

    public bool ValidateMove(gameBoard.Move move)
    {
        Debug.Log("ValidateMove: " + move.Row + " " + move.Column + " " + move.Value);
        return board.ValidateMove(move);
    }

    public bool ValidateMove(string move)
    {
        return board.ValidateMove(ConvertStringToMove(move));
    }

    public string ConvertMoveToString(gameBoard.Move move)
    {
        return board.ConvertMoveToString(move);
    }

    public gameBoard.Move ConvertStringToMove(string move)
    {
        return board.ConvertStringToMove(move);
    }

    public gameBoard.Move GetFirstAIMove()
    {
        gameBoard.Move move = new gameBoard.Move(0, 0, 0);

        move = ComputerOpponent.GetMove();
        Debug.Log("AI Move: " + move.Row + " " + move.Column + " " + move.Value);

        return move;
    }

    public gameBoard.Move convertMove(gameBoard.Move move)
    {
        gameBoard.Move newMove = new gameBoard.Move(0, 0, 0);
        newMove.Row = (byte)(10 - move.Row);
        newMove.Column = (byte)(10 - move.Column);
        newMove.Value = move.Value;
        return newMove;
    }

    public void ProcessMove(gameBoard.Move move)
    {
        LastMove = new gameBoard.Move(move);

        if (!move.Equals(null))
        {
            if (ValidateMove(move))
            {
                board.MakeMove(move);
            }
        }
    }

    public gameBoard.Move GetMove()
    {
        gameBoard.Move newMove = new gameBoard.Move(GetOpponentMove());
        
        return newMove;
    }

    public bool CheckForVictory()
    {
        return board.CheckForEndGame();
    }

    public bool IsJump(gameBoard.Move move)
    {
        return board.MoveIsJump(move);
    }

    private gameBoard.Move GetOpponentMove()
    {
        gameBoard.Move move = new gameBoard.Move(0,0,0);

        if (AIGame)
        {
            move = ComputerOpponent.GetMove(LastMove);
            Debug.Log("AI Move: " + move.Row + " " + move.Column + " " + move.Value);
        }
        else
        {
            //network.GetMove();
        }
        return move;
    }
}