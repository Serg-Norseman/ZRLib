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
package jzrlib.grammar;

import jzrlib.utils.TextUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Grammar
{
    public static final int VOICE_UNDEFINED = 0;
    public static final int VOICE_ACTIVE = 1;
    public static final int VOICE_PASSIVE = 2;

    private static final String SWordTooShort = "Word too short";
    private static final String SBaseTooShort = "Word base too short";
    private static final String vowels = "АаЕеЁёИиЙйОоУуЮюЫыЭэЯяEeUuIiOoAaJj";
    private static final String consonants = "БбВвГгДдЖжЗзКкЛлМмНнПпРрСсТтЦцШшЩщФфХхЧчBbCcDdFfGgHhKkLlMmNnPpQqRrSsTtVvWwXxYyZz";
    private static final String mixedconsonants = "гкхчшщж";

    private static final String[][][][] NounEndings;
    private static final String[][][][] AdjectiveEndings;

    private static final String EngUpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static final SpeechSign[] fSpeechTable;

    private static char endof(String word)
    {
        return word.charAt(((word != null) ? word.length() : 0) - 1);
    }

    private static boolean ends_in_one_of(String word, String letters)
    {
        int num = (letters != null) ? letters.length() : 0;
        for (int i = 0; i < num; i++) {
            if (Grammar.endof(word) == letters.charAt(i)) {
                return true;
            }
        }
        return false;
    }

    private static boolean isConsonant(char c)
    {
        return Grammar.consonants.indexOf(c) >= 0;
    }

    private static boolean isVowel(char c)
    {
        return Grammar.vowels.indexOf(c) >= 0;
    }

    public static String morphNoun(String noun, Case ncase, Number num, Gender gender, boolean animate, boolean endingstressed)
    {
        if (((noun != null) ? noun.length() : 0) < 2) {
            throw new RuntimeException(Grammar.SWordTooShort);
        }

        char e = noun.charAt(noun.length() - 1);
        String base;
        boolean jot;
        Declension decl;
        boolean soft;
        String result;

        switch (e) {
            case 'а':
                base = noun.substring(0, noun.length() - 1);
                jot = false;
                decl = Declension.d1;
                soft = false;
                break;
            case 'я':
                base = noun.substring(0, noun.length() - 1);
                jot = (Grammar.isVowel(Grammar.endof(base)) || Grammar.endof(base) == 'ь');
                decl = Declension.d1;
                soft = true;
                break;
            case 'о':
                base = noun.substring(0, noun.length() - 1);
                jot = false;
                decl = Declension.d3;
                soft = false;
                break;
            case 'е':
            case 'ё':
                base = noun.substring(0, noun.length() - 1);
                jot = (Grammar.isVowel(Grammar.endof(base)) || Grammar.endof(base) == 'ь');
                decl = Declension.d3;
                soft = true;
                break;
            case 'й':
                base = noun.substring(0, noun.length() - 1);
                jot = true;
                decl = Declension.d2;
                soft = true;
                break;
            case 'ь':
                base = noun.substring(0, noun.length() - 1);
                jot = false;
                decl = Declension.d4;
                soft = false;
                break;
            default:
                if (Grammar.isConsonant(e)) {
                    base = noun;
                    jot = false;
                    decl = Declension.d2;
                    soft = false;
                } else {
                    result = noun;
                    return result;
                }
                break;
        }

        if (((base != null) ? base.length() : 0) < 2) {
            throw new RuntimeException(Grammar.SBaseTooShort);
        }

        if (animate && ncase == Case.cAccusative && ((decl == Declension.d1 && num == Number.nPlural) || decl == Declension.d2)) {
            ncase = Case.cGenitive;
        }

        String ending = Grammar.NounEndings[decl.getValue() - 1][(soft ? 1 : 0)][num.getValue() - 1][ncase.getValue() - 1];

        if (gender == Gender.gNeutral && num == Number.nPlural && ncase == Case.cGenitive) {
            ending = "";
        }

        if (num == Number.nSingle && ncase == Case.cPrepositional && jot && Grammar.endof(base) == 'и') {
            ending = "и";
        }

        if (Grammar.ends_in_one_of(base, Grammar.mixedconsonants) && ending.compareTo("ы") == 0) {
            ending = "и";
        }

        if (decl == Declension.d1) {
            if (((ending != null) ? ending.length() : 0) == 0 & jot) {
                base += "й";
            }
            if (num == Number.nSingle) {
                if (ncase == Case.cInstrumental) {
                    if (Grammar.ends_in_one_of(base, "жшщчц")) {
                        if (endingstressed) {
                            ending = "ой";
                        } else {
                            ending = "ей";
                        }
                    } else {
                        if (soft & endingstressed) {
                            ending = "ёй";
                        }
                    }
                }
                if ((ncase == Case.cDative & jot) && Grammar.endof(base) == 'и') {
                    ending = "и";
                }
            } else {
                if (ncase == Case.cGenitive) {
                    if (base.charAt(((base != null) ? base.length() : 0) - 1) == 'ь') {
                        base = base.substring(0, base.length() - 1) + base.substring(base.length());
                    }
                    char c2 = base.charAt(((base != null) ? base.length() : 0) - 1 - 1);
                    char c3 = base.charAt(((base != null) ? base.length() : 0) - 1);
                    boolean harden = false;
                    if ((Grammar.isConsonant(c2) || c2 == 'ь') && Grammar.isConsonant(c3)) {
                        char vowel = '\0';
                        if (base.compareTo("кочерг") == 0) {
                            vowel = 'ё';
                        } else {
                            if (soft) {
                                if (jot & endingstressed) {
                                    vowel = 'е';
                                } else {
                                    vowel = 'и';
                                }
                                if (c3 == 'н') {
                                    harden = (base.compareTo("барышн") != 0 && base.compareTo("боярышн") != 0 && base.compareTo("деревн") != 0);
                                }
                            } else {
                                if (c2 == 'ь') {
                                    vowel = 'е';
                                } else {
                                    if (c3 == 'к') {
                                        if (c2 == 'й') {
                                            vowel = 'е';
                                        } else {
                                            vowel = 'о';
                                        }
                                    }
                                }
                            }
                        }

                        if (vowel != '\0') {
                            if (c2 == 'ь' || c2 == 'й') {
                                StringBuilder sb = new StringBuilder(base);
                                sb.setCharAt(base.length() - 2, vowel);
                                base = sb.toString();
                            } else {
                                StringBuilder sb = new StringBuilder(base);
                                sb.insert(base.length() - 1, vowel);
                                base = sb.toString();
                            }
                        }
                    }
                    if (soft && !jot && !harden) {
                        base += "ь";
                    }
                }
            }
        } else {
            if (decl == Declension.d2) {
                if (ncase == Case.cAccusative) {
                    ncase = Case.cNominative;
                }
                if (num == Number.nSingle && ncase == Case.cNominative) {
                    if (e == 'е') {
                        ending = "е";
                    }
                    if (e == 'о') {
                        ending = "о";
                    }
                }
                if (((ending != null) ? ending.length() : 0) == 0 & jot) {
                    base += "й";
                }
                if (gender == Gender.gNeutral && num == Number.nPlural && ncase == Case.cNominative) {
                    if (soft) {
                        ending = "я";
                    } else {
                        ending = "а";
                    }
                }
            }
        }
        result = base + ending;
        return result;
    }

    private static int findSymbol(SpeechSign[] table, String sym)
    {
        for (int i = 0; i < table.length; i++) {
            if (sym.indexOf(table[i].Sign) == 0) {
                return i;
            }
        }
        return -1;
    }

    public static String getTransliterateName(String name)
    {
        String result = "";

        String tmp = name.toLowerCase();
        while (name.length() != 0) {
            int signLen;

            int idx = findSymbol(fSpeechTable, tmp);
            if (idx >= 0) {
                String sign = fSpeechTable[idx].Sign;
                signLen = ((sign != null) ? sign.length() : 0);
                String src = name.substring(0, signLen);
                String tgt = fSpeechTable[idx].Sound;
                if (EngUpperChars.indexOf(src.charAt(0)) >= 0) {
                    tgt = TextUtils.upperFirst(tgt);
                }
                result += tgt;
            } else {
                signLen = 1;
                result += name.charAt(0);
            }

            tmp = tmp.substring(signLen);
            name = name.substring(signLen);
        }

        return result;
    }

    public static String morphAdjective(String adjective, Case c, Number q, Gender g)
    {
        if (((adjective != null) ? adjective.length() : 0) < 4) {
            throw new RuntimeException(Grammar.SWordTooShort);
        }

        char e2 = adjective.charAt(adjective.length() - 1);
        char e = adjective.charAt(adjective.length() - 2);

        String base;
        boolean soft;
        if (e == 'ы' && e2 == 'й') {
            base = adjective.substring(0, adjective.length() - 2);
            soft = false;
        } else {
            if (e == 'и' && e2 == 'й') {
                base = adjective.substring(0, adjective.length() - 2);
                soft = true;
            } else {
                if (e == 'о' && e2 == 'й') {
                    base = adjective.substring(0, adjective.length() - 2);
                    soft = true;
                } else {
                    if (e == 'а' && e2 == 'я') {
                        base = adjective.substring(0, adjective.length() - 2);
                        soft = false;
                    } else {
                        if (e == 'я' && e2 == 'я') {
                            base = adjective.substring(0, adjective.length() - 2);
                            soft = true;
                        } else {
                            if (e == 'о' && e2 == 'е') {
                                base = adjective.substring(0, adjective.length() - 2);
                                soft = false;
                            } else {
                                if (e == 'е' && e2 == 'е') {
                                    base = adjective.substring(0, adjective.length() - 2);
                                    soft = true;
                                } else {
                                    if (!Grammar.isConsonant(e)) {
                                        return adjective;
                                    }
                                    base = adjective;
                                    soft = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (((base != null) ? base.length() : 0) < 2) {
            throw new RuntimeException(Grammar.SBaseTooShort);
        }

        String ending = Grammar.AdjectiveEndings[g.getValue() - 1][(int) (soft ? 1 : 0)][q.getValue() - 1][c.getValue() - 1];
        return base + ending;
    }

    static {
        String[][][][] array = new String[4][2][2][6];
        array[0][0][0][0] = "а";
        array[0][0][0][1] = "ы";
        array[0][0][0][2] = "е";
        array[0][0][0][3] = "у";
        array[0][0][0][4] = "ой";
        array[0][0][0][5] = "е";
        array[0][0][1][0] = "ы";
        array[0][0][1][1] = "";
        array[0][0][1][2] = "ам";
        array[0][0][1][3] = "";
        array[0][0][1][4] = "ами";
        array[0][0][1][5] = "ах";
        array[0][1][0][0] = "я";
        array[0][1][0][1] = "и";
        array[0][1][0][2] = "е";
        array[0][1][0][3] = "ю";
        array[0][1][0][4] = "ей";
        array[0][1][0][5] = "е";
        array[0][1][1][0] = "и";
        array[0][1][1][1] = "ей";
        array[0][1][1][2] = "ям";
        array[0][1][1][3] = "ей";
        array[0][1][1][4] = "ями";
        array[0][1][1][5] = "ях";
        array[1][0][0][0] = "";
        array[1][0][0][1] = "а";
        array[1][0][0][2] = "у";
        array[1][0][0][3] = "а";
        array[1][0][0][4] = "ом";
        array[1][0][0][5] = "е";
        array[1][0][1][0] = "ы";
        array[1][0][1][1] = "ов";
        array[1][0][1][2] = "ам";
        array[1][0][1][3] = "ов";
        array[1][0][1][4] = "ами";
        array[1][0][1][5] = "ах";
        array[1][1][0][0] = "ь";
        array[1][1][0][1] = "я";
        array[1][1][0][2] = "ю";
        array[1][1][0][3] = "я";
        array[1][1][0][4] = "ем";
        array[1][1][0][5] = "е";
        array[1][1][1][0] = "и";
        array[1][1][1][1] = "ей";
        array[1][1][1][2] = "ям";
        array[1][1][1][3] = "ей";
        array[1][1][1][4] = "ями";
        array[1][1][1][5] = "ях";
        array[2][0][0][0] = "о";
        array[2][0][0][1] = "а";
        array[2][0][0][2] = "у";
        array[2][0][0][3] = "о";
        array[2][0][0][4] = "ом";
        array[2][0][0][5] = "е";
        array[2][0][1][0] = "а";
        array[2][0][1][1] = "";
        array[2][0][1][2] = "ам";
        array[2][0][1][3] = "а";
        array[2][0][1][4] = "ами";
        array[2][0][1][5] = "ах";
        array[2][1][0][0] = "е";
        array[2][1][0][1] = "я";
        array[2][1][0][2] = "ю";
        array[2][1][0][3] = "е";
        array[2][1][0][4] = "ем";
        array[2][1][0][5] = "е";
        array[2][1][1][0] = "я";
        array[2][1][1][1] = "ей";
        array[2][1][1][2] = "ям";
        array[2][1][1][3] = "я";
        array[2][1][1][4] = "ями";
        array[2][1][1][5] = "ях";
        array[3][0][0][0] = "ь";
        array[3][0][0][1] = "и";
        array[3][0][0][2] = "и";
        array[3][0][0][3] = "ь";
        array[3][0][0][4] = "ью";
        array[3][0][0][5] = "и";
        array[3][0][1][0] = "и";
        array[3][0][1][1] = "ей";
        array[3][0][1][2] = "ам";
        array[3][0][1][3] = "ей";
        array[3][0][1][4] = "ами";
        array[3][0][1][5] = "ах";
        array[3][1][0][0] = "ь";
        array[3][1][0][1] = "и";
        array[3][1][0][2] = "и";
        array[3][1][0][3] = "ь";
        array[3][1][0][4] = "ью";
        array[3][1][0][5] = "и";
        array[3][1][1][0] = "и";
        array[3][1][1][1] = "ей";
        array[3][1][1][2] = "ям";
        array[3][1][1][3] = "ей";
        array[3][1][1][4] = "ями";
        array[3][1][1][5] = "ях";
        NounEndings = array;

        fSpeechTable = new SpeechSign[35];
        fSpeechTable[0] = new SpeechSign("ae", "э", SyllableKind.sk_Undefined);
        fSpeechTable[1] = new SpeechSign("au", "о", SyllableKind.sk_Opened);
        fSpeechTable[2] = new SpeechSign("au", "ау", SyllableKind.sk_Closed);
        fSpeechTable[3] = new SpeechSign("ei", "ей", SyllableKind.sk_Undefined);
        fSpeechTable[4] = new SpeechSign("ey", "ей", SyllableKind.sk_Undefined);
        fSpeechTable[5] = new SpeechSign("ja", "ья", SyllableKind.sk_Undefined);
        fSpeechTable[6] = new SpeechSign("jo", "ьё", SyllableKind.sk_Undefined);
        fSpeechTable[7] = new SpeechSign("th", "т", SyllableKind.sk_Undefined);
        fSpeechTable[8] = new SpeechSign("we", "вэ", SyllableKind.sk_Undefined);
        fSpeechTable[9] = new SpeechSign("a", "а", SyllableKind.sk_Undefined);
        fSpeechTable[10] = new SpeechSign("b", "б", SyllableKind.sk_Undefined);
        fSpeechTable[11] = new SpeechSign("c", "к", SyllableKind.sk_Undefined);
        fSpeechTable[12] = new SpeechSign("d", "д", SyllableKind.sk_Undefined);
        fSpeechTable[13] = new SpeechSign("e", "е", SyllableKind.sk_Undefined);
        fSpeechTable[14] = new SpeechSign("f", "ф", SyllableKind.sk_Undefined);
        fSpeechTable[15] = new SpeechSign("g", "г", SyllableKind.sk_Undefined);
        fSpeechTable[16] = new SpeechSign("h", "х", SyllableKind.sk_Undefined);
        fSpeechTable[17] = new SpeechSign("i", "и", SyllableKind.sk_Undefined);
        fSpeechTable[18] = new SpeechSign("j", "ьи", SyllableKind.sk_Undefined);
        fSpeechTable[19] = new SpeechSign("k", "к", SyllableKind.sk_Undefined);
        fSpeechTable[20] = new SpeechSign("l", "л", SyllableKind.sk_Undefined);
        fSpeechTable[21] = new SpeechSign("m", "м", SyllableKind.sk_Undefined);
        fSpeechTable[22] = new SpeechSign("n", "н", SyllableKind.sk_Undefined);
        fSpeechTable[23] = new SpeechSign("o", "о", SyllableKind.sk_Undefined);
        fSpeechTable[24] = new SpeechSign("p", "п", SyllableKind.sk_Undefined);
        fSpeechTable[25] = new SpeechSign("q", "к", SyllableKind.sk_Undefined);
        fSpeechTable[26] = new SpeechSign("r", "р", SyllableKind.sk_Undefined);
        fSpeechTable[27] = new SpeechSign("s", "с", SyllableKind.sk_Undefined);
        fSpeechTable[28] = new SpeechSign("t", "т", SyllableKind.sk_Undefined);
        fSpeechTable[29] = new SpeechSign("u", "у", SyllableKind.sk_Undefined);
        fSpeechTable[30] = new SpeechSign("v", "в", SyllableKind.sk_Undefined);
        fSpeechTable[31] = new SpeechSign("w", "в", SyllableKind.sk_Undefined);
        fSpeechTable[32] = new SpeechSign("x", "кс", SyllableKind.sk_Undefined);
        fSpeechTable[33] = new SpeechSign("y", "и", SyllableKind.sk_Undefined);
        fSpeechTable[34] = new SpeechSign("z", "з", SyllableKind.sk_Undefined);

        String[][][][] array2 = new String[3][3][2][6];
        array2[0][0][0][0] = "ый";
        array2[0][0][0][1] = "ого";
        array2[0][0][0][2] = "ому";
        array2[0][0][0][3] = "ого";
        array2[0][0][0][4] = "ым";
        array2[0][0][0][5] = "ом";
        array2[0][0][1][0] = "ые";
        array2[0][0][1][1] = "ых";
        array2[0][0][1][2] = "ым";
        array2[0][0][1][3] = "ых";
        array2[0][0][1][4] = "ыми";
        array2[0][0][1][5] = "ых";
        array2[0][1][0][0] = "ий";
        array2[0][1][0][1] = "его";
        array2[0][1][0][2] = "ему";
        array2[0][1][0][3] = "его";
        array2[0][1][0][4] = "им";
        array2[0][1][0][5] = "ем";
        array2[0][1][1][0] = "ие";
        array2[0][1][1][1] = "их";
        array2[0][1][1][2] = "им";
        array2[0][1][1][3] = "их";
        array2[0][1][1][4] = "ими";
        array2[0][1][1][5] = "их";
        array2[0][2][0][0] = "ие";
        array2[0][2][0][1] = "их";
        array2[0][2][0][2] = "им";
        array2[0][2][0][3] = "их";
        array2[0][2][0][4] = "ими";
        array2[0][2][0][5] = "их";
        array2[0][2][1][0] = "ие";
        array2[0][2][1][1] = "их";
        array2[0][2][1][2] = "им";
        array2[0][2][1][3] = "их";
        array2[0][2][1][4] = "ими";
        array2[0][2][1][5] = "их";
        array2[1][0][0][0] = "ая";
        array2[1][0][0][1] = "ой";
        array2[1][0][0][2] = "ой";
        array2[1][0][0][3] = "ую";
        array2[1][0][0][4] = "ой";
        array2[1][0][0][5] = "ой";
        array2[1][0][1][0] = "ые";
        array2[1][0][1][1] = "ых";
        array2[1][0][1][2] = "ым";
        array2[1][0][1][3] = "ых";
        array2[1][0][1][4] = "ыми";
        array2[1][0][1][5] = "ых";
        array2[1][1][0][0] = "яя";
        array2[1][1][0][1] = "ей";
        array2[1][1][0][2] = "ей";
        array2[1][1][0][3] = "юю";
        array2[1][1][0][4] = "ей";
        array2[1][1][0][5] = "ей";
        array2[1][1][1][0] = "ие";
        array2[1][1][1][1] = "их";
        array2[1][1][1][2] = "им";
        array2[1][1][1][3] = "их";
        array2[1][1][1][4] = "ими";
        array2[1][1][1][5] = "их";
        array2[1][2][0][0] = "";
        array2[1][2][0][1] = "";
        array2[1][2][0][2] = "";
        array2[1][2][0][3] = "";
        array2[1][2][0][4] = "";
        array2[1][2][0][5] = "";
        array2[1][2][1][0] = "";
        array2[1][2][1][1] = "";
        array2[1][2][1][2] = "";
        array2[1][2][1][3] = "";
        array2[1][2][1][4] = "";
        array2[1][2][1][5] = "";
        array2[2][0][0][0] = "ое";
        array2[2][0][0][1] = "ого";
        array2[2][0][0][2] = "ому";
        array2[2][0][0][3] = "ое";
        array2[2][0][0][4] = "ым";
        array2[2][0][0][5] = "ом";
        array2[2][0][1][0] = "ые";
        array2[2][0][1][1] = "ых";
        array2[2][0][1][2] = "ым";
        array2[2][0][1][3] = "ых";
        array2[2][0][1][4] = "ыми";
        array2[2][0][1][5] = "ых";
        array2[2][1][0][0] = "ее";
        array2[2][1][0][1] = "его";
        array2[2][1][0][2] = "ему";
        array2[2][1][0][3] = "ее";
        array2[2][1][0][4] = "им";
        array2[2][1][0][5] = "ем";
        array2[2][1][1][0] = "ие";
        array2[2][1][1][1] = "их";
        array2[2][1][1][2] = "им";
        array2[2][1][1][3] = "их";
        array2[2][1][1][4] = "ими";
        array2[2][1][1][5] = "их";
        array2[2][2][0][0] = "";
        array2[2][2][0][1] = "";
        array2[2][2][0][2] = "";
        array2[2][2][0][3] = "";
        array2[2][2][0][4] = "";
        array2[2][2][0][5] = "";
        array2[2][2][1][0] = "";
        array2[2][2][1][1] = "";
        array2[2][2][1][2] = "";
        array2[2][2][1][3] = "";
        array2[2][2][1][4] = "";
        array2[2][2][1][5] = "";
        AdjectiveEndings = array2;
    }
}
