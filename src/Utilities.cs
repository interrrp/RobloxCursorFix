using System.Numerics;
using System.Runtime.InteropServices;
using PInvoke;
using static PInvoke.User32;

namespace RobloxCursorFix.Utilities;

/// <summary>
/// Unorganized utility methods.
/// </summary>
public class Utilities
{
    /// <summary>
    /// Check if the given key has just been pressed (not held down).
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>Whether the key has just been pressed.</returns>
    public static bool IsKeyJustPressed(VirtualKey key) => (GetAsyncKeyState((int)key) & 1) == 1;

    /// <summary>
    /// Check if the given key is currently held down.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>Whether the key is being held down.</returns>
    public static bool IsKeyDown(VirtualKey key) => GetAsyncKeyState((int)key) != 0;

    /// <summary>
    /// Get the cursor position as a <see cref="Vector2" />
    /// </summary>
    /// <returns>The cursor position as a vector.</returns>
    public static Vector2 GetCursorPosVec() => new(GetCursorPos().x, GetCursorPos().y);
    
    /// <summary>
    /// Set the cursor position using a <see cref="Vector2" />.
    /// </summary>
    /// <param name="pos">The vector of the new position.</param>
    public static void SetCursorPosVec(Vector2 pos) => SetCursorPos((int)pos.X, (int)pos.Y);

    /// <summary>
    /// Check if the currently focused window is Roblox.
    /// </summary>
    /// <returns>Whether the Roblox window is focused.</returns>
    public static bool IsRobloxFocused()
    {
        nint focusedWindowHandle = GetForegroundWindow();
        if (focusedWindowHandle == IntPtr.Zero)
            return false; // No window is focused. This can happen when the Start Menu is open.

        string focusedWindowTitle = GetWindowText(focusedWindowHandle);

        return focusedWindowTitle == "Roblox";
    }

    /// <summary>
    /// Get the center of the Roblox window.
    /// </summary>
    /// <returns>The Roblox window's center position as a vector.</returns>
    public static Vector2 GetRobloxCenter()
    {
        GetWindowRect(GetRobloxWindowHandle(), out RECT robloxRect);
        return new Vector2
        {
            X = robloxRect.right / 2,
            Y = robloxRect.bottom / 2
        };
    }

    /// <summary>
    /// Get the handle of the Roblox window.
    /// </summary>
    /// <returns>The Roblox window's handle.</returns>
    /// <exception cref="Exception">Thrown when the Roblox window is not found.</exception>
    public static nint GetRobloxWindowHandle()
    {
        nint robloxWindowHandle = FindWindow(null, "Roblox");
        if (robloxWindowHandle == IntPtr.Zero)
            throw new Exception("Roblox window not found");

        return robloxWindowHandle;
    }

    /// <summary>
    /// Transform a <see cref="VirtualKey" /> to a human-readable name.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The human-readable key name.</returns>
    public static string KeyToReadableName(VirtualKey key) => key.ToString().Replace("VK_", "");
}
