/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih (aka Alchemist).
 *
 *  this file is part of "NorseWorld: Ragnarok".
 *
 *  this program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  this program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using BSLib;

namespace ZRLib.Engine
{
    public class BaseImage : BaseObject
    {
        protected BaseScreen fScreen;

        public short Height;
        public int TransColor;
        public short Width;
        public bool PaletteMode;

        protected virtual void Done()
        {
        }

        public BaseImage(BaseScreen screen)
        {
            fScreen = screen;
        }

        public BaseImage(BaseScreen screen, string fileName, int transColor)
        {
            fScreen = screen;
            LoadFromFile(fileName, transColor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                Done();
            }
            base.Dispose(disposing);
        }

        public void LoadFromFile(string fileName, int transColor)
        {
            //fileName = AuxUtils.ConvertPath(fileName);
            if (File.Exists(fileName)) {
                try {
                    FileStream stm = new FileStream(fileName, FileMode.Open);
                    try {
                        LoadFromStream(stm, transColor);
                    } finally {
                        stm.Close();
                    }
                } catch (IOException ex) {
                    throw new Exception("BaseImage.loadFromFile(" + fileName + "): " + ex.Message);
                }
            }
        }

        public virtual void LoadFromStream(Stream stream, int transColor)
        {
        }

        public virtual void SetTransDefault()
        {
        }

        public virtual void ReplaceColor(int index, int replacement)
        {
        }
    }
}
