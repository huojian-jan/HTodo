using System.Windows.Input;

namespace ControlToolKits.Commands;

public static class ControlCommands
{
    /// <summary>
    ///     关闭
    /// </summary>
    public static RoutedCommand Close { get; } = new(nameof(Close), typeof(ControlCommands));

    /// <summary>
    ///     清除
    /// </summary>
    public static RoutedCommand Clear { get; } = new(nameof(Clear), typeof(ControlCommands));
}