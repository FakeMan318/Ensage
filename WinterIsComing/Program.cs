﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Attribute = Ensage.Attribute;

namespace WinterIsComing {
	internal class Program {
		private static bool inGame;
		private static Hero hero;

		private static readonly Menu Menu = new Menu("Winter is Coming", "winter", true);
		private static readonly Dictionary<string, bool> EnemiesMenu = new Dictionary<string, bool>();

		private static readonly string[] AdditionalDisableModifiers = {
			"modifier_slark_pounce_leash",
			"modifier_doom_bringer_doom",
			"modifier_axe_berserkers_call",
			"modifier_legion_commander_duel"
		};

		private static void Main() {

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("autoHealWhenDisabled", "Auto heal ally when disabled").SetValue(true));
			Menu.AddItem(new MenuItem("autoHealWhenLowHP", "Auto heal ally when HP% lower").SetValue(new Slider(20)))
				.SetTooltip("This option also includes your hero");
			Menu.AddItem(new MenuItem("autoUlt", "Auto ultimate"))
				.SetValue(new HeroToggler(EnemiesMenu, true, false, false)).DontSave();
			Menu.AddItem(new MenuItem("autoUltEnemies", "When enemies near").SetValue(new Slider(2, 0, 4)));

			Menu.AddToMainMenu();

			Game.OnUpdate += Game_OnUpdate;
		}

		private static void Game_OnUpdate(EventArgs args) {
			if (!Utils.SleepCheck("delay"))
				return;

			if (!Menu.Item("enabled").GetValue<bool>()) {
				Utils.Sleep(1000, "delay");
				return;
			}

			if (!inGame) {
				hero = ObjectMgr.LocalHero;

				if (!Game.IsInGame || hero == null || hero.ClassID != ClassID.CDOTA_Unit_Hero_Winter_Wyvern) {
					Utils.Sleep(1000, "delay");
					return;
				}

				EnemiesMenu.Clear();

				inGame = true;
			}

			if (!Game.IsInGame) {
				inGame = false;
				return;
			}

			var reloadMenu = false;
			var allEnemies = ObjectMgr.GetEntities<Hero>().Where(x => x.Team == hero.GetEnemyTeam() && !x.IsIllusion).ToList();

			foreach (var enemy in allEnemies.Where(enemy => !EnemiesMenu.ContainsKey(enemy.Name))) {
				EnemiesMenu.Add(enemy.Name, false);
				reloadMenu = true;
			}

			if (reloadMenu)
				Menu.Item("autoUlt").SetValue(new HeroToggler(EnemiesMenu, true, false, false)).DontSave();

			if (!hero.IsAlive || Game.IsPaused) {
				Utils.Sleep(1000, "delay");
				return;
			}

			var enemies = allEnemies.Where(x => x.IsVisible && x.IsAlive).ToList();
			var ult = hero.Spellbook.SpellR;
			var heal = hero.Spellbook.SpellE;

			if (ult.CanBeCasted()) {
				foreach (var enemy in enemies.Where(enemy =>
						 enemy.Distance2D(hero) < ult.CastRange + 100 &&
						 Menu.Item("autoUlt").GetValue<HeroToggler>().IsEnabled(enemy.Name))) {
					if (enemies.Count(x => x.Distance2D(enemy) <= 400) - 1 >= Menu.Item("autoUltEnemies").GetValue<Slider>().Value && !enemy.IsLinkensProtected()) {
						ult.UseAbility(enemy);
					}
				}
			}

			var allies =
				ObjectMgr.GetEntities<Hero>()
					.Where(x => x.Team == hero.Team && x.IsAlive && !x.IsIllusion)
					.OrderBy(x => (float) x.Health / x.MaximumHealth)
					.ToList();

			if (heal.CanBeCasted()) {
				foreach (var ally in allies.Where(ally => ally.Distance2D(hero) <= heal.CastRange + 100)) {
					if ((((float) ally.Health / ally.MaximumHealth) * 100 <= Menu.Item("autoHealWhenLowHP").GetValue<Slider>().Value) ||
						(IsDisbled(ally) && ally.IsValid && Menu.Item("autoHealWhenDisabled").GetValue<bool>())) {
							heal.UseAbility(ally);
					}
				}
				
			}

			Utils.Sleep(250, "delay");
		}

		private static bool IsDisbled(Hero unit) {
			return unit.IsHexed() 
				|| unit.IsStunned() 
				|| (unit.IsSilenced() && unit.PrimaryAttribute == Attribute.Intelligence) 
				|| unit.Modifiers.Any(x => AdditionalDisableModifiers.Any(x.Name.Contains));
		}

	}
}