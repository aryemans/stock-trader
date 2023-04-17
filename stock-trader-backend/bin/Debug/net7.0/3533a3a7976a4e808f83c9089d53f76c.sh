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

ps 66900;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 66900 > /dev/null;
done;

for child in $(list_child_processes 66931);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/aryemans/stock-trader/stock-trader-backend/stock-trader-backend/bin/Debug/net7.0/3533a3a7976a4e808f83c9089d53f76c.sh;
