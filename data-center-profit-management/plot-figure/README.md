## Plot Figure


### Compare First-Fit and Best-Fit
- code: [plotFFBF.m](./plotFFBF.m)
- how to plot
  - you can compare FirstFit and BestFit in different metrixs, including
    - ScheduledProfit, UsedGreenEnergy, UsedBrownEnergyAmount, UsedBrownEnergyCost
    - ScheduledJobs.Count, ScheduledWorkloadUtilization
  - you can choose which metrix you want to compare by feeding the script with the corresponding metrix value (in numeric, index start from 2)
  - example: to compare the profits
  ```
  plotFFBF(2)
  ```
  - sample figure:
