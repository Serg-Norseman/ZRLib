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
package jzrlib.external.bsp;

import java.util.List;
import jzrlib.core.Rect;
import jzrlib.map.BaseTile;
import jzrlib.map.CustomMap;
import jzrlib.map.Movements;
import jzrlib.map.TileType;
import jzrlib.utils.RefObject;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class BSPTreeTest
{
    public static final char TILE_GROUND = '.';
    public static final char TILE_STREET = 'x';
    public static final char TILE_DISTRICT = 'D';
    public static final char TILE_ROOM = '+';

    
    public BSPTreeTest()
    {
    }
    
    @BeforeClass
    public static void setUpClass()
    {
    }
    
    @AfterClass
    public static void tearDownClass()
    {
    }
    
    @Before
    public void setUp()
    {
    }
    
    @After
    public void tearDown()
    {
    }

    /**
     * Test of getRoot method, of class BSPTree.
     */
    @Test
    public void testGetRoot()
    {
        /*System.out.println("getRoot");
        BSPTree instance = null;
        BSPNode expResult = null;
        BSPNode result = instance.getRoot();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }

    /**
     * Test of dispose method, of class BSPTree.
     */
    @Test
    public void testDispose()
    {
        /*System.out.println("dispose");
        BSPTree instance = null;
        instance.dispose();
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }

    /**
     * Test of getLeaves method, of class BSPTree.
     */
    @Test
    public void testGetLeaves()
    {
        /*System.out.println("getLeaves");
        BSPTree instance = null;
        List<BSPNode> expResult = null;
        List<BSPNode> result = instance.getLeaves();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }

    /**
     * Test of generate method, of class BSPTree.
     */
    @Test
    public void testGenerate()
    {
        System.out.println("generate");
        /*BSPTree instance = null;
        instance.generate();
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/

        RLMap map = new RLMap(100, 100);
        for (int y = 0; y < map.getHeight(); y++) {
            for (int x = 0; x < map.getWidth(); x++) {
                map.setMetaTile(x, y, TileType.ttGround); // (int) TILE_GROUND
            }
        }

        BSPTree tree = new BSPTree(new Rect(0, 0, map.getWidth() - 1, map.getHeight() - 1), 6, 10, true, null, BSPTreeTest::splitHandler);

        List<BSPNode> leaves = tree.getLeaves();
        for (BSPNode node : leaves) {
            map.fillMetaBorder(node.x1, node.y1, node.x2, node.y2, TileType.ttWall);
        }

        for (int y = 0; y < map.getHeight(); y++) {
            for (int x = 0; x < map.getWidth(); x++) {
                System.out.print(map.getMetaTile(x, y));
            }
            System.out.println();
        }
    }

    private static void splitHandler(BSPNode node, RefObject<Integer> refSplitWidth)
    {
        refSplitWidth.argValue = 1;
    }

    /**
     * Test of checkLeaves method, of class BSPTree.
     */
    @Test
    public void testCheckLeaves()
    {
        /*System.out.println("checkLeaves");
        BSPTree instance = null;
        instance.checkLeaves();
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }
    
    private static class RLMap extends CustomMap
    {
        public RLMap(int width, int height)
        {
            super(width, height);
        }
    
        @Override
        public Movements getTileMovements(short tileID)
        {
            return new Movements();
        }

        @Override
        public short translateTile(TileType defTile)
        {
            switch (defTile) {
                case ttGround:
                    return '.';
                case ttWall:
                    return 'x';

                default:
                    return '.';
            }
        }

        @Override
        public void setMetaTile(int x, int y, TileType tile)
        {
            BaseTile baseTile = this.getTile(x, y);
            if (baseTile != null) {
                baseTile.Background = this.translateTile(tile);
            }
        }
        
        @Override
        public void fillMetaBorder(int x1, int y1, int x2, int y2, TileType tile)
        {
            short defTile = this.translateTile(tile);
            this.fillBorder(x1, y1, x2, y2, defTile, false);
        }

        public char getMetaTile(int x, int y)
        {
            BaseTile baseTile = this.getTile(x, y);
            if (baseTile != null) {
                return (char) baseTile.Background;
            } else {
                return ' ';
            }
        }
    }
}
