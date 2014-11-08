pkill rmiregistry
sleep 1
rmiregistry &

javac -d . ../src/rmi/calendar/*.java
