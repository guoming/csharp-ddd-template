set workDir=%cd%
cd %workDir%/src/CSharp.DDD.Template.API
dotnet publish --runtime linux-x64  --framework netcoreapp3.1 --self-contained -c  Release -o  %workDir%/dist/linux-x64/api

cd %workDir%/src/CSharp.DDD.Template.Worker
dotnet publish --runtime linux-x64  --framework netcoreapp3.1 --self-contained -c  Release -o  %workDir%/dist/linux-x64/worker

pause