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

}