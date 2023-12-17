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
using System.Collections.Generic;
using System.Linq;
using ZRLib.Core;

namespace ZRLib.Storyteller
{
    public enum Sex
    {
        None,
        Female,
        Male,
        Hermaphrodite,
    }


    /// <summary>
    /// 
    /// </summary>
    public class Personality
    {
        private readonly Dictionary<string, object> fVariables;

        public string Name;
        public Sex Sex;
        public float Age; // Years,Months
        public string Occupation;
        public string RelationshipStatus;
        public int Money;
        public int Debt;
        public string Acquisitions;

        public string AgeStr
        {
            get {
                string result;
                int year = (int)Math.Floor(Age);
                int month = (int)Math.Round((Age - year) * 12);
                result = year.ToString() + " : " + month.ToString();
                return result;
            }
        }

        public float Calmness // CA, Невозмутимость
        {
            get { return GetPropVar("CA"); }
        }

        public float Confidence // CN Уверенность
        {
            get { return GetPropVar("CN"); }
        }

        public float Expressiveness // EX Выразительность
        {
            get { return GetPropVar("EX"); }
        }

        public float Familial // FM Семейность
        {
            get { return GetPropVar("FM"); }
        }

        public float Gentleness // GN Мягкость
        {
            get { return GetPropVar("GN"); }
        }

        public float Happiness // HP Счастье
        {
            get { return GetPropVar("HP"); }
        }

        public float Intellectual // IN Интеллект
        {
            get { return GetPropVar("IN"); }
        }

        public float Physical // PH Конституция
        {
            get { return GetPropVar("PH"); }
        }

        public float Social // SC Социальность
        {
            get { return GetPropVar("SC"); }
        }

        public float Thoughtfulness // TH Глубокомыслие
        {
            get { return GetPropVar("TH"); }
        }

        public float Trustworthiness // TR Надежность
        {
            get { return GetPropVar("TR"); }
        }

        public float Vocational // VC Профессионализм
        {
            get { return GetPropVar("VC"); }
        }


        public Personality()
        {
            Sex = Sex.Male;
            fVariables = new Dictionary<string, object>();
        }

        public string GetDataPath()
        {
            string sxPath;
            switch (Sex) {
                case Sex.Female:
                    sxPath = @"female\";
                    break;

                case Sex.Male:
                    sxPath = @"male\";
                    break;

                case Sex.None:
                case Sex.Hermaphrodite:
                default:
                    sxPath = string.Empty;
                    break;
            }

            return AEUtils.GetAppPath() + @"data\" + sxPath;
        }

        public float GetPropVar(string varName)
        {
            object obj = GetVar(varName);
            return Convert.ToSingle(obj);
        }

        public object GetVar(string varName)
        {
            object varValue;
            if (!fVariables.TryGetValue(varName, out varValue)) {
                varValue = 0;
            }
            return varValue;
        }

        public void SetVar(string varName, object varValue)
        {
            fVariables[varName] = varValue;
        }

        public void ExecInstruction(string expressions)
        {
            string[] parts = expressions.Split(new char[] { ',' });
            foreach (string exp in parts) {
                ExecExpression(exp);
            }
        }

        public bool ExecCondition(string expressions)
        {
            bool result = true;
            string[] parts = expressions.Split(new char[] { ',' });
            foreach (string exp in parts) {
                bool res = Convert.ToBoolean(ExecExpression(exp));
                result = result && res;
            }
            return result;
        }

        public int ExecExpression(string expression)
        {
            int result = int.MinValue;

            var tokens = expression.SplitAndKeep(new char[] { '+', '-', '<', '>', '=' }).ToArray();
            if (tokens.Length == 3) {
                string varName = tokens[0];

                object varObj = GetVar(varName);
                int varValue = (int)varObj; // FIXME: add checks of type

                char op = tokens[1][0];
                int val = int.Parse(tokens[2]);
                bool isCalculation = false;

                switch (op) {
                    case '+':
                        varValue = varValue + val;
                        isCalculation = true;
                        break;
                    case '-':
                        varValue = varValue - val;
                        isCalculation = true;
                        break;
                    case '=':
                        varValue = val;
                        isCalculation = true;
                        break;
                    case '<':
                        varValue = Convert.ToInt32(varValue < val);
                        break;
                    case '>':
                        varValue = Convert.ToInt32(varValue > val);
                        break;
                    default:
                        Logger.Write("Unknown expression's operation: [" + op + "].");
                        break;
                }

                if (isCalculation) {
                    SetVar(varName, varValue);
                }

                result = varValue;
            }

            return result;
        }

        public override string ToString()
        {
            string result = string.Empty;

            foreach (var kvp in fVariables) {
                result += kvp.Key + "=" + kvp.Value.ToString() + "; ";
            }

            return result;
        }
    }
}
