// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.UIHelper
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework.UI;
using UnityEngine;

namespace ServiceVehicleSelector2
{
  public static class UIHelper
  {
    private static UIFont _font;

    public static UIFont Font
    {
      get
      {
        if ((Object) UIHelper._font != (Object) null)
          return UIHelper._font;
        UIHelper._font = GameObject.Find("(Library) PublicTransportInfoViewPanel").GetComponent<PublicTransportInfoViewPanel>().Find<UILabel>("Label").font;
        return UIHelper._font;
      }
    }

    public static bool IsFullyClippedFromParent(UIComponent component)
    {
      if ((Object) component.parent == (Object) null || (Object) component.parent == (Object) component)
        return false;
      UIScrollablePanel parent = component.parent as UIScrollablePanel;
      return (Object) parent != (Object) null && parent.clipChildren && ((double) component.relativePosition.x < 0.0 - (double) component.size.x - 1.0 || (double) component.relativePosition.x + (double) component.size.x > (double) component.parent.size.x + (double) component.size.x + 1.0 || ((double) component.relativePosition.y < 0.0 - (double) component.size.y - 1.0 || (double) component.relativePosition.y + (double) component.size.y > (double) component.parent.size.y + (double) component.size.y + 1.0));
    }
    
        public static UIDropDown CreateDropDown(UIComponent parent)
        {
            UIDropDown dropDown = parent.AddUIComponent<UIDropDown>();
            dropDown.size = new Vector2(90f, 30f);
            dropDown.listBackground = "GenericPanelLight";
            dropDown.itemHeight = 30;
            dropDown.itemHover = "ListItemHover";
            dropDown.itemHighlight = "ListItemHighlight";
            dropDown.normalBgSprite = "ButtonMenu";
            dropDown.disabledBgSprite = "ButtonMenuDisabled";
            dropDown.hoveredBgSprite = "ButtonMenuHovered";
            dropDown.focusedBgSprite = "ButtonMenu";
            dropDown.listWidth = 90;
            dropDown.listHeight = 500;
            dropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            dropDown.popupColor = new Color32(45, 52, 61, 255);
            dropDown.popupTextColor = new Color32(170, 170, 170, 255);
            dropDown.zOrder = 1;
            dropDown.textScale = 0.8f;
            dropDown.verticalAlignment = UIVerticalAlignment.Middle;
            dropDown.horizontalAlignment = UIHorizontalAlignment.Left;
            dropDown.selectedIndex = 0;
            dropDown.textFieldPadding = new RectOffset(8, 0, 8, 0);
            dropDown.itemPadding = new RectOffset(14, 0, 8, 0);

            UIButton button = dropDown.AddUIComponent<UIButton>();
            dropDown.triggerButton = button;
            button.text = "";
            button.size = dropDown.size;
            button.relativePosition = new Vector3(0f, 0f);
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textHorizontalAlignment = UIHorizontalAlignment.Left;
            button.normalFgSprite = "IconDownArrow";
            button.hoveredFgSprite = "IconDownArrowHovered";
            button.pressedFgSprite = "IconDownArrowPressed";
            button.focusedFgSprite = "IconDownArrowFocused";
            button.disabledFgSprite = "IconDownArrowDisabled";
            button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            button.horizontalAlignment = UIHorizontalAlignment.Right;
            button.verticalAlignment = UIVerticalAlignment.Middle;
            button.zOrder = 0;
            button.textScale = 0.8f;

            dropDown.eventSizeChanged += new PropertyChangedEventHandler<Vector2>((c, t) =>
            {
                button.size = t; dropDown.listWidth = (int)t.x;
            });

            return dropDown;
        }

    public static UIButton CreateButton(UIComponent parent)
    {
      UIButton uiButton = parent.AddUIComponent<UIButton>();
      UIFont font = UIHelper.Font;
      uiButton.font = font;
      RectOffset rectOffset = new RectOffset(0, 0, 4, 0);
      uiButton.textPadding = rectOffset;
      string str1 = "ButtonMenu";
      uiButton.normalBgSprite = str1;
      string str2 = "ButtonMenuDisabled";
      uiButton.disabledBgSprite = str2;
      string str3 = "ButtonMenuHovered";
      uiButton.hoveredBgSprite = str3;
      string str4 = "ButtonMenu";
      uiButton.focusedBgSprite = str4;
      string str5 = "ButtonMenuPressed";
      uiButton.pressedBgSprite = str5;
      Color32 color32_1 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      uiButton.textColor = color32_1;
      Color32 color32_2 = new Color32((byte) 7, (byte) 7, (byte) 7, byte.MaxValue);
      uiButton.disabledTextColor = color32_2;
      Color32 color32_3 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      uiButton.hoveredTextColor = color32_3;
      Color32 color32_4 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      uiButton.focusedTextColor = color32_4;
      Color32 color32_5 = new Color32((byte) 30, (byte) 30, (byte) 44, byte.MaxValue);
      uiButton.pressedTextColor = color32_5;
      return uiButton;
    }
  }
}
