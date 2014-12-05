from pystats import *

schedule = ['firstfit', 'bestfit']
for schedulerName in schedule:
	average = []
	error = []
	for i in range(1,16):
		filename = '../result/' + schedulerName +'_arrivalrate_'+ str(i*0.1) +'.txt'
		with open(filename) as f:
			# print filename
			res = stats(stream = f, field=2, delimiter=' ', skip = 0, confidence=0.95)
			# print res
			# print res[2], res[9][1] - res[2]
			average.append(res[2])
			error.append((float)(res[9][1] - res[2]))
		f.close()
	print average
	print error

	outputFileName = '../plot-figure/data/' + schedulerName +'.txt'
	with open(outputFileName,'w') as f:
		f.write('a')
		# f.write(str(average))
		# f.write(str(error))
		# f.write('\n')
		# for item in error:
		# 	f.write("%f ", item)
		# f.write('\n')
	f.close()
