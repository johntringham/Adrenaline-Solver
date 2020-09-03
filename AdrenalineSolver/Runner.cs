using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static AdrenalineSolver.LevelSolver;

namespace AdrenalineSolver
{
    public class Runner
    {
        const int gridWidth = 11;
        const int gridHeight = 11;

        private static Color wallColor = Color.FromArgb(255, 203, 157, 122);
        private static Color floorColor = Color.FromArgb(255, 238, 223, 187);
        private static Color floorVisitedColor = Color.FromArgb(255, 224, 194, 151);
        private static Color playerColor = Color.FromArgb(255, 86, 51, 51);
        private static Color targetColor = Color.FromArgb(255, 156, 106, 93);

        private static Dictionary<Color, Tile> TileLookupDictionary = new Dictionary<Color, Tile>()
        {
            [wallColor] = Tile.Wall,
            [floorColor] = Tile.Floor,
            [floorVisitedColor] = Tile.Floor,
            [playerColor] = Tile.Player,
            [targetColor] = Tile.Target,
        };

        private Tile[,] tileGrid;
        private List<Move> solution;

        public Bitmap AnalyseBitmap()
        {
            var bitmap = PinvokeHelpers.GetAdrenalineBitmap();
            this.tileGrid = GetTileGrid(bitmap);

            var solver = new LevelSolver(this.tileGrid);
            this.solution = solver.SolveLevel().ToList();

            DrawSolutionOnBitmap(bitmap);

            return bitmap;
        }

        private void DrawSolutionOnBitmap(Bitmap bitmap)
        {
            var tileSize = bitmap.Width / gridWidth;
            var halfCellSize = tileSize / 2;

            foreach (var move in solution)
            {
                var startPos = (x:move.start.x * tileSize + halfCellSize, y: move.start.y * tileSize + halfCellSize);
                var endPos = (x:move.end.x * tileSize + halfCellSize, y: move.end.y * tileSize + halfCellSize);

                var top = Math.Min(startPos.y, endPos.y) - 3;
                var bottom = Math.Max(startPos.y, endPos.y) + 3;
                var left = Math.Min(startPos.x, endPos.x) - 3;
                var right = Math.Max(startPos.x, endPos.x) + 3;

                var rect = Rectangle.FromLTRB(left, top, right, bottom);
                DrawRectangleOnBitmap(bitmap, rect, Color.Green);
            }
        }

        public async Task Run()
        {
            await Task.Yield();

            if(solution == null)
            {
                throw new Exception("Can't execute solution as bitmap analysis failed");
            }

            foreach (var move in solution)
            {
                await ExecuteMove(move);
            }
        }

        private async Task ExecuteMove(Move move)
        {
            WindowsVirtualKey virtualKey = WindowsVirtualKey.Up;

            switch (move.direction)
            {
                case Direction.Up:
                    virtualKey = WindowsVirtualKey.Up;
                    break;
                case Direction.Down:
                    virtualKey = WindowsVirtualKey.Down;
                    break;
                case Direction.Left:
                    virtualKey = WindowsVirtualKey.Left;
                    break;
                case Direction.Right:
                    virtualKey = WindowsVirtualKey.Right;
                    break;

                case Direction.None:
                    return;
            }

            var distance = Math.Max(Math.Abs(move.start.x - move.end.x), Math.Abs(move.start.y - move.end.y));
            var timeToHold = (30 * distance) + MainWindow.Delay;

            await PinvokeHelpers.SendKeyPress(virtualKey, timeToHold);
        }

        private Tile[,] GetTileGrid(Bitmap bitmap)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;

            var cellWidth = width / gridWidth;
            var cellHeight = height / gridHeight;

            var halfWidth = cellWidth / 2;
            var halfHeight = cellHeight / 2;

            Tile[,] grid = new Tile[gridWidth, gridHeight];

            HashSet<Color> colors = new HashSet<Color>();

            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridWidth; y++)
                {
                    var xPoint = cellWidth * x + halfWidth;
                    var yPoint = cellHeight * y + halfHeight;

                    var pixelColor = bitmap.GetPixel(xPoint, yPoint);
                    colors.Add(pixelColor);

                    grid[x, y] = GetTileFromBitmapColor(pixelColor);

                    var rect = new System.Drawing.Rectangle(xPoint - halfWidth, yPoint - halfHeight, cellWidth, cellHeight);
                    DrawRectangleOnBitmap(bitmap, rect, pixelColor);
                }
            }

            return grid;
        }

        public void DrawRectangleOnBitmap(Bitmap bitmap, Rectangle rect, Color color)
        {
            for(var x = rect.Left; x<rect.Right; x++)
            {
                for(var y = rect.Top; y < rect.Bottom; y++)
                {
                    bitmap.SetPixel(x, y, color);
                }
            }
        }

        public Tile GetTileFromBitmapColor(Color color)
        {
            if (TileLookupDictionary.ContainsKey(color))
            {
                return TileLookupDictionary[color];
            }

            throw new Exception("Unexpected color found in bitmap - check the game window is fully visible");
        }
    }
}
