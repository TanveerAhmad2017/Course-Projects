from pystats import *

for i in range(1,16):
	filename = '../result/bestfit_arrivalrate_'+i*0.1+'.txt'
	with open(filename) as f:
		print filename
		print i
	    res = stats(stream = f, field=2, delimiter=' ', skip = 0, confidence=0.95)
	    print resfilename