using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.characters
{
    public class CharacterOwnedMeshInstance : MeshInstance
    {
        protected internal bool isHidden = true;

        protected internal Node parentNode;
        protected internal Node characterBody;

        public Node Parent => parentNode;
        public Node CharacterBody => characterBody;

        public bool IsHidden => isHidden;

        public void DisconnectAndDispose()
        {
            if (!isHidden)
            {
                RemoveFromScene();
            }

            Dispose();
        }

        public void SetCharacterBody(Node characterBody) => this.characterBody = characterBody;

        public void SetParentNode(Node parent) => parentNode = parent;

        public override void _Ready()
        {
            base._Ready();

            isHidden = false;
        }

        protected internal void RemoveFromScene()
        {
            parentNode.RemoveChild(this);
            isHidden = true;
        }
    }
}