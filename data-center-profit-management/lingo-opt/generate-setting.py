import sys
import random

# settings
jobnum = 6
times = 10
vmnum = 2
valuerange = 100

#output jobs
f = open('jobs.txt','w')


f.write("%s%d%s" %('job1..job', jobnum ,'~\n'))
#arrive, deadline, process, VM, value
for x in range(0,jobnum):
	arrive = random.randint(1,times);
	deadline = random.randint(arrive, times);
	process = random.randint(1, deadline-arrive+1)
	vm = random.randint(1,vmnum)
	value = random.random()*100
	f.write('%d %d %d %d %d \n' %(arrive, deadline, process, vm, value))

f.close()


#output time
f = open('times.txt', 'w')

f.write('%s%d%s' %('time1..time', times, '~\n'))

f.close()


#output solar energy
f = open('solars.txt', 'w')

f.write('%s%d%s' %('green1..green', times, '~\n'))
green = [0]*times
for x in range((int)(times*9/48), (int)(times*21/48)):
	green[x] = random.randint(1, vmnum)
for x in range((int)(times*33/48), (int)(times*45/48)):
	green[x] = random.randint(1, vmnum)
for i in range(len(green))
	f.write(green[i])
f.write('\n')
f.close()


#output brown energy price
f = open ('brownPrice.txt', 'w')
f.write('%s%d%s' %('brown1..brown', times, '~\n'))
f.write('1 1 1 1 1 1 1 1 1 1 \n')
f.close()