using FaffLatest.scripts.characters;
using Godot;

namespace FaffLatest.scripts.effects
{
    public class BloodParticles : Particles
    {
        private Node parentCharacter;

        private float enabledTime;

        [Export]
        public float TimeToEnableForInSeconds { get; private set; } = 3.0f;

        public override void _Ready()
        {
            base._Ready();
            parentCharacter = GetParent().GetParent();

            parentCharacter.Connect(nameof(Character._Character_ReceivedDamage), this, "_On_Character_ReceivedDamage");
        }

        public override void _Process(float delta)
        {
            if(Emitting)
            {
                enabledTime += delta;

                if(enabledTime >= TimeToEnableForInSeconds)
                {
                    Emitting = false;
                    enabledTime = 0.0f;
                }
            }
        }

        private void _On_Character_ReceivedDamage(Character character, int damage, Vector3 origin, bool killingBlow)
        {
            this.Emitting = true;
            GD.Print($"Emitting");

            var direction = (origin - this.GlobalTransform.origin).Normalized();

            if(ProcessMaterial is ParticlesMaterial pm)
            {
                pm.Direction = direction;
            }
        }
    }
}