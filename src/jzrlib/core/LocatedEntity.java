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

import java.io.IOException;
import jzrlib.external.BinaryInputStream;
import jzrlib.external.BinaryOutputStream;
import jzrlib.utils.Logger;
import jzrlib.utils.StreamUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class LocatedEntity extends GameEntity
{
    private int fPosX;
    private int fPosY;

    public LocatedEntity(GameSpace space, Object owner)
    {
        super(space, owner);
    }

    public final int getPosX()
    {
        return this.fPosX;
    }

    public final int getPosY()
    {
        return this.fPosY;
    }

    public final Point getLocation()
    {
        return new Point(this.fPosX, this.fPosY);
    }

    public void setPos(int posX, int posY)
    {
        if (this.fPosX != posX || this.fPosY != posY) {
            this.fPosX = posX;
            this.fPosY = posY;
        }
    }

    public final boolean inRect(Rect rt)
    {
        return this.fPosX >= rt.Left && this.fPosX <= rt.Right && this.fPosY >= rt.Top && this.fPosY <= rt.Bottom;
    }

    @Override
    public void loadFromStream(BinaryInputStream stream, FileVersion version) throws IOException
    {
        try {
            super.loadFromStream(stream, version);

            this.fPosX = StreamUtils.readInt(stream);
            this.fPosY = StreamUtils.readInt(stream);
        } catch (Exception ex) {
            Logger.write("LocatedEntity.loadFromStream(): " + ex.getMessage());
            throw ex;
        }
    }

    @Override
    public void saveToStream(BinaryOutputStream stream, FileVersion version) throws IOException
    {
        super.saveToStream(stream, version);

        StreamUtils.writeInt(stream, this.fPosX);
        StreamUtils.writeInt(stream, this.fPosY);
    }
}
