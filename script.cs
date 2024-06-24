//Grid Initialization
//This script will auto-save specified entitys to customdata for calling;
//Inludes thuster, battery, o2tank, h2tank, block that has inventory, antenna, beacon, power-generator, gas-generator, gyro
//plz enter follow module's name after first run:
//1.The connector for parking
//2.the timer-block for counting down
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
    if (mainConnect == null) { errTxt += "Need Main-connect name to prevent setting error."; return; }
    List<IMyShipConnector> allConnect = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(allConnect);
    foreach (var c in allConnect)
    { if(c.CustomName != mainConnect.CustomName) { otherConnect.Add(c); } }
    errTxt += "Run the script to initialize."; Echo(errTxt);
}

public void Main(string argument) {
    errTxt = "";
    if (argument == "") { errTxt += "Waiting for reconnect...\n"; mainConnect.Disconnect(); foreach (var c in otherConnect) { c.Disconnect(); } TB.StartCountdown(); Echo(errTxt); }
    else
    {
        _ini.Clear(); _ini.TryParse(Me.CustomData);
        _ini.DeleteSection("Battery");
        GridTerminalSystem.GetBlocksOfType(pwr);
        if (pwr.Count != 0)
        { foreach (var p in pwr) { _ini.Set("Battery", p.CustomName, p.EntityId); } }
        _ini.DeleteSection("O2Tank");
        GridTerminalSystem.GetBlocksOfType(gasTank);
        if (gasTank.Count != 0)
        { foreach (var o in gasTank) { if (o.BlockDefinition.SubtypeId.Contains("Oxygen") || o.BlockDefinition.SubtypeId == "") { _ini.Set("O2Tank", o.CustomName, o.EntityId); } } }
        _ini.DeleteSection("H2Tank");
        GridTerminalSystem.GetBlocksOfType(gasTank);
        if (gasTank.Count != 0)
        { foreach (var h in gasTank) { if (h.BlockDefinition.SubtypeId.Contains("Hydrogen")) { _ini.Set("H2Tank", h.CustomName, h.EntityId); } } }
        _ini.DeleteSection("Thruster");
        GridTerminalSystem.GetBlocksOfType(ts);
        if (ts.Count != 0)
        { foreach (var t in ts) { _ini.Set("Thruster", t.CustomName, t.EntityId); } }
        _ini.DeleteSection("Inventory");
        GridTerminalSystem.GetBlocksOfType(inv);
        if (inv.Count != 0)
        { foreach (var i in inv) { if (i.HasInventory) { _ini.Set("Inventory", i.CustomName, i.EntityId); } } }
        _ini.DeleteSection("RadioAntenna");
        GridTerminalSystem.GetBlocksOfType(anT);
        if (anT.Count != 0)
        { foreach (var a in anT) { _ini.Set("RadioAntenna", a.CustomName, a.EntityId); } }
        _ini.DeleteSection("Beacon");
        GridTerminalSystem.GetBlocksOfType(beaC);
        if (beaC.Count != 0)
        { foreach (var b in beaC) { _ini.Set("Beacon", b.CustomName, b.EntityId); } }
        _ini.DeleteSection("PWRProducer");
        GridTerminalSystem.GetBlocksOfType(pwrProduc);
        if (pwrProduc.Count != 0)
        { foreach (var p in pwrProduc) { if (!p.BlockDefinition.TypeIdString.Contains("Battery")) { _ini.Set("PWRProducer", p.CustomName, p.EntityId); } } }
        _ini.DeleteSection("GasGenerator");
        GridTerminalSystem.GetBlocksOfType(gasGen);
        if (gasGen.Count != 0)
        { foreach (var g in gasGen) { _ini.Set("GasGenerator", g.CustomName, g.EntityId); } }
        _ini.DeleteSection("Gyro");
        GridTerminalSystem.GetBlocksOfType(gyRo);
        if (gyRo.Count != 0)
        { foreach (var g in gyRo) { _ini.Set("Gyro", g.CustomName, g.EntityId); } }
        Me.CustomData = _ini.ToString();
        foreach (var c in otherConnect) { c.Connect(); }
        errTxt += "Initialization completed.";
        Echo(errTxt);
    }
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