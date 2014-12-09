GreenDC-Job Schedule
====================

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
	


### Scheduler
- scheduler type
	* FirstFitScheduler: schedule job to the first fit position
	* BestFitScheduler: schedule job to the best fit position
	


### How to run?



