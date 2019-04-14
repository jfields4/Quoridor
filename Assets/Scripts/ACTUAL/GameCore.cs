using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using gameBoard = Board;
using AIOpponent = AI;

public class GameCore : ScriptableObject
{
    gameBoard board;

    public bool AIGame;
    public bool AIHard;
    AIOpponent ComputerOpponent;

    gameBoard.Move LastMove = new gameBoard.Move(0, 0, 0);

    public GameCore()
    {
    }

    public void OnEnable()
    {
        board = gameBoard.CreateInstance<gameBoard>();
        
    }

    public void Init(bool AI, bool AIHard)
    {
        AIGame = AI;
        if (AIGame)
        {
            AIOpponent.Parameters prms = new AIOpponent.Parameters();
            prms.FWD = 3;
            prms.LAT = 2;
            prms.BKWD = -1;
            prms.JUMP = 20;
            prms.WIN = 175;
            prms.WALL = 3;
            prms.DIST = 30;

            ComputerOpponent = AIOpponent.CreateInstance<AIOpponent>();
            ComputerOpponent.Init(AIHard, false, prms);
        }
    }

    public bool ValidateMove(gameBoard.Move move)
    {
        return board.ValidateMove(move);
    }

    public string ConvertMoveToString(gameBoard.Move move)
    {
        return board.ConvertMoveToString(move);
    }

    public void ProcessMove(gameBoard.Move move)
    {
        LastMove = new gameBoard.Move(move);

        if (!move.Equals(null))
        {
            if (ValidateMove(move))
            {
                board.MakeMove(move);

                //if (!AIGame)
                //{
                //    network.SendMove(move);
                //}
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
        }
        else
        {
            //network.GetMove();
        }
        return move;
    }
}