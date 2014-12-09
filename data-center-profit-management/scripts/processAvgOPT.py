from pystats import *
import os


def processAvgOPT(rate):

	schedule = ['firstfit', 'bestfit'] 
	fieldNum = 2
	for schedulerName in schedule:
		filename = '../result/opt/' + schedulerName +'_arrivalrate_'+ str(rate) +'.txt'
		with open(filename) as f:
			# print filename
			res = stats(stream = f, field=fieldNum, delimiter=' ', skip = 0, confidence=0.95)
			# print profile
			print schedulerName,  res[2], (float)(res[9][1] - res[2])


	# print job number
	fieldNum = 8
	filename = '../result/opt/' + 'firstfit' +'_arrivalrate_'+ str(rate) +'.txt'
	with open(filename) as f:
		# print filename
		res = stats(stream = f, field=fieldNum, delimiter=' ', skip = 0, confidence=0.95)
		# print profile
		print 'workload',  res[2], (float)(res[9][1] - res[2])

    #print opt
	fieldNum = 1
	filename = '../result/opt/opt_arrivalrate_'+ str(rate) +'.txt'
	with open(filename) as f:
		# print filename
		res = stats(stream = f, field=fieldNum, delimiter=' ', skip = 1, confidence=0.95)
		# print profile
		print 'opt',   res[2], (float)(res[9][1] - res[2])

if __name__ == "__main__":
	processAvgOPT(1.2)