using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Face_Recognition
{
    class InfoFounded
    {
        public int indexOfPix = 0;
        public string Name = "";
        public int x = 0;
        public int y = 0;   
   
        public InfoFounded(int IndexOfPix, string name, int X,int Y)
        {
            indexOfPix = IndexOfPix;
            Name = name;
            x = X;
            y = Y; 
        }

        public InfoFounded()
        {
            indexOfPix = 0;
            Name = "";
            x = 0;
            y = 0;
        }
    }
}
