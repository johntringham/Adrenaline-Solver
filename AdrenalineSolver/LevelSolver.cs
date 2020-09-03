using System;
using System.Collections.Generic;
using System.Linq;

namespace AdrenalineSolver
{
    internal class LevelSolver
    {
        private Tile[,] tileGrid;

        public LevelSolver(Tile[,] tileGrid)
        {
            this.tileGrid = tileGrid;
        }

        internal IEnumerable<Move> SolveLevel()
        {
            var startPosition = FindPosition(Tile.Player);
            var targetPosition = FindPosition(Tile.Target);

            var lastMove = SolveForLastMove(startPosition, targetPosition);

            var moveList = new List<Move>();
            var move = lastMove;
            while(move.previousMove != null)
            {
                moveList.Add(move);
                move = move.previousMove;
            }   

            return moveList.Reverse<Move>();
        }

        private Move SolveForLastMove(GridPosition position, GridPosition targetPosition)
        {
            var positionsToInspect = new Queue<Move>();
            positionsToInspect.Enqueue(new Move(position, position, Direction.None, null));

            var positionsInspected = new HashSet<GridPosition>();

            while (positionsToInspect.Any())
            {
                var moveBeingInspected = positionsToInspect.Dequeue();
                var moves = GetPotentialMovesFromPoint(moveBeingInspected.end);

                foreach(var neighbourMove in moves)
                {
                    if (!positionsInspected.Contains(neighbourMove.end))
                    {
                        neighbourMove.previousMove = moveBeingInspected;
                        positionsToInspect.Enqueue(neighbourMove);
                    }

                    if (neighbourMove.end.Equals(targetPosition))
                    {
                        // found it - return this move
                        
                        return neighbourMove;
                    }
                }

                positionsInspected.Add(moveBeingInspected.end);
            }

            throw new Exception("Can't solve maze :(");
        }

        public IEnumerable<Move> GetMovesForPlayer()
        {
            var startPosition = FindPosition(Tile.Player);
            return GetPotentialMovesFromPoint(startPosition);
        }

        public IEnumerable<Move> GetPotentialMovesFromPoint(GridPosition position)
        {
            var moves = Directions.Select(d => GetMoveForDirection(position, d)).Where(m => m != null);
            return moves.Select(m => m!);
        }

        private Move? GetMoveForDirection(GridPosition position, Direction direction)
        {
            var offset = GetOffsetForDirection(direction);

            GridPosition endPosition = position;
            GridPosition testingPosition = position;

            while (true)
            {
                testingPosition = endPosition.Plus(offset);

                var tile = GetTileForPosition(testingPosition);
                if(tile == null)
                {
                    throw new Exception("Invalid tile position used for solving calculations");
                }
                
                if(tile == Tile.Wall)
                {
                    break;
                }
                else if (tile == Tile.Target)
                {
                    endPosition = testingPosition;
                    break;
                }
                else
                {
                    endPosition = testingPosition;
                }
            }

            if (endPosition.Equals(position))
            {
                // Haven't actually moved anywhere, return null
                return null;
            }

            return new Move(position, endPosition, direction);
        }

        private Tile? GetTileForPosition(GridPosition position)
        {
            if(position.x > tileGrid.GetLength(0) || position.x < 0 || position.y > tileGrid.GetLength(1) || position.y < 0)
            {
                return null;
            }

            return tileGrid[position.x, position.y];
        }

        private GridPosition FindPosition(Tile tile)
        {
            for (var x = 0; x < tileGrid.GetLength(0); x++)
            {
                for (var y = 0; y < tileGrid.GetLength(0); y++)
                {
                    if (this.tileGrid[x, y] == tile)
                    {
                        return new GridPosition(x, y);
                    }
                }
            }

            throw new Exception("Can't find tile of type" + tile);
        }

        public struct GridPosition
        {
            public readonly int x;
            public readonly int y;

            public GridPosition(int x, int y) : this()
            {
                this.x = x;
                this.y = y;
            }

            public GridPosition Plus ((int xOffset, int yOffset) offset)
            {
                return new GridPosition(x + offset.xOffset, y + offset.yOffset);
            }
        }

        public class Move
        {
            public readonly Direction direction;
            public readonly GridPosition start;
            public readonly GridPosition end;

            public Move? previousMove { get; set; }

            public Move(GridPosition start, GridPosition end, Direction direction, Move? previousMove = null)
            {
                this.end = end;
                this.start = start;
                this.direction = direction;

                this.previousMove = previousMove;
            }
        }

        private (int xOffset, int yOffset) GetOffsetForDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return (0, -1);
                case Direction.Down:
                    return (0, 1);
                case Direction.Left:
                    return (-1, 0);
                case Direction.Right:
                    return (1, 0);
            }

            throw new Exception("Unexpected direction value: " + direction);
        }

        private IEnumerable<Direction> Directions
        {
            get
            {
                yield return Direction.Up;
                yield return Direction.Down;
                yield return Direction.Left;
                yield return Direction.Right;
            }
        }
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None,
    }
}