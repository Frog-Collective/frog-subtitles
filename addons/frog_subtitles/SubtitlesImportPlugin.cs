#if TOOLS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Godot;
using Godot.Collections;

namespace Frog;

public partial class SubtitlesImportPlugin : EditorImportPlugin
{
    private readonly static Regex timeRegex = new Regex(@"(?<start>[0-9,:\.]+)\ -->\ (?<end>[0-9,:\.]+)");
    private readonly static Regex colorRegex = new Regex(@"<font color=""(.+)"">");
    private readonly static Regex linePositionRegex = new Regex(@"\{\\a([0-9]+)\}");

    public override string _GetImporterName() => "frog-subtitles.plugin";

    public override string _GetVisibleName() => "Frog Subtitles";

    public override string[] _GetRecognizedExtensions() => new string[] { "srt" };

    public override string _GetSaveExtension() => "res";

    public override string _GetResourceType() => "Resource";

    public override int _GetPresetCount() => 1;

    public override float _GetPriority() => 1;

    public override int _GetImportOrder() => 0;

    public override string _GetPresetName(int presetIndex) => "Default";

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex)
    {
        return new Array<Dictionary>();
    }

    public override Error _Import(string sourceFile, string savePath, Dictionary options, Array<string> platformVariants, Array<string> genFiles)
    {
        Error parseResult = Error.Ok;
        ReaderState state = ReaderState.ReadId;
        List<SubtitleEntry> entries = new();
        try
        {
            int currentId = 0;
            TimeSpan startTime = TimeSpan.Zero;
            TimeSpan endTime = TimeSpan.Zero;
            StringBuilder content = new();

            void RegisterEntry()
            {
                content.Replace("<b>", "[b]").Replace("</b>", "[/b]").Replace("{b}", "[b]").Replace("{/b}", "[/b]");
                content.Replace("<i>", "[i]").Replace("</i>", "[/i]").Replace("{i}", "[i]").Replace("{/i}", "[/i]");
                content.Replace("<u>", "[u]").Replace("</u>", "[/u]").Replace("{u}", "[u]").Replace("{/u}", "[/u]");
                content.Replace("</font>", "[/color]");
                string contentString = content.ToString();
                contentString = colorRegex.Replace(contentString, match => $"[color={match.Groups[1].Value}]");
                contentString = linePositionRegex.Replace(contentString, match =>
                {
                    string result = string.Empty;
                    if (int.TryParse(match.Groups[1].Value, out int lineCount))
                    {
                        for (int i = 1; i < lineCount; ++i)
                        {
                            result += '\n';
                        }
                    }
                    else
                    {
                        Trace.TraceError($"Can't parse subtitles line position tag: {match.Value}");
                    }

                    return result;
                });

                entries.Add(new SubtitleEntry()
                {
                    Id = currentId,
                    StartTime = startTime.TotalSeconds,
                    EndTime = endTime.TotalSeconds,
                    Content = contentString,
                });

                content.Clear();
            }

            using (StreamReader reader = File.OpenText(ProjectSettings.GlobalizePath(sourceFile)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    switch (state)
                    {
                        case ReaderState.ReadId:
                            if (!int.TryParse(line, out currentId))
                            {
                                Trace.TraceError($"Can't parse subtitles id: {line}");
                                parseResult = Error.Failed;
                            }

                            state = ReaderState.ReadTime;
                            break;

                        case ReaderState.ReadTime:
                            Match match = SubtitlesImportPlugin.timeRegex.Match(line);
                            if (!match.Success ||
                                !TimeSpan.TryParse(match.Groups["start"].Value.Replace(',', '.'), out startTime) ||
                                !TimeSpan.TryParse(match.Groups["end"].Value.Replace(',', '.'), out endTime))
                            {
                                Trace.TraceError($"Can't parse subtitles time: {line}");
                                parseResult = Error.Failed;
                            }

                            state = ReaderState.ReadContent;
                            break;

                        case ReaderState.ReadContent:
                            if (string.IsNullOrEmpty(line))
                            {
                                // End of current entry. Store it and clear buffers.
                                RegisterEntry();
                                state = ReaderState.ReadId;
                                break;
                            }

                            if (content.Length > 0)
                            {
                                content.Append('\n');
                            }

                            content.Append(line);
                            break;
                    }
                }

                // Register last entry (no need to end a line, end of file is enough)
                if (content.Length > 0)
                {
                    RegisterEntry();
                }
            }
        }
        catch
        {
            return Error.Failed;
        }

        if (parseResult != Error.Ok)
        {
            return parseResult;
        }

        Debug.WriteLine($"{sourceFile} parsed. Found {entries.Count} subtitles entries.");
        entries.Sort((left, right) => left.Id.CompareTo(right.Id));
        var subtitles = new Subtitles()
        {
            Entries = new Array<SubtitleEntry>(entries),
        };

        string filename = $"{savePath}.{this._GetSaveExtension()}";
        return ResourceSaver.Save(subtitles, filename);
    }

    private enum ReaderState
    {
        ReadId,
        ReadTime,
        ReadContent,
    }
}

#endif
