using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Tile[,] TileGrid { get; private set; }

        public Bitmap AnalyseBitmap()
        {
            var bitmap = PinvokeHelpers.GetAdrenalineBitmap();
            this.TileGrid = GetTileGrid(bitmap);

            return bitmap;
        }

        public async Task Run()
        {
            var solver = new LevelSolver(this.TileGrid);
            var solution = solver.SolveLevel();

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
            var timeToHold = 30 * distance;

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

                    var rect = new System.Drawing.Rectangle(xPoint, yPoint, 20, 20);
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
