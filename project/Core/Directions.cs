/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace ZRLib.Core
{
    public sealed class Directions : FlagSet
    {
        public const int DtNone = 0;

        public const int DtNorth = 1;
        public const int DtSouth = 2;
        public const int DtWest = 3;
        public const int DtEast = 4;

        public const int DtNorthWest = 5;
        public const int DtNorthEast = 6;
        public const int DtSouthWest = 7;
        public const int DtSouthEast = 8;

        public const int DtZenith = 9;
        public const int DtNadir = 10;
        public const int DtPlace = 11;
        // TODO: checkit and remove


        public const int DtFirst = 1;
        public const int DtLast = 4;
        public const int DtFlatFirst = 1;
        public const int DtFlatLast = 8;

        public Directions(params int[] args)
            : base(args)
        {
        }

        public Directions(Directions set)
            : base(set)
        {
        }

        public static readonly DirectionRec[] Data;
        private static readonly int[] Idx2Dir;
        public static int[] IsoTrans;

        static Directions()
        {
            Data = new DirectionRec[12];
            Data[0] = new DirectionRec(0, 0, 0, Directions.DtNone, '?', '?');
            Data[1] = new DirectionRec(0, -1, 0, Directions.DtSouth, 'N', 'S');
            Data[2] = new DirectionRec(0, 1, 0, Directions.DtNorth, 'S', 'N');
            Data[3] = new DirectionRec(-1, 0, 0, Directions.DtEast, 'W', 'E');
            Data[4] = new DirectionRec(1, 0, 0, Directions.DtWest, 'E', 'W');
            Data[5] = new DirectionRec(-1, -1, 0, Directions.DtSouthEast, '\0', '\0');
            Data[6] = new DirectionRec(1, -1, 0, Directions.DtSouthWest, '\0', '\0');
            Data[7] = new DirectionRec(-1, 1, 0, Directions.DtNorthEast, '\0', '\0');
            Data[8] = new DirectionRec(1, 1, 0, Directions.DtNorthWest, '\0', '\0');
            Data[9] = new DirectionRec(0, 0, -1, Directions.DtNadir, '\0', '\0');
            Data[10] = new DirectionRec(0, 0, 1, Directions.DtZenith, '\0', '\0');
            Data[11] = new DirectionRec(0, 0, 0, Directions.DtNone, '\0', '\0');

            Idx2Dir = new int[] {
                Directions.DtNone,
                Directions.DtSouthWest,
                Directions.DtSouth,
                Directions.DtSouthEast,
                Directions.DtWest,
                Directions.DtPlace,
                Directions.DtEast,
                Directions.DtNorthWest,
                Directions.DtNorth,
                Directions.DtNorthEast
            };

            IsoTrans = new int[] {
                Directions.DtNone,
                Directions.DtNorthWest,
                Directions.DtSouthEast,
                Directions.DtSouthWest,
                Directions.DtNorthEast,
                Directions.DtWest,
                Directions.DtNorth,
                Directions.DtSouth,
                Directions.DtEast,
                Directions.DtZenith,
                Directions.DtNadir,
                Directions.DtPlace
            };
        }

        public sealed class DirectionRec
        {
            public int DX;
            public int DY;
            public int DZ;
            public int Opposite;
            public char SymDirect;
            public char SymOpposite;

            public DirectionRec(int dX, int dY, int dZ, int opposite, char symDirect, char symOpposite)
            {
                DX = dX;
                DY = dY;
                DZ = dZ;
                Opposite = opposite;
                SymDirect = symDirect;
                SymOpposite = symOpposite;
            }
        }

        /// <summary>
        /// Returns direction of movements by changes of coordinates. </summary>
        /// <param name="pX"> Old x-coord </param>
        /// <param name="pY"> Old y-coord </param>
        /// <param name="newX"> New x-coord </param>
        /// <param name="newY"> New y-coord </param>
        /// <returns> Direction of movement </returns>
        public static int GetDirByCoords(int pX, int pY, int newX, int newY)
        {
            int dx = Math.Sign(newX - pX);
            int dy = Math.Sign(newY - pY);
            int dir = (8 + dx - (dy + 1) * 3);
            return Idx2Dir[dir];
        }
    }
}
