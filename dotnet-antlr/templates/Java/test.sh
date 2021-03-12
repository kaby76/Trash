JAR=~/Downloads/antlr-4.9.1-complete.jar
CLASSPATH=$JAR:.
err=0
for g in ../examples/*
do
  file=$g
  x1="${g##*.}"
  if [ "$x1" != "errors" ]
  then
    echo $file
    java -classpath $CLASSPATH Program -file $file
    status="$?"
    if [ -f "$file".errors ]
    then
      if [ "$stat" = "0" ]
      then
        echo Expected parse fail.
        err=1
      else
        echo Expected.
      fi
    else
      if [ "$status" != "0" ]
      then
        err=1
      fi
    fi
  fi
done
exit $err
