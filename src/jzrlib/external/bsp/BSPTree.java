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

import java.util.ArrayList;
import java.util.List;
import jzrlib.utils.AuxUtils;
import jzrlib.core.Rect;
import jzrlib.utils.RefObject;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class BSPTree
{
    private static final int DEBUG_DEPTH = -1;

    private BSPNode fRoot;

    private final Rect fArea;
    private final int fMinNodeSize;
    private final int fMaxNodeSize;
    private final boolean fDistinctBounds;
    private final ISplitProc fSplitProc;
    private final INodeSplitHandler fNodeSplitHandler;

    public BSPTree(Rect area, int minNodeSize, int maxNodeSize, boolean distinctBounds, ISplitProc splitProc, INodeSplitHandler nodeSplitHandler)
    {
        this.fArea = area;
        this.fMinNodeSize = minNodeSize;
        this.fMaxNodeSize = maxNodeSize;
        this.fDistinctBounds = distinctBounds;
        this.fSplitProc = splitProc;
        this.fNodeSplitHandler = nodeSplitHandler;

        this.generate();
    }

    public final BSPNode getRoot()
    {
        return this.fRoot;
    }

    public final void dispose()
    {
        this.disposeNode(this.fRoot);
    }

    private void disposeNode(BSPNode node)
    {
        if (node != null) {
            if (node.left != null) {
                disposeNode(node.left);
                node.left = null;
            }

            if (node.right != null) {
                disposeNode(node.right);
                node.right = null;
            }
        }
    }

    public final List<BSPNode> getLeaves()
    {
        List<BSPNode> leaves = new ArrayList<>();
        getLeaves(leaves, fRoot);
        return leaves;
    }

    private void getLeaves(List<BSPNode> vec, BSPNode node)
    {
        if (node != null) {
            if (node.left == null && node.right == null) {
                vec.add(node);
            } else {
                getLeaves(vec, node.left);
                getLeaves(vec, node.right);
            }
        }
    }

    public final void generate()
    {
        BSPNode.resetId();
        this.fRoot = new BSPNode(null, 1, fArea.Left, fArea.Top, fArea.Right, fArea.Bottom);
        this.generate(this.fRoot);
    }

    private void generate(BSPNode node)
    {
        if (DEBUG_DEPTH > 0 && node.depth > DEBUG_DEPTH) {
            return;
        }

        // if splitting the node was successful, do the same to the newly
        // generated child nodes, if they're too big or with a 75% chance.
        if (this.splitNode(node)) {
            BSPNode leaf;

            leaf = node.left;
            if (leaf.getWidth() > fMaxNodeSize || leaf.getHeight() > fMaxNodeSize || AuxUtils.getRandom(100) > 25) {
                generate(leaf);
            }

            leaf = node.right;
            if (leaf.getWidth() > fMaxNodeSize || leaf.getHeight() > fMaxNodeSize || AuxUtils.getRandom(100) > 25) {
                generate(leaf);
            }
        }
    }

    private boolean splitNode(BSPNode node)
    {
        // if the nodes exist, return
        if (node.left != null || node.right != null) {
            return false;
        }

        // check which direction we're splitting.
        // if the height and width are similar, pick one randomly.
        boolean splitH = (AuxUtils.getRandom(2) == 1);
        int nW = node.getWidth();
        int nH = node.getHeight();
        if (nW > nH && ((float) nH / (float) nW >= 0.05f)) {
            splitH = false;
        } else if (nH > nW && ((float) nW / (float) nH >= 0.05f)) {
            splitH = true;
        }

        int splitWidth = 0;
        if (this.fNodeSplitHandler != null) {
            RefObject<Integer> refSplitWidth = new RefObject<>(splitWidth);
            this.fNodeSplitHandler.invoke(node, refSplitWidth);
            splitWidth = refSplitWidth.argValue;
        }

        // check to see if we have enough space to split
        int max = (splitH ? nH : nW) - fMinNodeSize - splitWidth;
        if (max <= fMinNodeSize) {
            return false;
        }

        // get a space to split on
        int split = fMinNodeSize + AuxUtils.getRandom(max - fMinNodeSize);
        int offset = ((this.fDistinctBounds) ? 1 : 0) + splitWidth;

        // gen the depth for new leafs
        int subDepth = node.depth + 1;

        // split the node
        if (splitH) {
            node.splitterType = SplitterType.stHorz;
            node.left = new BSPNode(node, subDepth, node.x1, node.y1, node.x2, node.y1 + split);
            node.right = new BSPNode(node, subDepth, node.x1, node.y1 + split + offset, node.x2, node.y2);

            if (fDistinctBounds) {
                this.processSplit(node.x1, node.y1 + split + 1, node.x2, node.y1 + split + offset - 1);
            }
        } else {
            node.splitterType = SplitterType.stVert;
            node.left = new BSPNode(node, subDepth, node.x1, node.y1, node.x1 + split, node.y2);
            node.right = new BSPNode(node, subDepth, node.x1 + split + offset, node.y1, node.x2, node.y2);

            if (fDistinctBounds) {
                this.processSplit(node.x1 + split + 1, node.y1, node.x1 + split + offset - 1, node.y2);
            }
        }

        return true;
    }

    private void processSplit(int x1, int y1, int x2, int y2)
    {
        if (this.fSplitProc != null) {
            this.fSplitProc.invoke(x1, y1, x2, y2);
        }
    }

    private boolean hasPoint(BSPNode node, int ax, int ay)
    {
        if (node == null) {
            return false;
        } else {
            return (ax >= node.x1 && ax <= node.x2 && ay >= node.y1 && ay <= node.y2);
        }
    }

    private boolean hasNode(List<BSPNode> leaves, int ax, int ay)
    {
        for (BSPNode node : leaves) {
            if (hasPoint(node, ax, ay)) {
                return true;
            }
        }

        return false;
    }

    private void checkNode(List<BSPNode> leaves, BSPNode node)
    {
        int cX = node.x1 + (node.getWidth() / 2);
        int cY = node.y1 + (node.getHeight() / 2);
        int halfMin = this.fMinNodeSize / 2;

        boolean resL, resT, resR, resB;

        resL = this.hasNode(leaves, node.x1 - halfMin, cY); // left
        node.sides.add(new NodeSide(SideType.stLeft, (resL) ? LocationType.ltInner : LocationType.ltOuter));

        resR = this.hasNode(leaves, node.x2 + halfMin, cY); // right
        node.sides.add(new NodeSide(SideType.stRight, (resR) ? LocationType.ltInner : LocationType.ltOuter));

        resT = this.hasNode(leaves, cX, node.y1 - halfMin); // top
        node.sides.add(new NodeSide(SideType.stTop, (resT) ? LocationType.ltInner : LocationType.ltOuter));

        resB = this.hasNode(leaves, cX, node.y2 + halfMin); // bottom
        node.sides.add(new NodeSide(SideType.stBottom, (resB) ? LocationType.ltInner : LocationType.ltOuter));


        if (!resL || !resR || !resT || !resB) {
            node.nodeType = LocationType.ltOuter;
        } else {
            node.nodeType = LocationType.ltInner;
        }
    }

    public void checkLeaves()
    {
        List<BSPNode> leaves = this.getLeaves();
        for (BSPNode node : leaves) {
            this.checkNode(leaves, node);
        }
    }
}
