// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.Utils
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework.Plugins;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ServiceVehicleSelector2
{
  public static class Utils
  {
    private static readonly string _fileName = "ServiceVehicleSelector.log";
    private static readonly string _logPrefix = "ServiceVehicleSelector: ";
    private static readonly bool _inGameDebug = System.Environment.OSVersion.Platform != PlatformID.Unix;

    public static void ClearLogFile()
    {
      try
      {
        File.WriteAllText(Utils._fileName, string.Empty);
      }
      catch
      {
        Debug.LogWarning((object) ("Error while clearing log file: " + Utils._fileName));
      }
    }

    public static void LogToTxt(object o)
    {
      try
      {
        File.AppendAllText(Utils._fileName, DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy ") + o + System.Environment.NewLine);
      }
      catch
      {
        Debug.LogWarning((object) ("Error while writing to log file: " + Utils._fileName));
      }
    }

    public static void Log(object o)
    {
      Utils.Log(PluginManager.MessageType.Message, o);
    }

    public static void LogError(object o)
    {
      Utils.Log(PluginManager.MessageType.Error, o);
    }

    public static void LogWarning(object o)
    {
      Utils.Log(PluginManager.MessageType.Warning, o);
    }

    private static void Log(PluginManager.MessageType type, object o)
    {
      string message = Utils._logPrefix + o;
      switch (type)
      {
        case PluginManager.MessageType.Error:
          Debug.LogError((object) message);
          break;
        case PluginManager.MessageType.Warning:
          Debug.LogWarning((object) message);
          break;
        case PluginManager.MessageType.Message:
          Debug.Log((object) message);
          break;
      }
      if (!Utils._inGameDebug)
        return;
      DebugOutputPanel.AddMessage(type, message);
    }

    public static Q GetPrivate<Q>(object o, string fieldName)
    {
      FieldInfo[] fields = o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      FieldInfo fieldInfo1 = (FieldInfo) null;
      foreach (FieldInfo fieldInfo2 in fields)
      {
        if (fieldInfo2.Name == fieldName)
        {
          fieldInfo1 = fieldInfo2;
          break;
        }
      }
      return (Q) fieldInfo1.GetValue(o);
    }

    public static float ToSingle(string value)
    {
      try
      {
        return Convert.ToSingle(value);
      }
      catch
      {
        return 0.0f;
      }
    }

    public static int ToInt32(string value)
    {
      try
      {
        return Convert.ToInt32(value);
      }
      catch
      {
        return 0;
      }
    }

    public static byte ToByte(string value)
    {
      try
      {
        return Convert.ToByte(value);
      }
      catch
      {
        return 0;
      }
    }

    public static bool Truncate(UILabel label, string text, string suffix = "…")
    {
      bool flag = false;
      using (UIFontRenderer renderer = label.ObtainRenderer())
      {
        float units = label.GetUIView().PixelsToUnits();
        float[] characterWidths = renderer.GetCharacterWidths(text);
        float num1 = 0.0f;
        float num2 = (float) ((double) label.width - (double) label.padding.horizontal - 2.0);
        for (int index = 0; index < characterWidths.Length; ++index)
        {
          num1 += characterWidths[index] / units;
          if ((double) num1 > (double) num2)
          {
            flag = true;
            text = text.Substring(0, index - 3) + suffix;
            break;
          }
        }
      }
      label.text = text;
      return flag;
    }

    public static string RemoveInvalidFileNameChars(string fileName)
    {
      return ((IEnumerable<char>) System.IO.Path.GetInvalidFileNameChars()).Aggregate<char, string>(fileName, (Func<string, char, string>) ((current, c) => current.Replace(c.ToString(), string.Empty)));
    }
  }
}
