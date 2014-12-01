import sys

# settings
jobnum = 3
times = 5

#output jobs
f = open('jobs.txt','w')


f.write("%s%d%s" %('job1..job', jobnum ,'~\n'))
f.write('1 5 1 1 1 \n')
f.write('1 5 1 1 1 \n')
f.write('1 5 1 1 1 \n')

f.close()


#output time
f = open('times.txt', 'w')

f.write('%s%d%s' %('time1..time', times, '~\n'))

f.close()


#output solar energy
f = open('solars.txt', 'w')

f.write('%s%d%s' %('green1..green', times, '~\n'))
f.write('0 0 0 0 1\n')

f.close()


#output brown energy price
f = open ('brownPrice.txt', 'w')
f.write('%s%d%s' %('brown1..brown', times, '~\n'))
f.write('1 1 1 1 1\n')
f.close()