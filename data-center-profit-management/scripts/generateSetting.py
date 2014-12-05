import sys
import random

def generateSetting(jobnum=0, times = 48, vmnum = 4, offpeak = 1, onpeak =2, revenuerate = 10, jobavglen =4, arrivalrate = 0.5):
	# # settings
	# jobnum = 0
	# times = 48
	# vmnum = 4
	# offpeak = 1
	# onpeak = 2
	# revenuerate = 10
	# jobavglen = 4
	# arrivalrate = (float)(1)/2

	

	#output vm
	f = open('../data/vm.txt', 'w')
	f.write("%d" %vmnum)
	f.close()

	#output revenue rate
	f = open("../data/revenuerate.txt", 'w')
	f.write("%d" %revenuerate)
	f.close()


	f = open("../data/totaltimeslots.txt","w")
	f.write("%d" %times)
	f.close()


	#output jobs
	f = open('../data/jobs.txt','w')
	curr = 0
	while curr < times:	
		#arrive, deadline, process, VM, value
		curr = (int)(random.expovariate(arrivalrate) + curr)
		if curr > times:
			break
		deadline = random.randint(curr, times);
		process = random.randint(1, min(jobavglen*2,deadline-curr+1))
		vm = 1
		f.write('%d %d %d %d\n' %(curr, deadline, process, vm))
		jobnum = jobnum+1
	f.close()

	#print jobnum

	f = open('../data/jobnum.txt','w')
	f.write("%s%d%s" %('job1..job', jobnum ,'~\n'))
	f.close()

	#output time
	f = open('../data/times.txt', 'w')
	f.write('%s%d%s' %('time1..time', times, '~\n'))
	f.close()


	#output solar energy
	f = open('../data/solars.txt', 'w')

	f.write('%s%d%s' %('green1..green', times, '~\n'))
	green = [0]*times
	#day 1 daytime
	for x in range((int)(times*6/48), (int)(times*17/48)):
		green[x] = random.randint(1, vmnum)
	#day 1 nighttime
	for x in range((int)(times*30/48), (int)(times*41/48)):
		green[x] = random.randint(1, vmnum)
	for i in range(len(green)):
		f.write('%d ' %green[i])
	f.write('\n')
	f.close()


	#output brown energy price
	f = open ('../data/brownPrice.txt', 'w')
	f.write('%s%d%s' %('brown1..brown', times, '~\n'))
	brown = [0]*times;
	#day 1 browntime
	for x in range((int)(times*9/48), (int)(times*21/48)):
		brown[x] = onpeak
	#day 1 onpeak
	for x in range((int)(times*33/48), (int)(times*45/48)):
		brown[x] = onpeak
	for x in range((int)(len(brown))):
		if (brown[x]!=onpeak): 
			brown[x] = offpeak
		f.write('%d ' %brown[x])
	f.close()

	# print 'jobnum = ', jobnum, 'times = ', times, "vmnum = ", vmnum, "arrivalrate = ", arrivalrate
	sys.stdout.write("jobnum {:>2} times {:>2} vmnum {:>2} arrivalrate {:>2}\n".format(jobnum, times, vmnum,arrivalrate))

if __name__ == "__main__":
	generateSetting()

