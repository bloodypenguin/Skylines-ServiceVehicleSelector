// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.SerializableDataExtension
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ICities;
using System;

namespace ServiceVehicleSelector2
{
  public class SerializableDataExtension : ISerializableDataExtension
  {
    public static SerializableDataExtension instance;
    private ISerializableData _serializableData;
    private bool _loaded;

    public ISerializableData SerializableData
    {
      get
      {
        return this._serializableData;
      }
    }

    public bool Loaded
    {
      get
      {
        return this._loaded;
      }
      set
      {
        this._loaded = value;
      }
    }

    public event SerializableDataExtension.SaveDataEventHandler EventSaveData;

    public void OnCreated(ISerializableData serializedData)
    {
      SerializableDataExtension.instance = this;
      this._serializableData = serializedData;
    }

    public void OnLoadData()
    {
    }

    public void OnSaveData()
    {
      // ISSUE: reference to a compiler-generated field
      if (!this._loaded || this.EventSaveData == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.EventSaveData();
    }

    public void OnReleased()
    {
      SerializableDataExtension.instance = (SerializableDataExtension) null;
    }

    public static void WriteUInt16(ushort value, FastList<byte> data)
    {
      SerializableDataExtension.AddToData(BitConverter.GetBytes(value), data);
    }

    public static ushort ReadUInt16(byte[] data, ref int index)
    {
      int uint16 = (int) BitConverter.ToUInt16(data, index);
      index = index + 2;
      return (ushort) uint16;
    }

    public static void WriteInt32(int value, FastList<byte> data)
    {
      SerializableDataExtension.AddToData(BitConverter.GetBytes(value), data);
    }

    public static int ReadInt32(byte[] data, ref int index)
    {
      int int32 = BitConverter.ToInt32(data, index);
      index = index + 4;
      return int32;
    }

    public static void WriteString(string s, FastList<byte> data)
    {
      char[] charArray = s.ToCharArray();
      SerializableDataExtension.WriteInt32(charArray.Length, data);
      for (ushort index = 0; (int) index < charArray.Length; ++index)
        SerializableDataExtension.AddToData(BitConverter.GetBytes(charArray[(int) index]), data);
    }

    public static string ReadString(byte[] data, ref int index)
    {
      string empty = string.Empty;
      int num = SerializableDataExtension.ReadInt32(data, ref index);
      for (int index1 = 0; index1 < num; ++index1)
      {
        empty += BitConverter.ToChar(data, index).ToString();
        index = index + 2;
      }
      return empty;
    }

    public static void WriteStringArray(string[] array, FastList<byte> data)
    {
      SerializableDataExtension.WriteInt32(array.Length, data);
      for (int index = 0; index < array.Length; ++index)
        SerializableDataExtension.WriteString(array[index], data);
    }

    public static string[] ReadStringArray(byte[] data, ref int index)
    {
      int length = SerializableDataExtension.ReadInt32(data, ref index);
      string[] strArray = new string[length];
      for (int index1 = 0; index1 < length; ++index1)
        strArray[index1] = SerializableDataExtension.ReadString(data, ref index);
      return strArray;
    }

    public static void AddToData(byte[] bytes, FastList<byte> data)
    {
      foreach (byte num in bytes)
        data.Add(num);
    }

    public delegate void SaveDataEventHandler();
  }
}
