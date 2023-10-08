using Godot;
using System;

public class BubbleContent : Node2D
{
    public AnimalType AnimalType;

    public Label LabelScore;

    public bool Popped = false;

    public bool HasAnimalType = false;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (var c in GetChildren())
            if (c is Label l)
                LabelScore = l;

        LabelScore.Visible = false;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!HasAnimalType)
        {
            HasAnimalType = true;
            AnimalType = GameState.Current(this).AnimalDistribution.Sample();
        }

        if (Popped)
            return;

        var state = GameState.Current(this);

        if (state.IsThrowing && state.LargestAnimal != null)
        {
            var refPos = GetParent().GetParent<Node2D>().GlobalPosition;
            var dis = (state.LargestAnimal.ComputeAveragePosition() - refPos).Length();

            if (dis < 400 && state.LargestAnimal.AnimalParts.ContainsKey(AnimalType))
            {
                Popped = true;
                foreach (var c in GetChildren())
                    if (c is Node2D n)
                        n.Visible = false;

                var score = state.LargestAnimal.AnimalParts[AnimalType];

                LabelScore.Text = $"+ {score}";
                LabelScore.Visible = true;
                state.Score += score;

                if (state.TimeInThrowing < 30)
                    state.TimeToLive += score / 100f;
            }
        }

    }
}
