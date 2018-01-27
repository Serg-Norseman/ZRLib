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
using System.IO;
using System.Xml;
using BSLib;

namespace ZRLib.Core
{
    public class BaseLocale : BaseObject
    {
        private static string[] RsList;

        protected static void InitList(int size)
        {
            RsList = new string[size];
        }

        protected static void LoadStr(int rsid, string value)
        {
            RsList[rsid] = value;
        }

        public static string GetStr(Enum rsid)
        {
            int id = Convert.ToInt32(rsid);
            return GetStr(id);
        }

        public static string GetStr(int rsid)
        {
            return RsList[rsid];
        }

        public static string Format(Enum rsid, params object[] args)
        {
            int id = Convert.ToInt32(rsid);
            return Format(id, args);
        }

        public static string Format(int rsid, params object[] args)
        {
            if (args != null) {
                args = (object[])args.Clone();
            }

            return string.Format(GetStr(rsid), args);
        }

        protected virtual void LoadLangTexts(Stream @is)
        {
            try {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(@is);
                XmlNode root = xmlDocument.DocumentElement;

                if (!root.Name.Equals("Texts")) {
                    throw new Exception("Its not texts!");
                }

                for (int i = 0; i < root.ChildNodes.Count; i++) {
                    XmlNode n = root.ChildNodes[i];
                    if (n.Name.Equals("RS")) {
                        try {
                            int id = Convert.ToInt32(n.Attributes["ID"].InnerText);
                            string txt = ReadElement(n, "Text");

                            LoadStr(id, txt);
                        } catch (Exception ex) {
                            Logger.Write("BaseLocale.loadLangTexts.1(): " + ex.Message);
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Write("BaseLocale.loadLangTexts(): " + ex.Message);
            }
        }

        private static string ReadElement(XmlNode parentElement, string tagName)
        {
            XmlNodeList nl = parentElement.ChildNodes;
            for (int i = 0; i < nl.Count; i++) {
                XmlNode node = nl[i];
                string nodeName = node.Name;
                if (nodeName.Equals(tagName, StringComparison.CurrentCultureIgnoreCase)) {
                    string value = node.InnerText;
                    return value;
                }
            }
            return "";
        }
    }
}
