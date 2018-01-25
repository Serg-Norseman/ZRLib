/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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

using System.Windows.Forms;
using MysteriesRL.Game;
using ZRLib.Terminal;

namespace MysteriesRL.Views
{
    public sealed class MainView : BaseView
    {
        private readonly MRLGame fGameSpace;
        private BaseView fSubView;
        private ViewType fCurView;

        private StartupView fStartupView;
        private GameView fGameView;
        private MinimapView fMinimapView;
        private HelpView fHelpView;
        private PlayerChoiceView fPlayerChoiceView;
        private SelfView fSelfView;

        public MainView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
            fGameSpace = new MRLGame(this);

            InitViews();

            View = ViewType.vtStartup;
        }

        public override MRLGame GameSpace
        {
            get { return fGameSpace; }
        }

        private void InitViews()
        {
            fStartupView = new StartupView(this, fTerminal);
            fGameView = new GameView(this, fTerminal);
            fMinimapView = new MinimapView(this, fTerminal);
            fHelpView = new HelpView(this, fTerminal);
            fPlayerChoiceView = new PlayerChoiceView(this, fTerminal);
            fSelfView = new SelfView(this, fTerminal);
        }

        public ViewType View
        {
            set {
                //ViewType prevView = fCurView;
    
                fCurView = value;
    
                switch (value) {
                    case ViewType.vtStartup:
                        fSubView = fStartupView;
                        break;
    
                    case ViewType.vtGame:
                        fSubView = fGameView;
                        break;
    
                    case ViewType.vtMinimap:
                        fSubView = fMinimapView;
                        break;
    
                    case ViewType.vtHelp:
                        fSubView = fHelpView;
                        break;
    
                    case ViewType.vtPlayerChoice:
                        fSubView = fPlayerChoiceView;
                        break;
    
                    case ViewType.vtSelf:
                        fSubView = fSelfView;
                        break;
                }
    
                fSubView.Show();
                UpdateView();
            }
        }

        internal override void UpdateView()
        {
            fSubView.UpdateView();
            //fTerminal.Invalidate();
        }

        public override void Tick()
        {
            fSubView.Tick();
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            fSubView.KeyPressed(e);
            UpdateView();
        }

        public override void KeyTyped(KeyPressEventArgs e)
        {
            fSubView.KeyTyped(e);
            UpdateView();
        }

        public override void MouseClicked(MouseEventArgs e)
        {
            fSubView.MouseClicked(e);
            UpdateView();
        }

        public override void MouseMoved(MouseEventArgs e)
        {
            fSubView.MouseMoved(e);
        }

        public override void Show()
        {
            // dummy
        }

        public void RepaintImmediately()
        {
            //fTerminal.Invalidate(false);
            Application.DoEvents();
        }
    }
}
