import os
import subprocess
from generateSetting import *

#c# executable file path  sharpPath = ..\GreenDC-Schedule\Simulation\bin\Release\Simulation.exe

def runSim(rep= 3, arrivalrate=0.5):
	sharpPath = "..\\GreenDC-Schedule\\Simulation\\bin\\Release\\Simulation.exe"
	dataPath = "..\\data\\"
	

	for i in range(rep):
		generateSetting(arrivalrate)
	
		for schedulerName in ['firstfit', 'bestfit']:
			outputFilePath = "..\\result\\" + schedulerName + "_arrivalrate_" + str(arrivalrate) + ".txt"
			cmd = sharpPath
			cmd += (' >' if i==1 else ' >> ')
			cmd += outputFilePath
			cmd += " " + dataPath 
			cmd += " " + schedulerName

			# cmds = [cmd,  dataPath, schedulerName]
			#print cmd
			os.system(cmd)
			# print cmds
			#subprocess.call(cmds)



if __name__ == "__main__":
	runSim(30)