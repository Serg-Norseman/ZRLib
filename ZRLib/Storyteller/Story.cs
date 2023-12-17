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

using System;
using ZRLib.Core;

namespace ZRLib.Storyteller
{
    /// <summary>
    /// 
    /// </summary>
    public class Story
    {
        private string fAnswersIdent;
        private int fAnswerIndex;
        private string fLocale;
        private Personality fPersonality;
        private int fPhaseIndex;
        private Random fRandom;
        private Scene fScene;
        private IStoryView fView;


        public Personality Personality
        {
            get {
                return fPersonality;
            }
        }


        public Story(string locale, Personality personality)
        {
            fLocale = locale;
            fPersonality = personality;
            fRandom = new Random();
            fPhaseIndex = 0;
        }

        public void SetView(IStoryView view)
        {
            fView = view;
        }

        public void Start()
        {
            fPhaseIndex = 0;
            fAnswersIdent = null;
            fAnswerIndex = 0;
        }

        public void NextSceneLine()
        {
            fView.Clear();
            if (fScene == null) return;

            while (true) {
                var lineInfo = fScene.Next();
                LineResult lineResult = ProcessLine(lineInfo);

                if (lineResult == LineResult.Break) {
                    break;
                } else if (lineResult == LineResult.ExitScene) {
                    fPersonality.Age += (fScene.IncMonths / 12.0f);
                    // Scene Exit
                    break;
                } else if (lineResult == LineResult.ExitStory) {
                    // Game Exit
                    break;
                }
            }
        }

        private LineResult ProcessLine(SceneLine lineInfo)
        {
            LineResult result = LineResult.Continue;

            string lbl;

            switch (lineInfo.Type) {
                case LineType.Unknown:
                    break;

                case LineType.Break:
                    if (!string.IsNullOrEmpty(fAnswersIdent)) {
                        fAnswersIdent = null;
                        result = LineResult.Break;
                    }
                    break;

                case LineType.Text:
                    string text = lineInfo.Text;
                    if (!string.IsNullOrEmpty(text) && text[0] != ')') {
                        text = ProcessSubstitutions(text);
                        if (string.IsNullOrEmpty(fAnswersIdent)) {
                            fView.AddText(text);
                        } else {
                            fAnswerIndex += 1;
                            string answerIdent = fAnswersIdent + fAnswerIndex.ToString();
                            fView.AddAnswerText(text, answerIdent);
                        }
                    }
                    break;

                case LineType.Label:
                    break;

                case LineType.AnswersBlock:
                    fView.BeginAnswers();
                    fAnswersIdent = lineInfo.Ident;
                    fAnswerIndex = 0;
                    break;

                case LineType.Jump:
                    fScene.Goto(lineInfo.Ident);
                    break;

                case LineType.EmptyTextLine:
                    fView.AddText(" ");
                    break;

                case LineType.Instruction:
                    fPersonality.ExecInstruction(lineInfo.Expression);
                    break;

                case LineType.SwitchInc:
                    lineInfo.IArg += 1;
                    lbl = lineInfo.Ident + lineInfo.IArg.ToString();
                    fScene.Goto(lbl);
                    break;

                case LineType.SwitchRnd:
                    int rnd = fRandom.Next(1, lineInfo.IArg);
                    lbl = lineInfo.Ident + rnd.ToString();
                    fScene.Goto(lbl);
                    break;

                case LineType.ExitScene:
                    result = LineResult.ExitScene;
                    break;

                case LineType.Condition:
                    bool condResult = fPersonality.ExecCondition(lineInfo.Expression);
                    lbl = lineInfo.Ident + (condResult ? ".T" : ".F");
                    fScene.Goto(lbl);
                    break;

                case LineType.Assignment:
                    fPersonality.SetVar(lineInfo.Ident, lineInfo.Expression);
                    break;

                case LineType.ExitStory:
                    result = LineResult.ExitStory;
                    break;
            }

            return result;
        }

        public void SelectAnswer(string label)
        {
            bool res = fScene.Goto(label);
            if (res) {
                NextSceneLine();
            } else {
                Logger.Write("Label unknown: " + label);
            }
        }

        private string ProcessSubstitutions(string text)
        {
            // TODO: implement replacing of '~'-substitutions
            return text;
        }
    }
}
