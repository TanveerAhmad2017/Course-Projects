import os
import numpy
import math

file70 = '../data/heart-c-pca70-back.txt'
file100 = '../data/heart-c-pca100-back.txt'


def readFileToMatrix(filename,  skipnum, validCol):
	x = []
	with open(filename) as f:
			for i in xrange(skipnum):
				next(f)
			for line in f:
				stripline = line.split(',')[0:validCol]
				#print stripline
				x.append([float(item) for item in stripline])
	a = numpy.matrix(x)
	return a


def computeEucliean():
	skipnum = 27
	validCol = 22

	x1 = readFileToMatrix(file70, skipnum, validCol)
	x2 = readFileToMatrix(file100, skipnum, validCol) 

	print len(x1)
	print len(x2)
	x = x1 - x2

	x = numpy.square(x)
	x = numpy.sum(x, axis = 1)
	x = numpy.sqrt(x)

	print 'mean' , numpy.mean(x)
	print 'std' , numpy.std(x)

	x = numpy.sort(x,axis=0)
	#print x


	










if __name__ == '__main__':
	computeEucliean()