Scheduler
=========

### Features
- Implement different schedulers, each scheduler use a different strategy

### Files
- **IScheduler.cs**: interface, to be implemented by different scheduler
- **SchedulerBase.cs**: abstract class, implement common methods shared by different schedulers
- **SchedulerType.cs**: define types of scheduler, include
	- FirstFitScheduler
	- BestFit
	- RandomFit 
	- LeadTimeFit
- Different schedulers
	- **BestFitScheduler.cs**:
	- **FirstFitScheduler.cs**:
	- **LeadTimeScheduler.cs**:
	- **RandomFitScheduler.cs**:


