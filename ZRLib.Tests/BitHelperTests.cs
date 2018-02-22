using System;
using NUnit.Framework;
using ZRLib.External.PQG;

namespace BSLib
{
    [TestFixture]
    public class BitHelperTests
    {
        [Test]
        public void Test_QuestBuilder()
        {
            QuestBuilder builder = new QuestBuilder(3);
            initBuilder(builder);
            builder.Build();
        }
    
        public static void initBuilder(QuestBuilder builder)
        {
            builder.RegAction("goto", new QNode[] {
                new QNode("Just wander around and look", new String[]{ ">explore" }),
                new QNode("Find out where to go and go there", new String[]{ "learn", ">goto" })
            });
            builder.RegAction("learn", new QNode[] {
                new QNode("Go someplace, get something, and read what is written on it", new String[] {
                    "goto",
                    "get",
                    ">read"
                })
            });
            builder.RegAction("get", new QNode[] {
                new QNode("Steal it from somebody", new String[]{ "steal" }),
                new QNode("Go someplace and pick something up that is lying around there", new String[] {
                    "goto",
                    ">gather"
                })
            });
            builder.RegAction("steal", new QNode[] {
                new QNode("Go someplace, sneak up on somebody, and take something", new String[] {
                    "goto",
                    ">stealth",
                    ">take"
                }),
                new QNode("Go someplace, kill somebody and take something", new String[]{ "goto", "kill", ">take" })
            });
            builder.RegAction("spy", new QNode[] {
                new QNode("Go someplace, spy on somebody, return and report", new String[] {
                    "goto",
                    ">spy",
                    "goto",
                    ">report"
                })
            });
            builder.RegAction("capture", new QNode[] {
                new QNode("Get something, go someplace and use it to capture somebody", new String[] {
                    "get",
                    "goto",
                    ">capture"
                })
            });
            builder.RegAction("kill", new QNode[] {
                new QNode("Go someplace and kill somebody", new String[]{ "goto", ">kill" })
            });

            //

            builder.RegMotivation("knowledge", new QNode[] {
                new QNode("Deliver item for study", new String[]{ "get", "goto", ">give" }),
                new QNode("Spy", new String[]{ "spy" }),
                new QNode("Interview NPC", new String[]{ "goto", ">listen", "goto", ">report" }),
                new QNode("Use an item in the field", new String[]{ "get", "goto", ">use", "goto", ">give" })
            });
            builder.RegMotivation("comfort", new QNode[] {
                new QNode("Obtain luxuries", new String[]{ "get", "goto", ">give" }),
                new QNode("Kill pests", new String[]{ "goto", ">damage", "goto", ">report" }),
            });
            builder.RegMotivation("reputation", new QNode[] {
                new QNode("Obtain rare items", new String[]{ "get", "goto", ">give" }),
                new QNode("Kill enemies", new String[]{ "goto", "kill", "goto", ">report" }),
                new QNode("Visit a dangerous place", new String[]{ "goto", "goto", ">report" })
            });
            builder.RegMotivation("serenity", new QNode[] {
                new QNode("Revenge, justice", new String[]{ "goto", ">damage" }),
                new QNode("Capture Criminal(1)", new String[]{ "get", "goto", ">use", "goto", ">give" }),
                new QNode("Capture Criminal(2)", new String[]{ "get", "goto", ">use", ">capture", "goto", ">give" }),
                new QNode("Check on NPC(1)", new String[]{ "goto", ">take", "goto", ">give" }),
                new QNode("Recover lost/stolen item", new String[]{ "get", "goto", ">give" }),
                new QNode("Rescue captured NPC", new String[]{ "goto", ">damage", ">escort", "goto", ">report" })
            });
            builder.RegMotivation("protection", new QNode[] {
                new QNode("Attack threatening entities", new String[]{ "goto", ">damage", "goto", ">report" }),
                new QNode("Treat or repair (1)", new String[]{ "get", "goto", ">use" }),
                new QNode("Treat or repair (2)", new String[]{ "goto", ">repair" }),
                new QNode("Create Diversion (1)", new String[]{ "get", "goto", ">use" }),
                new QNode("Create Diversion (2)", new String[]{ "goto", ">damage" }),
                new QNode("Assemble fortification", new String[]{ "goto", "repair" }),
                new QNode("Guard entity", new String[]{ "goto", ">defend" })
            });
            builder.RegMotivation("conquest", new QNode[] {
                new QNode("Attack enemy", new String[]{ "goto", ">damage" }),
                new QNode("Steal stuff", new String[]{ "goto", "steal", "goto", ">give" })
            });
            builder.RegMotivation("wealth", new QNode[] {
                new QNode("Gather raw materials", new String[]{ "goto", "get" }),
                new QNode("Steal valuables for resale", new String[]{ "goto", "steal" }),
                new QNode("Make valuables for resale", new String[]{ ">repair" })
            });
            builder.RegMotivation("ability", new QNode[] {
                new QNode("Assemble tool for new skill", new String[]{ ">repair", ">use" }),
                new QNode("Obtain training materials", new String[]{ ">get", ">use" }),
                new QNode("Use existing tools", new String[]{ ">use" }),
                new QNode("Practice combat", new String[]{ ">damage" }),
                new QNode("Practice skill", new String[]{ ">use" }),
                new QNode("Research a skill(1)", new String[]{ "get", ">use" }),
                new QNode("Research a skill(1)", new String[]{ "get", ">experiment" })
            });
            builder.RegMotivation("equipment", new QNode[] {
                new QNode("Assemble", new String[]{ ">repair" }),
                new QNode("Deliver supplies", new String[]{ "get", "goto", ">give" }),
                new QNode("Steal supplies", new String[]{ "steal" }),
                new QNode("Trade for supplies", new String[]{ "goto", ">exchange" })
            });
        }

        [Test]
        public void Test_SetBit()
        {
        }

        [Test]
        public void Test_UnsetBit()
        {
        }
    }
}
