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
package jzrlib.core;

import java.io.InputStream;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import jzrlib.utils.Logger;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXParseException;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class BaseLocale extends BaseObject
{
    private static String[] rsList;
    
    protected static final void initList(int size)
    {
        rsList = new String[size];
    }

    protected static void loadStr(int rsid, String value)
    {
        rsList[rsid] = value;
    }

    public static String getStr(int rsid)
    {
        return rsList[rsid];
    }

    public static String format(int rsid, Object... args)
    {
        if (args != null) {
            args = (Object[]) args.clone();
        }

        return String.format(getStr(rsid), args);
    }

    protected void loadLangTexts(InputStream is)
    {
        try {
            DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
            DocumentBuilder builder = factory.newDocumentBuilder();
            Document doc = builder.parse(is);

            Element root = doc.getDocumentElement();
            if (!root.getTagName().equals("Texts")) {
                throw new RuntimeException("Its not texts!");
            }

            NodeList nl = root.getChildNodes();
            for (int i = 0; i < nl.getLength(); i++) {
                Node n = nl.item(i);
                if (n instanceof Element) {
                    Element el = (Element) n;
                    if (el.getTagName().equals("RS")) {
                        try {
                            int id = Integer.parseInt(el.getAttribute("ID"));
                            String txt = readElement(el, "Text");

                            loadStr(id, txt);
                        } catch (Exception ex) {
                            Logger.write("BaseLocale.loadLangTexts.1(): " + ex.getMessage());
                        }
                    }
                }
            }
        } catch (SAXParseException ex) {
            Logger.write("BaseLocale.loadLangTexts.sax(): " + ex.getMessage());
        } catch (Exception ex) {
            Logger.write("BaseLocale.loadLangTexts(): " + ex.getMessage());
        }
    }

    private static String readElement(Element parentElement, String tagName)
    {
        NodeList nl = parentElement.getChildNodes();
        for (int i = 0; i < nl.getLength(); i++) {
            Node node = nl.item(i);
            String nodeName = node.getNodeName();
            if (node instanceof Element && nodeName.equalsIgnoreCase(tagName)) {
                String value = ((Element) node).getTextContent();
                return value;
            }
        }
        return "";
    }
}
