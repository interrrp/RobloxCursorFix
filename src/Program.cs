using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using PInvoke;
using static PInvoke.User32;
using static RobloxCursorFix.Utilities.Utilities;

namespace RobloxCursorFix;

public class Program
{
    private const VirtualKey FirstPersonToggleKey = VirtualKey.VK_RCONTROL;

    private static bool s_firstPerson = false;
    private static bool s_lastRightMouseButtonIsPressed;
    private static Vector2 s_thirdPersonRotationHoldCursorPos;

    [DllImport("user32.dll")]
    [SuppressMessage("Roslyn", "SYSLIB1054", Justification = "Prefer not to write unsafe bindings")]
    private static extern bool ClipCursor(RECT lpRect);

    public static void Main()
    {
        Console.WriteLine($"Press {KeyToReadableName(FirstPersonToggleKey)} to toggle first person mode");

        while (true)
        {
            Thread.Sleep(10);

            if (!IsRobloxFocused())
                continue;

            if (IsKeyJustPressed(FirstPersonToggleKey))
            {
                s_firstPerson = !s_firstPerson;
            }
            
            if (s_firstPerson)
                LockCursorToCenter();
            else
                FixThirdPersonRotation();
        }
    }

    /// <summary>
    /// Fix the third-person rotation using the mouse.
    /// 
    /// When the user lets go of the right mouse button, the mouse would normally go to a different location as if it
    /// were never locked. This method fixes that.
    /// </summary>
    private static void FixThirdPersonRotation()
    {
        if (IsRightMouseButtonJustPressed())
        {
            // Started rotating/holding right mouse button
            s_thirdPersonRotationHoldCursorPos = GetCursorPosVec();
        }
        else if (IsRightMouseButtonJustReleased())
        {
            // Stopped rotating/let go of right mouse button
            // This is where the mouse will go to a different location, so we need to fix it

            WaitForMouseMove();
            SetCursorPosVec(s_thirdPersonRotationHoldCursorPos);
        }

        s_lastRightMouseButtonIsPressed = IsKeyDown(VirtualKey.VK_RBUTTON);
    }

    /// <summary>
    /// Lock the cursor to the center of the Roblox window.
    /// </summary>
    private static void LockCursorToCenter() =>
        SetCursorPosVec(GetRobloxCenter());
    
    /// <summary>
    /// Check if the right mouse button had just been released.
    /// </summary>
    /// <returns>Whether the right mouse button had just been released.</returns>
    private static bool IsRightMouseButtonJustReleased() =>
        s_lastRightMouseButtonIsPressed && !IsKeyDown(VirtualKey.VK_RBUTTON);

    /// <summary>
    /// Check if the right mouse button had just been pressed.
    /// </summary>
    /// <returns>Whether the right mouse button had just been pressed.</returns>
    private static bool IsRightMouseButtonJustPressed() =>
        !s_lastRightMouseButtonIsPressed && IsKeyDown(VirtualKey.VK_RBUTTON);

    /// <summary>
    /// Block the current thread until the mouse moves.
    /// </summary>
    /// <param name="maxDistance">The maximum distance the cursor can go before being considered a movement.</param>
    private static void WaitForMouseMove(long maxDistance = 10)
    {
        var lastCursorPos = GetCursorPosVec();
        var currentCursorPos = GetCursorPosVec();
        while (currentCursorPos == lastCursorPos)
        {
            Thread.Sleep(1);
            lastCursorPos = GetCursorPosVec();

            var distance = Vector2.Distance(lastCursorPos, currentCursorPos);
            if (distance > maxDistance)
                break;
        }
    }
}
