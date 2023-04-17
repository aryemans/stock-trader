function list_child_processes () {
    local ppid=$1;
    local current_children=$(pgrep -P $ppid);
    local local_child;
    if [ $? -eq 0 ];
    then
        for current_child in $current_children
        do
          local_child=$current_child;
          list_child_processes $local_child;
          echo $local_child;
        done;
    else
      return 0;
    fi;
}

ps 53130;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 53130 > /dev/null;
done;

for child in $(list_child_processes 53152);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/aryemans/stock-trader/stock-trader-backend/stock-trader-backend/bin/Debug/net7.0/fdf7f6696a4d44f3b46a9b335e2ca2e8.sh;
