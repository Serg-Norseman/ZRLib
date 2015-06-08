package jzrlib.external.markovchains;

import jzrlib.external.markovchains.MarkovChain;
import jzrlib.external.markovchains.StringChain;
import jzrlib.external.markovchains.CharChain;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Random;
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
public class MarkovChainTest
{
    public MarkovChainTest()
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
     * Test of main method, of class MarkovChain.
     */
    @Test
    public void testMain()
    {
        System.out.println("main");
        //testImpl("a b r a k a d a b r a ");
        testImpl("n e t b e a n s ");

        String test2 = "I am not a number! I am a free man! ";
        //String test2 = "The quick brown fox jumped over the lazy dog. ";
        //String test2 = "I am the walrus. You are the eggman!! We are all together.";
        testImpl2(test2);
    }

    private void testImpl(String test)
    {
        Random rand = new Random();
        System.out.println("Input: " + test);

        System.out.println("\nTesting chars: ");

        CharChain charChain = new CharChain(3);
        charChain.init(test);

        List<Character> genChars = charChain.generate(1000, rand);
        StringBuilder sb = new StringBuilder();
        for (Character c : genChars) {
            if (c != null) {
                sb.append(c);
            }
        }
        System.out.println(sb.toString());

        System.out.println("\nTesting string: ");
        StringChain strChain = new StringChain(2);
        strChain.addItems(Arrays.asList(test.split("(?<= )")));
        sb = new StringBuilder();
        for (String s : strChain.generate(30, rand)) {
            sb.append(s);
        }
        System.out.println(sb.toString());
    }

    private void testImpl2(String test)
    {
        Random rand = new Random();
        System.out.println("\nAdditional input: " + test);
        System.out.println("Testing sentences: ");
        MarkovChain<Character> charChain = new MarkovChain<Character>(3, ' ');
        MarkovChain<String> strChain = new MarkovChain<String>(2, "");
        // Split string on ending punctuation
        for (String sentence : test.split("(?<=[\\.\\?\\!])+")) {
            System.out.println("add sentence: " + sentence);

            List<Character> charlist = new ArrayList<Character>();
            for (char c : sentence.toCharArray()) {
                charlist.add(c);
            }
            charChain.addItems(charlist);

            strChain.addItems(Arrays.asList(sentence.split("(?=\\b)")));
        }
        System.out.println("chars:");
        StringBuilder sb = new StringBuilder();
        for (Character c : charChain.generate(200, rand)) {
            if (c != null) {
                sb.append(c);
            }
        }
        System.out.println(sb.toString());

        System.out.println("strings:");
        sb = new StringBuilder();
        for (String s : strChain.generate(50, rand)) {
            sb.append(s);
        }
        System.out.println(sb.toString());
    }
}
