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
import jzrlib.utils.StreamUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class GameEntity extends BaseEntity implements ISerializable
{
    private static int LastUID;

    protected final GameSpace fSpace;

    public int CLSID;
    public int UID;

    public GameEntity(GameSpace space, Object owner)
    {
        super(owner);
        this.fSpace = space;

        this.UID = -1;
        this.newUID();
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            if (GameSpace.getInstance() != null) {
                GameSpace.getInstance().deleteEntity(this);
            }
        }
        super.dispose(disposing);
    }

    public GameSpace getSpace()
    {
        return this.fSpace;
    }

    public static void resetUID(int newID)
    {
        GameEntity.LastUID = newID;
    }

    public static int getNextUID()
    {
        int result = GameEntity.LastUID;
        GameEntity.LastUID++;
        return result;
    }

    public final void newUID()
    {
        this.setUID(GameEntity.getNextUID());
    }

    public final int getUID()
    {
        return this.UID;
    }

    public final void setUID(int value)
    {
        if (this.UID != value) {
            GameSpace space = GameSpace.getInstance();

            if (space != null && this.UID != -1) {
                space.deleteEntity(this);
            }

            this.UID = value;

            if (space != null && value != -1) {
                space.addEntity(this);
            }
        }
    }

    public final int getCLSID()
    {
        return this.CLSID;
    }

    public void setCLSID(int value)
    {
        this.CLSID = value;
    }

    public String getDesc()
    {
        return "";
    }

    public void setDesc(String value)
    {
    }

    public String getName()
    {
        return "";
    }

    public void setName(String value)
    {
    }

    public boolean assign(GameEntity entity)
    {
        return true;
    }

    @Override
    public byte getSerializeKind()
    {
        return 0;
    }

    @Override
    public void loadFromStream(BinaryInputStream stream, FileVersion version) throws IOException
    {
        this.setCLSID(StreamUtils.readInt(stream));
    }

    @Override
    public void saveToStream(BinaryOutputStream stream, FileVersion version) throws IOException
    {
        StreamUtils.writeInt(stream, this.CLSID);
    }
}
