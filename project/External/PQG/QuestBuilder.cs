/// <summary>
/// Random quest generator.
/// This is based on the paper "A Prototype Quest Generator Based on a Structural
/// Analysis of Quests from Four MMORPGs".
/// You can find this paper at http://larc.unt.edu/techreports/LARC-2011-02.pdf
/// If you do not read that paper, the generated quest may not make much sense.
///
/// The authors analyzed 3,000 quests from four RPGs and found that quests
/// generally could follow a predictable structure, though this structure was
/// often very complex.
///
/// The idea is to randomly generate quests based on multiple motivations. This is
/// a proof of concept for a game the author is writing. It's possible that I'll
/// expand this significantly for my personal work, or perhaps I'll use the
/// general information to inspiration instead of adhering too closely to the
/// output.
/// </summary>

using System;
using System.Collections.Generic;
using BSLib;
using ZRLib.Core;

namespace ZRLib.External.PQG
{
    public class QuestBuilder
    {
        private int DEPTH = 3;

        private readonly List<QObject> fActionsList;
        private readonly Dictionary<string, QObject> fActionsMap;

        private readonly List<QObject> fMotivationsList;
        private readonly Dictionary<string, QObject> fMotivationsMap;

        public QuestBuilder()
        {
            fActionsList = new List<QObject>();
            fActionsMap = new Dictionary<string, QObject>();

            fMotivationsList = new List<QObject>();
            fMotivationsMap = new Dictionary<string, QObject>();
        }

        public QuestBuilder(int depth)
            : this()
        {
            DEPTH = depth;
        }

        public void RegAction(QObject action)
        {
            fActionsList.Add(action);
            fActionsMap[action.Name] = action;
        }

        public void RegAction(string name, params QNode[] nodes)
        {
            QObject obj = new QObject(name, nodes);
            RegAction(obj);
        }

        public void RegMotivation(QObject motivation)
        {
            fMotivationsList.Add(motivation);
            fMotivationsMap[motivation.Name] = motivation;
        }

        public void RegMotivation(string name, params QNode[] nodes)
        {
            QObject obj = new QObject(name, nodes);
            RegMotivation(obj);
        }

        public void GenStep(string stepAction, int cur_depth)
        {
            QObject action = fActionsMap.GetValueOrNull(stepAction);
            if (action == null) {
                return;
            }

            QNode choice = action.RndNode();
            string description = choice.Description;
            string padding = ConvertHelper.Repeat(' ', cur_depth);
            Console.WriteLine(padding + "[" + description + "]");
            foreach (string step in choice.Sequence) {
                Console.WriteLine(padding + step);
                if (step[0] != '>' && cur_depth <= DEPTH) {
                    GenStep(step, cur_depth + 1);
                }
            }
        }

        public void Build()
        {
            Random rnd = new Random();

            int mtv_index = rnd.Next(fMotivationsList.Count);
            QObject motivation = fMotivationsList[mtv_index];
            QNode choice = motivation.RndNode();

            Console.WriteLine("generating a quest for: " + motivation.Name + ": " + choice.Description);

            foreach (string step in choice.Sequence) {
                Console.WriteLine(step);
                if (step[0] != '>') {
                    GenStep(step, 1);
                }
            }
        }
    }
}
