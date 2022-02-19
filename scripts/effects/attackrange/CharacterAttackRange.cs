using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.effects.attackrange
{
    public class CharacterAttackRange : MeshInstance
    {
        private Character character;
        private SphereMesh mesh;

        public async override void _Ready()
        {
            base._Ready();
            await ConnectSignals();
        }

        private async Task ConnectSignals()
        {
            Visible = false;
            var gsm = GetNode(NodeReferences.Systems.GAMESTATE_MANAGER);
            gsm.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
            gsm.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);

            var parentCharacter = GetParent().GetParent<Character>();
            character = parentCharacter;

            await ToSignal(parentCharacter, "ready");
            Scale = new Vector3(character.Stats.EquippedWeapon.Range + 0.5f, 1, character.Stats.EquippedWeapon.Range + 0.5f);
            Visible = true;     
        }

        private void _On_Character_SelectionCleared()
        {
            Visible = false;
        }

        private void _On_Character_Selected(Character character)

        {
            if(character == this.character)
            {
                Show();
            }
        }

    }
}