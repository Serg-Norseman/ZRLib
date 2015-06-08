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

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class CustomMap extends AbstractMap
{
    private BaseTile[] fTiles;

    public CustomMap(int width, int height)
    {
        super(width, height);
    }

    @Override
    public BaseTile getTile(int x, int y)
    {
        BaseTile result = null;
        if (super.isValid(x, y)) {
            result = this.fTiles[y * this.getWidth() + x];
        }
        return result;
    }

    @Override
    protected void createTiles()
    {
        int num = this.getHeight() * this.getWidth();
        this.fTiles = new BaseTile[num];
        for (int i = 0; i < num; i++) {
            this.fTiles[i] = this.createTile();
        }
    }

    @Override
    protected void destroyTiles()
    {
        if (this.fTiles != null) {
            int num = this.getHeight() * this.getWidth();
            for (int i = 0; i < num; i++) {
                this.fTiles[i] = null;
            }
            this.fTiles = null;
        }
    }
}
