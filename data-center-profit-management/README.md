## Data Center Profit Management

### Intro
- CS773 course project, 2014 Fall
- [CS773 course website](http://cs.gmu.edu/syllabus/syllabi-fall14/CS773AydinH.html)
- [Instructor: Dr. Hakan Aydin](http://cs.gmu.edu/~aydin/)

### Objective
- Design and implement offline optimal algorithm to management profit in data center
- Implement classical heuristic algorithms to study their performance in maximizing profit.

### Overview of this project
- We implement the offline optimal algorithm (OPT) using LINGO solver
- We implement two heuristic algorithms FirstFit and BestFit
- We evaluate the running time of optimal offline algorithm (OPT) under different settings
- We evaluate the performance of heuristic algorithms FirstFit and BestFit against OPT

### Overview of folders
|Name| Function|
|:----|:-------|
|[GreenDC-Schedule](./GreenDC-Schedule)| Implement FirstFit and BestFit, writing in C#|
|[data](./data)| Simulation settings|
|[lingo-opt](./lingo-opt)| Implementation of optimal offline algorithm, writing in LINGO solver|
|[plot-figure](./plot-figure)| Plot figures for writing report, writing in Matlab|
|[result](./result)| Simulation result|
|[scripts](./scripts)| Scripts for running different simulations, writing in Python|
