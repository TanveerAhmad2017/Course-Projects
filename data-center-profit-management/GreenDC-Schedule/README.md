GreenDC-Job Schedule
====================

### Introduction


### Scheduler
- [code](./GreenDC/Scheduler)
- scheduler type
	* FirstFitScheduler: schedule job to the first fit position
	* BestFitScheduler: schedule job to the best fit position
	* RandomFitScheduler: schedule job to the random fit position
	* GreenSlotScheduler: in Igcc'11 paper, similar to Best-Fit, but punish job which is about to miss its deadline
	* DensityFitScheduler: if using First-Fit can satisfy critical profit density, then use First-Fit; otherwise try Best-Fit. If job fails to be scheduled due to low profit density, then discard this job, and adjust the critical density.


### Config
- `-p`: allow semiPreemptive, otherwise, do not allow
- `-opt`: compared with optimal
- `-sf`: save figure, otherwise, not save
- `-s [arg]`: arg is the start workload, e.g. `-s 1` means the start workload is 10%
- `-e [arg]`: arg is the end workload, e.g. `-s 15` means the end workload is 150%
- `-r [arg]`: arg means the repetition times of the simulation
- `-jn [arg]`: states job number
- `-h`: output help information
- `-at [arg]`: states the arrival pattern of jobs
	- `-at u`: uniform arrival
	- `-at p`: Poisson arrival
	- `-at r`: real traces
	- `-at s`: staggered job arrival
	- `-at f`: fixed job arrival
- `-jl [arg]`: states job length type
	- `-jl u`: jobs have uniform length, i.e. length satisfy uniform distribution
	- `-jl e`: jobs have equal length, i.e. length satisfy equal distribution

### Output
- store at files with fileName
	- `fileName += sch.GetType().Name + "_" + Config.JobArrivalType + "_" + Config.JobLengthType;`
	
### How to Run
- Last updated: 11/20/2014
- run `.bat` files under path `GreenDC\Experiment\GreenDC-Schedule\GreenDC\bin\Release`, bat file name format is `run_[job arrival pattern][job length distribution]`.
	- **run_UU.bat**:
	- **run_SU.bat**:
	- **run_RU.bat**:
	- **run_PE.bat**:



