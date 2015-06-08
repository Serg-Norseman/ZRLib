/*
 *  "JZRLib", Java Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
package jzrlib.core;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Directions extends jzrlib.core.FlagSet
{
    public static final int dtNone = 0;

    public static final int dtNorth = 1;
    public static final int dtSouth = 2;
    public static final int dtWest = 3;
    public static final int dtEast = 4;

    public static final int dtNorthWest = 5;
    public static final int dtNorthEast = 6;
    public static final int dtSouthWest = 7;
    public static final int dtSouthEast = 8;

    public static final int dtZenith = 9;
    public static final int dtNadir = 10;
    public static final int dtPlace = 11; // TODO: checkit and remove


    public static final int dtFirst = 1;
    public static final int dtLast = 4;
    public static final int dtFlatFirst = 1;
    public static final int dtFlatLast = 8;

    public Directions(int... args)
    {
        super(args);
    }

    public Directions(Directions set)
    {
        super(set);
    }

    public static final DirectionRec[] Data;
    private static final int[] Idx2Dir;
    public static int[] IsoTrans;

    static {
        Data = new DirectionRec[12];
        Data[0] = new DirectionRec(0, 0, 0, Directions.dtNone, '?', '?');
        Data[1] = new DirectionRec(0, -1, 0, Directions.dtSouth, 'N', 'S');
        Data[2] = new DirectionRec(0, 1, 0, Directions.dtNorth, 'S', 'N');
        Data[3] = new DirectionRec(-1, 0, 0, Directions.dtEast, 'W', 'E');
        Data[4] = new DirectionRec(1, 0, 0, Directions.dtWest, 'E', 'W');
        Data[5] = new DirectionRec(-1, -1, 0, Directions.dtSouthEast, '\0', '\0');
        Data[6] = new DirectionRec(1, -1, 0, Directions.dtSouthWest, '\0', '\0');
        Data[7] = new DirectionRec(-1, 1, 0, Directions.dtNorthEast, '\0', '\0');
        Data[8] = new DirectionRec(1, 1, 0, Directions.dtNorthWest, '\0', '\0');
        Data[9] = new DirectionRec(0, 0, -1, Directions.dtNadir, '\0', '\0');
        Data[10] = new DirectionRec(0, 0, 1, Directions.dtZenith, '\0', '\0');
        Data[11] = new DirectionRec(0, 0, 0, Directions.dtNone, '\0', '\0');

        Idx2Dir = new int[]{
            Directions.dtNone, 
            Directions.dtSouthWest, 
            Directions.dtSouth, 
            Directions.dtSouthEast, 
            Directions.dtWest, 
            Directions.dtPlace, 
            Directions.dtEast, 
            Directions.dtNorthWest, 
            Directions.dtNorth, 
            Directions.dtNorthEast};

        IsoTrans = new int[]{Directions.dtNone, Directions.dtNorthWest, Directions.dtSouthEast, Directions.dtSouthWest, Directions.dtNorthEast, Directions.dtWest, Directions.dtNorth, Directions.dtSouth, Directions.dtEast, Directions.dtZenith, Directions.dtNadir, Directions.dtPlace};
    }

    public static final class DirectionRec
    {
        public int dX;
        public int dY;
        public int dZ;
        public int Opposite;
        public char symDirect;
        public char symOpposite;

        public DirectionRec(int dX, int dY, int dZ, int opposite, char symDirect, char symOpposite)
        {
            this.dX = dX;
            this.dY = dY;
            this.dZ = dZ;
            this.Opposite = opposite;
            this.symDirect = symDirect;
            this.symOpposite = symOpposite;
        }
    }

    /**
     * Returns direction of movements by changes of coordinates.
     * @param pX Old x-coord
     * @param pY Old y-coord
     * @param newX New x-coord
     * @param newY New y-coord
     * @return Direction of movement
     */
    public static int getDirByCoords(int pX, int pY, int newX, int newY)
    {
        int dx = Integer.signum(newX - pX);
        int dy = Integer.signum(newY - pY);
        int dir = (8 + dx - (dy + 1) * 3);
        return Idx2Dir[dir];
    }
}
