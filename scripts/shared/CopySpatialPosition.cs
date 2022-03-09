using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.shared
{
    public class CopySpatialPosition : Spatial
    {
        [Export]
        public string NodePathToCopy;

        private Spatial _toCopy;

        public override async void _Ready()
        {
            _toCopy = GetNode<Spatial>(NodePathToCopy);

            base._Ready();
            if( _toCopy == null )
            {
                await Task.Run(() => System.Threading.Thread.Sleep(500));
            }

            if (_toCopy == null)
                throw new Exception($"Could not find node for {_toCopy}");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            this.Transform = _toCopy.Transform;
        }
    }
}
