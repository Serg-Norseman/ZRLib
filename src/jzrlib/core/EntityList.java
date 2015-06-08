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
public class EntityList extends BaseEntity
{
    private final ExtList<GameEntity> fList;

    public EntityList(Object owner, boolean ownsObjects)
    {
        super(owner);
        this.fList = new ExtList<>(ownsObjects);
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.fList.dispose();
        }
        super.dispose(disposing);
    }

    public int getCount()
    {
        return this.fList.getCount();
    }

    public boolean getOwnsObjects()
    {
        return this.fList.OwnsObjects;
    }

    public void setOwnsObjects(boolean value)
    {
        this.fList.OwnsObjects = value;
    }

    public GameEntity getItem(int index)
    {
        return this.fList.get(index);
    }

    public int add(GameEntity entity)
    {
        return this.fList.add(entity);
    }

    public void assign(EntityList list)
    {
        this.fList.clear();

        int num = list.getCount();
        for (int i = 0; i < num; i++) {
            this.add(list.getItem(i));
        }
    }

    public void clear()
    {
        this.fList.clear();
    }

    public void delete(int index)
    {
        this.fList.delete(index);
    }

    public final GameEntity findByCLSID(int id)
    {
        int num = this.fList.getCount();
        for (int i = 0; i < num; i++) {
            GameEntity e = this.getItem(i);
            if (e.CLSID == id) {
                return e;
            }
        }

        return null;
    }

    public final GameEntity findByGUID(int id)
    {
        int num = this.fList.getCount();
        for (int i = 0; i < num; i++) {
            GameEntity e = this.getItem(i);
            if (e.UID == id) {
                return e;
            }
        }

        return null;
    }

    public void exchange(int index1, int index2)
    {
        this.fList.exchange(index1, index2);
    }

    public GameEntity extract(GameEntity item)
    {
        return (GameEntity) this.fList.extract(item);
    }

    public int indexOf(GameEntity entity)
    {
        return this.fList.indexOf(entity);
    }

    public int remove(GameEntity entity)
    {
        return this.fList.remove(entity);
    }

    public void loadFromStream(BinaryInputStream stream, FileVersion version) throws IOException
    {
        try {
            this.fList.clear();

            int count = StreamUtils.readInt(stream);
            for (int i = 0; i < count; i++) {
                byte kind = (byte) StreamUtils.readByte(stream);
                try {
                    GameEntity item = (GameEntity) SerializablesManager.createSerializable(kind, super.Owner);
                    item.loadFromStream(stream, version);
                    this.fList.add(item);
                } catch (Exception ex) {
                    Logger.write("EntityList.loadFromStream(" + Byte.toString(kind) + "): " + ex.getMessage());
                    throw ex;
                }
            }
        } catch (Exception ex) {
            Logger.write("EntityList.loadFromStream(): " + ex.getMessage());
            throw ex;
        }
    }

    public void saveToStream(BinaryOutputStream stream, FileVersion version) throws IOException
    {
        try {
            int count = this.fList.getCount();

            int num = this.fList.getCount();
            for (int i = 0; i < num; i++) {
                if (this.getItem(i).getSerializeKind() <= 0) {
                    count--;
                }
            }

            StreamUtils.writeInt(stream, count);

            for (int i = 0; i < num; i++) {
                GameEntity item = this.getItem(i);
                byte kind = item.getSerializeKind();
                if (kind > 0) {
                    StreamUtils.writeByte(stream, kind);
                    item.saveToStream(stream, version);
                }
            }
        } catch (Exception ex) {
            Logger.write("EntityList.saveToStream(): " + ex.getMessage());
            throw ex;
        }
    }
}
