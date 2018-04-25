@echo off
 
start cmd /c "mongod --port 27001 --replSet replica --dbpath %cd%\data\replica\data1 --bind_ip localhost --oplogSize 128 --wiredTigerJournalCompressor zlib --wiredTigerCollectionBlockCompressor zlib"
start cmd /c "mongod --port 27002 --replSet replica --dbpath %cd%\data\replica\data2 --bind_ip localhost --oplogSize 128 --wiredTigerJournalCompressor zlib --wiredTigerCollectionBlockCompressor zlib"
start cmd /c "mongod --port 27003 --replSet replica --dbpath %cd%\data\replica\data3 --bind_ip localhost --oplogSize 128 --wiredTigerJournalCompressor zlib --wiredTigerCollectionBlockCompressor zlib"
 
TIMEOUT /T 7
start cmd /c "mongo localhost:27001 --eval rs.initiate({_id:'replica',members:[{_id:0,host:'localhost:27001'},{_id:1,host:'localhost:27002'},{_id:2,host:'localhost:27003'}]})"