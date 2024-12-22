using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Shelegram;

public static class CommandHelper
{
    public static readonly FrozenDictionary<string, string> _dict = new Dictionary<string, string>()
    {
        {"/test","echo hello-world"},
        {"/cputemp","sudo vcgencmd measure_temp"},
        {"/restartnetwork","sudo service NetworkManager restart"},
        {"/restart","shutdown -r now"},
    }.ToFrozenDictionary();

    public const string UsageString = @"
Usage:
/test : test echo hello world
/cputemp : see cpu temperature
/restartnetwork : restart network manager
/restart : restart os
";

    public static bool TryGet(string shortcut, [MaybeNullWhen(false)] out string command)
    {
        return _dict.TryGetValue(shortcut, out command);
    }
}
