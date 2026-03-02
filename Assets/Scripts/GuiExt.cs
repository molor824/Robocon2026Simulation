using UnityEditor;
using UnityEngine;

public static class GuiExt
{
    public static void DrawRectOutline(Rect rect, float thickness, Color color)
    {
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
        EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), color);
        EditorGUI.DrawRect(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), color);
    }
}