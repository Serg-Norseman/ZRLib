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
package jzrlib.jterm;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.image.BufferedImage;
import java.awt.image.IndexColorModel;
import java.awt.image.LookupOp;
import java.awt.image.ShortLookupTable;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import javax.imageio.ImageIO;
import javax.swing.JPanel;

/**
 * This simulates a ASCII terminal display.
 *
 * @author Serg V. Zhdanovskih
 */
public class JTerminal extends JPanel
{
    private static final boolean DEBUG_PALETTE = false;
    private static final boolean DEBUG_GLOBAL = false;
    public static final boolean DEBUG_PAINT = false;
    private static int GlobalChars = 0;
    public int CharsPainted;
    
    private static final String FONT_NAME = "cp866_8x8.png"; // "cp437_8x8.png";
    private static final int FONT_COLS = 32; // 16
    private static final int CHAR_WIDTH = 8; //9;
    private static final int CHAR_HEIGHT = 8; //16;

    private BufferedImage[] fCharGlyphs;

    private Image fBufferImage;
    private Graphics fBufferGraphics;

    private int fTermWidth;
    private int fTermHeight;
    private int fSize;
    private int fTermCursor = 0;
    private TermChar[] fTermBuffer;

    private Color fCurrentBackground;
    private Color fCurrentForeground;

    /**
     * Class constructor. Default size is 80x24.
     */
    public JTerminal()
    {
        this(80, 24);
    }

    /**
     * Class constructor specifying the width and height in characters.
     *
     * @param width
     * @param height
     */
    public JTerminal(int width, int height)
    {
        super();

        if (width < 1) {
            throw new IllegalArgumentException("width " + width + " must be greater than 0.");
        }

        if (height < 1) {
            throw new IllegalArgumentException("height " + height + " must be greater than 0.");
        }

        fTermWidth = width;
        fTermHeight = height;
        setPreferredSize(new Dimension(CHAR_WIDTH * fTermWidth, CHAR_HEIGHT * fTermHeight));
        this.fSize = height * width;

        fCurrentBackground = Color.black;
        fCurrentForeground = Color.lightGray;

        fTermBuffer = new TermChar[fSize];
        for (int i = 0; i < this.fSize; i++) {
            fTermBuffer[i] = new TermChar();
        }

        this.fCharGlyphs = new BufferedImage[256];
        this.loadGlyphs();

        this.clear();
    }

    /*private void createNewFont()
    {
        int FONT_ROWS = 256 / FONT_COLS;
        int newH = FONT_ROWS * 10;
        int newW = FONT_COLS * 10;
        
        BufferedImage img = new BufferedImage(newW, newH, BufferedImage.TYPE_INT_RGB);
        Graphics g = img.createGraphics();
        
        g.setPaintMode();
        g.setColor(new Color(255, 0, 255));
        g.fillRect(0, 0, newW, newH);

        for (int i = 0; i < 256; i++) {
            int sx = (i % FONT_COLS) * 10 + 1;
            int sy = (i / FONT_COLS) * 10 + 1;

            LookupOp op = setColors(Color.black, Color.white);
            BufferedImage charimg = op.filter(fCharGlyphs[i], null);
            g.drawImage(charimg, sx, sy, null);

            //img.getGraphics().drawImage(fCharGlyphs[i], sx, sy, 8, 8, 0, 0, CHAR_WIDTH, CHAR_HEIGHT, null);
        }
        
        File f = new File("e:\\MyFile.bmp");
        try {
            ImageIO.write(img, "BMP", f);
        } catch (IOException ex) {
            
        }
    }*/
    
    // <editor-fold defaultstate="collapsed" desc="Private functions">
    
    private void loadGlyphs()
    {
        try {
            BufferedImage fontImage = ImageIO.read(JTerminal.class.getResource(FONT_NAME));

            int type = (DEBUG_PALETTE) ? BufferedImage.TYPE_BYTE_BINARY : BufferedImage.TYPE_INT_ARGB;

            for (int i = 0; i < 256; i++) {
                int sx = (i % FONT_COLS) * CHAR_WIDTH;
                int sy = (i / FONT_COLS) * CHAR_HEIGHT;

                fCharGlyphs[i] = new BufferedImage(CHAR_WIDTH, CHAR_HEIGHT, type);
                fCharGlyphs[i].getGraphics().drawImage(fontImage, 0, 0, CHAR_WIDTH, CHAR_HEIGHT, sx, sy, sx + CHAR_WIDTH, sy + CHAR_HEIGHT, null);
            }
        } catch (IOException e) {
            System.err.println("loadGlyphs(): " + e.getMessage());
        }
    }

    /**
     * Create a <code>LookupOp</code> object (lookup table) mapping the original
     * pixels to the background and foreground colors, respectively.
     *
     * @param bgColor the background color
     * @param fgColor the foreground color
     * @return the <code>LookupOp</code> object (lookup table)
     */
    private LookupOp setLookupColors(int bgColor, int fgColor)
    {
        int size = 256;
        short[] a = new short[size];
        short[] r = new short[size];
        short[] g = new short[size];
        short[] b = new short[size];

        byte bgr = (byte) ((bgColor >> 16) & 0xFF);
        byte bgg = (byte) ((bgColor >> 8) & 0xFF);
        byte bgb = (byte) ((bgColor) & 0xFF);

        byte fgr = (byte) ((fgColor >> 16) & 0xFF);
        byte fgg = (byte) ((fgColor >> 8) & 0xFF);
        byte fgb = (byte) ((fgColor) & 0xFF);

        for (int i = 0; i < size; i++) {
            if (i == 0) {
                a[i] = (byte) 255;
                r[i] = bgr;
                g[i] = bgg;
                b[i] = bgb;
            } else {
                a[i] = (byte) 255;
                r[i] = fgr;
                g[i] = fgg;
                b[i] = fgb;
            }
        }

        short[][] table = {r, g, b, a};
        return new LookupOp(new ShortLookupTable(0, table), null);
    }

    private static IndexColorModel setICMColors(int bgColor, int fgColor)
    {
        byte[] r = new byte[2];
        byte[] g = new byte[2];
        byte[] b = new byte[2];

        r[0] = (byte) ((bgColor >> 16) & 0xFF);
        g[0] = (byte) ((bgColor >> 8) & 0xFF);
        b[0] = (byte) ((bgColor) & 0xFF);

        r[1] = (byte) ((fgColor >> 16) & 0xFF);
        g[1] = (byte) ((fgColor >> 8) & 0xFF);
        b[1] = (byte) ((fgColor) & 0xFF);

        return new IndexColorModel(1, 2, r, g, b);
    }

    private static BufferedImage convertLookupImage(BufferedImage img, LookupOp op)
    {
        return op.filter(img, null);
    }

    private static BufferedImage convertICMImage(BufferedImage img, IndexColorModel icm)
    {
        BufferedImage dest = new BufferedImage(CHAR_WIDTH, CHAR_HEIGHT, BufferedImage.TYPE_BYTE_BINARY, icm);
        img.copyData(dest.getRaster());
        return dest;
    }
    
    /**
     * Write a single character and update cursor position
     */
    private int writeChar(char chr, int pos, Color fg, Color bg)
    {
        switch (chr) {
            case '\n':
                pos = ((pos + fTermWidth) / fTermWidth) * fTermWidth;
                break;

            default:
                TermChar tmc = fTermBuffer[pos];
                tmc.Character = chr;
                tmc.Foreground = fg.getRGB();
                tmc.Background = bg.getRGB();
                pos++;
        }
        if (pos >= fSize) {
            pos = 0;
        }
        return pos;
    }

    // </editor-fold>

    @Override
    public void paintComponent(Graphics g)
    {
        super.paintComponent(g);
        
        if (g == null) {
            throw new NullPointerException();
        }

        if (fBufferImage == null) {
            fBufferImage = createImage(this.getWidth(), this.getHeight());
            fBufferGraphics = fBufferImage.getGraphics();
        }

        if (DEBUG_PAINT) {
            this.CharsPainted = 0;
        }

        int prevbg = 0, prevfg = 0;
        Object obj = null;

        for (int y = 0; y < fTermHeight; y++) {
            for (int x = 0; x < fTermWidth; x++) {
                int idx = y * fTermWidth + x;
                TermChar tmc = this.fTermBuffer[idx];

                int bg = tmc.Background;
                int fg = tmc.Foreground;

                // optimization #1
                if ((tmc.prevBackground == bg) && (tmc.prevForeground == fg) && tmc.prevCharacter == tmc.Character) {
                    continue;
                } else {
                    tmc.prevBackground = bg;
                    tmc.prevForeground = fg;
                    tmc.prevCharacter = tmc.Character;
                }

                // optimization #2
                if ((prevbg != bg) || (prevfg != fg) || obj == null) {
                    if (DEBUG_PALETTE) {
                        obj = setICMColors(bg, fg);
                    } else {
                        obj = setLookupColors(bg, fg);
                    }

                    prevbg = bg;
                    prevfg = fg;
                }

                BufferedImage img;
                if (DEBUG_PALETTE) {
                    img = convertICMImage(fCharGlyphs[tmc.Character], (IndexColorModel) obj);
                } else {
                    img = convertLookupImage(fCharGlyphs[tmc.Character], (LookupOp) obj);
                }
                
                fBufferGraphics.drawImage(img, x * CHAR_WIDTH, y * CHAR_HEIGHT, null);
                
                if (DEBUG_PAINT) {
                    this.CharsPainted++;
                }
                
                if (DEBUG_GLOBAL) {
                    GlobalChars++;
                    if (GlobalChars >= 500_000) {
                        System.exit(0);
                    }
                }
            }
        }

        g.drawImage(fBufferImage, 0, 0, null);
    }
    
    /**
     * Gets the height, in pixels, of a character.
     *
     * @return
     */
    public final int getCharHeight()
    {
        return CHAR_HEIGHT;
    }

    /**
     * Gets the width, in pixels, of a character.
     *
     * @return
     */
    public final int getCharWidth()
    {
        return CHAR_WIDTH;
    }

    /**
     * Gets the height in characters. A standard terminal is 24 characters high.
     *
     * @return
     */
    public final int getTermHeight()
    {
        return fTermHeight;
    }

    /**
     * Gets the width in characters. A standard terminal is 80 characters wide.
     *
     * @return
     */
    public final int getTermWidth()
    {
        return fTermWidth;
    }

    /**
     * Gets the distance from the left new text will be written to.
     *
     * @return
     */
    public final int getCursorX()
    {
        return this.fTermCursor % this.fTermWidth;
    }

    /**
     * Gets the distance from the top new text will be written to.
     *
     * @return
     */
    public final int getCursorY()
    {
        return this.fTermCursor / this.fTermWidth;
    }

    /**
     * Sets the x and y position of where new text will be written to. The
     * origin (0,0) is the upper left corner. The x should be equal to or
     * greater than 0 and less than the the width in characters. The y should be
     * equal to or greater than 0 and less than the the height in characters.
     *
     * @param x the distance from the left new text should be written to
     * @param y the distance from the top new text should be written to
     */
    public final void setCursorPos(int x, int y)
    {
        if (x < 0 || x >= fTermWidth) {
            throw new IllegalArgumentException("x " + x + " must be within range [0," + fTermWidth + ")");
        }

        if (y < 0 || y >= fTermHeight) {
            throw new IllegalArgumentException("y " + y + " must be within range [0," + fTermHeight + ")");
        }

        this.fTermCursor = y * this.fTermWidth + x;
    }

    /**
     * Gets the default background color that is used when writing new text.
     *
     * @return
     */
    public final Color getTextBackground()
    {
        return fCurrentBackground;
    }

    /**
     * Sets the default background color that is used when writing new text.
     *
     * @param value
     */
    public final void setTextBackground(Color value)
    {
        if (value == null) {
            throw new NullPointerException("defaultBackgroundColor must not be null.");
        }

        this.fCurrentBackground = value;
    }

    /**
     * Gets the default foreground color that is used when writing new text.
     *
     * @return
     */
    public final Color getTextForeground()
    {
        return fCurrentForeground;
    }

    /**
     * Sets the default foreground color that is used when writing new text.
     *
     * @param value
     */
    public final void setTextForeground(Color value)
    {
        if (value == null) {
            throw new NullPointerException("defaultForegroundColor must not be null.");
        }

        this.fCurrentForeground = value;
    }

    /**
     * Clear the entire screen to whatever the default background color is.
     *
     * @return this for convenient chaining of method calls
     */
    public final JTerminal clear()
    {
        return clear(' ', 0, 0, fTermWidth, fTermHeight, fCurrentForeground, fCurrentBackground);
    }

    /**
     * Clear the entire screen with the specified character and whatever the
     * default foreground and background colors are.
     *
     * @param character the character to write
     * @return this for convenient chaining of method calls
     */
    public JTerminal clear(char character)
    {
        return clear(character, 0, 0, fTermWidth, fTermHeight, fCurrentForeground, fCurrentBackground);
    }

    /**
     * Clear the entire screen with the specified character and whatever the
     * specified foreground and background colors are.
     *
     * @param character the character to write
     * @param foreground the foreground color or null to use the default
     * @param background the background color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public JTerminal clear(char character, Color foreground, Color background)
    {
        return clear(character, 0, 0, fTermWidth, fTermHeight, foreground, background);
    }

    /**
     * Clear the section of the screen with the specified character and whatever
     * the default foreground and background colors are.
     *
     * @param character the character to write
     * @param x the distance from the left to begin writing from
     * @param y the distance from the top to begin writing from
     * @param width the height of the section to clear
     * @param height the width of the section to clear
     * @return this for convenient chaining of method calls
     */
    public JTerminal clear(char character, int x, int y, int width, int height)
    {
        return clear(character, x, y, width, height, fCurrentForeground, fCurrentBackground);
    }

    /**
     * Clear the section of the screen with the specified character and whatever
     * the specified foreground and background colors are.
     *
     * @param character the character to write
     * @param x the distance from the left to begin writing from
     * @param y the distance from the top to begin writing from
     * @param width the height of the section to clear
     * @param height the width of the section to clear
     * @param foreground the foreground color or null to use the default
     * @param background the background color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public JTerminal clear(char character, int x, int y, int width, int height, Color foreground, Color background)
    {
        if (character < 0 || character >= fCharGlyphs.length) {
            throw new IllegalArgumentException("character " + character + " must be within range [0," + fCharGlyphs.length + "].");
        }

        if (x < 0 || x >= fTermWidth) {
            throw new IllegalArgumentException("x " + x + " must be within range [0," + fTermWidth + ")");
        }

        if (y < 0 || y >= fTermHeight) {
            throw new IllegalArgumentException("y " + y + " must be within range [0," + fTermHeight + ")");
        }

        if (width < 1) {
            throw new IllegalArgumentException("width " + width + " must be greater than 0.");
        }

        if (height < 1) {
            throw new IllegalArgumentException("height " + height + " must be greater than 0.");
        }

        if (x + width > fTermWidth) {
            throw new IllegalArgumentException("x + width " + (x + width) + " must be less than " + (fTermWidth + 1) + ".");
        }

        if (y + height > fTermHeight) {
            throw new IllegalArgumentException("y + height " + (y + height) + " must be less than " + (fTermHeight + 1) + ".");
        }

        for (int xo = x; xo < x + width; xo++) {
            for (int yo = y; yo < y + height; yo++) {
                write(xo, yo, character, foreground, background);
            }
        }
        return this;
    }

    /**
     * Write a character to the cursor's position. This updates the cursor's
     * position.
     *
     * @param chr the character to write
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(char chr)
    {
        return write(chr, fCurrentForeground, fCurrentBackground);
    }

    /**
     * Write a character to the cursor's position with the specified foreground
     * color. This updates the cursor's position but not the default foreground
     * color.
     *
     * @param chr the character to write
     * @param foreground the foreground color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(char chr, Color foreground)
    {
        return write(chr, foreground, fCurrentBackground);
    }

    /**
     * Write a character to the cursor's position with the specified foreground
     * and background colors. This updates the cursor's position but not the
     * default foreground or background colors.
     *
     * @param chr the character to write
     * @param foreground the foreground color or null to use the default
     * @param background the background color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(char chr, Color foreground, Color background)
    {
        if (chr < 0 || chr >= fCharGlyphs.length) {
            throw new IllegalArgumentException("character " + (int) chr + " must be within range [0," + fCharGlyphs.length + "].");
        }

        if (foreground == null) {
            foreground = fCurrentForeground;
        }
        if (background == null) {
            background = fCurrentBackground;
        }

        int pos = this.fTermCursor;
        pos = this.writeChar(chr, pos, foreground, background);
        this.fTermCursor = pos;
        
        return this;
    }

    /**
     * Write a character to the specified position. This updates the cursor's
     * position.
     *
     * @param chr the character to write
     * @param x the distance from the left to begin writing from
     * @param y the distance from the top to begin writing from
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(int x, int y, char chr)
    {
        this.setCursorPos(x, y);
        return write(chr, fCurrentForeground, fCurrentBackground);
    }

    /**
     * Write a character to the specified position with the specified foreground
     * color. This updates the cursor's position but not the default foreground
     * color.
     *
     * @param chr the character to write
     * @param x the distance from the left to begin writing from
     * @param y the distance from the top to begin writing from
     * @param foreground the foreground color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(int x, int y, char chr, Color foreground)
    {
        this.setCursorPos(x, y);
        return write(chr, foreground, fCurrentBackground);
    }

    /**
     * Write a character to the specified position with the specified foreground
     * and background colors. This updates the cursor's position but not the
     * default foreground or background colors.
     *
     * @param chr the character to write
     * @param x the distance from the left to begin writing from
     * @param y the distance from the top to begin writing from
     * @param foreground the foreground color or null to use the default
     * @param background the background color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(int x, int y, char chr, Color foreground, Color background)
    {
        this.setCursorPos(x, y);
        return write(chr, foreground, background);
    }

    /**
     * Write a string to the cursor's position. This updates the cursor's
     * position.
     *
     * @param string the string to write
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(String string)
    {
        return write(string, fCurrentForeground, fCurrentBackground);
    }

    /**
     * Write a string to the cursor's position with the specified foreground
     * color. This updates the cursor's position but not the default foreground
     * color.
     *
     * @param string the string to write
     * @param foreground the foreground color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(String string, Color foreground)
    {
        return write(string, foreground, fCurrentBackground);
    }

    /**
     * Write a string to the cursor's position with the specified foreground and
     * background colors. This updates the cursor's position but not the default
     * foreground or background colors.
     *
     * @param string the string to write
     * @param foreground the foreground color or null to use the default
     * @param background the background color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(String string, Color foreground, Color background)
    {
        if (string == null) {
            throw new NullPointerException("string must not be null.");
        }

        try {
            byte[] data = string.getBytes("Cp866");
            for (int i = 0; i < data.length; i++) {
                char chr = (char) (data[i] & 0xFF);
                this.write(chr, foreground, background);
            }
        } catch (UnsupportedEncodingException ex) {
        }

        return this;
    }

    /**
     * Write a string to the specified position. This updates the cursor's
     * position.
     *
     * @param string the string to write
     * @param x the distance from the left to begin writing from
     * @param y the distance from the top to begin writing from
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(int x, int y, String string)
    {
        return write(x, y, string, fCurrentForeground, fCurrentBackground);
    }

    /**
     * Write a string to the specified position with the specified foreground
     * color. This updates the cursor's position but not the default foreground
     * color.
     *
     * @param string the string to write
     * @param x the distance from the left to begin writing from
     * @param y the distance from the top to begin writing from
     * @param foreground the foreground color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public JTerminal write(int x, int y, String string, Color foreground)
    {
        return write(x, y, string, foreground, fCurrentBackground);
    }

    /**
     * Write a string to the specified position with the specified foreground
     * and background colors. This updates the cursor's position but not the
     * default foreground or background colors.
     *
     * @param string the string to write
     * @param x the distance from the left to begin writing from
     * @param y the distance from the top to begin writing from
     * @param foreground the foreground color or null to use the default
     * @param background the background color or null to use the default
     * @return this for convenient chaining of method calls
     */
    public final JTerminal write(int x, int y, String string, Color foreground, Color background)
    {
        this.setCursorPos(x, y);
        return this.write(string, foreground, background);
    }

    public final void withEachTile(ICharTransformer transformer)
    {
        withEachTile(0, 0, fTermWidth, fTermHeight, transformer);
    }

    public final void withEachTile(int left, int top, int width, int height, ICharTransformer transformer)
    {
        if (transformer == null) {
            return;
        }

        for (int x0 = 0; x0 < width; x0++) {
            for (int y0 = 0; y0 < height; y0++) {
                int x = left + x0;
                int y = top + y0;

                if (x < 0 || y < 0 || x >= fTermWidth || y >= fTermHeight) {
                    continue;
                }

                int idx = y * this.fTermWidth + x;
                TermChar data = fTermBuffer[idx];
                transformer.invoke(x, y, data);
            }
        }
    }
}
