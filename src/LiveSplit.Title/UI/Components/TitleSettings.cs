using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components;

public enum TitleHeightMode { Auto, Custom }
public enum CounterVerticalAlignment { Top, Center, Bottom }

public partial class TitleSettings : UserControl
{
    private ComboBox heightModeControl;
    private NumericUpDown customHeightControl;
    private ComboBox counterAlignmentControl;
    private CheckBox overrideTitleFontControl;
    private Label titleFontLabel;
    private Button titleFontButton;
    private CheckBox overrideCounterFontControl;
    private Label counterFontLabel;
    private Button counterFontButton;
    public bool ShowGameName { get; set; }
    public bool ShowCategoryName { get; set; }
    public bool ShowAttemptCount { get; set; }
    public bool ShowFinishedRunsCount { get; set; }
    public bool ShowCount => ShowAttemptCount || ShowFinishedRunsCount;
    public AlignmentType TextAlignment { get; set; }
    public bool SingleLine { get; set; }
    public bool DisplayGameIcon { get; set; }
    public TitleHeightMode HeightMode { get; set; }
    public float CustomHeight { get; set; }
    public bool OverrideCounterFont { get; set; }
    public Font CounterFont { get; set; }
    public CounterVerticalAlignment CounterVerticalAlignment { get; set; }

    public bool ShowRegion { get; set; }
    public bool ShowPlatform { get; set; }
    public bool ShowVariables { get; set; }

    public Color TitleColor { get; set; }
    public bool OverrideTitleColor { get; set; }

    // Legacy font override — read from old configs, not written or exposed in UI
    public bool OverrideTitleFont { get; set; }
    public Font TitleFont { get; set; }

    public Color BackgroundColor { get; set; }
    public Color BackgroundColor2 { get; set; }
    public GradientType BackgroundGradient { get; set; }
    public string GradientString
    {
        get => BackgroundGradient.ToString();
        set => BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value);
    }

    public TitleSettings()
    {
        InitializeComponent();
        ShowGameName = true;
        ShowCategoryName = true;
        ShowAttemptCount = true;
        ShowFinishedRunsCount = false;
        DisplayGameIcon = true;
        TitleColor = Color.FromArgb(255, 255, 255, 255);
        OverrideTitleColor = false;
        TitleFont = (Font)SystemFonts.DefaultFont.Clone();
        SingleLine = false;
        HeightMode = TitleHeightMode.Auto;
        CustomHeight = 32;
        CounterFont = (Font)SystemFonts.DefaultFont.Clone();
        CounterVerticalAlignment = LiveSplit.UI.Components.CounterVerticalAlignment.Center;
        ShowRegion = false;
        ShowPlatform = false;
        ShowVariables = true;
        BackgroundColor = Color.FromArgb(255, 42, 42, 42);
        BackgroundColor2 = Color.FromArgb(255, 19, 19, 19);
        BackgroundGradient = GradientType.Vertical;
        TextAlignment = AlignmentType.Auto;

        chkGameName.DataBindings.Add("Checked", this, "ShowGameName", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCategoryName.DataBindings.Add("Checked", this, "ShowCategoryName", false, DataSourceUpdateMode.OnPropertyChanged);
        chkAttemptCount.DataBindings.Add("Checked", this, "ShowAttemptCount", false, DataSourceUpdateMode.OnPropertyChanged);
        chkFinishedRuns.DataBindings.Add("Checked", this, "ShowFinishedRunsCount", false, DataSourceUpdateMode.OnPropertyChanged);
        chkColor.DataBindings.Add("Checked", this, "OverrideTitleColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSingleLine.DataBindings.Add("Checked", this, "SingleLine", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor.DataBindings.Add("BackColor", this, "TitleColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkDisplayGameIcon.DataBindings.Add("Checked", this, "DisplayGameIcon", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRegion.DataBindings.Add("Checked", this, "ShowRegion", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlatform.DataBindings.Add("Checked", this, "ShowPlatform", false, DataSourceUpdateMode.OnPropertyChanged);
        chkVariables.DataBindings.Add("Checked", this, "ShowVariables", false, DataSourceUpdateMode.OnPropertyChanged);
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true };
        var heightPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = true };
        heightModeControl = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 85 };
        heightModeControl.Items.AddRange(new object[] { "Auto", "Custom" }); heightModeControl.SelectedIndex = 0;
        heightModeControl.SelectedIndexChanged += (_, _) => HeightMode = (TitleHeightMode)heightModeControl.SelectedIndex;
        customHeightControl = new NumericUpDown { Minimum = 1, Maximum = 1000, Width = 60, Value = 32 };
        customHeightControl.ValueChanged += (_, _) => CustomHeight = (float)customHeightControl.Value;
        counterAlignmentControl = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 80 };
        counterAlignmentControl.Items.AddRange(new object[] { "Top", "Center", "Bottom" }); counterAlignmentControl.SelectedIndex = 1;
        counterAlignmentControl.SelectedIndexChanged += (_, _) => CounterVerticalAlignment = (CounterVerticalAlignment)counterAlignmentControl.SelectedIndex;
        heightPanel.Controls.Add(new Label { Text = "Height Mode", AutoSize = true }); heightPanel.Controls.Add(heightModeControl);
        heightPanel.Controls.Add(new Label { Text = "Custom Height", AutoSize = true }); heightPanel.Controls.Add(customHeightControl);
        heightPanel.Controls.Add(new Label { Text = "Counter Vertical Alignment", AutoSize = true }); heightPanel.Controls.Add(counterAlignmentControl);
        panel.Controls.Add(heightPanel);

        overrideTitleFontControl = new CheckBox { Text = "Override Layout Settings", AutoSize = true };
        titleFontLabel = new Label { Text = SettingsHelper.FormatFont(TitleFont), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Right };
        titleFontButton = new Button { Text = "Choose...", AutoSize = true };
        overrideTitleFontControl.CheckedChanged += (_, _) => { OverrideTitleFont = overrideTitleFontControl.Checked; titleFontLabel.Enabled = titleFontButton.Enabled = overrideTitleFontControl.Checked; };
        titleFontButton.Click += (_, _) => SelectFont(TitleFont, 11, 26, font => { TitleFont = font; titleFontLabel.Text = SettingsHelper.FormatFont(TitleFont); });
        panel.Controls.Add(CreateFontGroup("Title Font", overrideTitleFontControl, titleFontLabel, titleFontButton));

        overrideCounterFontControl = new CheckBox { Text = "Override Setting", AutoSize = true };
        counterFontLabel = new Label { Text = SettingsHelper.FormatFont(CounterFont), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Right };
        counterFontButton = new Button { Text = "Choose...", AutoSize = true };
        overrideCounterFontControl.CheckedChanged += (_, _) => { OverrideCounterFont = overrideCounterFontControl.Checked; counterFontLabel.Enabled = counterFontButton.Enabled = overrideCounterFontControl.Checked; };
        counterFontButton.Click += (_, _) => SelectFont(CounterFont, 11, 26, font => { CounterFont = font; counterFontLabel.Text = SettingsHelper.FormatFont(CounterFont); });
        panel.Controls.Add(CreateFontGroup("Counter Font", overrideCounterFontControl, counterFontLabel, counterFontButton));
        tableLayoutPanel1.RowCount = 8;
        tableLayoutPanel1.RowStyles[6].Height = 82F;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 245F));
        tableLayoutPanel1.Controls.Add(panel, 0, 7); tableLayoutPanel1.SetColumnSpan(panel, 4);
        Size = new Size(Width, Height + 307);
    }

    private static GroupBox CreateFontGroup(string title, CheckBox overrideControl, Label fontLabel, Button fontButton)
    {
        var group = new GroupBox { Text = title, Width = 440, Height = 76 };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2, Padding = new Padding(3) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 81F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
        layout.SetColumnSpan(overrideControl, 3);
        layout.Controls.Add(overrideControl, 0, 0);
        layout.Controls.Add(new Label { Text = "Font:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        layout.Controls.Add(fontLabel, 1, 1);
        layout.Controls.Add(fontButton, 2, 1);
        group.Controls.Add(layout);
        return group;
    }

    private void SelectFont(Font currentFont, int minimumSize, int maximumSize, Action<Font> setFont)
    {
        CustomFontDialog.FontDialog dialog = SettingsHelper.GetFontDialog(currentFont, minimumSize, maximumSize);
        dialog.FontChanged += (_, e) => setFont(((CustomFontDialog.FontChangedEventArgs)e).NewFont);
        dialog.ShowDialog(this);
    }

    private void TitleSettings_Load(object sender, EventArgs e)
    {
        chkColor_CheckedChanged(null, null);
        cmbTextAlignment.SelectedIndex = (int)TextAlignment;
    }

    private void chkColor_CheckedChanged(object sender, EventArgs e)
    {
        label3.Enabled = btnColor.Enabled = chkColor.Checked;
    }

    private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
        btnColor2.DataBindings.Clear();
        btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        GradientString = cmbGradientType.SelectedItem.ToString();
    }

    private void cmbTextAlignment_SelectedIndexChanged(object sender, EventArgs e)
    {
        TextAlignment = (AlignmentType)cmbTextAlignment.SelectedIndex;
    }

    public void SetSettings(XmlNode node)
    {
        var element = (XmlElement)node;
        Version version = SettingsHelper.ParseVersion(element["Version"]);
        DisplayGameIcon = SettingsHelper.ParseBool(element["DisplayGameIcon"], true);

        if (version >= new Version(1, 2))
        {
            TitleFont = SettingsHelper.GetFontFromElement(element["TitleFont"]) ?? TitleFont;
            if (version >= new Version(1, 3))
            {
                OverrideTitleFont = SettingsHelper.ParseBool(element["OverrideTitleFont"]);
                TextAlignment = version >= new Version(1, 7, 3)
                    ? (AlignmentType)SettingsHelper.ParseInt(element["TextAlignment"], 0)
                    : DisplayGameIcon && SettingsHelper.ParseBool(element["CenterTitle"], false)
                        ? AlignmentType.Center
                        : AlignmentType.Auto;
            }
            else
            {
                OverrideTitleFont = !SettingsHelper.ParseBool(element["UseLayoutSettingsFont"]);
            }
        }

        ShowGameName = SettingsHelper.ParseBool(element["ShowGameName"], true);
        ShowCategoryName = SettingsHelper.ParseBool(element["ShowCategoryName"], true);
        ShowAttemptCount = SettingsHelper.ParseBool(element["ShowAttemptCount"]);
        TitleColor = SettingsHelper.ParseColor(element["TitleColor"], Color.FromArgb(255, 255, 255, 255));
        OverrideTitleColor = SettingsHelper.ParseBool(element["OverrideTitleColor"], false);
        BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.FromArgb(42, 42, 42, 255));
        BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.FromArgb(19, 19, 19, 255));
        GradientString = SettingsHelper.ParseString(element["BackgroundGradient"], GradientType.Vertical.ToString());
        ShowFinishedRunsCount = SettingsHelper.ParseBool(element["ShowFinishedRunsCount"], false);
        SingleLine = SettingsHelper.ParseBool(element["SingleLine"], false);
        ShowRegion = SettingsHelper.ParseBool(element["ShowRegion"], false);
        ShowPlatform = SettingsHelper.ParseBool(element["ShowPlatform"], false);
        ShowVariables = SettingsHelper.ParseBool(element["ShowVariables"], true);
        HeightMode = Enum.TryParse<TitleHeightMode>(SettingsHelper.ParseString(element["HeightMode"], "Auto"), out TitleHeightMode parsedMode) ? parsedMode : TitleHeightMode.Auto;
        float.TryParse(SettingsHelper.ParseString(element["CustomHeight"], "32"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsedHeight);
        CustomHeight = parsedHeight > 0 ? parsedHeight : 32;
        OverrideCounterFont = SettingsHelper.ParseBool(element["OverrideCounterFont"], false);
        CounterFont = SettingsHelper.GetFontFromElement(element["CounterFont"]) ?? CounterFont;
        CounterVerticalAlignment = Enum.TryParse(SettingsHelper.ParseString(element["CounterVerticalAlignment"], "Center"), out CounterVerticalAlignment parsedAlignment) ? parsedAlignment : LiveSplit.UI.Components.CounterVerticalAlignment.Center;
        heightModeControl.SelectedIndex = (int)HeightMode;
        customHeightControl.Value = (decimal)Math.Max(1, Math.Min(1000, CustomHeight));
        counterAlignmentControl.SelectedIndex = (int)CounterVerticalAlignment;
        overrideTitleFontControl.Checked = OverrideTitleFont;
        overrideCounterFontControl.Checked = OverrideCounterFont;
        titleFontLabel.Text = SettingsHelper.FormatFont(TitleFont);
        counterFontLabel.Text = SettingsHelper.FormatFont(CounterFont);
    }

    public XmlNode GetSettings(XmlDocument document)
    {
        XmlElement parent = document.CreateElement("Settings");
        CreateSettingsNode(document, parent);
        return parent;
    }

    public int GetSettingsHashCode()
    {
        return CreateSettingsNode(null, null);
    }

    private int CreateSettingsNode(XmlDocument document, XmlElement parent)
    {
        return SettingsHelper.CreateSetting(document, parent, "Version", "1.7.3") ^
        SettingsHelper.CreateSetting(document, parent, "ShowGameName", ShowGameName) ^
        SettingsHelper.CreateSetting(document, parent, "ShowCategoryName", ShowCategoryName) ^
        SettingsHelper.CreateSetting(document, parent, "ShowAttemptCount", ShowAttemptCount) ^
        SettingsHelper.CreateSetting(document, parent, "ShowFinishedRunsCount", ShowFinishedRunsCount) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideTitleColor", OverrideTitleColor) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideTitleFont", OverrideTitleFont) ^
        SettingsHelper.CreateSetting(document, parent, "TitleFont", TitleFont) ^
        SettingsHelper.CreateSetting(document, parent, "SingleLine", SingleLine) ^
        SettingsHelper.CreateSetting(document, parent, "TitleColor", TitleColor) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
        SettingsHelper.CreateSetting(document, parent, "DisplayGameIcon", DisplayGameIcon) ^
        SettingsHelper.CreateSetting(document, parent, "ShowRegion", ShowRegion) ^
        SettingsHelper.CreateSetting(document, parent, "ShowPlatform", ShowPlatform) ^
        SettingsHelper.CreateSetting(document, parent, "ShowVariables", ShowVariables) ^
        SettingsHelper.CreateSetting(document, parent, "TextAlignment", (int)TextAlignment) ^
        SettingsHelper.CreateSetting(document, parent, "HeightMode", HeightMode) ^
        SettingsHelper.CreateSetting(document, parent, "CustomHeight", CustomHeight) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideCounterFont", OverrideCounterFont) ^
        SettingsHelper.CreateSetting(document, parent, "CounterVerticalAlignment", CounterVerticalAlignment) ^
        SettingsHelper.CreateSetting(document, parent, "CounterFont", CounterFont);
    }

    private void ColorButtonClick(object sender, EventArgs e)
    {
        SettingsHelper.ColorButtonClick((Button)sender, this);
    }
}
