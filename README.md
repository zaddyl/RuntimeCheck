# ShipInitialization_for_SpaceEngineers
Grid Initialization  
This script will auto-save specified entitys to customdata for calling;  
Inludes thuster, battery, o2tank, h2tank, block that has inventory, antenna, beacon, power-generator, gas-generator, gyro  
plz enter follow module's name after first run:  
1.The connector for parking  
2.The timer-block for counting down  
Then set timer-block's action to run this pb with argument "Start"  
And if run this script with "DockOn", it'll set battery to "Recharge", thruster to "turn-off", beacon to "turn-off", gyro to "turn-off" and vice versa for argument "Dockoff".  
DO NOT MOVE your grid when initializing  
Script will disconnect all of the connector then re-connect by async

網格初始化腳本，該腳本會將指定實體的ID存儲到自定義數據中  
目前包括：推進器、電池、氧氣罐、氫氣罐、庫存模塊、天線、信標、發電模塊、產氣模塊、陀螺儀  
請在首次運行後，于自定義數據中輸入如下方塊的名字：  
1、停泊用連接器  
2、用於計時重連的計時器  
再給用於的計時器設置動作——以“Start”參數運行該腳本  

使用“Dockon”參數運行此腳本，會讀取自定義數據中的conf，將電池組設為“充電”、推進器組設為“關閉”、信標設為“關閉”、陀螺儀也設為“關閉”；使用“Dockoff”反之  
請勿在初始化時移動網格，腳本會在執行時斷開所有連接器並異步重連

