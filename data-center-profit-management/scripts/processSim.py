from pystats import *


def processSim(arrivalrate = 0.1):
	schedule = ['firstfit', 'bestfit']
	metrix = ['SchedulerType', 'ScheduledProfit', 'UsedGreenEnergy', 'UsedBrownEnergyAmount', 
	'UsedBrownEnergyCost', 'ScheduledJobs.Count', 'ScheduledWorkloadUtilization',
	'ArrivedWorkloadUtilization','AvgUnitBrownCost']
	# 2: ScheduledProfit
	# 7: ScheduledWorkloadUtilization
	# 3: UsedGreenEnergy
	# 4: UsedBrownEnergyAmount
	# 5: UsedBrownEnergyCost

	for fm in range(2,10):
		fieldNum = fm
		for schedulerName in schedule:
			average = []
			error = []
			for i in range(1,16):
				filename = '../result/' + schedulerName +'_arrivalrate_'+ str(i*arrivalrate) +'.txt'
				with open(filename) as f:
					# print filename
					res = stats(stream = f, field=fieldNum, delimiter=' ', skip = 0, confidence=0.95)
					# print res
					# print res[2], res[9][1] - res[2]
					average.append(res[2])
					error.append((float)(res[9][1] - res[2]))
				f.close()
			print average
			print error

			outputFileName = '../plot-figure/data/' + schedulerName + '_' + metrix[fieldNum-1]+'.txt'
			f = open(outputFileName,'w')
			print outputFileName
			for item in average:
				f.write("%f " %item )
			f.write('\n')
			for item in error:
				f.write("%f " %item )
			f.write('\n')		
				# for item in error:
				# 	f.write("%f ", item)
				# f.write('\n')
			f.close()


if __name__ == "__main__":
	arrivalrate = 0.2
	processSim(arrivalrate = arrivalrate)