﻿//------------------------------------------------------------------------------
/*
 * TowerD (2023)
 * User: Kurt Gav
 * Date: 11/4/2023
 * Time: 3:05 pm
 * 
 */
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense
{
    internal static class GridParams
    {
        public const int GridSizeX = 16; //how many tiles per row
        public const int GridSizeY = 12; //how many tiles per column
        public const int TileSize = 50; //how many pixels is one tile long
        public const int StartX = 160;
        public const int StartY = 120; //starting position (top-left) for painting grid
        public const int FormTitleSize=28;
        public const int PaddingSize = 0;
    }
}
