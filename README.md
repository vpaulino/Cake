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

### Microservices

AppServc and AppServc2 represent two microservices that define the solution being developed. 

### Libs 

the Libs folder represents all the Libs that this solution export as package nugets

### Tests

The tests folder groups all the tests in the solution

## Conventions

The folder structure of this solution helped in many ways.

1. Build source projects
2. Running tests
3. set version on the services per microservice using the Directory.Build.Props file per microservice
