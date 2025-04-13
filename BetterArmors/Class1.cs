using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using UnityEngine;

namespace BetterArmors
{
    public class BetterArmors : Plugin<Config>
    {
        public override string Name => "BetterArmors";
        public override string Author => "Adamczyli<3";
        public override Version Version => new(1, 4, 0);
        public static BetterArmors Instance { get; private set; }
        private readonly Dictionary<Player, float> _grantedHp = new();
        private CoroutineHandle _checkArmorCoroutine;

        public override void OnEnabled()
        {
            Instance = this;
            Log.Info("Pomyślnie załadowano plugin");
            SubscribeEvents();
            _checkArmorCoroutine = Timing.RunCoroutine(MonitorArmor());
        }

        public override void OnDisabled()
        {
            UnsubscribeEvents();
            if (_checkArmorCoroutine.IsRunning)
                Timing.KillCoroutines(_checkArmorCoroutine);
            Instance = null;
        }

        private void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        private void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
        }

        private IEnumerator<float> MonitorArmor()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player == null || !player.IsConnected || player.IsDead)
                        continue;

                    if (player.Items.Any(i => i is Armor))
                    {
                        if (!_grantedHp.ContainsKey(player))
                            SprawdzajArmor(player);
                    }
                    else
                    {
                        if (_grantedHp.ContainsKey(player))
                            ZabierzHPjeslibezarmoru(player);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        private void SprawdzajArmor(Player player)
        {
            if (player == null || !player.IsConnected)
                return;

            ZabierzHPjeslibezarmoru(player);

            Armor armor = player.Items.FirstOrDefault(i => i is Armor) as Armor;
            if (armor == null)
                return;

            float baseHp = UżywajprzypisanegoHP(armor.Type);
            if (baseHp <= 0) return;

            float bonusHp = Config.ZbierajInformacjeOstanieArmoru ? baseHp * 0.8f : baseHp;

            player.Health += bonusHp;
            _grantedHp[player] = bonusHp;
        }

        private void ZabierzHPjeslibezarmoru(Player player)
        {
            if (player != null && _grantedHp.TryGetValue(player, out float hp))
            {
                player.Health = Mathf.Max(1, player.Health - hp);
                _grantedHp.Remove(player);
            }
        }

        private float UżywajprzypisanegoHP(ItemType armorType) => armorType switch
        {
            ItemType.ArmorLight => Config.LightArmorHP,
            ItemType.ArmorCombat => Config.CombatArmorHP,
            ItemType.ArmorHeavy => Config.HeavyArmorHP,
            _ => 0f
        };

        private void OnSpawned(SpawnedEventArgs ev) =>
            Timing.CallDelayed(0.3f, () => SprawdzajArmor(ev.Player));

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (ev.Player.Items.Any(i => i is Armor))
                Timing.CallDelayed(0.1f, () => SprawdzajArmor(ev.Player));
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player == null || !ev.Player.IsConnected)
                return;

            Timing.CallDelayed(0.05f, () =>
            {
                if (!_grantedHp.TryGetValue(ev.Player, out float bonusHp))
                {
                    SprawdzajArmor(ev.Player);
                    return;
                }

                float currentHp = ev.Player.Health;

                if (currentHp < bonusHp)
                {
                    Armor armor = ev.Player.Items.FirstOrDefault(i => i is Armor) as Armor;
                    if (armor != null)
                    {
                        armor.Destroy();
                        ZabierzHPjeslibezarmoru(ev.Player);
                        ev.Player.ShowHint("<color=red>Armor został zniszczony</color>", 3f);
                    }
                }
                else
                {
                    SprawdzajArmor(ev.Player);
                }
            });
        }

        private void OnDied(DiedEventArgs ev) => ZabierzHPjeslibezarmoru(ev.Player);
        private void OnLeft(LeftEventArgs ev) => ZabierzHPjeslibezarmoru(ev.Player);
        private void OnRoundEnded(RoundEndedEventArgs _) => _grantedHp.Clear();
    }
}
