package jzrlib.core;

import jzrlib.utils.TextUtils;
import org.junit.After;
import org.junit.AfterClass;
import static org.junit.Assert.assertEquals;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class TextUtilsTest
{
    public TextUtilsTest()
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
     * Test of equals method, of class TextUtils.
     */
    @Test
    public void testEquals()
    {
        System.out.println("equals");
        String str1 = "a";
        String str2 = "b";
        assertEquals(false, TextUtils.equals(str1, str2));
        str1 = "aa";
        str2 = "aa";
        assertEquals(true, TextUtils.equals(str1, str2));
    }

    /**
     * Test of isNullOrEmpty method, of class TextUtils.
     */
    @Test
    public void testIsNullOrEmpty()
    {
        System.out.println("isNullOrEmpty");
        assertEquals(true, TextUtils.isNullOrEmpty(null));
        assertEquals(true, TextUtils.isNullOrEmpty(""));
        assertEquals(false, TextUtils.isNullOrEmpty("false"));
    }

    /**
     * Test of compareStr method, of class TextUtils.
     */
    @Test
    public void testCompareStr()
    {
        System.out.println("compareStr");
        assertEquals(0, TextUtils.compareStr("Alpha", "Alpha"));
        assertEquals(-32, TextUtils.compareStr("Alpha", "alpha"));
    }

    /**
     * Test of compareText method, of class TextUtils.
     */
    @Test
    public void testCompareText()
    {
        System.out.println("compareText");
        assertEquals(0, TextUtils.compareText("Alpha", "alpha"));
        assertEquals(0, TextUtils.compareText("AlPhA", "alpha"));
    }

    /**
     * Test of upperFirst method, of class TextUtils.
     */
    @Test
    public void testUpperFirst()
    {
        System.out.println("upperFirst");
        String val = "alpha";
        assertEquals("Alpha", TextUtils.upperFirst(val));
    }

    /**
     * Test of getToken method, of class TextUtils.
     */
    @Test
    public void testGetToken()
    {
        System.out.println("getToken");
        String str = "aaa bbbb ccccc";

        assertEquals("aaa", TextUtils.getToken(str, " ", 1));
        assertEquals("bbbb", TextUtils.getToken(str, " ", 2));
        assertEquals("ccccc", TextUtils.getToken(str, " ", 3));

        assertEquals("", TextUtils.getToken(str, " ", 5));
        
        assertEquals("", TextUtils.getToken("", "", 5));
    }

    /**
     * Test of getTokensCount method, of class TextUtils.
     */
    @Test
    public void testGetTokensCount()
    {
        System.out.println("getTokensCount");
        String str = "aaa bbbb ccccc";
        assertEquals(3, TextUtils.getTokensCount(str, " "));
    }
    
}
