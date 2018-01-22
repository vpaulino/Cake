# Cake

## Introduction 

this repository goal is to maintain a Cake approaches reference. The apllication example is not important, but it represents a microservice solution implementation,
where each microservice source code is grouped in a folder inside the source folder. 


## Struture

-Src <br/>
--AppServc2 <br/>
--AppServc <br/>
--Libs <br/>
-Tests <br/>
--AppServc2 <br/>
--AppServc <br/>

### Microservices

AppServc and AppServc2 represent two microservices that define the solution being developed. 

### Libs 

the Libs folder represents all the Libs that this solution export as package nugets

### Tests

The tests folder groups all the tests in the solution per microservice host application

## Conventions

The folder structure of this solution helped in many ways.

1. Build source projects
2. Running tests
3. set version on the services dlls per microservice using the Directory.Build.Props file per microservice
4. parameterized build per host. this allows saving time and resources when building large codebase

## Execute cake build

To a full list of the parameters see inside the script build.cake

the command to execute the build for only one service is: 

.\build.ps1 -t build --buildNumber=123123 --appName="AppSrvc"

