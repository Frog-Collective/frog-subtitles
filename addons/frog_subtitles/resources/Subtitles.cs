using Godot;
using Godot.Collections;

namespace Frog;

public partial class Subtitles : Resource
{
    [Export] public Array<SubtitleEntry> Entries;
}
