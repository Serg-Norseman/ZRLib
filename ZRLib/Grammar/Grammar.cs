/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
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
using System.Text;
using BSLib;

namespace ZRLib.Grammar
{
    public enum Case
    {
        cUndefined,
        cNominative,
        cGenitive,
        cDative,
        cAccusative,
        cInstrumental,
        cPrepositional
    }

    public enum Declension
    {
        dUndefined,
        d1,
        d2,
        d3,
        d4
    }

    public enum Gender
    {
        gUndefined,
        gMale,
        gFemale,
        gNeutral
    }

    public enum Number
    {
        nUndefined,
        nSingle,
        nPlural
    }

    public static class Grammar
    {
        public const int VOICE_UNDEFINED = 0;
        public const int VOICE_ACTIVE = 1;
        public const int VOICE_PASSIVE = 2;

        private const string SWordTooShort = "Word too short";
        private const string SBaseTooShort = "Word base too short";
        private const string vowels = "АаЕеЁёИиЙйОоУуЮюЫыЭэЯяEeUuIiOoAaJj";
        private const string consonants = "БбВвГгДдЖжЗзКкЛлМмНнПпРрСсТтЦцШшЩщФфХхЧчBbCcDdFfGgHhKkLlMmNnPpQqRrSsTtVvWwXxYyZz";
        private const string mixedconsonants = "гкхчшщж";

        private static readonly string[, , , ] NounEndings;
        private static readonly string[, , , ] AdjectiveEndings;

        private const string EngUpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly SpeechSign[] fSpeechTable;

        private static char endof(string word)
        {
            return word[((word != null) ? word.Length : 0) - 1];
        }

        private static bool ends_in_one_of(string word, string letters)
        {
            int num = (letters != null) ? letters.Length : 0;
            for (int i = 0; i < num; i++) {
                if (endof(word) == letters[i]) {
                    return true;
                }
            }
            return false;
        }

        private static bool isConsonant(char c)
        {
            return consonants.IndexOf(c) >= 0;
        }

        private static bool isVowel(char c)
        {
            return vowels.IndexOf(c) >= 0;
        }

        public static string morphNoun(string noun, Case ncase, Number num, Gender gender, bool animate, bool endingstressed)
        {
            if (((noun != null) ? noun.Length : 0) < 2) {
                throw new Exception(SWordTooShort);
            }

            char e = noun[noun.Length - 1];
            string _base;
            bool jot;
            Declension decl;
            bool soft;
            string result;

            switch (e) {
                case 'а':
                    _base = noun.Substring(0, noun.Length - 1);
                    jot = false;
                    decl = Declension.d1;
                    soft = false;
                    break;
                case 'я':
                    _base = noun.Substring(0, noun.Length - 1);
                    jot = (isVowel(endof(_base)) || endof(_base) == 'ь');
                    decl = Declension.d1;
                    soft = true;
                    break;
                case 'о':
                    _base = noun.Substring(0, noun.Length - 1);
                    jot = false;
                    decl = Declension.d3;
                    soft = false;
                    break;
                case 'е':
                case 'ё':
                    _base = noun.Substring(0, noun.Length - 1);
                    jot = (isVowel(endof(_base)) || endof(_base) == 'ь');
                    decl = Declension.d3;
                    soft = true;
                    break;
                case 'й':
                    _base = noun.Substring(0, noun.Length - 1);
                    jot = true;
                    decl = Declension.d2;
                    soft = true;
                    break;
                case 'ь':
                    _base = noun.Substring(0, noun.Length - 1);
                    jot = false;
                    decl = Declension.d4;
                    soft = false;
                    break;
                default:
                    if (isConsonant(e)) {
                        _base = noun;
                        jot = false;
                        decl = Declension.d2;
                        soft = false;
                    } else {
                        result = noun;
                        return result;
                    }
                    break;
            }

            if (((_base != null) ? _base.Length : 0) < 2) {
                throw new Exception(SBaseTooShort);
            }

            if (animate && ncase == Case.cAccusative && ((decl == Declension.d1 && num == Number.nPlural) || decl == Declension.d2)) {
                ncase = Case.cGenitive;
            }

            string ending = NounEndings[(int)decl - 1, (soft ? 1 : 0), (int)num - 1, (int)ncase - 1];

            if (gender == Gender.gNeutral && num == Number.nPlural && ncase == Case.cGenitive) {
                ending = "";
            }

            if (num == Number.nSingle && ncase == Case.cPrepositional && jot && endof(_base) == 'и') {
                ending = "и";
            }

            if (ends_in_one_of(_base, mixedconsonants) && ending.CompareTo("ы") == 0) {
                ending = "и";
            }

            if (decl == Declension.d1) {
                if (((ending != null) ? ending.Length : 0) == 0 & jot) {
                    _base += "й";
                }
                if (num == Number.nSingle) {
                    if (ncase == Case.cInstrumental) {
                        if (ends_in_one_of(_base, "жшщчц")) {
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
                    if ((ncase == Case.cDative & jot) && endof(_base) == 'и') {
                        ending = "и";
                    }
                } else {
                    if (ncase == Case.cGenitive) {
                        if (_base[((_base != null) ? _base.Length : 0) - 1] == 'ь') {
                            _base = _base.Substring(0, _base.Length - 1) + _base.Substring(_base.Length);
                        }
                        char c2 = _base[((_base != null) ? _base.Length : 0) - 1 - 1];
                        char c3 = _base[((_base != null) ? _base.Length : 0) - 1];
                        bool harden = false;
                        if ((isConsonant(c2) || c2 == 'ь') && isConsonant(c3)) {
                            char vowel = '\0';
                            if (_base.CompareTo("кочерг") == 0) {
                                vowel = 'ё';
                            } else {
                                if (soft) {
                                    if (jot & endingstressed) {
                                        vowel = 'е';
                                    } else {
                                        vowel = 'и';
                                    }
                                    if (c3 == 'н') {
                                        harden = (_base.CompareTo("барышн") != 0 && _base.CompareTo("боярышн") != 0 && _base.CompareTo("деревн") != 0);
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
                                    StringBuilder sb = new StringBuilder(_base);
                                    sb[_base.Length - 2] = vowel;
                                    _base = sb.ToString();
                                } else {
                                    StringBuilder sb = new StringBuilder(_base);
                                    sb.Insert(_base.Length - 1, vowel);
                                    _base = sb.ToString();
                                }
                            }
                        }
                        if (soft && !jot && !harden) {
                            _base += "ь";
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
                    if (((ending != null) ? ending.Length : 0) == 0 & jot) {
                        _base += "й";
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
            result = _base + ending;
            return result;
        }

        private static int findSymbol(SpeechSign[] table, string sym)
        {
            for (int i = 0; i < table.Length; i++) {
                if (sym.IndexOf(table[i].Sign) == 0) {
                    return i;
                }
            }
            return -1;
        }

        public static string getTransliterateName(String name)
        {
            string result = "";

            string tmp = name.ToLower();
            while (name.Length != 0) {
                int signLen;

                int idx = findSymbol(fSpeechTable, tmp);
                if (idx >= 0) {
                    string sign = fSpeechTable[idx].Sign;
                    signLen = ((sign != null) ? sign.Length : 0);
                    string src = name.Substring(0, signLen);
                    string tgt = fSpeechTable[idx].Sound;
                    if (EngUpperChars.IndexOf(src[0]) >= 0) {
                        tgt = ConvertHelper.UniformName(tgt);
                    }
                    result += tgt;
                } else {
                    signLen = 1;
                    result += name[0];
                }

                tmp = tmp.Substring(signLen);
                name = name.Substring(signLen);
            }

            return result;
        }

        public static string morphAdjective(String adjective, Case c, Number q, Gender g)
        {
            if (((adjective != null) ? adjective.Length : 0) < 4) {
                throw new Exception(SWordTooShort);
            }

            char e2 = adjective[adjective.Length - 1];
            char e = adjective[adjective.Length - 2];

            string _base;
            bool soft;
            if (e == 'ы' && e2 == 'й') {
                _base = adjective.Substring(0, adjective.Length - 2);
                soft = false;
            } else {
                if (e == 'и' && e2 == 'й') {
                    _base = adjective.Substring(0, adjective.Length - 2);
                    soft = true;
                } else {
                    if (e == 'о' && e2 == 'й') {
                        _base = adjective.Substring(0, adjective.Length - 2);
                        soft = true;
                    } else {
                        if (e == 'а' && e2 == 'я') {
                            _base = adjective.Substring(0, adjective.Length - 2);
                            soft = false;
                        } else {
                            if (e == 'я' && e2 == 'я') {
                                _base = adjective.Substring(0, adjective.Length - 2);
                                soft = true;
                            } else {
                                if (e == 'о' && e2 == 'е') {
                                    _base = adjective.Substring(0, adjective.Length - 2);
                                    soft = false;
                                } else {
                                    if (e == 'е' && e2 == 'е') {
                                        _base = adjective.Substring(0, adjective.Length - 2);
                                        soft = true;
                                    } else {
                                        if (!isConsonant(e)) {
                                            return adjective;
                                        }
                                        _base = adjective;
                                        soft = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (((_base != null) ? _base.Length : 0) < 2) {
                throw new Exception(SBaseTooShort);
            }

            string ending = AdjectiveEndings[(int)g - 1, (int)(soft ? 1 : 0), (int)q - 1, (int)c - 1];
            return _base + ending;
        }

        static Grammar()
        {
            string[,,,] array = new string[4, 2, 2, 6];
            array[0, 0, 0, 0] = "а";
            array[0, 0, 0, 1] = "ы";
            array[0, 0, 0, 2] = "е";
            array[0, 0, 0, 3] = "у";
            array[0, 0, 0, 4] = "ой";
            array[0, 0, 0, 5] = "е";
            array[0, 0, 1, 0] = "ы";
            array[0, 0, 1, 1] = "";
            array[0, 0, 1, 2] = "ам";
            array[0, 0, 1, 3] = "";
            array[0, 0, 1, 4] = "ами";
            array[0, 0, 1, 5] = "ах";
            array[0, 1, 0, 0] = "я";
            array[0, 1, 0, 1] = "и";
            array[0, 1, 0, 2] = "е";
            array[0, 1, 0, 3] = "ю";
            array[0, 1, 0, 4] = "ей";
            array[0, 1, 0, 5] = "е";
            array[0, 1, 1, 0] = "и";
            array[0, 1, 1, 1] = "ей";
            array[0, 1, 1, 2] = "ям";
            array[0, 1, 1, 3] = "ей";
            array[0, 1, 1, 4] = "ями";
            array[0, 1, 1, 5] = "ях";
            array[1, 0, 0, 0] = "";
            array[1, 0, 0, 1] = "а";
            array[1, 0, 0, 2] = "у";
            array[1, 0, 0, 3] = "а";
            array[1, 0, 0, 4] = "ом";
            array[1, 0, 0, 5] = "е";
            array[1, 0, 1, 0] = "ы";
            array[1, 0, 1, 1] = "ов";
            array[1, 0, 1, 2] = "ам";
            array[1, 0, 1, 3] = "ов";
            array[1, 0, 1, 4] = "ами";
            array[1, 0, 1, 5] = "ах";
            array[1, 1, 0, 0] = "ь";
            array[1, 1, 0, 1] = "я";
            array[1, 1, 0, 2] = "ю";
            array[1, 1, 0, 3] = "я";
            array[1, 1, 0, 4] = "ем";
            array[1, 1, 0, 5] = "е";
            array[1, 1, 1, 0] = "и";
            array[1, 1, 1, 1] = "ей";
            array[1, 1, 1, 2] = "ям";
            array[1, 1, 1, 3] = "ей";
            array[1, 1, 1, 4] = "ями";
            array[1, 1, 1, 5] = "ях";
            array[2, 0, 0, 0] = "о";
            array[2, 0, 0, 1] = "а";
            array[2, 0, 0, 2] = "у";
            array[2, 0, 0, 3] = "о";
            array[2, 0, 0, 4] = "ом";
            array[2, 0, 0, 5] = "е";
            array[2, 0, 1, 0] = "а";
            array[2, 0, 1, 1] = "";
            array[2, 0, 1, 2] = "ам";
            array[2, 0, 1, 3] = "а";
            array[2, 0, 1, 4] = "ами";
            array[2, 0, 1, 5] = "ах";
            array[2, 1, 0, 0] = "е";
            array[2, 1, 0, 1] = "я";
            array[2, 1, 0, 2] = "ю";
            array[2, 1, 0, 3] = "е";
            array[2, 1, 0, 4] = "ем";
            array[2, 1, 0, 5] = "е";
            array[2, 1, 1, 0] = "я";
            array[2, 1, 1, 1] = "ей";
            array[2, 1, 1, 2] = "ям";
            array[2, 1, 1, 3] = "я";
            array[2, 1, 1, 4] = "ями";
            array[2, 1, 1, 5] = "ях";
            array[3, 0, 0, 0] = "ь";
            array[3, 0, 0, 1] = "и";
            array[3, 0, 0, 2] = "и";
            array[3, 0, 0, 3] = "ь";
            array[3, 0, 0, 4] = "ью";
            array[3, 0, 0, 5] = "и";
            array[3, 0, 1, 0] = "и";
            array[3, 0, 1, 1] = "ей";
            array[3, 0, 1, 2] = "ам";
            array[3, 0, 1, 3] = "ей";
            array[3, 0, 1, 4] = "ами";
            array[3, 0, 1, 5] = "ах";
            array[3, 1, 0, 0] = "ь";
            array[3, 1, 0, 1] = "и";
            array[3, 1, 0, 2] = "и";
            array[3, 1, 0, 3] = "ь";
            array[3, 1, 0, 4] = "ью";
            array[3, 1, 0, 5] = "и";
            array[3, 1, 1, 0] = "и";
            array[3, 1, 1, 1] = "ей";
            array[3, 1, 1, 2] = "ям";
            array[3, 1, 1, 3] = "ей";
            array[3, 1, 1, 4] = "ями";
            array[3, 1, 1, 5] = "ях";
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

            string[, , , ] array2 = new string[3, 3, 2, 6];
            array2[0, 0, 0, 0] = "ый";
            array2[0, 0, 0, 1] = "ого";
            array2[0, 0, 0, 2] = "ому";
            array2[0, 0, 0, 3] = "ого";
            array2[0, 0, 0, 4] = "ым";
            array2[0, 0, 0, 5] = "ом";
            array2[0, 0, 1, 0] = "ые";
            array2[0, 0, 1, 1] = "ых";
            array2[0, 0, 1, 2] = "ым";
            array2[0, 0, 1, 3] = "ых";
            array2[0, 0, 1, 4] = "ыми";
            array2[0, 0, 1, 5] = "ых";
            array2[0, 1, 0, 0] = "ий";
            array2[0, 1, 0, 1] = "его";
            array2[0, 1, 0, 2] = "ему";
            array2[0, 1, 0, 3] = "его";
            array2[0, 1, 0, 4] = "им";
            array2[0, 1, 0, 5] = "ем";
            array2[0, 1, 1, 0] = "ие";
            array2[0, 1, 1, 1] = "их";
            array2[0, 1, 1, 2] = "им";
            array2[0, 1, 1, 3] = "их";
            array2[0, 1, 1, 4] = "ими";
            array2[0, 1, 1, 5] = "их";
            array2[0, 2, 0, 0] = "ие";
            array2[0, 2, 0, 1] = "их";
            array2[0, 2, 0, 2] = "им";
            array2[0, 2, 0, 3] = "их";
            array2[0, 2, 0, 4] = "ими";
            array2[0, 2, 0, 5] = "их";
            array2[0, 2, 1, 0] = "ие";
            array2[0, 2, 1, 1] = "их";
            array2[0, 2, 1, 2] = "им";
            array2[0, 2, 1, 3] = "их";
            array2[0, 2, 1, 4] = "ими";
            array2[0, 2, 1, 5] = "их";
            array2[1, 0, 0, 0] = "ая";
            array2[1, 0, 0, 1] = "ой";
            array2[1, 0, 0, 2] = "ой";
            array2[1, 0, 0, 3] = "ую";
            array2[1, 0, 0, 4] = "ой";
            array2[1, 0, 0, 5] = "ой";
            array2[1, 0, 1, 0] = "ые";
            array2[1, 0, 1, 1] = "ых";
            array2[1, 0, 1, 2] = "ым";
            array2[1, 0, 1, 3] = "ых";
            array2[1, 0, 1, 4] = "ыми";
            array2[1, 0, 1, 5] = "ых";
            array2[1, 1, 0, 0] = "яя";
            array2[1, 1, 0, 1] = "ей";
            array2[1, 1, 0, 2] = "ей";
            array2[1, 1, 0, 3] = "юю";
            array2[1, 1, 0, 4] = "ей";
            array2[1, 1, 0, 5] = "ей";
            array2[1, 1, 1, 0] = "ие";
            array2[1, 1, 1, 1] = "их";
            array2[1, 1, 1, 2] = "им";
            array2[1, 1, 1, 3] = "их";
            array2[1, 1, 1, 4] = "ими";
            array2[1, 1, 1, 5] = "их";
            array2[1, 2, 0, 0] = "";
            array2[1, 2, 0, 1] = "";
            array2[1, 2, 0, 2] = "";
            array2[1, 2, 0, 3] = "";
            array2[1, 2, 0, 4] = "";
            array2[1, 2, 0, 5] = "";
            array2[1, 2, 1, 0] = "";
            array2[1, 2, 1, 1] = "";
            array2[1, 2, 1, 2] = "";
            array2[1, 2, 1, 3] = "";
            array2[1, 2, 1, 4] = "";
            array2[1, 2, 1, 5] = "";
            array2[2, 0, 0, 0] = "ое";
            array2[2, 0, 0, 1] = "ого";
            array2[2, 0, 0, 2] = "ому";
            array2[2, 0, 0, 3] = "ое";
            array2[2, 0, 0, 4] = "ым";
            array2[2, 0, 0, 5] = "ом";
            array2[2, 0, 1, 0] = "ые";
            array2[2, 0, 1, 1] = "ых";
            array2[2, 0, 1, 2] = "ым";
            array2[2, 0, 1, 3] = "ых";
            array2[2, 0, 1, 4] = "ыми";
            array2[2, 0, 1, 5] = "ых";
            array2[2, 1, 0, 0] = "ее";
            array2[2, 1, 0, 1] = "его";
            array2[2, 1, 0, 2] = "ему";
            array2[2, 1, 0, 3] = "ее";
            array2[2, 1, 0, 4] = "им";
            array2[2, 1, 0, 5] = "ем";
            array2[2, 1, 1, 0] = "ие";
            array2[2, 1, 1, 1] = "их";
            array2[2, 1, 1, 2] = "им";
            array2[2, 1, 1, 3] = "их";
            array2[2, 1, 1, 4] = "ими";
            array2[2, 1, 1, 5] = "их";
            array2[2, 2, 0, 0] = "";
            array2[2, 2, 0, 1] = "";
            array2[2, 2, 0, 2] = "";
            array2[2, 2, 0, 3] = "";
            array2[2, 2, 0, 4] = "";
            array2[2, 2, 0, 5] = "";
            array2[2, 2, 1, 0] = "";
            array2[2, 2, 1, 1] = "";
            array2[2, 2, 1, 2] = "";
            array2[2, 2, 1, 3] = "";
            array2[2, 2, 1, 4] = "";
            array2[2, 2, 1, 5] = "";
            AdjectiveEndings = array2;
        }
    }
}
