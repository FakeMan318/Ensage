﻿namespace SimpleAbilityLeveling
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly AbilityLeveling abilityLeveling = new AbilityLeveling();

        #endregion

        #region Public Methods and Operators

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        #endregion

        #region Methods

        private void Drawing_OnDraw(EventArgs args)
        {
            abilityLeveling.OnDraw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            abilityLeveling.OnUpdate();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            abilityLeveling.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            abilityLeveling.OnLoad();
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        #endregion
    }
}