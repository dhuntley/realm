using System;

[AttributeUsage(AttributeTargets.Method)]
public class GUIPanelAttribute : System.Attribute {
    public string buttonText;

    public GUIPanelAttribute(string text) {
        buttonText = text;
    }
}
