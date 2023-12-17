/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015, 2020 by Serg V. Zhdanovskih.
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

using System.Collections.Generic;
using System.IO;
using System.Text;
using ZRLib.Core;

namespace ZRLib.Storyteller
{
    /// <summary>
    /// 
    /// </summary>
    public enum LineType
    {
        Unknown,
        Break,          // empty line
        Text,           // 'A'..'Z', 'a'..'z', digits?
        Label,          // ':'
        AnswersBlock,   // '!', '#' - the difference is not clear
        Jump,           // '&'
        EmptyTextLine,  // ','
        Instruction,    // ';'
        SwitchInc,      // '$'
        SwitchRnd,      // '?'
        ExitScene,      // '/'
        Condition,      // '='
        Assignment,     // '@'
        ExitStory,      // '.'

        // not implemented yet
        MultiChoice,    // '%'
        // '^' in line ends - carrying
    }


    /// <summary>
    /// 
    /// </summary>
    public enum LineResult
    {
        Continue,
        Break,
        ExitScene,
        ExitStory,
    }


    /// <summary>
    /// 
    /// </summary>
    public class SceneLine
    {
        public string Text;
        public LineType Type;

        public string Ident;
        public string Expression;
        public int IArg;
    }


    /// <summary>
    /// 
    /// </summary>
    public class Scene
    {
        private readonly List<SceneLine> fLines;
        private int fLineIndex;

        private SceneLine fCurrent;
        private SceneLine fPrevious;
        private int fIncMonths;

        public IList<SceneLine> Lines
        {
            get { return fLines; }
        }

        public int IncMonths
        {
            get { return fIncMonths; }
        }


        public Scene(int incMonths)
        {
            fLines = new List<SceneLine>();
            fIncMonths = incMonths;
        }

        public void Load(string path, Encoding encoding)
        {
            Logger.Write("Loading scene: " + path);

            string ext = Path.GetExtension(path);
            if (ext != ".vig" && ext != ".sie" && ext != ".sic") {
                Logger.Write("This is not a scene file.");
                return;
            }

            fLines.Clear();
            string[] lines = File.ReadAllLines(path, encoding);
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                SceneLine lineInfo = IdentifyLine(line);
                fLines.Add(lineInfo);
            }

            Verify();
        }

        private void Verify()
        {
            for (int i = 0; i < fLines.Count; i++) {
                SceneLine lineInfo = fLines[i];
                if (lineInfo.Type == LineType.Unknown) {
                    Logger.Write(i + ": unknown [ " + lineInfo.Text + " ]");
                } else if (lineInfo.Type == LineType.Instruction) {
                    Logger.Write(i + ": Expression [ " + lineInfo.Text + " ]");
                }
            }
        }

        public void Start()
        {
            fLineIndex = 0;
            fPrevious = null;
            fCurrent = null;
        }

        public SceneLine Next()
        {
            fPrevious = fCurrent;
            if (fLineIndex < fLines.Count) {
                fCurrent = fLines[fLineIndex];
                fLineIndex += 1;
            } else {
                fCurrent = null;
            }
            return fCurrent;
        }

        public bool Goto(string label)
        {
            for (int i = 0; i < fLines.Count; i++) {
                SceneLine lineInfo = fLines[i];
                if (lineInfo.Type == LineType.Label && lineInfo.Ident == label) {
                    fLineIndex = i;
                    return true;
                }
            }
            return false;
        }

        public SceneLine IdentifyLine(string line)
        {
            SceneLine result = new SceneLine();
            result.Text = line;
            result.Type = LineType.Unknown;

            if (string.IsNullOrEmpty(line)) {
                result.Type = LineType.Break;
            } else {
                char firstChar = line[0];

                if (char.IsLetterOrDigit(firstChar)) {
                    result.Type = LineType.Text;
                } else {
                    switch (firstChar) {
                        case '"':
                        case '-':
                        case '*':
                        case ' ':
                        case '(':
                        case ')':
                        case '~':
                        case '>':
                            result.Type = LineType.Text; // without certainty
                            break;
                        case ':':
                            result.Type = LineType.Label;
                            result.Ident = line.Remove(0, 1);
                            break;
                        case '!':
                        case '#':
                            result.Type = LineType.AnswersBlock;
                            result.Ident = line.Remove(0, 1);
                            break;
                        case '&':
                            result.Type = LineType.Jump;
                            result.Ident = line.Remove(0, 1);
                            break;
                        case ',':
                            result.Type = LineType.EmptyTextLine;
                            break;
                        case ';':
                            result.Type = LineType.Instruction;
                            result.Expression = line.Remove(0, 1);
                            break;
                        case '$':
                            result.Type = LineType.SwitchInc;
                            result.Ident = line.Remove(0, 1);
                            result.IArg = 0;
                            break;
                        case '?':
                            result.Type = LineType.SwitchRnd;
                            string param = line.Remove(0, 1);
                            param = AEUtils.ExtractString(param, out result.Ident, "");
                            param = AEUtils.ExtractNumber(param, out result.IArg, true, 0);
                            break;
                        case '/':
                            result.Type = LineType.ExitScene;
                            break;
                        case '=':
                            result.Type = LineType.Condition;
                            string[] parts = line.Remove(0, 1).Split(new char[] { ',' }, 2);
                            if (parts.Length == 2) {
                                result.Ident = parts[0];
                                result.Expression = parts[1];
                            } else {
                                Logger.Write("strange condition");
                            }
                            break;
                        case '@':
                            result.Type = LineType.Assignment;
                            break;
                        case '.':
                            result.Type = LineType.ExitStory;
                            break;
                        case '%':
                            // multiple choice from 2 group of answers, each group give one number of final jump's label
                            result.Type = LineType.MultiChoice;
                            break;
                        default:
                            result.Type = LineType.Unknown;
                            break;
                    }
                }
            }

            return result;
        }
    }
}
