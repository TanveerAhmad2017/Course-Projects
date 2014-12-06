import os
import subprocess
from generateSetting import *

#c# executable file path  sharpPath = ..\GreenDC-Schedule\Simulation\bin\Release\Simulation.exe

def runSim(rep, arrivalrate):
	sharpPath = "..\\GreenDC-Schedule\\Simulation\\bin\\Release\\Simulation.exe"
	dataPath = "..\\data\\"
	

	for i in range(1,rep+1):		
		generateSetting(arrivalrate = arrivalrate)
	
		for schedulerName in ['firstfit', 'bestfit']:
			outputFilePath = "..\\result\\" + schedulerName + "_arrivalrate_" + str(arrivalrate) + ".txt"
			cmd = sharpPath
			#replace files if not in the same sets of repeatition
			cmd += (' >' if i==1 else ' >> ')
			cmd += outputFilePath
			cmd += " " + dataPath 
			cmd += " " + schedulerName

			# call c-sharp program to run the scheduler
			os.system(cmd)
		



if __name__ == "__main__":
	#arrival rate change from 0.1 to 1.5, which corresponds to workload utilization change from 20% to 200%
	for i in range(1, 16):
		print i
		arrivalrate = 0.2
		runSim(rep = 30, arrivalrate = i*arrivalrate)