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
package jzrlib.utils;

import java.io.BufferedWriter;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.Writer;
import java.util.Date;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Logger
{
    private static String fLogFile;

    private static void writeLine(String message)
    {
        try {
            try (Writer writer = new BufferedWriter(new OutputStreamWriter(
                    new FileOutputStream(fLogFile, true), "Cp1251"))) {
                writer.write("[" + new Date().toString() + "] -> " + message + AuxUtils.CRLF);
                writer.flush();
            }
        } catch (IOException ex) {
        }
    }

    public static void init(String fileName)
    {
        Logger.fLogFile = fileName;
        Logger.writeLine("log begin");
    }

    public static void write(String msg)
    {
        Logger.writeLine(msg);
    }

    public static void done()
    {
        Logger.writeLine("log end");
    }
    
}
