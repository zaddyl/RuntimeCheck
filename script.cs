//This script's for monitoring every system which's registered.
//Just give the script which you wanna check a timer block to hold in triggering

IMyTextSurface Dis;
float DisX;
float DisY;
MyIni _ini = new MyIni();
string errTxt = "";
MySpriteDrawFrame _F;

Program(){
    iniInitialization();
    _ini.TryParse(Me.CustomData);
    Dis = GridTerminalSystem.GetBlockWithId(NameToId("MainDisplay", "LCD")) as IMyTextSurface;
    if (Dis == null) { errTxt += "LCD's not found.\n\n"; Echo(errTxt); return; }
    Dis.ContentType = ContentType.SCRIPT;
    scriptDisplay("Runtime Check");
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

void Main(string argument)
{
    errTxt = ""; _F = Dis.DrawFrame();
    _F.Add(drawShape("SquareSimple", new Vector2(Dis.SurfaceSize.X / 2, Dis.SurfaceSize.Y / 2), Dis.SurfaceSize, Color.Black));
    _ini.TryParse(Me.CustomData);
    if (argument != "")
    {
        string[] arguments = argument.Split(',');
        string scriptName = arguments[0];
        string timerId = arguments[1];
        _ini.Set("Systems", timerId, scriptName);
        Me.CustomData = _ini.ToString();
    }
    _ini.TryParse(Me.CustomData);
    List<MyIniKey> pbInfos = new List<MyIniKey>();
    _ini.GetKeys("Systems", pbInfos);
    int i = 0; int j = 0;
    foreach (var pbId in pbInfos)
    {
        DisX = Dis.SurfaceSize.X; DisY = Dis.SurfaceSize.Y;
        string pbStatTxt = "Offline";
        Color pbStatColor = Color.Red;
        IMyTimerBlock tb = GridTerminalSystem.GetBlockWithId(long.Parse(pbId.Name)) as IMyTimerBlock;
        if (tb.IsCountingDown) {
            pbStatTxt = "Online";
            pbStatColor = Color.Green;
        }
        _F.Add(drawText(_ini.Get(pbId).ToString(), new Vector2((DisX - 20)*i/2 +10, (DisY - 20)*j / 6 + 10), 1.3f, Color.Yellow, TextAlignment.LEFT));
        _F.Add(drawText(pbStatTxt, new Vector2((DisX - 20)*i / 2 + 10, (DisY - 20)*j / 6 + 50), 1.3f, pbStatColor, TextAlignment.LEFT));
        i++;
        if (i == 2) { i = 0; j++; }
    }
    errTxt += "Script's running.";
    Echo(errTxt);
    _F.Dispose();
}

//绘制形状
MySprite drawShape(string type, Vector2 position, Vector2 size, Color color, TextAlignment alignment = TextAlignment.CENTER, float rotationScale = 0)
{ var Sprite = MySprite.CreateSprite(type, position, size); Sprite.Color = color; Sprite.Alignment = alignment; Sprite.RotationOrScale = MathHelper.ToRadians(rotationScale); return Sprite; }

//绘制文字
MySprite drawText(string content, Vector2 position, float size, Color color, TextAlignment alignment = TextAlignment.CENTER)
{ var Sprite = MySprite.CreateText(content, "Monospace", color, size, alignment); Sprite.Position = position; return Sprite; }

//转义模块
//结合例：
//Dis = GridTerminalSystem.GetBlockWithId(NameToId("MainDisplay", "LCD")) as IMyTextSurface;
//if (Dis == null) { errTxt += "LCD's not found.\n\n"; Echo(errTxt); return; }
long NameToId(string section, string key)
{
    _ini.TryParse(Me.CustomData);
    long entityId;
    string entityName;
    if (!_ini.Get(section, key).TryGetInt64(out entityId))
    {
        if(!_ini.Get(section, key).TryGetString(out entityName)){
            _ini.Set(section, key, "[Enter the name]");
            Me.CustomData = _ini.ToString();
            return 0;
        }
        IMyTerminalBlock entity = GridTerminalSystem.GetBlockWithName(_ini.Get(section, key).ToString());
        if (entity == null)
        {
            errTxt += $"Error: the {key} called {_ini.Get(section, key).ToString()} is not found.";
            Echo(errTxt + "\n\n");
            return 0;
        }
        _ini.Set(section, key, entity.EntityId.ToString());
        Me.CustomData = _ini.ToString();
        return entityId;
    }
    else { return entityId; }
}

//黏贴至其他PB使用，记得填写脚本名称并结合转义模块使用
void requestStat(string sysTitle)
{
    IMyProgrammableBlock statPB = GridTerminalSystem.GetBlockWithId(NameToId("ProgrammableBlock", "StateManager")) as IMyProgrammableBlock;
    if (statPB == null) { errTxt += "PB's not found.\n\n"; Echo(errTxt); return; }
    if (TB != null) { statPB.TryRun(sysTitle + "," + TB.EntityId.ToString()); }
}

//利用所关联的定时器方块进行检测，请黏贴至其他PB使用
IMyTimerBlock TB;
void setStat(bool i)
{
    if (i)
    { _ini.TryParse(Me.CustomData); TB = GridTerminalSystem.GetBlockWithId(NameToId("RunningStat", "TB")) as IMyTimerBlock; }
    else
    {
        if (TB == null)
        { errTxt += "Timer block not found,\nmay not detect script's stat."; }
        else
        { TB.TriggerDelay = 1; TB.Enabled = true; TB.StartCountdown(); }
    }
}

//PB画面初始化
void scriptDisplay(string scriptTitle)
{
    Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(1).FontSize = 4.1f;
    Me.GetSurface(1).FontColor = new Color(255, 255, 0);
    Me.GetSurface(1).Alignment = TextAlignment.CENTER;
    Me.GetSurface(1).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(1).WriteText("\n" + scriptTitle);
}

//主显LCD初始化
void iniInitialization()
{
    if (Me.CustomData == "")
    {
        _ini.Set("MainDisplay", "LCD", "[Enter the name.]");
        Me.CustomData = _ini.ToString();
        errTxt += "Configuring then reset this script to try again.";
        Echo(errTxt);
    }
}

//常用字库
Dictionary<string, string> trans = new Dictionary<string, string>(){
    {"Magnesium", "Mg"},
    {"Iron", "Fe"},
    {"Nickel", "Ne"},
    {"Cobalt", "Co"},
    {"Stone", "Gravel"},
    {"Gold", "Au"},
    {"Platinum", "Pt"},
    {"Silicon", "Si"},
    {"Silver", "Ag"},
    {"Uranium", "U"},
    {"Ice", "Ice"},
    {"LargeCalibreAmmo", "Artillery Shell"},
    {"MediumCalibreAmmo", "Assault Cannon Shell"},
    {"AutocannonClip", "Autocannon Magazine"},
    {"Missile200mm", "Rocket"},
    {"SmallRailgunAmmo", "Small Railgun Sabot"},
    {"LargeRailgunAmmo", "Large Railgun Sabot"},
    {"NATO_25x184mm", "Gatling Ammo Box"},
};