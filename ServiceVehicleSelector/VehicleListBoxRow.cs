// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.VehicleListBoxRow
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework.UI;
using UnityEngine;

namespace ServiceVehicleSelector2
{
  public class VehicleListBoxRow : UIPanel
  {
    private UILabel _label;
    private PrefabData _prefab;
    private bool _isSelected;

    public UIFont Font { get; set; }

    public PrefabData Prefab
    {
      get
      {
        return this._prefab;
      }
      set
      {
        this._prefab = value;
      }
    }

    public bool IsSelected
    {
      get
      {
        return this._isSelected;
      }
      set
      {
        this._isSelected = value;
        if (this._isSelected)
          this.backgroundSprite = "ListItemHighlight";
        else
          this.backgroundSprite = "";
      }
    }

    protected override void OnMouseEnter(UIMouseEventParameter p)
    {
      if (!this.IsSelected)
        this.backgroundSprite = "ListItemHover";
      base.OnMouseEnter(p);
    }

    protected override void OnMouseLeave(UIMouseEventParameter p)
    {
      if (!this.IsSelected)
        this.backgroundSprite = "";
      base.OnMouseLeave(p);
    }

    public override void Start()
    {
      base.Start();
      this.width = this.parent.width;
      this.height = 27f;
      this.autoLayoutDirection = LayoutDirection.Horizontal;
      this.autoLayoutStart = LayoutStart.TopLeft;
      this.autoLayoutPadding = new RectOffset(4, 0, 0, 0);
      this.autoLayout = true;
      this._label = this.AddUIComponent<UILabel>();
      this._label.textScale = 0.8f;
      this._label.font = this.Font;
      this._label.autoSize = false;
      this._label.height = this.height;
      this._label.width = this.width - (float) this.autoLayoutPadding.left;
      this._label.verticalAlignment = UIVerticalAlignment.Middle;
      if (!Utils.Truncate(this._label, this._prefab.Title, "…"))
        return;
      this._label.tooltip = this._prefab.Title;
    }

    public override void OnDestroy()
    {
      if ((Object) this._label != (Object) null)
        Object.Destroy((Object) this._label.gameObject);
      base.OnDestroy();
    }
  }
}
