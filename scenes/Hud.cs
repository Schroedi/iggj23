using Godot;
using System;

public class Hud : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var btnPhase2 = GetNode<Button>("btnPhase2");
        btnPhase2.Connect("pressed", this, "_on_btnPhase2_pressed");
        
        var btnPhase3 = GetNode<Button>("btnPhase3");
        btnPhase3.Connect("pressed", this, "_on_btnPhase3_pressed");
    }

    private void _on_btnPhase2_pressed()
    {
        GD.Print("Phase 2");
        GameState.Current(this).StartPhase2();
    }
    
    private void _on_btnPhase3_pressed()
    {
        GD.Print("Phase 3");
        GameState.Current(this).StartPhase3();
    }

    public override void _Process(float delta)
    {
        var gs = GameState.Current(this);
        GetNode<Button>("btnPhase2").Visible = gs.IsCutting;
        GetNode<Button>("btnPhase3").Visible = gs.IsStitching;
        GetNode<Label>("lPhase1").Visible = gs.IsCutting;
        GetNode<Label>("lPhase2").Visible = gs.IsStitching;
        GetNode<Label>("lPhase3").Visible = gs.IsThrowing;
    }
}
