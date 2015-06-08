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
package jzrlib.external.pqg;

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
public class QuestBuilderTest
{
    
    public QuestBuilderTest()
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
     * Test of regAction method, of class QuestBuilder.
     */
    @Test
    public void testRegAction_QObject()
    {
        /*System.out.println("regAction");
        QObject action = null;
        QuestBuilder instance = new QuestBuilder();
        instance.regAction(action);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }

    /**
     * Test of regAction method, of class QuestBuilder.
     */
    @Test
    public void testRegAction_String_QNodeArr()
    {
        /*System.out.println("regAction");
        String name = "";
        QNode[] nodes = null;
        QuestBuilder instance = new QuestBuilder();
        instance.regAction(name, nodes);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }

    /**
     * Test of regMotivation method, of class QuestBuilder.
     */
    @Test
    public void testRegMotivation_QObject()
    {
        /*System.out.println("regMotivation");
        QObject motivation = null;
        QuestBuilder instance = new QuestBuilder();
        instance.regMotivation(motivation);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }

    /**
     * Test of regMotivation method, of class QuestBuilder.
     */
    @Test
    public void testRegMotivation_String_QNodeArr()
    {
        /*System.out.println("regMotivation");
        String name = "";
        QNode[] nodes = null;
        QuestBuilder instance = new QuestBuilder();
        instance.regMotivation(name, nodes);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }

    /**
     * Test of gen_step method, of class QuestBuilder.
     */
    @Test
    public void testGen_step()
    {
        /*System.out.println("gen_step");
        String stepAction = "";
        int cur_depth = 0;
        QuestBuilder instance = new QuestBuilder();
        instance.gen_step(stepAction, cur_depth);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");*/
    }

    /**
     * Test of build method, of class QuestBuilder.
     */
    @Test
    public void testBuild()
    {
        System.out.println("build");

        QuestBuilder builder = new QuestBuilder(3);
        
        initBuilder(builder);
        
        builder.build();
    }
    
    public static void initBuilder(QuestBuilder builder)
    {
        builder.regAction("goto", new QNode[]{
            new QNode("Just wander around and look", new String[]{">explore"}),
            new QNode("Find out where to go and go there", new String[]{"learn", ">goto"})
        });
        builder.regAction("learn", new QNode[]{
            new QNode("Go someplace, get something, and read what is written on it", new String[]{"goto", "get", ">read"})
        });
        builder.regAction("get", new QNode[]{
            new QNode("Steal it from somebody", new String[]{"steal"}),
            new QNode("Go someplace and pick something up that is lying around there", new String[]{"goto", ">gather"})
        });
        builder.regAction("steal", new QNode[]{
            new QNode("Go someplace, sneak up on somebody, and take something", new String[]{"goto", ">stealth", ">take"}),
            new QNode("Go someplace, kill somebody and take something", new String[]{"goto", "kill", ">take"})
        });
        builder.regAction("spy", new QNode[]{
            new QNode("Go someplace, spy on somebody, return and report", new String[]{"goto", ">spy", "goto", ">report"})
        });
        builder.regAction("capture", new QNode[]{
            new QNode("Get something, go someplace and use it to capture somebody", new String[]{"get", "goto", ">capture"})
        });
        builder.regAction("kill", new QNode[]{
            new QNode("Go someplace and kill somebody", new String[]{"goto", ">kill"})
        });

        //

        builder.regMotivation("knowledge", new QNode[]{
            new QNode("Deliver item for study", new String[]{"get", "goto", ">give"}),
            new QNode("Spy", new String[]{"spy"}),
            new QNode("Interview NPC", new String[]{"goto", ">listen", "goto", ">report"}),
            new QNode("Use an item in the field", new String[]{"get", "goto", ">use", "goto", ">give"})
        });
        builder.regMotivation("comfort", new QNode[]{
            new QNode("Obtain luxuries", new String[]{"get", "goto", ">give"}),
            new QNode("Kill pests", new String[]{"goto", ">damage", "goto", ">report"}),});
        builder.regMotivation("reputation", new QNode[]{
            new QNode("Obtain rare items", new String[]{"get", "goto", ">give"}),
            new QNode("Kill enemies", new String[]{"goto", "kill", "goto", ">report"}),
            new QNode("Visit a dangerous place", new String[]{"goto", "goto", ">report"})
        });
        builder.regMotivation("serenity", new QNode[]{
            new QNode("Revenge, justice", new String[]{"goto", ">damage"}),
            new QNode("Capture Criminal(1)", new String[]{"get", "goto", ">use", "goto", ">give"}),
            new QNode("Capture Criminal(2)", new String[]{"get", "goto", ">use", ">capture", "goto", ">give"}),
            new QNode("Check on NPC(1)", new String[]{"goto", ">take", "goto", ">give"}),
            new QNode("Recover lost/stolen item", new String[]{"get", "goto", ">give"}),
            new QNode("Rescue captured NPC", new String[]{"goto", ">damage", ">escort", "goto", ">report"})
        });
        builder.regMotivation("protection", new QNode[]{
            new QNode("Attack threatening entities", new String[]{"goto", ">damage", "goto", ">report"}),
            new QNode("Treat or repair (1)", new String[]{"get", "goto", ">use"}),
            new QNode("Treat or repair (2)", new String[]{"goto", ">repair"}),
            new QNode("Create Diversion (1)", new String[]{"get", "goto", ">use"}),
            new QNode("Create Diversion (2)", new String[]{"goto", ">damage"}),
            new QNode("Assemble fortification", new String[]{"goto", "repair"}),
            new QNode("Guard entity", new String[]{"goto", ">defend"})
        });
        builder.regMotivation("conquest", new QNode[]{
            new QNode("Attack enemy", new String[]{"goto", ">damage"}),
            new QNode("Steal stuff", new String[]{"goto", "steal", "goto", ">give"})
        });
        builder.regMotivation("wealth", new QNode[]{
            new QNode("Gather raw materials", new String[]{"goto", "get"}),
            new QNode("Steal valuables for resale", new String[]{"goto", "steal"}),
            new QNode("Make valuables for resale", new String[]{">repair"})
        });
        builder.regMotivation("ability", new QNode[]{
            new QNode("Assemble tool for new skill", new String[]{">repair", ">use"}),
            new QNode("Obtain training materials", new String[]{">get", ">use"}),
            new QNode("Use existing tools", new String[]{">use"}),
            new QNode("Practice combat", new String[]{">damage"}),
            new QNode("Practice skill", new String[]{">use"}),
            new QNode("Research a skill(1)", new String[]{"get", ">use"}),
            new QNode("Research a skill(1)", new String[]{"get", ">experiment"})
        });
        builder.regMotivation("equipment", new QNode[]{
            new QNode("Assemble", new String[]{">repair"}),
            new QNode("Deliver supplies", new String[]{"get", "goto", ">give"}),
            new QNode("Steal supplies", new String[]{"steal"}),
            new QNode("Trade for supplies", new String[]{"goto", ">exchange"})
        });
    }

}
