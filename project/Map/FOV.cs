/*
 *  "ZRLib", Roguelike games development Library.
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

using System;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Map
{
    /// <summary>
    /// The field of view algorithm is based on "FOV using recursive shadowcasting".
    /// @author Bjorn Bergstrom
    /// </summary>
    public static class FOV
    {
        private const float LOSDelta = 0.2f;
        private static AbstractMap fMap;

        public static void Reset(AbstractMap map, ExtRect frame, bool clearVisited)
        {
            int x1 = frame.Left;
            int y1 = frame.Top;
            int x2 = frame.Right;
            int y2 = frame.Bottom;

            for (int y = y1; y <= y2; y++) {
                for (int x = x1; x <= x2; x++) {
                    BaseTile tile = map.GetTile(x, y);
                    if (tile != null) {
                        tile.ExcludeState(BaseTile.TS_SEEN);
                        if (clearVisited) {
                            tile.ExcludeState(BaseTile.TS_VISITED);
                        }
                    }
                }
            }
        }

        public static void OpenVisited(IMap map, ExtRect frame)
        {
            int x1 = frame.Left;
            int y1 = frame.Top;
            int x2 = frame.Right;
            int y2 = frame.Bottom;

            for (int y = y1; y <= y2; y++) {
                for (int x = x1; x <= x2; x++) {
                    BaseTile tile = map.GetTile(x, y);
                    if (tile != null) {
                        tile.IncludeState(BaseTile.TS_VISITED);
                    }
                }
            }
        }

        public static void FOV_Prepare(AbstractMap map, bool clearVisited)
        {
            fMap = map;
            Reset(map, map.AreaRect, clearVisited);
        }

        public static void FOV_Start(AbstractMap map, int ax, int ay, int fovRadius, int moveDir, ExtRect viewport)
        {
            fMap = map;
            Reset(fMap, viewport, false);
            FOV_Start(ax, ay, fovRadius, moveDir);
        }

        public static void FOV_Start(int ax, int ay, int fovRadius, int moveDir)
        {
            Directions validDirs;
            if (moveDir == Directions.DtNone) {
                validDirs = new Directions(Directions.DtNorth, Directions.DtSouth, Directions.DtWest, Directions.DtEast, Directions.DtNorthWest, Directions.DtNorthEast, Directions.DtSouthWest, Directions.DtSouthEast);
            } else {
                switch (moveDir) {
                    case Directions.DtNorth:
                        validDirs = new Directions(Directions.DtNorth, Directions.DtNorthWest, Directions.DtNorthEast);
                        break;

                    case Directions.DtSouth:
                        validDirs = new Directions(Directions.DtSouth, Directions.DtSouthWest, Directions.DtSouthEast);
                        break;

                    case Directions.DtWest:
                        validDirs = new Directions(Directions.DtWest, Directions.DtNorthWest, Directions.DtSouthWest);
                        break;

                    case Directions.DtEast:
                        validDirs = new Directions(Directions.DtEast, Directions.DtNorthEast, Directions.DtSouthEast);
                        break;

                    case Directions.DtNorthWest:
                        validDirs = new Directions(Directions.DtNorth, Directions.DtWest, Directions.DtNorthWest);
                        break;

                    case Directions.DtNorthEast:
                        validDirs = new Directions(Directions.DtNorth, Directions.DtEast, Directions.DtNorthEast);
                        break;

                    case Directions.DtSouthWest:
                        validDirs = new Directions(Directions.DtSouth, Directions.DtWest, Directions.DtSouthWest);
                        break;

                    case Directions.DtSouthEast:
                        validDirs = new Directions(Directions.DtSouth, Directions.DtEast, Directions.DtSouthEast);
                        break;

                    default:
                        validDirs = new Directions();
                        break;
                }
            }

            fMap.SetSeen(ax, ay);

            if (fovRadius > 0) {
                int RdS2 = (int)((double)fovRadius / Math.Sqrt(2.0));

                int nL = 1;
                if (validDirs.Contains(Directions.DtNorth)) {
                    for (nL = 1; nL <= fovRadius; nL++) {
                        fMap.SetSeen(ax, ay - nL);
                        if (fMap.IsBlockLOS(ax, ay - nL)) {
                            break;
                        }
                    }
                }

                int neL = 1;
                if ((validDirs.Contains(Directions.DtNorthEast))) {
                    for (neL = 1; neL <= RdS2; neL++) {
                        fMap.SetSeen(ax + neL, ay - neL);
                        if (fMap.IsBlockLOS(ax + neL, ay - neL)) {
                            break;
                        }
                    }
                }

                int eL = 1;
                if ((validDirs.Contains(Directions.DtEast))) {
                    for (eL = 1; eL <= fovRadius; eL++) {
                        fMap.SetSeen(ax + eL, ay);
                        if (fMap.IsBlockLOS(ax + eL, ay)) {
                            break;
                        }
                    }
                }

                int seL = 1;
                if ((validDirs.Contains(Directions.DtSouthEast))) {
                    for (seL = 1; seL <= RdS2; seL++) {
                        fMap.SetSeen(ax + seL, ay + seL);
                        if (fMap.IsBlockLOS(ax + seL, ay + seL)) {
                            break;
                        }
                    }
                }

                int sL = 1;
                if ((validDirs.Contains(Directions.DtSouth))) {
                    for (sL = 1; sL <= fovRadius; sL++) {
                        fMap.SetSeen(ax, ay + sL);
                        if (fMap.IsBlockLOS(ax, ay + sL)) {
                            break;
                        }
                    }
                }

                int swL = 1;
                if ((validDirs.Contains(Directions.DtSouthWest))) {
                    for (swL = 1; swL <= RdS2; swL++) {
                        fMap.SetSeen(ax - swL, ay + swL);
                        if (fMap.IsBlockLOS(ax - swL, ay + swL)) {
                            break;
                        }
                    }
                }

                int wL = 1;
                if ((validDirs.Contains(Directions.DtWest))) {
                    for (wL = 1; wL <= fovRadius; wL++) {
                        fMap.SetSeen(ax - wL, ay);
                        if (fMap.IsBlockLOS(ax - wL, ay)) {
                            break;
                        }
                    }
                }

                int nwL = 1;
                if ((validDirs.Contains(Directions.DtNorthWest))) {
                    for (nwL = 1; nwL <= RdS2; nwL++) {
                        fMap.SetSeen(ax - nwL, ay - nwL);
                        if (fMap.IsBlockLOS(ax - nwL, ay - nwL)) {
                            break;
                        }
                    }
                }

                if ((nL != 1 || nwL != 1) && ((validDirs.Contains(Directions.DtNorthWest)) || (validDirs.Contains(Directions.DtNorth)))) {
                    ScanNW2N(ax, ay, fovRadius, 1, 1.0f, 0.0f);
                }
                if ((nL != 1 || neL != 1) && ((validDirs.Contains(Directions.DtNorthEast)) || (validDirs.Contains(Directions.DtNorth)))) {
                    ScanNE2N(ax, ay, fovRadius, 1, -1.0f, 0.0f);
                }
                if ((nwL != 1 || wL != 1) && ((validDirs.Contains(Directions.DtNorthWest)) || (validDirs.Contains(Directions.DtWest)))) {
                    ScanNW2W(ax, ay, fovRadius, 1, 1.0f, 0.0f);
                }
                if ((swL != 1 || wL != 1) && ((validDirs.Contains(Directions.DtSouthWest)) || (validDirs.Contains(Directions.DtWest)))) {
                    ScanSW2W(ax, ay, fovRadius, 1, -1.0f, 0.0f);
                }
                if ((swL != 1 || sL != 1) && ((validDirs.Contains(Directions.DtSouthWest)) || (validDirs.Contains(Directions.DtSouth)))) {
                    ScanSW2S(ax, ay, fovRadius, 1, -1.0f, 0.0f);
                }
                if ((seL != 1 || sL != 1) && ((validDirs.Contains(Directions.DtSouthEast)) || (validDirs.Contains(Directions.DtSouth)))) {
                    ScanSE2S(ax, ay, fovRadius, 1, 1.0f, 0.0f);
                }
                if ((neL != 1 || eL != 1) && ((validDirs.Contains(Directions.DtNorthEast)) || (validDirs.Contains(Directions.DtEast)))) {
                    ScanNE2E(ax, ay, fovRadius, 1, -1.0f, 0.0f);
                }
                if ((seL != 1 || eL != 1) && ((validDirs.Contains(Directions.DtSouthEast)) || (validDirs.Contains(Directions.DtEast)))) {
                    ScanSE2E(ax, ay, fovRadius, 1, 1.0f, 0.0f);
                }
            }
        }

        private static float Slope(float x1, float y1, float x2, float y2)
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

        private static float SlopeInv(float x1, float y1, float x2, float y2)
        {
            float s = Slope(x1, y1, x2, y2);

            float result;
            if (s != 0f) {
                result = (1.0f / s);
            } else {
                result = 0.0f;
            }
            return result;
        }

        private static void ScanNW2N(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
        {
            if (distance > radius) {
                return;
            }

            int xStart = (int)((double)xCenter + 0.5 - startSlope * (double)distance);
            int xEnd = (int)((double)xCenter + 0.5 - endSlope * (double)distance);
            int yCheck = yCenter - distance;

            int horCrossCircle = (int)((double)xCenter + 0.5 - Math.Sqrt((((double)radius + (double)LOSDelta) * ((double)radius + (double)LOSDelta) - (double)((yCenter - yCheck) * (yCenter - yCheck)))));
            if (xStart < horCrossCircle) {
                xStart = horCrossCircle;
            }

            if (xStart != xCenter - 1 * distance && xStart <= xEnd) {
                fMap.SetSeen(xStart, yCheck);
            }

            bool prevBlocked = fMap.IsBlockLOS(xStart, yCheck);

            for (int xCheck = xStart + 1; xCheck <= xEnd; xCheck++) {
                if (xCheck != xCenter) {
                    fMap.SetSeen(xCheck, yCheck);
                }

                if (fMap.IsBlockLOS(xCheck, yCheck)) {
                    if (!prevBlocked) {
                        ScanNW2N(xCenter, yCenter, radius, distance + 1, startSlope, Slope(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), ((float)xCheck - 0.00001f), ((float)yCheck + 0.999999f)));
                    }
                    prevBlocked = true;
                } else {
                    if (prevBlocked) {
                        startSlope = Slope(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), (float)xCheck, (float)yCheck);
                    }
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) {
                ScanNW2N(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
            }
        }

        private static void ScanNE2N(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
        {
            if (distance > radius) {
                return;
            }

            int xStart = (int)((double)xCenter + 0.5 - startSlope * (double)distance);
            int xEnd = (int)((double)xCenter + 0.5 - endSlope * (double)distance);
            int yCheck = yCenter - distance;

            int horCrossCircle = (int)((double)xCenter + 0.5 + Math.Sqrt((((double)radius + (double)LOSDelta) * ((double)radius + (double)LOSDelta) - (double)((yCenter - yCheck) * (yCenter - yCheck)))));
            if (xStart > horCrossCircle) {
                xStart = horCrossCircle;
            }

            if (xStart != xCenter - -1 * distance && xStart >= xEnd) {
                fMap.SetSeen(xStart, yCheck);
            }

            bool prevBlocked = fMap.IsBlockLOS(xStart, yCheck);

            for (int xCheck = xStart - 1; xCheck >= xEnd; xCheck--) {
                if (xCheck != xCenter) {
                    fMap.SetSeen(xCheck, yCheck);
                }

                if (fMap.IsBlockLOS(xCheck, yCheck)) {
                    if (!prevBlocked) {
                        ScanNE2N(xCenter, yCenter, radius, distance + 1, startSlope, Slope(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), (float)(xCheck + 1f), ((float)yCheck + 0.99999f)));
                    }
                    prevBlocked = true;
                } else {
                    if (prevBlocked) {
                        startSlope = Slope(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), ((float)xCheck + 0.9999999f), (float)yCheck);
                    }
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) {
                ScanNE2N(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
            }
        }

        private static void ScanNW2W(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
        {
            if (distance > radius) {
                return;
            }

            int yStart = (int)((double)yCenter + 0.5 - startSlope * (double)distance);
            int yEnd = (int)((double)yCenter + 0.5 - endSlope * (double)distance);
            int xCheck = xCenter - distance;

            int horCrossCircle = (int)((double)yCenter + 0.5 - Math.Sqrt((((double)radius + (double)LOSDelta) * ((double)radius + (double)LOSDelta) - (double)((xCheck - xCenter) * (xCheck - xCenter)))));
            if (yStart < horCrossCircle) {
                yStart = horCrossCircle;
            }

            if (yStart != yCenter - 1 * distance && yStart <= yEnd) {
                fMap.SetSeen(xCheck, yStart);
            }

            bool prevBlocked = fMap.IsBlockLOS(xCheck, yStart);

            for (int yCheck = yStart + 1; yCheck <= yEnd; yCheck++) {
                if (yCheck != yCenter) {
                    fMap.SetSeen(xCheck, yCheck);
                }

                if (fMap.IsBlockLOS(xCheck, yCheck)) {
                    if (!prevBlocked) {
                        ScanNW2W(xCenter, yCenter, radius, distance + 1, startSlope, SlopeInv(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), ((float)xCheck + 0.99999f), ((float)yCheck - 0.00001f)));
                    }
                    prevBlocked = true;
                } else {
                    if (prevBlocked) {
                        startSlope = SlopeInv(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), (float)xCheck, (float)yCheck);
                    }
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) {
                ScanNW2W(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
            }
        }

        private static void ScanSW2W(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
        {
            if (distance > radius) {
                return;
            }

            int yStart = (int)((double)yCenter + 0.5 - startSlope * (double)distance);
            int yEnd = (int)((double)yCenter + 0.5 - endSlope * (double)distance);
            int xCheck = xCenter - distance;

            int horCrossCircle = (int)((double)yCenter + 0.5 + Math.Sqrt((((double)radius + (double)LOSDelta) * ((double)radius + (double)LOSDelta) - (double)((xCheck - xCenter) * (xCheck - xCenter)))));
            if (yStart > horCrossCircle) {
                yStart = horCrossCircle;
            }

            if (yStart != yCenter - -1 * distance && yStart >= yEnd) {
                fMap.SetSeen(xCheck, yStart);
            }

            bool prevBlocked = fMap.IsBlockLOS(xCheck, yStart);

            for (int yCheck = yStart - 1; yCheck >= yEnd; yCheck--) {
                if (yCheck != yCenter) {
                    fMap.SetSeen(xCheck, yCheck);
                }

                if (fMap.IsBlockLOS(xCheck, yCheck)) {
                    if (!prevBlocked) {
                        ScanSW2W(xCenter, yCenter, radius, distance + 1, startSlope, SlopeInv(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), ((float)xCheck + 0.99999f), (float)(yCheck + 1f)));
                    }
                    prevBlocked = true;
                } else {
                    if (prevBlocked) {
                        startSlope = SlopeInv(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), (float)xCheck, ((float)yCheck + 0.99999f));
                    }
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) {
                ScanSW2W(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
            }
        }

        private static void ScanNE2E(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
        {
            if (distance > radius) {
                return;
            }

            int yStart = (int)((double)yCenter + 0.5 + startSlope * (double)distance);
            int yEnd = (int)((double)yCenter + 0.5 + endSlope * (double)distance);
            int xCheck = xCenter + distance;

            int horCrossCircle = (int)((double)yCenter + 0.5 - Math.Sqrt((((double)radius + (double)LOSDelta) * ((double)radius + (double)LOSDelta) - (double)((xCheck - xCenter) * (xCheck - xCenter)))));
            if (yStart < horCrossCircle) {
                yStart = horCrossCircle;
            }

            if (yStart != yCenter + -1 * distance && yStart <= yEnd) {
                fMap.SetSeen(xCheck, yStart);
            }

            bool prevBlocked = fMap.IsBlockLOS(xCheck, yStart);

            for (int yCheck = yStart + 1; yCheck <= yEnd; yCheck++) {
                if (yCheck != yCenter) {
                    fMap.SetSeen(xCheck, yCheck);
                }

                if (fMap.IsBlockLOS(xCheck, yCheck)) {
                    if (!prevBlocked) {
                        ScanNE2E(xCenter, yCenter, radius, distance + 1, startSlope, SlopeInv(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), (float)xCheck, ((float)yCheck - 0.00001f)));
                    }
                    prevBlocked = true;
                } else {
                    if (prevBlocked) {
                        startSlope = SlopeInv(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), ((float)xCheck + 0.99999f), (float)yCheck);
                    }
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) {
                ScanNE2E(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
            }
        }

        private static void ScanSE2E(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
        {
            if (distance > radius) {
                return;
            }

            int yStart = (int)((double)yCenter + 0.5 + startSlope * (double)distance);
            int yEnd = (int)((double)yCenter + 0.5 + endSlope * (double)distance);
            int xCheck = xCenter + distance;

            int horCrossCircle = (int)((double)yCenter + 0.5 + Math.Sqrt((((double)radius + (double)LOSDelta) * ((double)radius + (double)LOSDelta) - (double)((xCheck - xCenter) * (xCheck - xCenter)))));
            if (yStart > horCrossCircle) {
                yStart = horCrossCircle;
            }

            if (yStart != yCenter + 1 * distance && yStart >= yEnd) {
                fMap.SetSeen(xCheck, yStart);
            }

            bool prevBlocked = fMap.IsBlockLOS(xCheck, yStart);

            for (int yCheck = yStart - 1; yCheck >= yEnd; yCheck--) {
                if (yCheck != yCenter) {
                    fMap.SetSeen(xCheck, yCheck);
                }

                if (fMap.IsBlockLOS(xCheck, yCheck)) {
                    if (!prevBlocked) {
                        ScanSE2E(xCenter, yCenter, radius, distance + 1, startSlope, SlopeInv(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), (float)xCheck, (float)(yCheck + 1f)));
                    }
                    prevBlocked = true;
                } else {
                    if (prevBlocked) {
                        startSlope = SlopeInv(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), ((float)xCheck + 0.99999f), ((float)yCheck + 0.99999f));
                    }
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) {
                ScanSE2E(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
            }
        }

        private static void ScanSW2S(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
        {
            if (distance > radius) {
                return;
            }

            int xStart = (int)((double)xCenter + 0.5 + startSlope * (double)distance);
            int xEnd = (int)((double)xCenter + 0.5 + endSlope * (double)distance);
            int yCheck = yCenter + distance;

            int horCrossCircle = (int)((double)xCenter + 0.5 - Math.Sqrt((((double)radius + (double)LOSDelta) * ((double)radius + (double)LOSDelta) - (double)((yCenter - yCheck) * (yCenter - yCheck)))));
            if (xStart < horCrossCircle) {
                xStart = horCrossCircle;
            }

            if (xStart != xCenter + -1 * distance && xStart <= xEnd) {
                fMap.SetSeen(xStart, yCheck);
            }

            bool prevBlocked = fMap.IsBlockLOS(xStart, yCheck);

            for (int xCheck = xStart + 1; xCheck <= xEnd; xCheck++) {
                if (xCheck != xCenter) {
                    fMap.SetSeen(xCheck, yCheck);
                }

                if (fMap.IsBlockLOS(xCheck, yCheck)) {
                    if (!prevBlocked) {
                        ScanSW2S(xCenter, yCenter, radius, distance + 1, startSlope, Slope(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), ((float)xCheck - 0.00001f), (float)yCheck));
                    }
                    prevBlocked = true;
                } else {
                    if (prevBlocked) {
                        startSlope = Slope(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), (float)xCheck, ((float)yCheck + 0.99999f));
                    }
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) {
                ScanSW2S(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
            }
        }

        private static void ScanSE2S(int xCenter, int yCenter, int radius, int distance, float startSlope, float endSlope)
        {
            if (distance > radius) {
                return;
            }

            int xStart = (int)((double)xCenter + 0.5 + startSlope * (double)distance);
            int xEnd = (int)((double)xCenter + 0.5 + endSlope * (double)distance);
            int yCheck = yCenter + distance;

            int horCrossCircle = (int)((double)xCenter + 0.5 + Math.Sqrt((((double)radius + (double)LOSDelta) * ((double)radius + (double)LOSDelta) - (double)((yCenter - yCheck) * (yCenter - yCheck)))));
            if (xStart > horCrossCircle) {
                xStart = horCrossCircle;
            }

            if (xStart != xCenter + 1 * distance && xStart >= xEnd) {
                fMap.SetSeen(xStart, yCheck);
            }

            bool prevBlocked = fMap.IsBlockLOS(xStart, yCheck);

            for (int xCheck = xStart - 1; xCheck >= xEnd; xCheck--) {
                if (xCheck != xCenter) {
                    fMap.SetSeen(xCheck, yCheck);
                }

                if (fMap.IsBlockLOS(xCheck, yCheck)) {
                    if (!prevBlocked) {
                        ScanSE2S(xCenter, yCenter, radius, distance + 1, startSlope, Slope(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), (float)(xCheck + 1f), (float)yCheck));
                    }
                    prevBlocked = true;
                } else {
                    if (prevBlocked) {
                        startSlope = Slope(((float)xCenter + 0.5f), ((float)yCenter + 0.5f), ((float)xCheck + 0.99999f), ((float)yCheck + 0.99999f));
                    }
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) {
                ScanSE2S(xCenter, yCenter, radius, distance + 1, startSlope, endSlope);
            }
        }
    }
}