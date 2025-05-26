using System;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CorruptCrimsonMimic
{
    [ApiVersion(2, 1)]
    public class CorruptCrimsonMimic : TerrariaPlugin
    {
        public override string Author => "Ruff Trigger";
        public override string Name => "CorruptCrimsonMimic";
        public override Version Version => new Version(3, 2);
        public override string Description => "Replaces any Mimic spawned by Key of Night with 50% Corrupt/Crimson";

        public CorruptCrimsonMimic(Main game) : base(game) { }

        public override void Initialize()
        {
            ServerApi.Hooks.NpcSpawn.Register(this, OnNpcSpawn);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnNpcSpawn);
            }
            base.Dispose(disposing);
        }

        private void OnNpcSpawn(NpcSpawnEventArgs args)
        {
            if (args.NpcId < 0 || args.NpcId >= Main.maxNPCs)
                return;

            NPC npc = Main.npc[args.NpcId];
            if (npc == null || !npc.active)
                return;

            // Intercept both Crimson and Corruption mimic spawns
            if (npc.type == NPCID.BigMimicCrimson || npc.type == NPCID.BigMimicCorruption)
            {
                Vector2 pos = npc.position;

                // Replace with randomized mimic type
                int newType = Main.rand.Next(2) == 0 ? NPCID.BigMimicCorruption : NPCID.BigMimicCrimson;

                if (npc.type != newType)
                {
                    npc.active = false; // deactivate original

                    int newId = NPC.NewNPC(null, (int)pos.X, (int)pos.Y, newType);
                    if (newId >= 0)
                    {
                        NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, newId);
                        //TSPlayer.All.SendInfoMessage($"A {Lang.GetNPCNameValue(newType)} has awoken!");
                    }
                }
            }
        }
    }
}
