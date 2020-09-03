using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;

namespace AdrenalineSolver
{
    class Solver
    {
        public Bitmap Go()
        {
            var bitmap = PinvokeHelpers.GetAdrenalineBitmap();

            return bitmap;
        }
    }
}
