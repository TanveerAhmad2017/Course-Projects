## Source Code


----------------------------------------------------------------------------------------------------------------------------
### File
##### Code
|file| content|
|:---|:-------|
|[offline-opt.lg4](./offline-opt.lg4)| Integer Nonlinear Programming (INLP) formulation of the offline problem of maximizing profit|


##### Input File
|file| content|
|:---|:-------|
|[jobs.txt](./jobs.txt)| each line defines a job (arrive, deadline, process, VM, value)|
|[brownPrice.txt](./brownPrice.txt)| brown energy price at each time slot|
|[solars.txt](./solars.txt)| solar energy at each time slot|
|[times.txt](./times.txt)| define total time slots|


#####  Output File
|file| content|
|:---|:-------|
|[ScheduleJobNum.txt](./ScheduledJobNum.txt)| total number of jobs scheduled|
|[UsedGreenEnergy.txt](./UsedGreenEnergy.txt)| total amount of green energy used|

-----------------------------------------------------------------------------------------------------------------------------
### How to run
##### Generate setting
```
python generate-setting.py
```

##### Offline optimal
- run in Lingo software

##### Heuristic algorithm
