// Development start date: 1/26/2019
// Author: Hailey Fields

//https://int8.io/monte-carlo-tree-search-beginners-guide/


using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using Move = Board.Move;

public class AI : ScriptableObject
{
    public class Board
    {
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
                        //if not wall in front
                        if (GameBoard[PlayerPositions[Current].Row, PlayerPositions[Current].Column] != 1 &&
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
                        if (GameBoard[PlayerPositions[Current].Row, PlayerPositions[Current].Column - 1] != 1 &&
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
                        if (GameBoard[PlayerPositions[Current].Row - 1, PlayerPositions[Current].Column] != -1 &&
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
                        if (GameBoard[PlayerPositions[Current].Row - 1, PlayerPositions[Current].Column] != -1 &&
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
            PlayerPositions[Current] = move;
        }

        private void SetNewWall(Move move)
        {
            GameBoard[move.Row, move.Column] = move.Value;
            Walls[Current]--;
        }
    }
    public struct State
    {
        public byte[] walls;
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
    public Move GetMove(Move move)
    {
        Move selection = new Move(0, 0, 0);
        byte me = 1;
        if (firstPlayer)
        {
            me = 0;
        }
        gameBoard.MakeMove(move);
        if (move.Value != 0)
        {
            gameState.walls[1 - me] -= 1;
        }

        Move[] possibilities = CurrentPossibleMoves(me);

        Stopwatch sw = new Stopwatch();
        sw.Start();
        double[] values = new double[MAX_MOVES];
        double max = double.NegativeInfinity;
        int i = 0;
        int chosen = 0;
        foreach (Move m in possibilities)
        {
            if (m != selection && (m == gameState.previous[0] ||
                m == gameState.previous[1] ||
                m == gameState.previous[2] ||
                m == gameState.previous[3]))
            {
                values[i] = double.NegativeInfinity;
                if (values[i] >= max)
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
                        if (m != gameState.previous[0] &&
                            m != gameState.previous[1] &&
                            m != gameState.previous[2] &&
                            m != gameState.previous[3])
                        {
                            gameState.previous[gameState.oldest] = new Move(gameBoard.PlayerPositions[me]);
                            gameState.oldest++;
                            if (gameState.oldest >= NUM_PREV)
                            {
                                gameState.oldest = 0;
                            }
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
                if (m.Value != 0)
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

        if (selection.Value == 0)
        {
            if (selection != gameState.previous[0] &&
                selection != gameState.previous[1] &&
                selection != gameState.previous[2] &&
                selection != gameState.previous[3])
            {
                gameState.previous[gameState.oldest] = new Move(gameBoard.PlayerPositions[me]);
                gameState.oldest++;
                if (gameState.oldest >= NUM_PREV)
                {
                    gameState.oldest = 0;
                }
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

        int arrayPos = 0;

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
                    result[arrayPos] = new Move(selection);
                    legalMoves++;
                    arrayPos++;
                }
                if (jump)
                {
                    if (d == Direction.Forward || d == Direction.Backward)
                    {
                        selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.NotCurrent], Direction.Left, turn);
                        if (node.gameboard.ValidateMove(selection))
                        {
                            result[arrayPos] = new Move(selection);
                            legalMoves++;
                            arrayPos++;
                        }
                        selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.NotCurrent], Direction.Right, turn);
                        if (node.gameboard.ValidateMove(selection))
                        {
                            result[arrayPos] = new Move(selection);
                            legalMoves++;
                            arrayPos++;
                        }
                    }
                    else
                    {
                        selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.NotCurrent], Direction.Forward, turn);
                        if (node.gameboard.ValidateMove(selection))
                        {
                            result[arrayPos] = new Move(selection);
                            legalMoves++;
                            arrayPos++;
                        }
                        selection = ExecuteMove(node.gameboard.PlayerPositions[node.gameboard.NotCurrent], Direction.Backward, turn);
                        if (node.gameboard.ValidateMove(selection))
                        {
                            result[arrayPos] = new Move(selection);
                            legalMoves++;
                            arrayPos++;
                        }
                    }
                }
            }
        }

        if (node.currentstate.walls[turn] > 0)
        {
            GenerateWalls(result, node, turn, arrayPos);
        }

        return result;
    }

    private void GenerateWalls(Move[] possibilities, Node node, byte turn, int arrayPos)
    {
        if (node.gameboard.Turns < 8)
        {
            OpeningWalls(possibilities, node, turn, arrayPos);
        }
        else //if (node.gameboard.Turns < 20)
        {
            MiddleWalls(possibilities, node, turn, arrayPos);
        }
    }

    private void OpeningWalls(Move[] possibilities, Node node, byte turn, int arrayPos)
    {
        int i = arrayPos;
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

    private void MiddleWalls(Move[] possibilities, Node node, byte turn, int arrayPos)
    {
        int i = arrayPos;
        for (int row = (int)(node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Row - wallArea);
            row <= (int)(node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Row + wallArea - 1); row++)
        {
            for (int col = (int)(node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Column - wallArea);
                col < (int)(node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Column + wallArea - 1); col++)
            {
                if (row < 0 || col < 0 || row > 9 || col > 9) { }
                else if (node.gameboard.GameBoard[row, col] == 0)
                {
                    Move wall = new Move((byte)row, (byte)col, 1);
                    if (node.gameboard.ValidateMove(wall))
                    {
                        if (i > MAX_MOVES)
                        {
                            break;
                        }
                        possibilities[i] = wall;
                        i++;
                    }
                    wall = new Move((byte)row, (byte)col, -1);
                    if (node.gameboard.ValidateMove(wall))
                    {
                        if (i > MAX_MOVES)
                        {
                            break;
                        }
                        possibilities[i] = wall;
                        i++;
                    }

                }
            }
        }
    }

    private Move ExecuteMove(Move current, Direction togo, byte turn)
    {
        Move destination = new Move(current);

        if (turn == 0 && togo == Direction.Forward || turn != 0 && togo == Direction.Backward)
        {
            destination.Row = (byte)(destination.Row - 1);
        }
        else if (turn == 0 && togo == Direction.Backward || turn != 0 && togo == Direction.Forward)
        {
            destination.Row = (byte)(destination.Row + 1);
        }
        else if (turn == 0 && togo == Direction.Left || turn != 0 && togo == Direction.Right)
        {
            destination.Column = (byte)(destination.Column - 1);
        }
        else
        { // firstPlayer going Right or !firstPlayer going left
            destination.Column = (byte)(destination.Column + 1);
        }

        return destination;
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
            foreach (Move m in curPosMoves)
            {
                if (m != empty)
                {
                    Node toConsider = new Node(node.gameboard, node.currentstate);
                    if (m.Value != 0)
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
                    if (m.Value != 0)
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

    private bool Terminal(Node node, bool maximizingPlayer)
    {
        bool isTerminal = false;

        int cur = node.gameboard.PlayerPositions[node.gameboard.Current].Row;
        int notcur = node.gameboard.PlayerPositions[node.gameboard.NotCurrent].Row;
        if (cur == goals[node.gameboard.Current] || notcur == goals[1 - node.gameboard.Current])
        {
            isTerminal = true;
        }

        return isTerminal;
    }

    private double Evaluation(Node potential, bool maximizingPlayer, bool terminal)
    {
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
            if (OneAway(potential, i, maximizingPlayer))
            {
                score += evParam.WIN;
            }
            else if (TwoAway(potential, i, maximizingPlayer))
            {
                score += evParam.WIN / 2;
            }

            // includes: 
            //	* remaining walls
            score += potential.currentstate.walls[i];
            score -= potential.currentstate.walls[1 - i];

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

            //if (i == 0)
            //{
            //    for (int j = potential.gameboard.PlayerPositions[i].Row - 1; j > 1; j--)
            //    {
            //        int col = potential.gameboard.PlayerPositions[i].Column;

            //        if (potential.gameboard.GameBoard[j,col] != 0)
            //        {
            //            wallsByFirst++;
            //        }
            //        if (potential.gameboard.GameBoard[j, col-1] != 0)
            //        {
            //            wallsByFirst++;
            //        }
            //    }
            //    for (int j = potential.gameboard.PlayerPositions[1-i].Row; j < 8; j++)
            //    {
            //        int col = potential.gameboard.PlayerPositions[1-i].Column;

            //        if (potential.gameboard.GameBoard[j, col] != 0)
            //        {
            //            wallsBySecond++;
            //        }
            //        if (potential.gameboard.GameBoard[j, col - 1] != 0)
            //        {
            //            wallsBySecond++;
            //        }
            //    }
            //}
            //else
            //{
            //    for (int j = potential.gameboard.PlayerPositions[1-i].Row - 1; j > 1; j--)
            //    {
            //        int col = potential.gameboard.PlayerPositions[1-i].Column;

            //        if (potential.gameboard.GameBoard[j, col] != 0)
            //        {
            //            wallsByFirst++;
            //        }
            //        if (potential.gameboard.GameBoard[j, col - 1] != 0)
            //        {
            //            wallsByFirst++;
            //        }
            //    }
            //    for (int j = potential.gameboard.PlayerPositions[i].Row; j < 8; j++)
            //    {
            //        int col = potential.gameboard.PlayerPositions[i].Column;

            //        if (potential.gameboard.GameBoard[j, col] != 0)
            //        {
            //            wallsBySecond++;
            //        }
            //        if (potential.gameboard.GameBoard[j, col - 1] != 0)
            //        {
            //            wallsBySecond++;
            //        }
            //    }
            //}


            score -= wallsByFirst * evParam.WALL;
            score += wallsBySecond * evParam.WALL;
            score += firstMovement;
            score -= secondMovement;

            if (!maximizingPlayer)
            {
                score = score * (-1);
            }
        }

        return score;
    }

    private bool OneAway(Node node, int player, bool mp)
    {
        bool result = false;

        Move m = new Move(ExecuteMove(node.gameboard.PlayerPositions[player], Direction.Forward, (byte)player));

        if (node.gameboard.ValidateMove(m))
        {
            node.gameboard.MakeMove(m);
            if (Terminal(node, mp))
            {
                result = true;
            }
        }

        return result;
    }

    private bool CallOne(Node node, int player, bool mp, Direction d)
    {
        bool result = false;

        Move m = new Move(ExecuteMove(node.gameboard.PlayerPositions[player], d, (byte)player));
        if (node.gameboard.ValidateMove(m))
        {
            node.gameboard.MakeMove(m);
            if (OneAway(node, player, mp))
            {
                result = true;
            }
        }

        return result;
    }

    private bool TwoAway(Node node, int player, bool mp)
    {
        bool result = false;

        if (CallOne(node, player, mp, Direction.Forward))
        {
            return true;
        }
        if (CallOne(node, player, mp, Direction.Left))
        {
            return true;
        }
        if (CallOne(node, player, mp, Direction.Right))
        {
            return true;
        }

        return result;
    }


    private bool Adjacent(Move first, Move second)
    {
        bool adjacent = false;

        if (Math.Abs(first.Row - second.Row) == 1 ^
                Math.Abs(first.Column - second.Column) == 1)
        {
            adjacent = true;
        }

        return adjacent;
    }

    public void OnEnable()
    {
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
        
    }

    public void Init(bool lev, bool first, Parameters param)
    {
        level = lev;
        firstPlayer = first;
        evParam = param;
        if (lev)
        {
            wallArea = 1.75;
        }
        else
        {
            wallArea = 0.75;
        }
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

    private double wallArea;

    private Board gameBoard;
    private State gameState;
    private byte[] goals = { 1, 9 };
    private bool level;
    private bool firstPlayer;
}


