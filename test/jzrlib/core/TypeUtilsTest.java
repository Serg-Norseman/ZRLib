package jzrlib.core;

import jzrlib.utils.TypeUtils;
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
public class TypeUtilsTest
{
    public TypeUtilsTest()
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
     * Test of getUByte method, of class TypeUtils.
     */
    @Test
    public void testGetUByte()
    {
        System.out.println("getUByte");
        byte val = (byte) 200;
        assertEquals(200, TypeUtils.getUByte(val));
    }

    /**
     * Test of fitShort method, of class TypeUtils.
     */
    @Test
    public void testFitShort_byte_byte()
    {
        System.out.println("fitShort");
        byte lo = (byte) 200;
        byte hi = (byte) 15;
        assertEquals(4040 /*0FC8*/, TypeUtils.fitShort(lo, hi));
    }

    /**
     * Test of fitShort method, of class TypeUtils.
     */
    @Test
    public void testFitShort_int_int()
    {
        System.out.println("fitShort");
        int lo = 200;
        int hi = 15;
        assertEquals(4040 /*0FC8*/, TypeUtils.fitShort(lo, hi));
    }

    /**
     * Test of getShortLo method, of class TypeUtils.
     */
    @Test
    public void testGetShortLo()
    {
        System.out.println("getShortLo");
        short val = 4040;
        assertEquals(200, TypeUtils.getShortLo(val));
    }

    /**
     * Test of getShortHi method, of class TypeUtils.
     */
    @Test
    public void testGetShortHi()
    {
        System.out.println("getShortHi");
        short val = 4040;
        assertEquals(15, TypeUtils.getShortHi(val));
    }

    /**
     * Test of setBit method, of class TypeUtils.
     */
    @Test
    public void testSetBit()
    {
        System.out.println("setBit");
        int changeValue = 0;
        assertEquals(1, TypeUtils.setBit(changeValue, 0, 1));
        assertEquals(2, TypeUtils.setBit(changeValue, 1, 1));
        assertEquals(4, TypeUtils.setBit(changeValue, 2, 1));
        assertEquals(16, TypeUtils.setBit(changeValue, 4, 1));
        assertEquals(64, TypeUtils.setBit(changeValue, 6, 1));
    }

    /**
     * Test of hasBit method, of class TypeUtils.
     */
    @Test
    public void testHasBit()
    {
        System.out.println("hasBit");
        int testValue = 36 /* 100100 */;
        assertEquals(true, TypeUtils.hasBit(testValue, 2));
        assertEquals(true, TypeUtils.hasBit(testValue, 5));

        testValue = TypeUtils.setBit(testValue, 2, 0);
        assertEquals(false, TypeUtils.hasBit(2, testValue));
    }

    /**
     * Test of hasFlag method, of class TypeUtils.
     */
    @Test
    public void testHasFlag()
    {
        System.out.println("hasFlag");
        int testValue = 2;
        int flag = 0b10;
        assertEquals(true, TypeUtils.hasFlag(testValue, flag));
        flag = 0b100;
        assertEquals(false, TypeUtils.hasFlag(testValue, flag));
    }
    
}
