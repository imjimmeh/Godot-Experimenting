using FaffLatest.scripts.state;
using Godot;
using System;

public class FinishTurn : Button
{
    private GameStateManager gsm;

    public override void _GuiInput(InputEvent inputEvent)
    {
        base._GuiInput(inputEvent);

        if(inputEvent is InputEventMouseButton button)
        {
            if(button.ButtonIndex == 1)
            {
                gsm.SetCurrentTurn(Faction.ENEMY);
            }
        }
    }

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        gsm = GetNode<GameStateManager>("/root/Root/Systems/GameStateManager");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
