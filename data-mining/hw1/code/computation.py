import os
import numpy
import math
#import matplotlib.pyplot as plt

# get complete path from relative path, and current directory
zpath = '../data/z.txt'
script_dir = os.path.dirname(__file__)
abszpath = os.path.join(script_dir, zpath)





def loadData():
	count = 0
	#read column from file, and skip the last 3 lines
	x, y = numpy.genfromtxt(abszpath,  unpack = True, skip_footer=3)
	xOrigin, yOrigin = numpy.genfromtxt(abszpath,  unpack = True)
	#computeDist(x, y, xOrigin, yOrigin)
	print 'x equal length'
	equal_width(x)
	print 'y equal length'
	equal_width(y)
	print 'x equal depth'
	equal_depth(x)
	print 'y equal depth'
	equal_depth(y)



def computeDist(x, y, xOrigin, yOrigin):
	# get covariance of X and Y
	covxy =  numpy.cov(x,y)
	print 'covariance metrix'
	print covxy

	# get centroid
	centroidx = numpy.mean(x)
	centroidy = numpy.mean(y)
	print 'centroid: '
	print centroidx
	print centroidy

	# get last three rows
	
	print 'last three columns: '
	print xOrigin[99], yOrigin[99]
	print xOrigin[100], yOrigin[100]
	print xOrigin[101], yOrigin[101]

	# Mahalanobis distance
	# idx is row number of the specific records
	idx = 101
	pointa = numpy.matrix([xOrigin[idx], yOrigin[idx]])
	centroid = numpy.matrix([centroidx, centroidy])
	print 'pointa: ', pointa
	print 'centroid: ', centroid
	diff = numpy.subtract(pointa, centroid)
	print 'diff: ', diff
	transpose = diff.T
	print 'transpose: ', transpose
	inverse = numpy.linalg.inv(covxy)
	print 'inverse: ', inverse
	mahal_dist = diff * inverse * transpose
	print 'mahal dist: ', math.sqrt(mahal_dist)

	# compute euclidean distance
	square_matrix = numpy.power(diff, 2)
	print 'square_matrix', square_matrix
	square_sum =  numpy.sum(square_matrix, axis=1)
 	print 'square_sum: ', square_sum
 	euclidean_dist = math.sqrt(square_sum)
 	print 'euclidean_dist: ', euclidean_dist


def equal_width(x):
	minx = numpy.min(x)
	maxx = numpy.max(x)
	slotx = (maxx-minx)/3
	slotx1 = minx + slotx
	slotx2 = slotx1 + slotx
	print ' %f %f %f %f' %(minx, slotx1, slotx2, maxx)


def equal_depth(x):
	slot0 = numpy.percentile(x, 0)
	slot1 = numpy.percentile(x, 100/3)
	slot2 = numpy.percentile(x, 100-100/3)
	slot3 = numpy.percentile(x, 100)
	print '%f %f %f %f' %(slot0, slot1, slot2, slot3)


if __name__ == '__main__':

	loadData()
	
	
	


