﻿using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ICities;

namespace ServiceVehicleSelector2
{
  public class SerializableDataExtension : SerializableDataExtensionBase
  {
    private const string DataID = "CTS_BuildingData";
    private const string DataVersion = "v001";

    public static Dictionary<ushort, HashSet<string>> BuildingData;

    public override void OnLoadData()
    {
      if (!TryLoadData(out BuildingData))
        Utils.Log("SVS2 - No data was found in the save file. Default building data was used.");
    }

    public override void OnSaveData()
    {
      var data = new FastList<byte>();
      var invalidBuildingIds = new HashSet<ushort>();
      try
      {
        WriteString(DataVersion, data);
        foreach (var keyValuePair in BuildingData)
        {
          if (keyValuePair.Value.Count == 0)
          {
            invalidBuildingIds.Add(keyValuePair.Key);
          }
          else
          {
            WriteUInt16(keyValuePair.Key, data);
            WriteStringArray(keyValuePair.Value.ToArray(), data);
          }
        }

        serializableDataManager.SaveData(DataID,
          data.ToArray());
      }
      catch (Exception ex)
      {
        var msg = $"SVS2 - Error while saving building data!\n{ex.Message}\n{ex.StackTrace}";
        Utils.LogError(msg);
        CODebugBase<LogChannel>.Log(LogChannel.Modding, msg, ErrorLevel.Error);
        return;
      }

      foreach (var key in invalidBuildingIds)
        BuildingData.Remove(key);
    }
    
    private bool TryLoadData(out Dictionary<ushort, HashSet<string>> data)
    {
      data = new Dictionary<ushort, HashSet<string>>();
      var serializedData = serializableDataManager.LoadData(DataID);
      if (serializedData == null)
        return false;
      var index1 = 0;
      try
      {
        Utils.Log("SVS2 - Try to load building data.");
        var dataVersion = ReadString(serializedData, ref index1);
        Utils.Log($"SVS2 - Found building data version: {dataVersion}");
        if (string.IsNullOrEmpty(dataVersion) || dataVersion.Length != 4)
        {
          Utils.LogError("SVS2 - Found data version was in an unsupported format");
          return false;
        }
        while (index1 < serializedData.Length)
        {
          var stringSet = new HashSet<string>();
          var key = ReadUInt16(serializedData, ref index1);
          var num = ReadInt32(serializedData, ref index1);
          for (var index2 = 0; index2 < num; ++index2)
          {
            var name = ReadString(serializedData, ref index1);
            stringSet.Add(name);
          }

          data.Add(key, stringSet);
        }
        Utils.Log("SVS2 - Building data was successfully loaded.");
        return true;
      }
      catch (Exception ex)
      {
        Utils.LogError($"SVS2 - Could not load building data.\n{ex.Message}\n{ex.StackTrace}");
        data = new Dictionary<ushort, HashSet<string>>();
        return false;
      }
    }


    private static void WriteUInt16(ushort value, FastList<byte> data)
    {
      AddToData(BitConverter.GetBytes(value), data);
    }

    private static ushort ReadUInt16(byte[] data, ref int index)
    {
      int uint16 = BitConverter.ToUInt16(data, index);
      index = index + 2;
      return (ushort) uint16;
    }

    private static void WriteInt32(int value, FastList<byte> data)
    {
      AddToData(BitConverter.GetBytes(value), data);
    }

    private static int ReadInt32(byte[] data, ref int index)
    {
      var int32 = BitConverter.ToInt32(data, index);
      index = index + 4;
      return int32;
    }

    private static void WriteString(string s, FastList<byte> data)
    {
      var charArray = s.ToCharArray();
      WriteInt32(charArray.Length, data);
      for (ushort index = 0; index < charArray.Length; ++index)
        AddToData(BitConverter.GetBytes(charArray[index]), data);
    }

    private static string ReadString(byte[] data, ref int index)
    {
      var empty = string.Empty;
      var num = ReadInt32(data, ref index);
      for (var index1 = 0; index1 < num; ++index1)
      {
        empty += BitConverter.ToChar(data, index).ToString();
        index += 2;
      }
      return empty;
    }

    private static void WriteStringArray(ICollection<string> array, FastList<byte> data)
    {
      WriteInt32(array.Count, data);
      foreach (var t in array)
        WriteString(t, data);
    }

    public static string[] ReadStringArray(byte[] data, ref int index)
    {
      var length = ReadInt32(data, ref index);
      var strArray = new string[length];
      for (var index1 = 0; index1 < length; ++index1)
        strArray[index1] = ReadString(data, ref index);
      return strArray;
    }

    private static void AddToData(IEnumerable<byte> bytes, FastList<byte> data)
    {
      foreach (var num in bytes)
        data.Add(num);
    }
  }
}
