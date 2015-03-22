/*
 *  "MysteriesRL", Java Roguelike game.
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
package mrl.generators;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import jzrlib.utils.Logger;
import jzrlib.utils.TextUtils;

/**
 * 
 * @author Serg V. Zhdanovskih
 */
public final class NameGenerator
{
    private static final List<String> fFemaleNames = new ArrayList<>(1024);
    private static final List<String> fMaleNames = new ArrayList<>(1024);
    private static final List<String> fSurnames = new ArrayList<>(1024);

    static {
        parseNames(fFemaleNames, "female");
        parseNames(fMaleNames, "male");
        parseSurnames();
    }

    private final Random fRandom;

    public NameGenerator()
    {
        fRandom = new Random();
    }

    private static void parseSurnames()
    {
        InputStream is = NameGenerator.class.getResourceAsStream("/resources/namegen/surnames.csv");

        DataInputStream in = new DataInputStream(is);
        BufferedReader br = new BufferedReader(new InputStreamReader(in));
        String strLine;

        try {
            while ((strLine = br.readLine()) != null) {
                String[] line = strLine.replace("\"", "").split(",");
                fSurnames.add(line[0]);
            }
        } catch (IOException ex) {
            Logger.write("NameGenerator.parseSurnames(): " + ex.getMessage());
        }
    }

    private static void parseNames(List<String> names, String sex)
    {
        InputStream is = NameGenerator.class.getResourceAsStream("/resources/namegen/" + sex + ".txt");

        DataInputStream in = new DataInputStream(is);
        BufferedReader br = new BufferedReader(new InputStreamReader(in));
        String strLine;

        try {
            while ((strLine = br.readLine()) != null) {
                String[] line = strLine.split(" ");
                names.add(line[0]);
            }

            in.close();
        } catch (IOException ex) {
            Logger.write("NameGenerator.parseNames(): " + ex.getMessage());
        }
    }

    public final String generateFullName(boolean isMale)
    {
        String name = generateName(isMale);
        String surname = generateSurname();

        return name + " " + surname;
    }

    public final String generateName(boolean isMale)
    {
        String name = "";
        if (isMale) {
            name = fMaleNames.get(fRandom.nextInt(fMaleNames.size()));
        } else {
            name = fFemaleNames.get(fRandom.nextInt(fFemaleNames.size()));
        }

        return TextUtils.uniformName(name);
    }

    public final String generateSurname()
    {
        return TextUtils.uniformName(fSurnames.get(fRandom.nextInt(fSurnames.size())));
    }
}
