from pystats import *
from generateSetting import *
import os

def oneOPTSimulation(rate):
	generateSetting(times = rate)


	## execute firstfit, bestfit
	sharpPath = "..\\GreenDC-Schedule\\Simulation\\bin\\Release\\Simulation.exe"
	dataPath = "..\\data\\"

	## Read profit to optimal file
	f = open("..\\lingo-opt\\profit.txt")
	a = f.readline()
	rnt = a.join(a.split()) 
	optOutPutPath =  "..\\result\\opt\\opt_arrivalrate_" + str(rate) + ".txt"
	outf = open(optOutPutPath, "a")
	outf.write(rnt + "\n")
	outf.close()
	f.close()



	for schedulerName in ['firstfit', 'bestfit']:
		outputFilePath = "..\\result\\opt\\" + schedulerName + "_arrivalrate_" + str(rate) + ".txt"
		cmd = sharpPath
		#replace files if not in the same sets of repeatition
		cmd += ' >> '
		cmd += outputFilePath
		cmd += " " + dataPath 
		cmd += " " + schedulerName

		# call c-sharp program to run the scheduler
		os.system(cmd)

	## copy file to lingo folder
	cmd = "copyFileSettingToLingo.bat"
	os.system(cmd)


if __name__ == "__main__":
	oneOPTSimulation(24)