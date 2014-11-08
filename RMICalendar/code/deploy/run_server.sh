pkill rmiregistry
sleep 1
rmiregistry 1086 &

javac -d . ../src/rmi/calendar/*.java ../src/DHash/*.java ../src/util/*.java
java rmi.calendar.CalendarManager
