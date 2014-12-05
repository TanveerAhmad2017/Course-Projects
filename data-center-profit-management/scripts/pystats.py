#!/usr/bin/env python

# Author: Zhonghua Xi
# Date: 12/3/2014

import sys
import numpy as np
import scipy as sp
import scipy.stats

VERSION = "0.0.3"

delimiter = ' '
field = 1
i = 1
skip = 0
precision = 5
confidence = 0.95

def print_help():
    print 'pystats - a summary statistics script'
    print 'Version', VERSION, 'by Zhonghua Xi'
    print 'Project website: https://github.com/xizhonghua/pystats'
    print 'Usage:',sys.argv[0],'[options]'
    print 'Options:'
    print '    -h  ', 'help message'
    print '    -f# ', 'field index, start from 1'
    print '    -d# ', 'field delimiter'
    print '    -s# ', 'skip first # lines, default is one for header'
    print '    -c# ', 'confidence default =', confidence
    print '    -p# ', 'precision, default =', precision


def parse_args():
    global delimiter
    global field
    global i
    global skip
    global precision
    global confidence

    while i < len(sys.argv):
        arg = sys.argv[i]
        if arg[0] != '-':
            print 'unknown option:', i
            sys.exit(0)
        if arg == '-h':
            print_help()
            sys.exit(0)
        elif arg.startswith('-f'):
            field = int(arg[2:])
        elif arg.startswith('-d'):
            delimiter = (arg[2:])
        elif arg.startswith('-s'):
            skip = 1 if arg == '-s' else int(arg[2:])
        elif arg.startswith('-p'):
            precision = int(arg[2:])
        elif arg.startswith('-c'):
            confidence = float(arg[2:])
        i+=1

def mean(data, s = None):
    if len(data) == 0:
        return 0

    # use pre-computed sum if we have it
    ss = sum(data) if s == None else s

    m = ss / float(len(data))

    return m

def variance(data, miu = None):
    if len(data) == 0:
        return 0
    # use pre-computed miu
    m = mean(data) if miu == None else miu
    v = sum((m - value) ** 2 for value in data) / float(len(data))
    return v

def median(data):
    if len(data) == 0:
        return 0

    mid = len(data)/2

    if len(data)%2==0: 
        return (data[mid] + data[mid-1])/2

    return data[mid]

def mean_confidence_interval(data, confidence=0.95):
    a = 1.0*np.array(data)
    n = len(a)
    m, se = np.mean(a), scipy.stats.sem(a)
    h = se * sp.stats.t._ppf((1+confidence)/2., n-1)
    return m-h, m+h


def print_st(key, value):
    if not isinstance( value, int ):
        format_str = "%s = %0." + str(precision) + "f"        
        print format_str % (key.rjust(12,'_'), value)
    else:
        print "%s = %d" % (key.rjust(12,'_'), value)

def stats(stream, field=1, delimiter=' ', skip = 0, confidence=0.95):
    data = []
    lineNum = 0
    for line in stream:

        lineNum += 1

        # skip first skip lines
        if lineNum <= skip: continue

        # if line is empty or white space, do nothing
        if len(line) == 0 or line.isspace(): continue

        items = line.split(delimiter)
        data.append(float(items[field-1]))

    s = sum(data)
    l = len(data)
    m = mean(data, s)
    v = variance(data, m)
    min_value = 0 if len(data) == 0 else min(data)
    max_value = 0 if len(data) == 0 else max(data)
    std_dev = v ** 0.5
    median_value = median(data)
    ci = mean_confidence_interval(data, confidence)

    return (field, l, m, v, std_dev, s, min_value, max_value, median_value, ci)

if  __name__ == "__main__":
    parse_args()

    field, l, m, v, std_dev, s, min_value, max_value, median_value, ci = stats(sys.stdin, field, delimiter, skip, confidence)

    print_st("Field", field)
    print_st("Lines", l)
    print_st("Mean", m)
    print_st("Variance", v)
    print_st("StdDev", std_dev)
    print_st("Sum", s)
    print_st("Min", min_value)
    print_st("Max", max_value)
    print_st("Median", median_value)
    print_st("Confidence", confidence)
    print_st("Cnf.Itv.L", ci[0])
    print_st("Cnf.Itv.U", ci[1])