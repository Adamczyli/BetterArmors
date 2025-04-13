using System.ComponentModel;
using Exiled.API.Interfaces;

namespace BetterArmors
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("Informacje na temat HP itp.")]
        public bool ZbierajInformacjeOstanieArmoru { get; set; } = true;

        [Description("Ilość HP dla Light Armor")]
        public float LightArmorHP { get; set; } = 25f;

        [Description("Ilość HP dla Combat Armor")]
        public float CombatArmorHP { get; set; } = 50f;

        [Description("Ilość HP dla Heavy Armor")]
        public float HeavyArmorHP { get; set; } = 100f;
    }
}