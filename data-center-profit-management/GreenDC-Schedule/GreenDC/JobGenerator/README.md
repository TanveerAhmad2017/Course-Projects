JobGenerator
============

### Features
- generate different types of jobs, the jobs will act as input in the simulations.

### Files
- **IJobGenerator.cs**: interface for generating job, can be implemented to generate different types of jobs
- **JobGeneratorFactory.cs**: create different types of job generator according to the input types
- Different job arrival pattern
	- **FixedJobGenerator.cs**: `#TODO` explain!
	- **PoissonArrivalJobGenerator.cs**: generate jobs that arrive in Poisson pattern
	- **StaggeredJobJobGenerator.cs**: generate jobs that arrive in staggered pattern
	- **RealJobGenerator.cs**: generate jobs from real traces
	- **UniformArrivalJobGenerator.cs**: generate jobs that arrive in uniform pattern 
	
### Job Types
- Fixed jobs
- Poisson arrival jobs
- Staggered jobs
	- `#TODO` explain
- Uniform arrival jobs
- Real trace jobs