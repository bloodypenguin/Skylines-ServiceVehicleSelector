using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace ServiceVehicleSelector2
{
  public class VehicleListBox : UIPanel
  {
    private float _nextKeyCombi = Time.time;
    private UICheckBox[] _items = new UICheckBox[0];
    private UIScrollablePanel _scrollablePanel;
    private UIScrollbar _scrollbar;
    
    public event PropertyChangedEventHandler<HashSet<string>> eventSelectedItemsChanged;

    public UIFont Font { get; set; }

    public new float height
    {
      get => m_Size.y;
      set
      {
        size = new Vector2(m_Size.x, value);
        if (_scrollablePanel != null)
          _scrollablePanel.height = value;
        if (!(_scrollbar != null))
          return;
        _scrollbar.parent.height = value;
        _scrollbar.height = value;
        _scrollbar.trackObject.height = value;
      }
    }

    public int SelectedIndex
    {
      get
      {
        var num = -1;
        for (var index = 0; index < _items.Length; ++index)
        {
          if (!_items[index].isChecked)
          {
            continue;
          }
          num = index;
          break;
        }
        return num;
      }
    }

    public int[] SelectedIndexes
    {
      get
      {
        var intList = new List<int>();
        for (var index = 0; index < _items.Length; ++index)
        {
          if (_items[index].isChecked)
            intList.Add(index);
        }
        return intList.ToArray();
      }
    }

    public HashSet<string> SelectedItems
    {
      get
      {
        var stringSet = new HashSet<string>();
        foreach (var vehicleListBoxRow in _items)
        {
          if (vehicleListBoxRow.isChecked)
            stringSet.Add(vehicleListBoxRow.tooltip); //tooltip contains the prefab name
        }
        return stringSet;
      }
      set
      {
        if (value == null || SelectedItems == value)
          return;
        foreach (var vehicleListBoxRow in _items)
        {
          vehicleListBoxRow.eventCheckChanged -= OnCheckChanged;
          vehicleListBoxRow.isChecked = value.Contains(vehicleListBoxRow.tooltip);
          vehicleListBoxRow.eventCheckChanged += OnCheckChanged;
        }
      }
    }

    public HashSet<string> Items
    {
      get
      {
        var stringSet = new HashSet<string>();
        foreach (var vehicleListBoxRow in _items)
        {
          stringSet.Add(vehicleListBoxRow.tooltip);
        }

        return stringSet;
      }
    }

    private bool AllSelected()
    {
      return _items.All(item => item.isChecked);
    }

    public void SetSelectionStateToAll(bool state)
    {
      foreach (var item in _items)
      {
        item.eventCheckChanged -= OnCheckChanged;
        item.isChecked = state;
        item.eventCheckChanged += OnCheckChanged;
      }

      FireSelectedItemsChangedEvent();
    }

    public void ClearItems()
    {
      if (_items.Length == 0)
        return;
      foreach (var item in _items)
        Destroy(item.gameObject);

      _items = new UICheckBox[0];
      _scrollablePanel.scrollPosition = new Vector2(0.0f, 0.0f);
    }

    public void AddItem(PrefabData data)
    {
      var vehicleListBoxRowArray = new UICheckBox[_items.Length + 1];
      Array.Copy(_items, vehicleListBoxRowArray, _items.Length);
      var vehicleListBoxRow =
        UIHelper.CreateCheckBox(_scrollablePanel, data.PrefabName, data.DisplayName, false);
      vehicleListBoxRow.tooltip = data.PrefabName; //important! it's used to later save data
      vehicleListBoxRow.eventCheckChanged += OnCheckChanged;
      vehicleListBoxRowArray[_items.Length] = vehicleListBoxRow;
      _items = vehicleListBoxRowArray;
    }

    private void OnCheckChanged(UIComponent component, bool value)
    {
      FireSelectedItemsChangedEvent();
    }

    private void FireSelectedItemsChangedEvent()
    {
      eventSelectedItemsChanged?.Invoke(this, SelectedItems);
    }

    protected override void OnMouseHover(UIMouseEventParameter p)
    {
      base.OnMouseHover(p);
      if (!((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) & Input.GetKey(KeyCode.A)) || _nextKeyCombi >= (double) Time.time)
        return;
      _nextKeyCombi = Time.time + 0.25f;
      SetSelectionStateToAll(!AllSelected());
    }

    public static VehicleListBox Create(UIComponent parent)
    {
      return parent.AddUIComponent<VehicleListBox>();
    }

    public override void Start()
    {
      base.Start();
      autoLayoutDirection = LayoutDirection.Horizontal;
      autoLayoutStart = LayoutStart.TopLeft;
      autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      autoLayout = true;
      _scrollablePanel = AddUIComponent<UIScrollablePanel>();
      _scrollablePanel.width = width - 10f;
      _scrollablePanel.height = height;
      _scrollablePanel.autoLayoutDirection = LayoutDirection.Vertical;
      _scrollablePanel.autoLayoutStart = LayoutStart.TopLeft;
      _scrollablePanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      _scrollablePanel.autoLayout = true;
      _scrollablePanel.clipChildren = true;
      _scrollablePanel.backgroundSprite = "UnlockingPanel";
      _scrollablePanel.color = Color.black;
      var uiPanel = AddUIComponent<UIPanel>();
      uiPanel.width = 10f;
      uiPanel.height = height;
      uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      uiPanel.autoLayout = true;
      var scrollbar = uiPanel.AddUIComponent<UIScrollbar>();
      scrollbar.width = 10f;
      scrollbar.height = scrollbar.parent.height;
      scrollbar.orientation = UIOrientation.Vertical;
      scrollbar.pivot = UIPivotPoint.BottomLeft;
      scrollbar.AlignTo(uiPanel, UIAlignAnchor.TopRight);
      scrollbar.minValue = 0.0f;
      scrollbar.value = 0.0f;
      scrollbar.incrementAmount = 27f;
      _scrollbar = scrollbar;
      var uiSlicedSprite1 = scrollbar.AddUIComponent<UISlicedSprite>();
      uiSlicedSprite1.relativePosition = Vector2.zero;
      uiSlicedSprite1.autoSize = true;
      uiSlicedSprite1.size = uiSlicedSprite1.parent.size;
      uiSlicedSprite1.fillDirection = UIFillDirection.Vertical;
      uiSlicedSprite1.spriteName = "ScrollbarTrack";
      scrollbar.trackObject = uiSlicedSprite1;
      var uiSlicedSprite2 = uiSlicedSprite1.AddUIComponent<UISlicedSprite>();
      uiSlicedSprite2.relativePosition = Vector2.zero;
      uiSlicedSprite2.fillDirection = UIFillDirection.Vertical;
      uiSlicedSprite2.autoSize = true;
      uiSlicedSprite2.width = uiSlicedSprite2.parent.width - 4f;
      uiSlicedSprite2.spriteName = "ScrollbarThumb";
      scrollbar.thumbObject = uiSlicedSprite2;
      _scrollablePanel.verticalScrollbar = scrollbar;
      _scrollablePanel.eventMouseWheel += (_, param) => _scrollablePanel.scrollPosition += new Vector2(0.0f, Mathf.Sign(param.wheelDelta) * -1f * scrollbar.incrementAmount);
    }
  }
}
