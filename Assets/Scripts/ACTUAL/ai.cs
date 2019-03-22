// Development start date: 1/26/2019
// Author: Hailey Fields

//https://int8.io/monte-carlo-tree-search-beginners-guide/

using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using static Board;
using static Board.Move;

public class AI
{
    public struct State
    {
        public byte [] walls;
        public Move[] previous;
        public int oldest;

        public State(State other)
        {
            walls = new byte[2];
            walls[0] = other.walls[0];
            walls[1] = other.walls[1];

            previous = new Move[NUM_PREV];
            for (int i = 0; i < NUM_PREV; i++)
            {
                previous[i] = new Move(other.previous[i]);
            }
            oldest = other.oldest;
        }
    }
    public struct Node
    {
        public Node(Board currentboard, State state)
        {
            gameboard = new Board(currentboard);
            currentstate = new State(state);
        }

        public State currentstate;
        public readonly Board gameboard;
    }

    enum Direction { Forward, Left, Right, Backward };

    public Move GetMove()
    {
        Move move = new Move(0, 0, 0);
        if (firstPlayer)
        {
            move = ExecuteMove(gameBoard.PlayerPositions[0], Direction.Forward, 0);
            gameState.previous[gameState.oldest] = new Move(gameBoard.PlayerPositions[0]);
            gameState.oldest++;
            
            gameBoard.MakeMove(move);
            return move;
        }
        return move;
    }
    public Move GetMove(Move move) {
        Move selection = new Move(0, 0, 0);
        byte me = 1;
        if (firstPlayer)
        {
            me = 0;
        }
        gameBoard.MakeMove(move);
        //if (gameBoard.CheckForEndGame())
        //{
        //    return selection;
        //}
        if (move.Value != 0)
        {
            gameState.walls[1-me] -= 1;
        }

        Move[] possibilities = CurrentPossibleMoves(me);

        if (ReadyForAlphaBeta()) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double[] values = new double[MAX_MOVES];
            double max = -INF - 1;
            int i = 0;
            int chosen = 0;
            foreach (Move m in possibilities)
            {                
                if (m != selection && (m == gameState.previous[0] ||
                    m == gameState.previous[1] ||
                    m == gameState.previous[2] ||
                    m == gameState.previous[3]))
                {
                    values[i] = -evParam.WIN;
                    if (values[i] > max)
                    {
                        chosen = i;
                        max = values[i];
                    }
                }
                else if (m != selection)
                {
                    if (sw.ElapsedMilliseconds > MAX_TIME)
                    {
                        if (possibilities[chosen].Value == 0)
                        {
                            gameState.previous[gameState.oldest] = new Move(gameBoard.PlayerPositions[me]);
                            gameState.oldest++;
                            if (gameState.oldest >= NUM_PREV)
                            {
                                gameState.oldest = 0;
                            }
                        }
                        else
                        {
                            gameState.walls[me] -= 1;
                        }

                        gameBoard.MakeMove(possibilities[chosen]);
                        return possibilities[chosen];
                    }
                    Node toConsider = new Node(gameBoard, gameState);
                    toConsider.gameboard.MakeMove(m);
                    if (m.Value == 0)
                    {
                        //toConsider.currentstate.previous[toConsider.currentstate.oldest] = new Move(toConsider.gameboard.PlayerPositions[toConsider.currentstate.oldest]);
                        //toConsider.currentstate.oldest++;
                        //if (toConsider.currentstate.oldest == NUM_PREV)
                        //{
                        //    toConsider.currentstate.oldest = 0;
                        //}
                    }
                    else
                    {
                        toConsider.currentstate.walls[me] -= 1;
                    }
                    values[i] = AlphaBeta(toConsider, DEPTH, -INF, INF, !firstPlayer);
                    if (values[i] > max)
                    {
                        chosen = i;
                        max = values[i];
                    }
                }
                i++;
            }
            selection = new Move(possibilities[chosen]);
        }
        else {
            bool found = false;
            for (int i = 0; !found && i < MAX_MOVES; i++)
            {
                if (possibilities[i] != selection)
                {
                    selection = new Move(possibilities[i]);
                    found = true;
                }
            }
        }

        if (selection.Value == 0)
        {
            gameState.previous[gameState.oldest] = new Move(gameBoard.PlayerPositions[me]);
            gameState.oldest++;
            if (gameState.oldest >= NUM_PREV)
            {
                gameState.oldest = 0;
            }
        }
        else
        {
            gameState.walls[me] -= 1;
        }

        gameBoard.MakeMove(selection);
        return selection;
    }

    public Move[] CurrentPossibleMoves(byte turn)
    {
        Node node = new Node(gameBoard, gameState);

        return CurrentPossibleMoves(node, turn);
    }

    public Move[] CurrentPossibleMoves(Node node, byte turn)
    {
        Move[] result = new Move[MAX_MOVES];
        for (int i = 0; i < MAX_MOVES; i++)
        {
            result[i] = new Move(0, 0, 0);
        }

        if (node.gameboard.Turns != 4)
        {
            Move selection;
            int legalMoves = 0;
            bool jump;
            for (Direction d = Direction.Forward; d <= Direction.Backward; d++)
            {
                jump = false;
                selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.Current], d, turn);
                if (selection == node.gameboard.PlayerPositions[node.gameboard.NotCurrent])
                {
                    selection = ExecuteMove(selection, d, turn);
                    jump = true;
                }
                else if (node.gameboard.ValidateMove(selection))
                {
                    result[(int)d] = new Move(selection);
                    legalMoves++;
                }
                if (jump)
                {
                    if (d == Direction.Forward || d == Direction.Backward)
                    {
                        selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.NotCurrent], Direction.Left, turn);
                        if (node.gameboard.ValidateMove(selection))
                        {
                            result[4] = new Move(selection);
                            legalMoves++;
                        }
                        selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.NotCurrent], Direction.Right, turn);
                        if (node.gameboard.ValidateMove(selection))
                        {
                            result[5] = new Move(selection);
                            legalMoves++;
                        }
                    }
                    else
                    {
                        selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.NotCurrent], Direction.Forward, turn);
                        if (node.gameboard.ValidateMove(selection))
                        {
                            result[4] = new Move(selection);
                            legalMoves++;
                        }
                        selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.NotCurrent], Direction.Backward, turn);
                        if (node.gameboard.ValidateMove(selection))
                        {
                            result[5] = new Move(selection);
                            legalMoves++;
                        }
                    }
                }
            }
        }
        
        if (node.currentstate.walls[turn] > 0)
        {
            GenerateWalls(result, node, turn);
        }

        //TEMPORARY FIX
        //result[1] = new Move(0, 0, 0);

        return result;
    }

    private void GenerateWalls(Move[] possibilities, Node node, byte turn)
    {
        // Start walls at index 6

        if (node.gameboard.Turns < 8)
        {
            OpeningWalls(possibilities, node, turn);
        }
        else //if (node.gameboard.Turns < 20)
        {
            MiddleWalls(possibilities, node, turn);
        }
    }

    private void OpeningWalls (Move[] possibilities, Node node, byte turn)
    {
        int i = 6;
        if (level && turn == 0)
        {
            Move wall = new Move(3, 4, 1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(3, 5, -1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(4, 4, 1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
        }
        else if (!level && turn == 0)
        {
            Move wall = new Move(2, 5, 1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(4, 5, 1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(4, 3, -1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(4, 5, -1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
        }
        else if (level && turn == 1)
        {
            Move wall = new Move(7, 6, 1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(7, 5, -1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(4, 6, 1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
        }
        else
        {
            Move wall = new Move(8, 5, 1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(6, 5, 1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(6, 7, -1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
            wall = new Move(6, 5, -1);
            if (node.gameboard.ValidateMove(wall))
            {
                possibilities[i] = wall;
                i++;
            }
        }
    }

    private void MiddleWalls (Move[] possibilities, Node node, byte turn)
    {
        int i = 6;
        for (int row = node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Row - 2; 
            row <= node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Row + 1; row++)
        {
            for (int col = node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Column - 1; 
                col < node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Column; col++)
            {
                if (row < 0 || col < 0 || row > 9 || col > 9) { }
                else if (node.gameboard.GameBoard[row, col] == 0)
                {
                    Move wall = new Move((byte)row, (byte)col, 1);
                    if (node.gameboard.ValidateMove(wall))
                    {
                        possibilities[i] = wall;
                        i++;
                    }
                    wall = new Move((byte)row, (byte)col, -1);
                    if (node.gameboard.ValidateMove(wall))
                    {
                        possibilities[i] = wall;
                        i++;
                    }

                }
            }
        }
    }

    private Move ExecuteMove(Move current, Direction togo, byte turn) {
        Move destination = new Move(current);

        if (turn == 0 && togo == Direction.Forward || turn != 0 && togo == Direction.Backward) {
            destination.Row = (byte)(destination.Row - 1);
        }
        else if (turn == 0 && togo == Direction.Backward || turn != 0 && togo == Direction.Forward) {
            destination.Row = (byte)(destination.Row + 1);
        }
        else if (turn == 0 && togo == Direction.Left || turn != 0 && togo == Direction.Right) {
            destination.Column = (byte)(destination.Column - 1);
        }
        else { // firstPlayer going Right or !firstPlayer going left
            destination.Column = (byte)(destination.Column + 1);
        }

        return destination;
    }
        
    private bool ReadyForAlphaBeta() {
        bool isReady = true;

        return isReady;
    }

    private double AlphaBeta(Node node, int depth, double alpha, double beta, bool maximizingPlayer)
    {
        bool terminal = Terminal(node, maximizingPlayer);

        if (depth == 0 || terminal)
        {
            return Evaluation(node, maximizingPlayer, terminal);
        }

        Move empty = new Move(0, 0, 0);
        byte turn = 1;
        if (maximizingPlayer == firstPlayer)
        {
            turn = 0;
        }
        Move[] curPosMoves = CurrentPossibleMoves(node, turn);
        if (maximizingPlayer)
        {
            foreach (Move m in curPosMoves) {
                if (m != empty)
                {
                    Node toConsider = new Node(node.gameboard, node.currentstate);
                    if (m.Value == 0)
                    {
                        //toConsider.currentstate.previous[toConsider.currentstate.oldest] = new Move(toConsider.gameboard.PlayerPositions[turn]);
                        //toConsider.currentstate.oldest++;
                        //if (toConsider.currentstate.oldest == NUM_PREV)
                        //{
                        //    toConsider.currentstate.oldest = 0;
                        //}
                    }
                    else
                    {
                        toConsider.currentstate.walls[turn] -= 1;
                    }
                    double value = AlphaBeta(toConsider, depth - 1, alpha, beta, !maximizingPlayer);
                    alpha = Math.Max(alpha, value);
                    if (beta <= alpha)
                    {
                        return beta;
                    }
                }
            }
            return alpha;
        }
        else
        {
            foreach (Move m in curPosMoves)
            {
                if (m != empty)
                {
                    Node toConsider = new Node(node.gameboard, node.currentstate);
                    toConsider.gameboard.MakeMove(m);
                    if (m.Value == 0)
                    {
                        //toConsider.currentstate.previous[toConsider.currentstate.oldest] = new Move(toConsider.gameboard.PlayerPositions[turn]);
                        //toConsider.currentstate.oldest++;
                        //if (toConsider.currentstate.oldest == NUM_PREV)
                        //{
                        //    toConsider.currentstate.oldest = 0;
                        //}
                    }
                    else
                    {
                        toConsider.currentstate.walls[turn] -= 1;
                    }
                    double value = AlphaBeta(toConsider, depth - 1, alpha, beta, !maximizingPlayer);

                    beta = Math.Min(value, beta);

                    if (beta <= alpha)
                    {
                        return alpha;
                    }
                }
            }
            return beta;
        }
    }

    private bool Terminal(Node node, bool maximizingPlayer) {
        bool isTerminal = false;
        
        int cur = node.gameboard.PlayerPositions[node.gameboard.Current].Row;
        int notcur = node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Row;
        if (cur == goals[node.gameboard.Current] || notcur == goals[1- node.gameboard.Current])
        {
            isTerminal = true;
        }

        return isTerminal;
    }

    private double Evaluation(Node potential, bool maximizingPlayer, bool terminal) {
        double score = 0;
        int i;
        if (maximizingPlayer == firstPlayer)
        {
            i = 0;
        }
        else
        {
            i = 1;
        }

        if (Terminal(potential, maximizingPlayer))
        {
            if (potential.gameboard.PlayerPositions[potential.gameboard.Current].Row == goals[potential.gameboard.Current])
            {
                score = evParam.WIN;
            }
            else
            {
                score = -evParam.WIN;
            }
        }
        else
        {
            // includes: 
            //	* remaining walls
            score += potential.currentstate.walls[i];
            score -= potential.currentstate.walls[1-i];

            //	? availability of jump (for next player)
            if (potential.gameboard.PlayerPositions[potential.gameboard.Current].Column ==
                potential.gameboard.PlayerPositions[potential.gameboard.NotCurrent].Column)
            {
                if (Math.Abs(potential.gameboard.PlayerPositions[potential.gameboard.Current].Row -
                    potential.gameboard.PlayerPositions[potential.gameboard.NotCurrent].Row) == 1)
                {

                    if (Math.Abs(goals[i] - potential.gameboard.PlayerPositions[potential.gameboard.Current].Row) >
                        Math.Abs(goals[i] - potential.gameboard.PlayerPositions[potential.gameboard.NotCurrent].Row))
                    {
                        score = (short)(score - evParam.JUMP);
                    }
                }
            }

            // Directional availability
            double firstMovement = 0;
            double secondMovement = 0;

            int firstRow, firstCol, secondRow, secondCol;

            firstRow = potential.gameboard.PlayerPositions[0].Row;
            firstCol = potential.gameboard.PlayerPositions[0].Column;

            if (potential.gameboard.GameBoard[firstRow - 1, firstCol - 1] <= 0 &&
                potential.gameboard.GameBoard[firstRow - 1, firstCol] <= 0)
                firstMovement += evParam.BKWD;
            if (potential.gameboard.GameBoard[firstRow, firstCol - 1] <= 0 &&
                potential.gameboard.GameBoard[firstRow, firstCol] <= 0)
                firstMovement += evParam.FWD;
            if (potential.gameboard.GameBoard[firstRow - 1, firstCol] <= 0 &&
                potential.gameboard.GameBoard[firstRow, firstCol] <= 0)
                firstMovement += evParam.LAT;
            if (potential.gameboard.GameBoard[firstRow, firstCol - 1] <= 0 &&
                potential.gameboard.GameBoard[firstRow, firstCol] <= 0)
                firstMovement += evParam.LAT;

            secondRow = potential.gameboard.PlayerPositions[0].Row;
            secondCol = potential.gameboard.PlayerPositions[0].Column;

            if (potential.gameboard.GameBoard[secondRow - 1, secondRow - 1] <= 0 &&
                potential.gameboard.GameBoard[secondRow - 1, secondRow] <= 0)
                secondMovement += evParam.BKWD;
            if (potential.gameboard.GameBoard[secondRow, secondRow - 1] <= 0 &&
                potential.gameboard.GameBoard[secondRow, secondRow] <= 0)
                secondMovement += evParam.FWD;
            if (potential.gameboard.GameBoard[secondRow - 1, secondRow] <= 0 &&
                potential.gameboard.GameBoard[secondRow, secondRow] <= 0)
                secondMovement += evParam.LAT;
            if (potential.gameboard.GameBoard[secondRow, secondRow - 1] <= 0 &&
                potential.gameboard.GameBoard[secondRow, secondRow] <= 0)
                secondMovement += evParam.LAT;

            //Distance from goal
            score -= (evParam.DIST * Math.Abs(potential.gameboard.PlayerPositions[potential.gameboard.Current].Row - goals[i]));
            score += (evParam.DIST * Math.Abs(potential.gameboard.PlayerPositions[potential.gameboard.NotCurrent].Row - goals[1 - i]));

            // Count walls around players
            int wallsByFirst = 0;
            int wallsBySecond = 0;
            //for (int rowMod = -2; rowMod <= 2; rowMod++)
            //{
            //    for (int colMod = -2; colMod < 2; colMod++)
            //    {
            //        firstRow = potential.gameboard.PlayerPositions[0].Row + rowMod;
            //        firstCol = potential.gameboard.PlayerPositions[0].Column + colMod;
            //        if (firstRow < 0 || firstCol < 0 || firstRow > 9 || firstCol > 9) { }
            //        else if (potential.gameboard.GameBoard[firstRow, firstCol] != 0)
            //        {
            //            wallsByFirst++;
            //        }
            //        secondRow = potential.gameboard.PlayerPositions[0].Row + rowMod;
            //        secondCol = potential.gameboard.PlayerPositions[0].Column + colMod;
            //        if (secondRow < 0 || secondCol < 0 || secondRow > 9 || secondCol > 9) { }
            //        else if (potential.gameboard.GameBoard[secondRow, secondCol] != 0)
            //        {
            //            wallsBySecond++;
            //        }
            //    }
            //}

            if (i == 0)
            {
                for (int j = potential.gameboard.PlayerPositions[i].Row - 1; j > 1; j--)
                {
                    int col = potential.gameboard.PlayerPositions[i].Column;
                   
                    if (potential.gameboard.GameBoard[j,col] != 0)
                    {
                        wallsByFirst++;
                    }
                    if (potential.gameboard.GameBoard[j, col-1] != 0)
                    {
                        wallsByFirst++;
                    }
                }
                for (int j = potential.gameboard.PlayerPositions[1-i].Row; j < 8; j++)
                {
                    int col = potential.gameboard.PlayerPositions[1-i].Column;

                    if (potential.gameboard.GameBoard[j, col] != 0)
                    {
                        wallsBySecond++;
                    }
                    if (potential.gameboard.GameBoard[j, col - 1] != 0)
                    {
                        wallsBySecond++;
                    }
                }
            }
            else
            {
                for (int j = potential.gameboard.PlayerPositions[1-i].Row - 1; j > 1; j--)
                {
                    int col = potential.gameboard.PlayerPositions[1-i].Column;

                    if (potential.gameboard.GameBoard[j, col] != 0)
                    {
                        wallsByFirst++;
                    }
                    if (potential.gameboard.GameBoard[j, col - 1] != 0)
                    {
                        wallsByFirst++;
                    }
                }
                for (int j = potential.gameboard.PlayerPositions[i].Row; j < 8; j++)
                {
                    int col = potential.gameboard.PlayerPositions[i].Column;

                    if (potential.gameboard.GameBoard[j, col] != 0)
                    {
                        wallsBySecond++;
                    }
                    if (potential.gameboard.GameBoard[j, col - 1] != 0)
                    {
                        wallsBySecond++;
                    }
                }
            }

            
            score -= wallsByFirst * evParam.WALL;
            score += wallsBySecond * evParam.WALL;
            score += firstMovement;
            score -= secondMovement;
            
            if (!maximizingPlayer)
            {
                score = score * (-1);
            }
        }

        //Console.WriteLine("Current: Row " + potential.gameboard.PlayerPositions[potential.gameboard.Current].Row +
        //    " Column " + potential.gameboard.PlayerPositions[potential.gameboard.Current].Column);
        //Console.WriteLine("Not Current: Row " + potential.gameboard.PlayerPositions[potential.gameboard.NotCurrent].Row +
        //    " Column " + potential.gameboard.PlayerPositions[potential.gameboard.NotCurrent].Column);
        //Console.WriteLine("Score = " + score + "\n");

        return score;
    }

    private int ShortestPath (Node node)
    {
        int result = 0;

        return result;
    }

    private bool Adjacent (Move first, Move second)
    {
        bool adjacent = false;

        if (Math.Abs(first.Row - second.Row) == 1 ^
                Math.Abs(first.Column - second.Column) == 1)
        {
            adjacent = true;
        }

        return adjacent;
    }

    public AI(bool lev, bool first, Parameters param) {
        level = lev;
        firstPlayer = first;
        gameBoard = new Board();
        gameState.walls = new byte[2];
        
        gameState.walls[0] = 10;
        gameState.walls[1] = 10;

        gameState.previous = new Move[NUM_PREV];
        for (int i = 0; i < NUM_PREV; i++)
        {
            gameState.previous[i] = new Move(0, 0, 0);
        }
        gameState.oldest = 0;
		
		evParam = param;		
    }

    // This does not exist
    private AI() { }

    public struct Parameters
    {
        public double FWD;
        public double LAT;
        public double BKWD;
        public double JUMP;
        public double WIN;
        public double WALL;
        public double DIST;
    }
    // Parameters for evaluation, play around to find best values
    private Parameters evParam;

    // Game constants (N.B. DEPTH is best when odd)
    private const short INF = 1000;
    private const byte DEPTH = 5;
    private const byte MAX_MOVES = 50;
    private const int MAX_TIME = 5000;
    private const byte NUM_PREV = 4;
    
    private Board gameBoard;
    private State gameState;
    private readonly byte[] goals = { 1,9 };
    private readonly bool level;
    private readonly bool firstPlayer;
}


