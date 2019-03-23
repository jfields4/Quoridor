using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using gameBoard = Board;
using AIOpponent = AI;

public class GameCore : MonoBehaviour
{
    gameBoard board = new gameBoard();

    bool AIGame;
    AIOpponent ComputerOpponent;

    gameBoard.Move LastMove;

    public GameCore(bool AIOption)
    {
        AIGame = AIOption;

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

            ComputerOpponent = new AIOpponent(true, false, prms);
        }
    }

    public bool ValidateMove(gameBoard.Move move)
    {
        return board.ValidateMove(move);
    }

    public bool ProcessMove(gameBoard.Move move)
    {
        bool result = false;
        
        LastMove = new gameBoard.Move(move);
        if (!move.Equals(null))
        {
            if (ValidateMove(move))
            {
                board.MakeMove(move);
                result = true;

                //if (!AIGame)
                //{
                //    network.SendMove(move);
                //}

                gameBoard.Move newMove = GetOpponentMove();

                //board.MakeMove(newMove);
            }
        }

        return result;
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
            //getAIMove(LastMove);
        }
        else
        {
            //network.GetMove();
        }

        return move;
    }
}