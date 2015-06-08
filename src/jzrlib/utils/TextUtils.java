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

import java.util.Arrays;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class TextUtils
{
    public static boolean equals(String str1, String str2)
    {
        return (str1 == null ? str2 == null : str1.equals(str2));
    }

    public static boolean isNullOrEmpty(String str)
    {
        return (str == null || str.length() == 0);
    }

    public static int compareStr(String S1, String S2)
    {
        return S1.compareTo(S2);
    }

    public static int compareText(String S1, String S2)
    {
        return S1.compareToIgnoreCase(S2);
    }

    public static String upperFirst(String val)
    {
        StringBuilder str = new StringBuilder(val);
        str.setCharAt(0, Character.toUpperCase(str.charAt(0)));
        return str.toString();
    }

    public static String uniformName(String val)
    {
        if (TextUtils.isNullOrEmpty(val)) {
            return null;
        }

        StringBuilder str = new StringBuilder(val.toLowerCase());
        str.setCharAt(0, Character.toUpperCase(str.charAt(0)));
        return str.toString();
    }

    public static String getToken(String str, String sepChars, int tokenNum)
    {
        if (isNullOrEmpty(str) || isNullOrEmpty(sepChars)) {
            return "";
        }
        
        if (sepChars.indexOf(str.charAt(str.length() - 1)) < 0) {
            str += sepChars.charAt(0);
        }

        int tok_num = 0;
        String tok = "";

        for (int i = 0; i < str.length(); i++) {
            if (sepChars.indexOf(str.charAt(i)) >= 0) {
                tok_num++;
                if (tok_num == tokenNum) {
                    return tok;
                }
                tok = "";
            } else {
                tok += (str.charAt(i));
            }

        }
        return "";
    }

    public static int getTokensCount(String str, String sepChars)
    {
        if (isNullOrEmpty(str) || isNullOrEmpty(sepChars)) {
            return 0;
        }

        int result = 0;

        if (sepChars.indexOf(str.charAt(str.length() - 1)) < 0) {
            str += sepChars.charAt(0);
        }
        int num = str.length();
        for (int i = 0; i < num; i++) {
            if (sepChars.indexOf(str.charAt(i)) >= 0) {
                result += 1;
            }
        }

        return result;
    }
    
    public static String repeat(char ch, int repeat)
    {
        char[] chars = new char[repeat];
        Arrays.fill(chars, ch);
        return new String(chars);
    }
}
