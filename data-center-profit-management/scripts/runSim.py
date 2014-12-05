import os
import subprocess
from generateSetting import *

#c# executable file path  sharpPath = ..\GreenDC-Schedule\Simulation\bin\Release\Simulation.exe

def runSim(rep, arrivalrate):
	sharpPath = "..\\GreenDC-Schedule\\Simulation\\bin\\Release\\Simulation.exe"
	dataPath = "..\\data\\"
	

	for i in range(rep):
		generateSetting(arrivalrate)
	
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
	for i in range(15):
		runSim(30, arrivalrate = i*0.1)