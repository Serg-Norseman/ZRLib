/**
 * Random quest generator.
 * This is based on the paper "A Prototype Quest Generator Based on a Structural 
 * Analysis of Quests from Four MMORPGs".
 * You can find this paper at http://larc.unt.edu/techreports/LARC-2011-02.pdf
 * If you do not read that paper, the generated quest may not make much sense.
 * 
 * The authors analyzed 3,000 quests from four RPGs and found that quests
 * generally could follow a predictable structure, though this structure was
 * often very complex.
 * 
 * The idea is to randomly generate quests based on multiple motivations. This is
 * a proof of concept for a game the author is writing. It's possible that I'll
 * expand this significantly for my personal work, or perhaps I'll use the
 * general information to inspiration instead of adhering too closely to the
 * output.
*/
package jzrlib.external.pqg;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Random;
import jzrlib.utils.TextUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class QuestBuilder
{
    private int DEPTH = 3;

    private final ArrayList<QObject> fActionsList;
    private final HashMap<String, QObject> fActionsMap;

    private final ArrayList<QObject> fMotivationsList;
    private final HashMap<String, QObject> fMotivationsMap;

    public QuestBuilder()
    {
        this.fActionsList = new ArrayList<>();
        this.fActionsMap = new HashMap<>();

        this.fMotivationsList = new ArrayList<>();
        this.fMotivationsMap = new HashMap<>();
    }

    public QuestBuilder(int depth)
    {
        this();
        this.DEPTH = depth;
    }

    public final void regAction(QObject action)
    {
        this.fActionsList.add(action);
        this.fActionsMap.put(action.name, action);
    }

    public final void regAction(String name, QNode... nodes)
    {
        QObject obj = new QObject(name, nodes);
        regAction(obj);
    }

    public final void regMotivation(QObject motivation)
    {
        this.fMotivationsList.add(motivation);
        this.fMotivationsMap.put(motivation.name, motivation);
    }

    public final void regMotivation(String name, QNode... nodes)
    {
        QObject obj = new QObject(name, nodes);
        regMotivation(obj);
    }

    public final void gen_step(String stepAction, int cur_depth)
    {
        QObject action = fActionsMap.get(stepAction);
        if (action == null) {
            return;
        }

        QNode choice = action.rndNode();
        String description = choice.description;
        String padding = TextUtils.repeat(' ', cur_depth);
        System.out.println(padding + "[" + description + "]");
        for (String step : choice.sequence) {
            System.out.println(padding + step);
            if (step.charAt(0) != '>' && cur_depth <= DEPTH) {
                gen_step(step, cur_depth + 1);
            }
        }
    }

    public final void build()
    {
        Random rnd = new Random();

        int mtv_index = rnd.nextInt(fMotivationsList.size());
        QObject motivation = fMotivationsList.get(mtv_index);
        QNode choice = motivation.rndNode();

        System.out.println("generating a quest for: " + motivation.name + ": " + choice.description);

        for (String step : choice.sequence) {
            System.out.println(step);
            if (step.charAt(0) != '>') {
                gen_step(step, 1);
            }
        }
    }
}
