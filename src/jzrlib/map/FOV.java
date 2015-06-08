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
package jzrlib.map;

import jzrlib.core.Directions;
import jzrlib.core.Rect;

/**
 * The field of view algorithm is based on "FOV using recursive shadowcasting".
 * @author Bjorn Bergstrom
 * @author Serg V. Zhdanovskih
 */
public final class FOV
{
    private static final float LOSDelta = 0.2f;
    private static AbstractMap fMap;

    public static void reset(AbstractMap map, Rect frame, boolean clearVisited)
    {
        int x1 = frame.Left;
        int y1 = frame.Top;
        int x2 = frame.Right;
        int y2 = frame.Bottom;
        
        for (int y = y1; y <= y2; y++) {
            for (int x = x1; x <= x2; x++) {
                BaseTile tile = map.getTile(x, y);
                if (tile != null) {
                    tile.excludeState(TileStates.TS_SEEN);
                    if (clearVisited) {
                        tile.excludeState(TileStates.TS_VISITED);
                    }
                }
            }
        }
    }

    public static void openVisited(IMap map, Rect frame)
    {
        int x1 = frame.Left;
        int y1 = frame.Top;
        int x2 = frame.Right;
        int y2 = frame.Bottom;
        
        for (int y = y1; y <= y2; y++) {
            for (int x = x1; x <= x2; x++) {
                BaseTile tile = map.getTile(x, y);
                if (tile != null) {
                    tile.includeState(TileStates.TS_VISITED);
                }
            }
        }
    }

    public static void FOV_Prepare(AbstractMap map, boolean clearVisited)
    {
        fMap = map;
        reset(map, map.getAreaRect(), clearVisited);
    }

    public static void FOV_Start(AbstractMap map, int ax, int ay, int fovRadius, int moveDir, Rect viewport)
    {
        fMap = map;
        reset(fMap, viewport, false);
        FOV_Start(ax, ay, fovRadius, moveDir);
    }

    public static void FOV_Start(int ax, int ay, int fovRadius, int moveDir)
    {
        Directions validDirs;
        if (moveDir == Directions.dtNone) {
            validDirs = new Directions(Directions.dtNorth, Directions.dtSouth, Directions.dtWest, Directions.dtEast, Directions.dtNorthWest, Directions.dtNorthEast, Directions.dtSouthWest, Directions.dtSouthEast);
        } else {
            switch (moveDir) {
                case Directions.dtNorth:
                    validDirs = new Directions(Directions.dtNorth, Directions.dtNorthWest, Directions.dtNorthEast);
                    break;

                case Directions.dtSouth:
                    validDirs = new Directions(Directions.dtSouth, Directions.dtSouthWest, Directions.dtSouthEast);
                    break;

                case Directions.dtWest:
                    validDirs = new Directions(Directions.dtWest, Directions.dtNorthWest, Directions.dtSouthWest);
                    break;

                case Directions.dtEast:
                    validDirs = new Directions(Directions.dtEast, Directions.dtNorthEast, Directions.dtSouthEast);
                    break;

                case Directions.dtNorthWest:
                    validDirs = new Directions(Directions.dtNorth, Directions.dtWest, Directions.dtNorthWest);
                    break;

                case Directions.dtNorthEast:
                    validDirs = new Directions(Directions.dtNorth, Directions.dtEast, Directions.dtNorthEast);
                    break;

                case Directions.dtSouthWest:
                    validDirs = new Directions(Directions.dtSouth, Directions.dtWest, Directions.dtSouthWest);
                    break;

                case Directions.dtSouthEast:
                    validDirs = new Directions(Directions.dtSouth, Directions.dtEast, Directions.dtSouthEast);
                    break;

                default:
                    validDirs = new Directions();
                    break;
            }
        }

        fMap.setSeen(ax, ay);

        if (fovRadius > 0) {
            int RdS2 = (int) ((double) fovRadius / Math.sqrt(2.0));

            int nL = 1;
            if (validDirs.contains(Directions.dtNorth)) {
                for (nL = 1; nL <= fovRadius; nL++) {
                    fMap.setSeen(ax, ay - nL);
                    if (fMap.isBlockLOS(ax, ay - nL)) {
                        break;
                    }
                }
            }

            int neL = 1;
            if ((validDirs.contains(Directions.dtNorthEast))) {
                for (neL = 1; neL <= RdS2; neL++) {
                    fMap.setSeen(ax + neL, ay - neL);
                    if (fMap.isBlockLOS(ax + neL, ay - neL)) {
                        break;
                    }
                }
            }

            int eL = 1;
            if ((validDirs.contains(Directions.dtEast))) {
                for (eL = 1; eL <= fovRadius; eL++) {
                    fMap.setSeen(ax + eL, ay);
                    if (fMap.isBlockLOS(ax + eL, ay)) {
                        break;
                    }
                }
            }

            int seL = 1;
            if ((validDirs.contains(Directions.dtSouthEast))) {
                for (seL = 1; seL <= RdS2; seL++) {
                    fMap.setSeen(ax + seL, ay + seL);
                    if (fMap.isBlockLOS(ax + seL, ay + seL)) {
                        break;
                    }
                }
            }

            int sL = 1;
            if ((validDirs.contains(Directions.dtSouth))) {
                for (sL = 1; sL <= fovRadius; sL++) {
                    fMap.setSeen(ax, ay + sL);
                    if (fMap.isBlockLOS(ax, ay + sL)) {
                        break;
                    }
                }
            }

            int swL = 1;
            if ((validDirs.contains(Directions.dtSouthWest))) {
                for (swL = 1; swL <= RdS2; swL++) {
                    fMap.setSeen(ax - swL, ay + swL);
                    if (fMap.isBlockLOS(ax - swL, ay + swL)) {
                        break;
                    }
                }
            }

            int wL = 1;
            if ((validDirs.contains(Directions.dtWest))) {
                for (wL = 1; wL <= fovRadius; wL++) {
                    fMap.setSeen(ax - wL, ay);
                    if (fMap.isBlockLOS(ax - wL, ay)) {
                        break;
                    }
                }
            }

            int nwL = 1;
            if ((validDirs.contains(Directions.dtNorthWest))) {
                for (nwL = 1; nwL <= RdS2; nwL++) {
                    fMap.setSeen(ax - nwL, ay - nwL);
                    if (fMap.isBlockLOS(ax - nwL, ay - nwL)) {
                        break;
                    }
                }
            }

            if ((nL != 1 || nwL != 1) && ((validDirs.contains(Directions.dtNorthWest)) || (validDirs.contains(Directions.dtNorth)))) {
                scanNW2N(ax, ay, fovRadius, 1, 1.0f, 0.0f);
            }
            if ((nL != 1 || neL != 1) && ((validDirs.contains(Directions.dtNorthEast)) || (validDirs.contains(Directions.dtNorth)))) {
                scanNE2N(ax, ay, fovRadius, 1, -1.0f, 0.0f);
            }
            if ((nwL != 1 || wL != 1) && ((validDirs.contains(Directions.dtNorthWest)) || (validDirs.contains(Directions.dtWest)))) {
                scanNW2W(ax, ay, fovRadius, 1, 1.0f, 0.0f);
            }
            if ((swL != 1 || wL != 1) && ((validDirs.contains(Directions.dtSouthWest)) || (validDirs.contains(Directions.dtWest)))) {
                scanSW2W(ax, ay, fovRadius, 1, -1.0f, 0.0f);
            }
            if ((swL != 1 || sL != 1) && ((validDirs.contains(Directions.dtSouthWest)) || (validDirs.contains(Directions.dtSouth)))) {
                scanSW2S(ax, ay, fovRadius, 1, -1.0f, 0.0f);
            }
            if ((seL != 1 || sL != 1) && ((validDirs.contains(Directions.dtSouthEast)) || (validDirs.contains(Directions.dtSouth)))) {
                scanSE2S(ax, ay, fovRadius, 1, 1.0f, 0.0f);
            }
            if ((neL != 1 || eL != 1) && ((validDirs.contains(Directions.dtNorthEast)) || (validDirs.contains(Directions.dtEast)))) {
                scanNE2E(ax, ay, fovRadius, 1, -1.0f, 0.0f);
            }
            if ((seL != 1 || eL != 1) && ((validDirs.contains(Directions.dtSouthEast)) || (validDirs.contains(Directions.dtEast)))) {
                scanSE2E(ax, ay, fovRadius, 1, 1.0f, 0.0f);
            }
        }
    }

    private static float slope(float x1, float y1, float x2, float y2)
    {
        float xDiff = (x1 - x2);
        float yDiff = (y1 - y2);

        float result;
        if (yDiff != 0f) {
            result = (xDiff / yDiff);
        } else {
            result = 0.0f;
        }
        return result;
    }

    private static float slopeInv(float x1, float y1, float x2, float y2)
    {
        float s = slope(x1, y1, x2, y2);

        float result;
        if (s != 0f) {
            result = (1.0f / s);
        } else {
            result = 0.0f;
        }
        return result;
    }

    private static void scanNW2N(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
    {
        if (distance > radius) {
            return;
        }

        int xStart = (int) ((double) xCenter + 0.5 - startSlope * (double) distance);
        int xEnd = (int) ((double) xCenter + 0.5 - endSlope * (double) distance);
        int yCheck = yCenter - distance;

        int horCrossCircle = (int) ((double) xCenter + 0.5 - Math.sqrt((((double) radius + (double) LOSDelta) * ((double) radius + (double) LOSDelta) - (double) ((yCenter - yCheck) * (yCenter - yCheck)))));
        if (xStart < horCrossCircle) {
            xStart = horCrossCircle;
        }

        if (xStart != xCenter - 1 * distance && xStart <= xEnd) {
            fMap.setSeen(xStart, yCheck);
        }

        boolean prevBlocked = fMap.isBlockLOS(xStart, yCheck);

        for (int xCheck = xStart + 1; xCheck <= xEnd; xCheck++) {
            if (xCheck != xCenter) {
                fMap.setSeen(xCheck, yCheck);
            }

            if (fMap.isBlockLOS(xCheck, yCheck)) {
                if (!prevBlocked) {
                    scanNW2N(xCenter, yCenter, radius, distance + 1, startSlope, slope(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), ((float) xCheck - 0.00001f), ((float) yCheck + 0.999999f)));
                }
                prevBlocked = true;
            } else {
                if (prevBlocked) {
                    startSlope = slope(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), (float) xCheck, (float) yCheck);
                }
                prevBlocked = false;
            }
        }

        if (!prevBlocked) {
            scanNW2N(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
        }
    }

    private static void scanNE2N(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
    {
        if (distance > radius) {
            return;
        }

        int xStart = (int) ((double) xCenter + 0.5 - startSlope * (double) distance);
        int xEnd = (int) ((double) xCenter + 0.5 - endSlope * (double) distance);
        int yCheck = yCenter - distance;

        int horCrossCircle = (int) ((double) xCenter + 0.5 + Math.sqrt((((double) radius + (double) LOSDelta) * ((double) radius + (double) LOSDelta) - (double) ((yCenter - yCheck) * (yCenter - yCheck)))));
        if (xStart > horCrossCircle) {
            xStart = horCrossCircle;
        }

        if (xStart != xCenter - -1 * distance && xStart >= xEnd) {
            fMap.setSeen(xStart, yCheck);
        }

        boolean prevBlocked = fMap.isBlockLOS(xStart, yCheck);

        for (int xCheck = xStart - 1; xCheck >= xEnd; xCheck--) {
            if (xCheck != xCenter) {
                fMap.setSeen(xCheck, yCheck);
            }

            if (fMap.isBlockLOS(xCheck, yCheck)) {
                if (!prevBlocked) {
                    scanNE2N(xCenter, yCenter, radius, distance + 1, startSlope, slope(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), (float) (xCheck + 1f), ((float) yCheck + 0.99999f)));
                }
                prevBlocked = true;
            } else {
                if (prevBlocked) {
                    startSlope = slope(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), ((float) xCheck + 0.9999999f), (float) yCheck);
                }
                prevBlocked = false;
            }
        }

        if (!prevBlocked) {
            scanNE2N(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
        }
    }

    private static void scanNW2W(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
    {
        if (distance > radius) {
            return;
        }

        int yStart = (int) ((double) yCenter + 0.5 - startSlope * (double) distance);
        int yEnd = (int) ((double) yCenter + 0.5 - endSlope * (double) distance);
        int xCheck = xCenter - distance;

        int horCrossCircle = (int) ((double) yCenter + 0.5 - Math.sqrt((((double) radius + (double) LOSDelta) * ((double) radius + (double) LOSDelta) - (double) ((xCheck - xCenter) * (xCheck - xCenter)))));
        if (yStart < horCrossCircle) {
            yStart = horCrossCircle;
        }

        if (yStart != yCenter - 1 * distance && yStart <= yEnd) {
            fMap.setSeen(xCheck, yStart);
        }

        boolean prevBlocked = fMap.isBlockLOS(xCheck, yStart);

        for (int yCheck = yStart + 1; yCheck <= yEnd; yCheck++) {
            if (yCheck != yCenter) {
                fMap.setSeen(xCheck, yCheck);
            }

            if (fMap.isBlockLOS(xCheck, yCheck)) {
                if (!prevBlocked) {
                    scanNW2W(xCenter, yCenter, radius, distance + 1, startSlope, slopeInv(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), ((float) xCheck + 0.99999f), ((float) yCheck - 0.00001f)));
                }
                prevBlocked = true;
            } else {
                if (prevBlocked) {
                    startSlope = slopeInv(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), (float) xCheck, (float) yCheck);
                }
                prevBlocked = false;
            }
        }

        if (!prevBlocked) {
            scanNW2W(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
        }
    }

    private static void scanSW2W(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
    {
        if (distance > radius) {
            return;
        }

        int yStart = (int) ((double) yCenter + 0.5 - startSlope * (double) distance);
        int yEnd = (int) ((double) yCenter + 0.5 - endSlope * (double) distance);
        int xCheck = xCenter - distance;

        int horCrossCircle = (int) ((double) yCenter + 0.5 + Math.sqrt((((double) radius + (double) LOSDelta) * ((double) radius + (double) LOSDelta) - (double) ((xCheck - xCenter) * (xCheck - xCenter)))));
        if (yStart > horCrossCircle) {
            yStart = horCrossCircle;
        }

        if (yStart != yCenter - -1 * distance && yStart >= yEnd) {
            fMap.setSeen(xCheck, yStart);
        }

        boolean prevBlocked = fMap.isBlockLOS(xCheck, yStart);

        for (int yCheck = yStart - 1; yCheck >= yEnd; yCheck--) {
            if (yCheck != yCenter) {
                fMap.setSeen(xCheck, yCheck);
            }

            if (fMap.isBlockLOS(xCheck, yCheck)) {
                if (!prevBlocked) {
                    scanSW2W(xCenter, yCenter, radius, distance + 1, startSlope, slopeInv(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), ((float) xCheck + 0.99999f), (float) (yCheck + 1f)));
                }
                prevBlocked = true;
            } else {
                if (prevBlocked) {
                    startSlope = slopeInv(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), (float) xCheck, ((float) yCheck + 0.99999f));
                }
                prevBlocked = false;
            }
        }

        if (!prevBlocked) {
            scanSW2W(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
        }
    }

    private static void scanNE2E(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
    {
        if (distance > radius) {
            return;
        }

        int yStart = (int) ((double) yCenter + 0.5 + startSlope * (double) distance);
        int yEnd = (int) ((double) yCenter + 0.5 + endSlope * (double) distance);
        int xCheck = xCenter + distance;

        int horCrossCircle = (int) ((double) yCenter + 0.5 - Math.sqrt((((double) radius + (double) LOSDelta) * ((double) radius + (double) LOSDelta) - (double) ((xCheck - xCenter) * (xCheck - xCenter)))));
        if (yStart < horCrossCircle) {
            yStart = horCrossCircle;
        }

        if (yStart != yCenter + -1 * distance && yStart <= yEnd) {
            fMap.setSeen(xCheck, yStart);
        }

        boolean prevBlocked = fMap.isBlockLOS(xCheck, yStart);

        for (int yCheck = yStart + 1; yCheck <= yEnd; yCheck++) {
            if (yCheck != yCenter) {
                fMap.setSeen(xCheck, yCheck);
            }

            if (fMap.isBlockLOS(xCheck, yCheck)) {
                if (!prevBlocked) {
                    scanNE2E(xCenter, yCenter, radius, distance + 1, startSlope, slopeInv(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), (float) xCheck, ((float) yCheck - 0.00001f)));
                }
                prevBlocked = true;
            } else {
                if (prevBlocked) {
                    startSlope = slopeInv(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), ((float) xCheck + 0.99999f), (float) yCheck);
                }
                prevBlocked = false;
            }
        }

        if (!prevBlocked) {
            scanNE2E(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
        }
    }

    private static void scanSE2E(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
    {
        if (distance > radius) {
            return;
        }

        int yStart = (int) ((double) yCenter + 0.5 + startSlope * (double) distance);
        int yEnd = (int) ((double) yCenter + 0.5 + endSlope * (double) distance);
        int xCheck = xCenter + distance;

        int horCrossCircle = (int) ((double) yCenter + 0.5 + Math.sqrt((((double) radius + (double) LOSDelta) * ((double) radius + (double) LOSDelta) - (double) ((xCheck - xCenter) * (xCheck - xCenter)))));
        if (yStart > horCrossCircle) {
            yStart = horCrossCircle;
        }

        if (yStart != yCenter + 1 * distance && yStart >= yEnd) {
            fMap.setSeen(xCheck, yStart);
        }

        boolean prevBlocked = fMap.isBlockLOS(xCheck, yStart);

        for (int yCheck = yStart - 1; yCheck >= yEnd; yCheck--) {
            if (yCheck != yCenter) {
                fMap.setSeen(xCheck, yCheck);
            }

            if (fMap.isBlockLOS(xCheck, yCheck)) {
                if (!prevBlocked) {
                    scanSE2E(xCenter, yCenter, radius, distance + 1, startSlope, slopeInv(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), (float) xCheck, (float) (yCheck + 1f)));
                }
                prevBlocked = true;
            } else {
                if (prevBlocked) {
                    startSlope = slopeInv(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), ((float) xCheck + 0.99999f), ((float) yCheck + 0.99999f));
                }
                prevBlocked = false;
            }
        }

        if (!prevBlocked) {
            scanSE2E(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
        }
    }

    private static void scanSW2S(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
    {
        if (distance > radius) {
            return;
        }

        int xStart = (int) ((double) xCenter + 0.5 + startSlope * (double) distance);
        int xEnd = (int) ((double) xCenter + 0.5 + endSlope * (double) distance);
        int yCheck = yCenter + distance;

        int horCrossCircle = (int) ((double) xCenter + 0.5 - Math.sqrt((((double) radius + (double) LOSDelta) * ((double) radius + (double) LOSDelta) - (double) ((yCenter - yCheck) * (yCenter - yCheck)))));
        if (xStart < horCrossCircle) {
            xStart = horCrossCircle;
        }

        if (xStart != xCenter + -1 * distance && xStart <= xEnd) {
            fMap.setSeen(xStart, yCheck);
        }

        boolean prevBlocked = fMap.isBlockLOS(xStart, yCheck);

        for (int xCheck = xStart + 1; xCheck <= xEnd; xCheck++) {
            if (xCheck != xCenter) {
                fMap.setSeen(xCheck, yCheck);
            }

            if (fMap.isBlockLOS(xCheck, yCheck)) {
                if (!prevBlocked) {
                    scanSW2S(xCenter, yCenter, radius, distance + 1, startSlope, slope(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), ((float) xCheck - 0.00001f), (float) yCheck));
                }
                prevBlocked = true;
            } else {
                if (prevBlocked) {
                    startSlope = slope(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), (float) xCheck, ((float) yCheck + 0.99999f));
                }
                prevBlocked = false;
            }
        }

        if (!prevBlocked) {
            scanSW2S(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
        }
    }

    private static void scanSE2S(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
    {
        if (distance > radius) {
            return;
        }

        int xStart = (int) ((double) xCenter + 0.5 + startSlope * (double) distance);
        int xEnd = (int) ((double) xCenter + 0.5 + endSlope * (double) distance);
        int yCheck = yCenter + distance;

        int horCrossCircle = (int) ((double) xCenter + 0.5 + Math.sqrt((((double) radius + (double) LOSDelta) * ((double) radius + (double) LOSDelta) - (double) ((yCenter - yCheck) * (yCenter - yCheck)))));
        if (xStart > horCrossCircle) {
            xStart = horCrossCircle;
        }

        if (xStart != xCenter + 1 * distance && xStart >= xEnd) {
            fMap.setSeen(xStart, yCheck);
        }

        boolean prevBlocked = fMap.isBlockLOS(xStart, yCheck);

        for (int xCheck = xStart - 1; xCheck >= xEnd; xCheck--) {
            if (xCheck != xCenter) {
                fMap.setSeen(xCheck, yCheck);
            }

            if (fMap.isBlockLOS(xCheck, yCheck)) {
                if (!prevBlocked) {
                    scanSE2S(xCenter, yCenter, radius, distance + 1, startSlope, slope(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), (float) (xCheck + 1f), (float) yCheck));
                }
                prevBlocked = true;
            } else {
                if (prevBlocked) {
                    startSlope = slope(((float) xCenter + 0.5f), ((float) yCenter + 0.5f), ((float) xCheck + 0.99999f), ((float) yCheck + 0.99999f));
                }
                prevBlocked = false;
            }
        }

        if (!prevBlocked) {
            scanSE2S(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
        }
    }
}
