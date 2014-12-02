Model
=====

### Features
- define components used in the simulations

### Cluster
- BitMap: a 2D array (time, node)
- TimeSlots: length of time in the snapshot
- NodeNum: number of nodes in the cluster

### Job
- Properties
	- ArrivalTime
	- Deadline
	- Weight: for current version, the weight is proportional to the computing cost
	- EnergyCost: for current version, the energy cost is proportional to the computing cost
	- RequiredTimeSlots
	- ScheduledCost
	- AssignedTimeSlotsList: time slots assigned to the jobs(may be continous or not)
	- ...
- Sort
	- JobArrialTimeASCComparer: sort in increasing order of arrival time
	- JobDeadlineASCComparer: sort in increasing order of deadline
	- WeightDescComparer: sort in decreasing order of weight
	

### Job Arrival Type
- Uniform
- Poisson
- Staggered
- Real
- Fixed

### Job Length Type
- Uniform
- Equal
- Real

### JobTraces
- load jobs from real traces files

### SolarTraces
- `ReadEntries`: read solar traces from file
- `writeSolarTraceToFile`: store solar traces to file, for plotting the solar traces


### SimulateResults
- workload
- ScheduledJobNum
- ScheduledRevenue
- UsedGreenEnergy
- UsedBrownEnergy
- UsedBrownEnergyCost

### Problem Setting
- Jobs
	- `jobs = JobGenerator.JobGeneratorFactory.GetJobGenerator(config.JobArrivalType).GenerateJob(config);`
- SolarEnergyList
- BrownPriceList
- TimeSlots

	
