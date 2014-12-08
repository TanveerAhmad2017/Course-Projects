from pystats import *


def processRunTime():
	filename = './48time-16vm-10jl-0.2rate.txt'
	fieldNum = 2
	for fieldNum in range(4,7):
		with open(filename) as f:
			# print filename
			res = stats(stream = f, field=fieldNum, delimiter=' ', skip = 1, confidence=0.95)
			print res

		f.close()


if __name__ == "__main__":
	processRunTime()