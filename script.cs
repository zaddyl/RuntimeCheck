//Grid Initialization
//This script will auto-save specified entitys to customdata for calling;
//Inludes thuster, battery, o2tank, h2tank, block that has inventory, antenna, beacon, power-generator, gas-generator, gyro
//plz enter follow module's name after first run:
//1.The connector for parking
//2.The timer-block for counting down
//Then set timer-block's action to run this pb with argument "Start"
//And if run this script with "DockOn", it'll set battery to "Recharge", thruster to "turn-off", beacon to "turn-off", gyro to "turn-off" and vice versa for argument "Dockoff".
//DO NOT MOVE your grid when initializing
//Script will disconnect all of the connector then re-connect after 1 sec

List<IMyThrust> ts = new List<IMyThrust>();
List<IMyBatteryBlock> pwr = new List<IMyBatteryBlock>();
List<IMyGasTank> gasTank = new List<IMyGasTank>();
List<IMyTerminalBlock> inv = new List<IMyTerminalBlock>();
List<IMyRadioAntenna> anT = new List<IMyRadioAntenna>();
List<IMyBeacon> beaC = new List<IMyBeacon>();
List<IMyPowerProducer> pwrProduc = new List<IMyPowerProducer>();
List<IMyGasGenerator> gasGen = new List<IMyGasGenerator>();
List<IMyGyro> gyRo = new List<IMyGyro>();
List<IMyTerminalBlock> all = new List<IMyTerminalBlock>();
IMyProgrammableBlock dockGroup;
MyIni _ini = new MyIni();
string errTxt = "";
List<IMyShipConnector> otherConnect = new List<IMyShipConnector>();
IMyShipConnector mainConnect;

public Program(){
    _ini.Clear();
    _ini.TryParse(Me.CustomData);
    scriptDisplay("Grid Config");
    setStat(true);
    mainConnect = GridTerminalSystem.GetBlockWithId(NameToId("MainConnect", "Connector")) as IMyShipConnector;
    dockGroup = GridTerminalSystem.GetBlockWithId(NameToId("MainConnect", "Connector")) as IMyProgrammableBlock;
    if (mainConnect == null) { errTxt += "Need Main-connect name to prevent setting error."; return; }
    List<IMyShipConnector> allConnect = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(allConnect);
    foreach (var c in allConnect)
    { if(c.CustomName != mainConnect.CustomName) { otherConnect.Add(c); } }
    errTxt += "Run the script to initialize."; Echo(errTxt);
}

public void Main(string argument) {
    errTxt = "";
    if (argument == "") { errTxt += "Waiting for reconnect...\n"; foreach (var c in otherConnect) { c.Disconnect(); }  mainConnect.Disconnect(); TB.Trigger(); }
    else if (argument == "Start")
    {
        _ini.Clear(); _ini.TryParse(Me.CustomData);
        delKeys("Battery");
        GridTerminalSystem.GetBlocksOfType(pwr);
        if (pwr.Count != 0)
        { foreach (var p in pwr) { _ini.Set("Battery", p.CustomName, p.EntityId); } }
        delKeys("O2Tank");
        GridTerminalSystem.GetBlocksOfType(gasTank);
        if (gasTank.Count != 0)
        { foreach (var o in gasTank) { if (o.BlockDefinition.SubtypeId.Contains("Oxygen") || o.BlockDefinition.SubtypeId == "") { _ini.Set("O2Tank", o.CustomName, o.EntityId); } } }
        delKeys("H2Tank");
        GridTerminalSystem.GetBlocksOfType(gasTank);
        if (gasTank.Count != 0)
        { foreach (var h in gasTank) { if (h.BlockDefinition.SubtypeId.Contains("Hydrogen")) { _ini.Set("H2Tank", h.CustomName, h.EntityId); } } }
        delKeys("Thruster");
        GridTerminalSystem.GetBlocksOfType(ts);
        if (ts.Count != 0)
        { foreach (var t in ts) { _ini.Set("Thruster", t.CustomName, t.EntityId); } }
        delKeys("Inventory");
        GridTerminalSystem.GetBlocksOfType(inv);
        if (inv.Count != 0)
        { foreach (var i in inv) { if (i.HasInventory) { _ini.Set("Inventory", i.CustomName, i.EntityId); } } }
        delKeys("RadioAntenna");
        GridTerminalSystem.GetBlocksOfType(anT);
        if (anT.Count != 0)
        { foreach (var a in anT) { _ini.Set("RadioAntenna", a.CustomName, a.EntityId); } }
        delKeys("Beacon");
        GridTerminalSystem.GetBlocksOfType(beaC);
        if (beaC.Count != 0)
        { foreach (var b in beaC) { _ini.Set("Beacon", b.CustomName, b.EntityId); } }
        delKeys("PWRProducer");
        GridTerminalSystem.GetBlocksOfType(pwrProduc);
        if (pwrProduc.Count != 0)
        { foreach (var p in pwrProduc) { if (!p.BlockDefinition.TypeIdString.Contains("Battery")) { _ini.Set("PWRProducer", p.CustomName, p.EntityId); } } }
        delKeys("GasGenerator");
        GridTerminalSystem.GetBlocksOfType(gasGen);
        if (gasGen.Count != 0)
        { foreach (var g in gasGen) { _ini.Set("GasGenerator", g.CustomName, g.EntityId); } }
        delKeys("Gyro");
        GridTerminalSystem.GetBlocksOfType(gyRo);
        if (gyRo.Count != 0)
        { foreach (var g in gyRo) { _ini.Set("Gyro", g.CustomName, g.EntityId); } }
        Me.CustomData = _ini.ToString();
        mainConnect.Connect();
        foreach (var c in otherConnect) { c.Connect(); }
        errTxt += "Initialization completed.";
    }
    else if (argument == "DockOn")
    {
        foreach (var key in getBlocks("Thruster")) { IMyThrust t = GridTerminalSystem.GetBlockWithId(key) as IMyThrust; t.Enabled = false; }
        foreach (var key in getBlocks("Battery")) { IMyBatteryBlock b = GridTerminalSystem.GetBlockWithId(key) as IMyBatteryBlock; b.ChargeMode = ChargeMode.Recharge; }
        foreach (var key in getBlocks("Beacon")) { IMyBeacon b = GridTerminalSystem.GetBlockWithId(key) as IMyBeacon; b.Enabled = false; }
        foreach (var key in getBlocks("Gyro")) { IMyGyro g = GridTerminalSystem.GetBlockWithId(key) as IMyGyro; g.Enabled = false; }
        errTxt += "Dock-on setting applied.";
    }
    else if (argument == "DockOff") {
        foreach (var key in getBlocks("Thruster")) { IMyThrust t = GridTerminalSystem.GetBlockWithId(key) as IMyThrust; t.Enabled = true; }
        foreach (var key in getBlocks("Battery")) { IMyBatteryBlock b = GridTerminalSystem.GetBlockWithId(key) as IMyBatteryBlock; b.ChargeMode = ChargeMode.Auto; }
        foreach (var key in getBlocks("Beacon")) { IMyBeacon b = GridTerminalSystem.GetBlockWithId(key) as IMyBeacon; b.Enabled = true; }
        foreach (var key in getBlocks("Gyro")) { IMyGyro g = GridTerminalSystem.GetBlockWithId(key) as IMyGyro; g.Enabled = true; }
        errTxt += "Dock-off setting applied.";
    }
    Echo(errTxt);
}

List<long> getBlocks(string section)
{
    List<long> keyls = new List<long>();
    _ini.TryParse(Me.CustomData);
    List<MyIniKey> Keys = new List<MyIniKey>(); _ini.GetKeys(section, Keys);
    if (!_ini.ContainsSection(section)) { return keyls; }
    foreach (var key in Keys)
    {
        keyls.Add(_ini.Get(key).ToInt64());
    }
    return keyls;
}

long NameToId(string section, string key)
{
    long entityId;
    string entityName;
    if (!_ini.Get(section, key).TryGetInt64(out entityId))
    {
        if (!_ini.Get(section, key).TryGetString(out entityName))
        {
            _ini.Set(section, key, "[Enter its name.]");
            Me.CustomData = _ini.ToString();
            errTxt += $"Error: cant get {key}'s value,\nPlz re-enter its name then reset the script.";
            Echo(errTxt + "\n");
            return 0;
        }
        IMyTerminalBlock entity = GridTerminalSystem.GetBlockWithName(_ini.Get(section, key).ToString());
        if (entity == null)
        {
            errTxt += $"Error: the {key} called {_ini.Get(section, key).ToString()} is not found.";
            Echo(errTxt + "\n");
        }
        _ini.Set(section, key, entity.EntityId.ToString());
        Me.CustomData = _ini.ToString();
    }
    return entityId;
}

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

void scriptDisplay(string scriptTitle)
{
    Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(1).FontSize = 4.1f;
    Me.GetSurface(1).FontColor = new Color(255, 255, 0);
    Me.GetSurface(1).Alignment = TextAlignment.CENTER;
    Me.GetSurface(1).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(1).WriteText("\n" + scriptTitle);
}

void delKeys(string section)
{
    List<MyIniKey> keys = new List<MyIniKey>();
    _ini.GetKeys(section, keys);
    foreach (var key in keys) { _ini.Delete(key); }
}