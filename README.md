# WebAPI with Docker

## How to setup and run the service using docker
- To setup and get the docker containers up and running using Visual Studio you need to have the latest [.Net sdk](https://dotnet.microsoft.com/en-us/download). 
- You also have to be able to run [docker cli](https://www.docker.com/get-started/).
- clone this git-repository.
- Open up a terminal and browse to the location of the docker-compose file and execute: <code>docker compose up</code> 
<br/> 

or <br/>

- Just run Docker Compose as startup project in Visual Studio
- Then you get a nice Swagger interface to test the api.

## How to query the API using Postman installed.
To query the API you can make get, post, patch and delete requests to the various endpoint: admissions, doctors, medicaljournals and departments.

HTTPGET: http://localhost:8080/admissions

HTTPGET (by id, id is a guid): http://localhost:8080/admissions/id

HTTPPOST and HTTPPATCH is easiest from Postman using a body with formdata

HTTPDELETE (id is a guid) : http://localhost:8080/admissions/id

### To test if a doctor has access to a patients medical journal:<br/>

*Doctor with access :* <br/> 
You need to get an admission and copy the medicalJournal: ssn and the doctor: id. <br/>
http://localhost:8080/medicaljournals/patient?patientSsn='medicalJournalSsn'&doctorId='doctorId'</code><br/>

*Doctor without access :* <br/> 
You need to get two admissions and copy one of the medicalJournal: ssn and a doctor: id that is not correspondend. <br/>
http://localhost:8080/medicaljournals/patient?patientSsn='medicalJournalSsn'&doctorId='doctorId'</code><br/>


## Explanation on why I made choices
I have used .Net/C#, mssql and EntityFramework as my tools.<br/>
In the project I have made a docker compose file to orchestrate the deployment of the 2 docker containers. It is auto-generated and I have refactored it to my needs.<br/>
I have made 4 entities (Admission, Department, Doctor and MedicalJournal) and used EntityFramework to make migrations and apply them to the database.<br/>
I have seeded the database using [BOGUS](https://github.com/bchavez/Bogus). <br/> 
There are 4 different controllers representing each of the 4 entities with full CRUD-functionality.<br/>

---
### Secrets, passwords and connectionstrings
I have not done anything to hide the secrets, passwords or connectionstrings in the project for the sake of transparency.
