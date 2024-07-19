using Godot;
using Godot.Collections;

namespace FrogSubtitle;

public partial class Subtitles : Resource
{
    [Export] public Array<SubtitleEntry> Entries;
}