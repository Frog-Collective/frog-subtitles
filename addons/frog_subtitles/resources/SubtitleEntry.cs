using Godot;

namespace Frog;

public partial class SubtitleEntry : Resource
{
    [Export] public int Id;
    [Export] public double StartTime;
    [Export] public double EndTime;
    [Export] public string Content;
}
