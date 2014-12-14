GreenDC-Job Schedule
====================

### Scheduler Type
* FirstFitScheduler: schedule job to the first fit position
* BestFitScheduler: schedule job to the best fit position
	


### Project 1: [CallLINGO](./CallLingo)
- Objective: create a LINGO caller which could be used to run LINGO file `.lng`.
- How to compile
	- download LINDO API, and released the `DLL` files to `bin`
	- compile this project to produce executable file in `bin`
- How to use
	- step 1: put executable file and dll file in a folder, will be pinpoint to in the usage
	- step 2: use the following command to execute your lingo program, assume your compiled CallLingo.exe is in bin folder
	```
	bin\CallLingo.exe offline-opt.lng setting.txt
	```
- Note: we didn't use this program to call LINGO solver automatically due to the model size constraints of LINGO API. We didn't upgrade LINGO version to get rid of the constrains since it is expensive.
	
### Project 2: [LibGreenDC](./LibGreenDC)
- Objective: provider the implementation of FirstFit and BestFit

### Project 3: [Simulation](./Simulation)
- Objective: provide an APi to call either FirstFit and BestFit to schedule a set of jobs given a certain setting, and output the schedule result
- How to use
You need to specify the OUTPUT_PATH where you would like to put the output data, and the scheduler you want to call
```
Simulation.exe OUTPUT-PATH SCHEDULER
```

- Note
	- We use python scripts to run repeated simulations, as shown in [runSim.py](./scripts/runSim.py)




### How to run?
- compile to exe, then call by [scripts](../scripts)


