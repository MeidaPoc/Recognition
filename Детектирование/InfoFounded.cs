using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Face_Recognition
{
    class InfoFounded
    {
        public int indexOfPix = 0;
        public string Name = "";
        public int x = 0;
        public int y = 0;
        public int x2 = 0;
        public int y2 = 0;
   
        public InfoFounded(int IndexOfPix, string name, int X,int Y, int X2, int Y2)
        {
            indexOfPix = IndexOfPix;
            Name = name;
            x = X;
            x2 = X2;
            y = Y;
            y2 = Y2;
        }

        public InfoFounded(int IndexOfPix, string name, int X, int Y)
        {
            indexOfPix = IndexOfPix;
            Name = name;
            x = X;
            y = Y;
        }

        public InfoFounded(int IndexOfPix, string name, Rectangle rect)
        {
            indexOfPix = IndexOfPix;
            Name = name;
            x = rect.X;
            x2 = rect.X + rect.Width;
            y = rect.Y;
            y2 = rect.Y + rect.Height;
        }

        public InfoFounded()
        {
            indexOfPix = 0;
            Name = "";
            x = 0;
            y = 0;
            x2 = 0;            
            y2 = 0;
        }
    }
}
