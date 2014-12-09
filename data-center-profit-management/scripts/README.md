## Scripts

### Shared Tool Functions
|File| Contents|
|:----|:-------|
|[generate-setting.py](./generateSetting.py)| generate problem setting to folder [data](../data), act as input for simulation|
|[pystats.py](./pystats.py)|  [Python statistic Tool](https://github.com/xizhonghua/pystats),process repetition results to get mean, variances, confidential interval etc|

### Simulation: Compare FirstFit, BestFit
|File| Contents|
|:----|:-------|
|[processSim.py](./processSim.py)| process simulation result produced by [runSim.py](./runSim.py)|
|[runSim.py](./runSim.py)| Simulations to evaluate FirstFit and BestFit, simulation with input from [data](../data), output to [result](../result)


### Simulation: Compare OPT, FirstFit, BestFit
|File| Contents|
|:----|:-------|
|[oneOPTSimulation.py](./oneOPTSimulation.py)| Simulations to compare OPT, FirstFit and BestFit| 
|[processAvgOPT.py](./processAvgOPT.py)| Process the result of [oneOPTSimulation.py](./oneOPTSimulation.py) to get the average profit of OPT, FirstFit and BestFit|
|[copyFileSettingToLingo.bat](./copyFileSettingToLingo.bat)| copy settings from ./data folder to lingo-opt folder, since Lingo requires the setting files to in the execution directory|

### Input Date
|File| Contents|
|:----|:-------|
|[realSolars.txt](./realSolars.txt)| Real solar trace from UMass Weather Station, acts as solar traces input of [generate-setting.py](./generate-setting.py)|






