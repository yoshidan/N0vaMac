using System.Runtime.InteropServices;

public class WindowApi
{
    public const string Target = "WindowApi";
    
    [DllImport(Target)]
    private static extern void background();

    [DllImport(Target)]
    private static extern void hideTitleBar();
    
    [DllImport(Target)]
    private static extern void initializePosition();
    
    [DllImport(Target)]
    private static extern void maximize();

    public static void Background()
    {
        background();
    }

    public static void HideTitleBar()
    {
        hideTitleBar();
    }
    
    public static void InitializePosition()
    {
        initializePosition();
    }
    
    public static void Maximize()
    {
        maximize();
    }

}