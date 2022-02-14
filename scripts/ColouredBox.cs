using FaffLatest.scripts.characters;
using Godot;
using System;

public class ColouredBox : CSGBox
{
    private const string SHADER_PARAM_COLOUR = "colour";
    private const string SHADER_PARAM_IN_ATTACK_RANGE = "inAttackRange";
    private const string SHADER_PARAM_HIGHLIGHTED = "isHighlighted";
    private const string SHADER_PARAM_IS_SELECTED = "isSelected";

    [Export(PropertyHint.ColorNoAlpha)]
    public Vector3 PlayerCharacterColour { get; private set; } = new Vector3(1.0f, 0.0f, 1.0f);

    [Export(PropertyHint.ColorNoAlpha)]
    public Vector3 ZombieCharacterColour { get; private set; } = new Vector3(0.0f, 1.0f, 0.0f);

    private ShaderMaterial material;

    private Character parent;

    public override void _Ready()
    {
        base._Ready();
        material = Material as ShaderMaterial;
    }

    public void SetParent(Character parent)
        => this.parent = parent;

    public void SetColour(CharacterStats stats)
    {
        var colourToUse = stats.IsPlayerCharacter ? PlayerCharacterColour : ZombieCharacterColour;

        material.SetShaderParam(SHADER_PARAM_COLOUR, colourToUse);
    }

    private void _On_Character_Selected(Character character)
    {
        if (character == parent)
        {
            material.SetShaderParam(SHADER_PARAM_IS_SELECTED, true);
            return;
        }

        CalculateAndSetAttackRangeShaderParam(character);
    }

    private void CalculateAndSetAttackRangeShaderParam(Character character)
    {
        if (character.Stats.IsPlayerCharacter == character.Stats.IsPlayerCharacter)
            return;

        var distance = (character.ProperBody.GlobalTransform.origin - GlobalTransform.origin).Length();

        var isInRange = distance <= character.Stats.EquippedWeapon.Range;

        material.SetShaderParam(SHADER_PARAM_IN_ATTACK_RANGE, isInRange);
    }

    private void _On_Character_SelectionCleared()
    {
        material.SetShaderParam(SHADER_PARAM_IS_SELECTED, false);
        material.SetShaderParam(SHADER_PARAM_IN_ATTACK_RANGE, false);
    }

    private void _On_Character_MouseEntered()
    {
        material.SetShaderParam(SHADER_PARAM_HIGHLIGHTED, true);
    }

    private void _On_Character_MouseExited()
    {
        material.SetShaderParam(SHADER_PARAM_HIGHLIGHTED, false);
    }
}
