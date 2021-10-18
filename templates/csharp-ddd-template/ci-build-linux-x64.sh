cd ./src/CSharp.DDD.Template.API
dotnet publish --runtime linux-x64  --framework netcoreapp3.1 --self-contained -c  Release -o  ../../dist/linux-x64/api

cd ../../
cd ./src/CSharp.DDD.Template.Worker
dotnet publish --runtime linux-x64  --framework netcoreapp3.1 --self-contained -c  Release -o  ../../dist/linux-x64/worker
